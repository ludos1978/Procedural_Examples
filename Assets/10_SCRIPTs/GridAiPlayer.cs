using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GridAiPlayer : MonoBehaviour {

	public int xPos = 5;
	public int yPos = 7;
	public Grid grid;
	public GridPlayer player;

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
		int desiredXPos = xPos;
		int desiredYPos = yPos;

		// here comes the AI
		// zielpos - sourcepos -> bewegungsvector
		int xOffset = player.xPos - xPos;
		int yOffset = player.yPos - yPos;

		// move in the x direction, if horizontal distance larger then vertical distance
		if (Mathf.Abs(xOffset) > Mathf.Abs(yOffset)) {
			if (xOffset > 0) {
				desiredXPos += 1;
			}
			else if (xOffset < 0) {
				desiredXPos -= 1;
			}
		}
		// move in x direction othervise
		else {
			if (yOffset > 0) {
				desiredYPos += 1;
			}
			else if (yOffset < 0) {
				desiredYPos -= 1;
			}
		}

		// move the player if the desired position is free
		if (grid.Get(desiredXPos, desiredYPos) == 0) {
			Debug.Log("moving from "+xPos+" "+yPos+" to "+desiredXPos+" "+desiredYPos);
			xPos = desiredXPos;
			yPos = desiredYPos;
			transform.position = new Vector3(xPos, 0, yPos);
		}
		else {
			Debug.LogWarning("you bumped into something!");
		}

		if ((player.xPos == xPos) && (player.yPos == yPos)) {
			Debug.LogWarning("on player");
		}
	}
}
