using UnityEngine;
using System.Collections;

public class SetPosition : MonoBehaviour {

	public Transform pos1;
	public Transform pos2;
	public Transform pos3;
	public Transform pos4;

	public Transform moveObject;

	// Use this for initialization
	void Start () {
	
	}
	
	// Update is called once per frame
	void Update () {
		if (Input.GetKey (KeyCode.Alpha1)) {
			moveObject.transform.position = pos1.position;
		}
		if (Input.GetKey (KeyCode.Alpha2)) {
			moveObject.transform.position = pos2.position;
		}
		if (Input.GetKey (KeyCode.Alpha3)) {
			moveObject.transform.position = pos3.position;
		}
		if (Input.GetKey (KeyCode.Alpha4)) {
			moveObject.transform.position = pos4.position;
		}
	}
}
