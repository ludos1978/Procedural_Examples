using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CubeGenerator : MonoBehaviour {

	public GameObject objectPrefab;

	public Terrain terrain;

	// Use this for initialization
	void Start () {
		// find object size
		// Bounds objecvBounds = objectPrefab.GetComponent<MeshCollider>().bounds;

		// setze ein objekt auf position
		// CreateObjectInstance( GetHeightPosition( new Vector3(500,0,500) ) );

		for (int i=0; i<1000; i++) {
			Vector3 basisPosition = new Vector3(
				Random.Range(0f, 1000f), 0, Random.Range(0f, 1000f)
			);
			// mittels mehreren raycast die hoehe bestimmen
			Vector3 pos0 = GetHeightPosition( new Vector3(basisPosition.x+5, 0, basisPosition.z+5) );
			Vector3 pos1 = GetHeightPosition( new Vector3(basisPosition.x+5, 0, basisPosition.z-5) );
			Vector3 pos2 = GetHeightPosition( new Vector3(basisPosition.x-5, 0, basisPosition.z+5) );
			Vector3 pos3 = GetHeightPosition( new Vector3(basisPosition.x-5, 0, basisPosition.z-5) );
			float minY = Mathf.Min( Mathf.Min(pos0.y, pos1.y), Mathf.Min(pos2.y, pos3.y) );
			Vector3 pos = new Vector3(basisPosition.x, minY, basisPosition.z);
			CreateObjectInstance( pos );
		}
		// SetHeight(pos);
	}
	
	/// <summary>
	/// not yet fully functional...
	/// </summary>
	public void SetHeight (Vector3 position) {
		int startX = (int)(position.x / terrain.terrainData.heightmapWidth);
		int startY = (int)(position.y / terrain.terrainData.heightmapHeight);

		float h = 0; //position.z / terrain.terrainData.heightmapScale.y;
		Debug.Log(""+startX+"/"+startY+" "+h);
		float[,] heights = new float[4,4];
		for (int x=0; x<4; x++) {
			for (int y=0; y<4; y++) {
				heights[x,y] = h;
			}
		}
		terrain.terrainData.SetHeights(startX, startY, heights);
		terrain.Flush();
	}

	/// <summary>
	/// bestimme hoehe ueber boden eines objektes
	/// </summary>
	public Vector3 GetHeightPosition (Vector3 position) {
		RaycastHit hit;
		position.y = 200;
        if (Physics.Raycast(position, Vector3.down, out hit)) {
            Debug.Log("Found an object at " + hit.point + " " + hit.collider.name);
			return hit.point;
		}
		Debug.LogError("did not found a ground!");
		return Vector3.zero;
	}

	/// <summary>
	/// generiere ein object an der position
	/// </summary>
	public void CreateObjectInstance (Vector3 position) {
		GameObject go = Instantiate(objectPrefab, position, Quaternion.Euler(-90,0,0), transform);
	}
}
