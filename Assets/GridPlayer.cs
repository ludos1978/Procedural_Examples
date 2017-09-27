using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GridPlayer : MonoBehaviour {

	public int xPos = 3;
	public int yPos = 3;
	int lastPlayerInput = -1;

	// Use this for initialization
	void Start () {
		HandleInput();
		StartCoroutine(Interval());
	}

	IEnumerator Interval () {
		while (true) {
			yield return new WaitForSeconds(3);
			Debug.Log("interval "+Time.time);
			HandleInput();
		}
	}

	void HandleInput() {
		if (lastPlayerInput == -1) {
			// ... spieler hat nichts gedrueckt...
		}
		if (lastPlayerInput == 0) {
			xPos += 1;
		}
		if (lastPlayerInput == 1) {
			yPos -= 1;
		}
		if (lastPlayerInput == 2) {
			xPos -= 1;
		}
		if (lastPlayerInput == 3) {
			yPos += 1;
		}
		// reset the last player input after executing the movement function
		lastPlayerInput = -1;

		transform.position = new Vector3(xPos, 0, yPos);
	}

	// Update is called once per frame
	void Update () {
		if (Input.GetKeyDown(KeyCode.D)) {
			lastPlayerInput = 0;
		}
		if (Input.GetKeyDown(KeyCode.S)) {
			lastPlayerInput = 1;
		}
		if (Input.GetKeyDown(KeyCode.A)) {
			lastPlayerInput = 2;
		}
		if (Input.GetKeyDown(KeyCode.W)) {
			lastPlayerInput = 3;
		}
	}
}
