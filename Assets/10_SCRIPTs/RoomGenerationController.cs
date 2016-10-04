// MIT License

using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class RoomGenerationController : MonoBehaviour {

	public GameObject[] roomTemplates;

	public List<Transform> unusedDoors;

	// Use this for initialization
	void Start () {
		CreateRoom (null);

        StartCoroutine (Coroutine ());
    }

    IEnumerator Coroutine () {
        while (true) {

            // use free doors to create new rooms
            foreach (Transform tr in unusedDoors.ToArray()) {
                CreateRoom (tr);
                unusedDoors.Remove (tr);
            }

            // wait one second and loop
            yield return new WaitForSeconds (1.0f);
        }
    }
	
	// Update is called once per frame
	void CreateRoom (Transform parent) {
        // select type of model to create
        int index = Random.Range (0, roomTemplates.Length);
        if (parent == null) {
            index = 0;
        }

        GameObject activeInstance = Instantiate (roomTemplates[index], Vector3.zero, Quaternion.Euler(270,0,0)) as GameObject;
		activeInstance.SetActive(true);

		if (parent == null) {
			activeInstance.transform.rotation = Quaternion.Euler(270,0,0);
		} else {
            Debug.Log ("RoomGenerationController.CrateRoom: " + parent.transform.position);
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
