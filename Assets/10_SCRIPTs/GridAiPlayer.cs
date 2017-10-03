using System.Collections;
using System.Collections.Generic;
using UnityEngine;


public class GridAiPlayer : MonoBehaviour {

	public int xPos = 5;
	public int yPos = 7;
	public Grid grid;
	public GridPlayer player;
	public float timeInterval = 1.5f;

	public IntegerPosition[] waypoints;
	public int currentWaypointIndex = 0;

	// Use this for initialization
	void Start () {
		HandleInput(xPos, yPos, true);
		StartCoroutine(Interval());
	}

	IEnumerator Interval () {
		while (true) {
			yield return new WaitForSeconds(timeInterval);
			Debug.Log("interval "+Time.time);
			if ((Mathf.Abs(player.xPos - xPos) + Mathf.Abs(player.yPos - yPos)) < 4) {
				// follow player character
				HandleInput(player.xPos, player.yPos, false);
			}
			else {
				// waypoint controlled character
				int targetXPos = waypoints[currentWaypointIndex].posX;
				int targetYPos = waypoints[currentWaypointIndex].posY;
				if (HandleInput(targetXPos, targetYPos, true)) {
					// am ziel
					currentWaypointIndex = (currentWaypointIndex + 1) % waypoints.Length;
				} else {
					// noch nicht am ziel
				}
			}
		}
	}

	bool HandleInput(int targetXPos, int targetYPos, bool follow) {
		int desiredXPos = xPos;
		int desiredYPos = yPos;

		// here comes the AI
		// zielpos - sourcepos -> bewegungsvector
		// if follow
		int xOffset = targetXPos - xPos;
		int yOffset = targetYPos - yPos;
		if (!follow) {
			// flee
			xOffset = xPos - targetXPos;
			yOffset = yPos - targetYPos;
		}

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

		if ((targetXPos == xPos) && (targetYPos == yPos)) {
			Debug.LogWarning("on target");
			return true;
		}
		return false;
	}
}
