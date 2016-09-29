using UnityEngine;
using System.Collections;

public class FSMMove : MonoBehaviour {

	public GameObject target;
	public float speed = 5.0f;

	public enum STATE {
		CHASE,
		IDLE,
		PATTERN,
		FLEE,
		FLEE_ENTER
	}
	public STATE currentState = STATE.IDLE;
	public float timer;

	public Vector3[] positions;
	public int positionIndex = 0;

	// Update is called once per frame
	void Update () {
		switch (currentState) {
		case STATE.IDLE:
			if ((target.transform.position - transform.position).magnitude < 10f) {
				currentState = STATE.CHASE;
			}
			break;

		case STATE.CHASE:
			Chase ();
			if ((target.transform.position - transform.position).magnitude < 1f) {
				currentState = STATE.FLEE_ENTER;
			}
			break;
		
		case STATE.PATTERN:
			Vector3 movementTarget = positions [positionIndex];
			if (MoveTo (movementTarget)) {
				positionIndex = (positionIndex + 1) % positions.Length;
			}
			if ((target.transform.position - transform.position).magnitude < 3f) {
				currentState = STATE.CHASE;
			}
			break;

		case STATE.FLEE_ENTER:
			timer = 3.0f;
			currentState = STATE.FLEE;
			goto case STATE.FLEE;
		case STATE.FLEE:
			Flee ();
			timer -= Time.deltaTime;
			if (timer < 0) {
				currentState = STATE.PATTERN;
			}
			break;
		}
	}

	void Chase () {
		MoveTo (target.transform.position);
	}

	void Flee () {
		Vector3 deltaPos = target.transform.position - transform.position;

		float thisSpeed = Mathf.Min(speed * Time.deltaTime, deltaPos.magnitude);
		transform.position -= deltaPos.normalized * thisSpeed;
	}

	bool MoveTo (Vector3 movementTarget) {
		Vector3 deltaPos = movementTarget - transform.position;

		float thisSpeed = Mathf.Min(speed * Time.deltaTime, deltaPos.magnitude);
		transform.position += deltaPos.normalized * thisSpeed;

		if ((transform.position - movementTarget).magnitude < 0.01f)
			return true;
		return false;
	}
}
