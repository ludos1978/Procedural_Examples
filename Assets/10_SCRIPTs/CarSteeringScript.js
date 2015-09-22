/* fahrzeugsteuerung
  setzt einige werte f�r die friction (reibung zwischen r�dern und boden) 
  bietet funktionen an damit das fahrzeug gesteuert (SetDirection) sowie beschleunigt (SetAcceleration) werden kann
  SetDirection in grad, sinnvollerweise werte zischen -45 (links) und 45 (rechts), (0 ist geradeaus)
  SetAcceleration als kraft, beim aktuellen fahrzeug werte zwischen 20 und 30 (vorw�rts) oder -20 und -30 (r�ckw�rts)
  */
var inputDirection : float = 0.0;
var inputAcceleration : float = 0.0;
var lastDirection : float;

private var body : GameObject;
private var blWheel : GameObject;
private var brWheel : GameObject;
private var flWheel : GameObject;
private var frWheel : GameObject;

function Start () {
	body = transform.Find("carBody").gameObject;
	blWheel = transform.Find("carBody/b-l-j").gameObject;
	print(blWheel);
	brWheel = transform.Find("carBody/b-r-j").gameObject;
	flWheel = transform.Find("carBody/f-l-j").gameObject;
	frWheel = transform.Find("carBody/f-r-j").gameObject;
	
	var wheelForwardFrictionExtremum = 10000;
	var wheelForwardFrictionAsymptote = 5000;
	body.GetComponent.<Rigidbody>().centerOfMass = Vector3(0,0,0);
	blWheel.GetComponent.<Collider>().forwardFriction.extremumValue = wheelForwardFrictionExtremum;
	brWheel.GetComponent.<Collider>().forwardFriction.extremumValue = wheelForwardFrictionExtremum;
	flWheel.GetComponent.<Collider>().forwardFriction.extremumValue = wheelForwardFrictionExtremum;
	frWheel.GetComponent.<Collider>().forwardFriction.extremumValue = wheelForwardFrictionExtremum;
	blWheel.GetComponent.<Collider>().sidewaysFriction.asymptoteValue = wheelForwardFrictionAsymptote;
	brWheel.GetComponent.<Collider>().sidewaysFriction.asymptoteValue = wheelForwardFrictionAsymptote;
	flWheel.GetComponent.<Collider>().sidewaysFriction.asymptoteValue = wheelForwardFrictionAsymptote;
	frWheel.GetComponent.<Collider>().sidewaysFriction.asymptoteValue = wheelForwardFrictionAsymptote;
	
	var wheelSidewaysFrictionExtremum = 3000;
	var wheelSidewaysFrictionAsymptote = 1500;
	blWheel.GetComponent.<Collider>().sidewaysFriction.extremumValue = wheelSidewaysFrictionExtremum;
	brWheel.GetComponent.<Collider>().sidewaysFriction.extremumValue = wheelSidewaysFrictionExtremum;
	flWheel.GetComponent.<Collider>().sidewaysFriction.extremumValue = wheelSidewaysFrictionExtremum;
	frWheel.GetComponent.<Collider>().sidewaysFriction.extremumValue = wheelSidewaysFrictionExtremum;
	blWheel.GetComponent.<Collider>().sidewaysFriction.asymptoteValue = wheelSidewaysFrictionAsymptote;
	brWheel.GetComponent.<Collider>().sidewaysFriction.asymptoteValue = wheelSidewaysFrictionAsymptote;
	flWheel.GetComponent.<Collider>().sidewaysFriction.asymptoteValue = wheelSidewaysFrictionAsymptote;
	frWheel.GetComponent.<Collider>().sidewaysFriction.asymptoteValue = wheelSidewaysFrictionAsymptote;
}

function SetDirection (inInputDirection : float) {
	inputDirection = inInputDirection;
}

function GetDirection () {
	return inputDirection;
}

function SetAcceleration (inInputAcceleration : float) {
	inputAcceleration = inInputAcceleration;
}

function LateUpdate () {
	lastDirection = inputDirection;
	
	blWheel.GetComponent.<Collider>().steerAngle = inputDirection;
	brWheel.GetComponent.<Collider>().steerAngle = inputDirection;
	inputDirection = 0.0;
	
	blWheel.GetComponent.<Collider>().motorTorque = inputAcceleration;
	brWheel.GetComponent.<Collider>().motorTorque = inputAcceleration;
	flWheel.GetComponent.<Collider>().motorTorque = inputAcceleration;
	frWheel.GetComponent.<Collider>().motorTorque = inputAcceleration;
	inputAcceleration = 0.0;	
}



