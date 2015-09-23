// from http://answers.unity3d.com/questions/196649/combinemeshes-with-different-materials.html
// with some small changes by reto.spoerri@zhdk.ch
// MIT License

using UnityEngine;
using System;
using System.Collections;
using System.Collections.Generic;

public class CombineMeshes : MonoBehaviour {

	// i have no idea why this is not 65536, but having a larger number (> ~45000 causes an error)
	private int maxVertices = 45000;

	public void Combine (List<GameObject> mergeObjects) {

		int maxA = 0;

		while (mergeObjects.Count > 0) {
			maxA ++;
			if (maxA > 100) {
				Debug.Log ("RESCUE ABORT!");
				return;
			}

			// Find all mesh filter submeshes and separate them by their cooresponding materials
			ArrayList materials = new ArrayList ();
			ArrayList combineInstanceArrays = new ArrayList ();

			int vertexCount = 0;

			while ((vertexCount < maxVertices) && (mergeObjects.Count > 0)) {

				GameObject obj = mergeObjects[0];
				if (!obj) {
					Destroy (obj);
					mergeObjects.RemoveAt(0);
					break;
				}
				
				MeshFilter[] meshFilters = obj.GetComponentsInChildren<MeshFilter> ();
				foreach (MeshFilter meshFilter in meshFilters) {
					vertexCount += meshFilter.sharedMesh.vertexCount;
				}

				if (vertexCount < maxVertices) {
					foreach (MeshFilter meshFilter in meshFilters) {
						vertexCount += meshFilter.sharedMesh.vertexCount;
						MeshRenderer meshRenderer = meshFilter.GetComponent<MeshRenderer> ();

						// Handle bad input
						if (!meshRenderer) { 
							Debug.LogError ("MeshFilter does not have a coresponding MeshRenderer."); 
							continue; 
						}
						if (meshRenderer.sharedMaterials.Length != meshFilter.sharedMesh.subMeshCount) { 
							Debug.LogError ("Mismatch between material count and submesh count. Is this the correct MeshRenderer?"); 
							continue; 
						}
						
						for (int s = 0; s < meshFilter.sharedMesh.subMeshCount; s++) {
							int materialArrayIndex = 0;
							for (materialArrayIndex = 0; materialArrayIndex < materials.Count; materialArrayIndex++) {
								if (materials [materialArrayIndex] == meshRenderer.sharedMaterials [s])
									break;
							}
							
							if (materialArrayIndex == materials.Count) {
								materials.Add (meshRenderer.sharedMaterials [s]);
								combineInstanceArrays.Add (new ArrayList ());
							}
							
							CombineInstance combineInstance = new CombineInstance ();
							combineInstance.transform = meshRenderer.transform.localToWorldMatrix;
							combineInstance.subMeshIndex = s;
							combineInstance.mesh = meshFilter.sharedMesh;
							(combineInstanceArrays [materialArrayIndex] as ArrayList).Add (combineInstance);
						}
					}

					Destroy (obj);
					mergeObjects.RemoveAt(0);
				}
			}

			
			GameObject child = new GameObject ("combiner");
			child.transform.parent = transform;
			MeshFilter meshFilterCombine = child.AddComponent<MeshFilter> ();
			MeshRenderer meshRendererCombine = child.AddComponent<MeshRenderer> ();
			MeshCollider meshCollider = child.AddComponent<MeshCollider>();
			
			// Combine by material index into per-material meshes
			// also, Create CombineInstance array for next step
			Mesh[] meshes = new Mesh[materials.Count];
			CombineInstance[] combineInstances = new CombineInstance[materials.Count];
			
			for (int m = 0; m < materials.Count; m++) {
				CombineInstance[] combineInstanceArray = (combineInstanceArrays [m] as ArrayList).ToArray (typeof(CombineInstance)) as CombineInstance[];
				meshes [m] = new Mesh ();
				meshes [m].CombineMeshes (combineInstanceArray, true, true);
				
				combineInstances [m] = new CombineInstance ();
				combineInstances [m].mesh = meshes [m];
				combineInstances [m].subMeshIndex = 0;
			}
			
			// Combine into one
			meshFilterCombine.sharedMesh = new Mesh ();
			meshFilterCombine.sharedMesh.CombineMeshes (combineInstances, false, false);
			
			// Destroy other meshes
			foreach (Mesh mesh in meshes) {
				mesh.Clear ();
				DestroyImmediate (mesh);
			}
			
			// Assign materials
			Material[] materialsArray = materials.ToArray (typeof(Material)) as Material[];
			meshRendererCombine.materials = materialsArray;    

			meshCollider.sharedMesh = meshFilterCombine.sharedMesh;
		}

	}
}