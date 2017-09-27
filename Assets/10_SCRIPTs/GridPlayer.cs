using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GridPlayer : MonoBehaviour {

	public int xPos = 3;
	public int yPos = 3;
	int lastPlayerInput = -1;
	public Grid grid;
	public float timeInterval = 1.5f;


	// Use this for initialization
	void Start () {
		HandleInput();
		StartCoroutine(Interval());
	}

	IEnumerator Interval () {
		while (true) {
			yield return new WaitForSeconds(timeInterval);
			Debug.Log("interval "+Time.time);
			HandleInput();
		}
	}

	void HandleInput() {
		if (lastPlayerInput == -1) {
			// ... spieler hat nichts gedrueckt...
		}
		int desiredXPos = xPos;
		int desiredYPos = yPos;
		if (lastPlayerInput == 0) {
			desiredXPos += 1;
		}
		if (lastPlayerInput == 1) {
			desiredYPos -= 1;
		}
		if (lastPlayerInput == 2) {
			desiredXPos -= 1;
		}
		if (lastPlayerInput == 3) {
			desiredYPos += 1;
		}
		// reset the last player input after executing the movement function
		lastPlayerInput = -1;

		// move the player if the desired position is free
		if (grid.Get(desiredXPos, desiredYPos) == 0) {
			xPos = desiredXPos;
			yPos = desiredYPos;
			transform.position = new Vector3(xPos, 0, yPos);
		}
		else {
			Debug.LogWarning("you bumped into something!");
		}
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
