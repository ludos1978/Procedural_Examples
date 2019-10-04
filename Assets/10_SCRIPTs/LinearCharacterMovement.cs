using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class LinearCharacterMovement : MonoBehaviour {

	public Vector3[] wayPoints;
	public int wayPointIndex = 0;

	public Transform target;
	public float movementSpeed = 3;

	// Update is called once per frame
	void Update () {
		// goto other object if closer then 10 units
		if ((target.transform.position - transform.position).magnitude < 10) {
			MoveTo(target.transform.position);
		} else {
            // follow waypoints
			if (MoveTo(wayPoints[wayPointIndex])) {
				// arrived at position
				wayPointIndex = (wayPointIndex + 1) % wayPoints.Length;
			} else {
				// goto current target
			}
		}
	}

	bool MoveTo (Vector3 targetPos) {
		Vector3 targetOffset = targetPos - transform.position;
		targetOffset = targetOffset.normalized;
		transform.position += targetOffset * Time.deltaTime * movementSpeed;
		if ((targetPos - transform.position).magnitude < 0.1f) {
			return true;
		}
		return false;
	}
}
