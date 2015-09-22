private var carSteeringScript : CarSteeringScript;

function Start () {
	carSteeringScript = GetComponentInChildren(CarSteeringScript);
}

function Update () {
	var forwardForce = 30;
	var reverseForce = -20;
	var noForce = 0;
	if (Input.GetKey ("w")) {
		carSteeringScript.SetAcceleration(forwardForce);
	} else if (Input.GetKey ("s")) {
		carSteeringScript.SetAcceleration(reverseForce);
	} else if (Input.GetKey ("space")) {
		carSteeringScript.SetAcceleration(noForce);
	}

	turnAngle = 10;
	if (Input.GetKey("a")) {
		carSteeringScript.SetDirection(-turnAngle);
	} else if (Input.GetKey("d")) {
		carSteeringScript.SetDirection(turnAngle);
	}
}