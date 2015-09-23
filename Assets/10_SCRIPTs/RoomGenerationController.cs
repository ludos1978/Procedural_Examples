using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class RoomGenerationController : MonoBehaviour {

	public GameObject roomTemplate;

	public List<Transform> unusedDoors;

	// Use this for initialization
	void Start () {
		CreateRoom (null);
		foreach (Transform tr in unusedDoors.ToArray()) {
			CreateRoom(tr);
		}
		foreach (Transform tr in unusedDoors.ToArray()) {
			CreateRoom(tr);
		}
	}
	
	// Update is called once per frame
	void CreateRoom (Transform parent) {
		GameObject activeInstance = Instantiate (roomTemplate, Vector3.zero, Quaternion.Euler(270,0,0)) as GameObject;
		activeInstance.SetActive(true);
		if (parent == null) {
			activeInstance.transform.rotation = Quaternion.Euler(270,0,0);
		} else {
			activeInstance.transform.parent = parent.transform;
			activeInstance.transform.localPosition = Vector3.zero;
			activeInstance.transform.localRotation = Quaternion.identity;
		}

		for (int childId = 0; childId < activeInstance.transform.childCount; childId ++ ) {
			GameObject child = activeInstance.transform.GetChild(childId).gameObject;
			if (child.name.Contains("connection-A")) {
				unusedDoors.Add (child.transform);
			}
		}
	}
}
