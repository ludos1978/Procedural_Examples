/* ki steuerung mit 2 rays zum ausweichen von objekten */
import UnityStandardAssets.Vehicles.Car;

enum STATE {
  CHASE,
  EVADE,
  ROAM
}
private var currentState : STATE = STATE.ROAM;

//private var carSteeringScript : CarSteeringScript;
private var carController : CarController;
private var carBody : GameObject;

var avgMoveSpeed : float = 50;

function Start () {
	print ("I: CarAiControllerScript");

	// gib mir eine referenz auf des fahrzeugsteuerungsscript
	//carSteeringScript = GetComponentInChildren(CarSteeringScript);
	carController = transform.GetComponentInChildren(CarController);
	// setze eine zufaellig position
	//transform.position = Vector3(Random.Range(-50, 50),transform.position.y + Random.Range(0, 10),Random.Range(-50, 50));
	// eine refernez auf das fahrzeug (wo der rigidbody angemacht ist)
	carBody = transform.gameObject; //.Find("carBody").gameObject;
}

function FixedUpdate () {
  var flee = false;
  if (Input.GetKey ("i")) {
    flee = true;
  }
  var chase = false;
  if (Input.GetKey ("o")) {
    chase = true;
  }
  var roam = false;
  if (Input.GetKey ("p")) {
    roam = true;
  }


  switch (currentState) {
    case STATE.ROAM:
      Roam();
      if (flee) {
        currentState = STATE.EVADE;
      }
      if (chase) {
        currentState = STATE.CHASE;
      }
      break;

    case STATE.CHASE:
      Chase();
      if (roam) {
        currentState = STATE.ROAM;
      }
      break;


    case STATE.EVADE:
      Evade();
      // in welchen zustand maechte ich wechseln
      if (chase) {
        currentState = STATE.CHASE;
      }
      break;
	}
}

function Evade () {
}

function Chase () {
}

function Roam () {
	var acceleration : float 	= 0;
	var rotation : float 		= 0;
	var handbrake : float 		= 0;
	
	
	// startposition des raycasts
	var raySource : Vector3;
	// richtung des raycasts
	var rayDirection : Vector3;
	// laenge des raycasts
	var rayLength : float = 20.0;
	
	// entfernung zum objekt auf das der linke strahl trifft
	// als standart wird ein wert verwendet der bei einer kollision niemals auftreten kann
	// (da der raycast mittels der variable rayLength auf die maximale laenge
	//   beschraenkt wird, kann kein graesserer wert auftreten)
	var distanceToLeftCollision : float = rayLength + 1;
	// die berechnete kollision von strahl wird hier gespeichert
	var leftHit : RaycastHit;
	// startpunkt des strahls
	// position des fahrzeugs + (umwandlungsmatrix von lokalkoordinaten in weltkooridnaten * vector aus der fahrzeugsicht)
	raySource = carBody.transform.position+(carBody.transform.TransformDirection(Vector3(-.7,.6,0.8)));
	// richtung des strahls
	// umwandlungsmatrix von lokalkoordinaten in weltkooridnaten * normalisierter richtungsvektor in fahrzeugkoodinaten
	rayDirection = carBody.transform.TransformDirection(Vector3(-1,0,3).normalized);
	// zeichne eine linie die den raycast representiert
	Debug.DrawLine (raySource, raySource+rayDirection*rayLength);
	// aeberpraefe auf kollisionen
	if (Physics.Raycast (raySource, rayDirection, leftHit, rayLength)) {
		
		if (leftHit.collider.gameObject.name == "Terrain") {
			// wenn der strahl auf das terrain trifft, ignoriere die kollision
		} else {
			// wenn der strahl auf ein objekt trifft, speichere die distanz zom objekt
			distanceToLeftCollision = leftHit.distance;
			// zeichne eine rote linie zwischen start und kollisionspunkt
			Debug.DrawLine (raySource, leftHit.point, Color.red);
		}
	}
	
	// alles wie oben, nur faer einen zweiten strahl der nach rechts geht
	var distanceToRightCollision : float = rayLength + 1;
	var rightHit : RaycastHit;
	raySource = carBody.transform.position+carBody.transform.localToWorldMatrix * Vector3(.7,.6,0.8);
	rayDirection = carBody.transform.localToWorldMatrix * (Vector3(1,0,3).normalized);
	Debug.DrawLine (raySource, raySource+rayDirection*rayLength);
	if (Physics.Raycast (raySource, rayDirection, rightHit, rayLength)) {
		if (rightHit.collider.gameObject.name == "Terrain") {
			// skip terrain collision
		} else {
			distanceToRightCollision = rightHit.distance;
			Debug.DrawLine (raySource, rightHit.point, Color.red);
		}
	}
	
	// setze die raeder auf geradeaus
	rotation = 0;
	// setze eine standartbeschleunigung
	acceleration = 30;
	
	// die standart einschlagstaerke der raeder
	var turnAngle = 10;
	
	// wenn das fahrzeug weiter als 250 einheiten vom welt-zentrum entfernt ist
	if ((carBody.transform.position - Vector3(0,carBody.transform.position.y,0)).magnitude > 250) {
		// berechne, aus der fahrzeugausrichtung und position, wo das zentrum der welt ist
		var centerOfWorldFromMyView = carBody.transform.InverseTransformPoint(0, 0, 0);
		// die x koorinate sagt mir ob das zentrum der welt links (negativ) oder rechts (positiv) von mir ist
		// schraenke diesen wert zwischen -10 und 10 ein (-54.2 wird zu -10, 123.0 wird zu 10.0, 5.0 bleibt konstanz)
		turnAngle = Mathf.Min(10, Mathf.Max(-10, centerOfWorldFromMyView.x));
		// setze den wert als drehwinkel faer die raeder
		rotation = turnAngle;
	}
	
	turnAngle = 10;
	// wenn der linke oder der rechte strahl eine kollision ausgelaest hat
	// (distanz der kollision ungleich dem standartwert von oben)
	if ((distanceToLeftCollision != rayLength + 1) || (distanceToRightCollision != rayLength + 1)) {
		// der kleinere wert von linker und rechter kollisionsentfernung
		var minHitDistance = Mathf.Min(distanceToLeftCollision, distanceToRightCollision);
		// ist die kollisionsentfernung klein
		if (minHitDistance < 5) {
			// erhaehe den einschlagwinkel der raeder
			turnAngle = 20;
		// ist die kollisionsentfernung sehr klein
		} else if (minHitDistance < 1) {
			// erhaehe den einschlagwinkel der raeder noch mehr
			turnAngle = 40;
		}
		
		// ist die linke kollision naeher als die rechte
		if (distanceToLeftCollision < distanceToRightCollision) {
			// drehe nach links, mit dem einschlagwinkel
			rotation = turnAngle;
		// ansonsten (die rechte kollision ist naeher als die linke)
		} else {
			// drehe nach rechts mit dem einschlagwinkel
			rotation = -turnAngle;
		}
	}
	
	// berechne einen wert der die durchschnittsgeschwindigkeit repraesentiert
	avgMoveSpeed = (avgMoveSpeed * 0.9) + (carBody.GetComponent.<Rigidbody>().velocity.magnitude * 0.1);
	
	/*if (carBody.transform.parent.gameObject.name == "carprefabPlayer") {
		Debug.Log("avgSpeed "+avgMoveSpeed+" vel "+carBody.GetComponent.<Rigidbody>().velocity.magnitude);
	}*/
	// ist die durchschnittsgeschwindigkeit sehr klein aber nicht negativ
	// (nach ungefaehr 3 sekunden gestoppt trifft dies zu)
	// 
	if ((avgMoveSpeed < 0.05) && (avgMoveSpeed > 0)) {
		// setze die durchschnittsgeschwindigkeit als negativ
		avgMoveSpeed = -1000;
	}
	// ist die durchschnittsgeschwindigkeit negativ
	// (dies wurde durch die vorherige if-abfrage ausgelaest)
	if (avgMoveSpeed < 0) {
		// bewege dich raeckwaerts
		acceleration = -30;
	}
	// anmerkung:
	// die negative durchschnittsgeschwindigkeit wird nach einigen sekunden wieder 
	//   positiv (das weil carBody.rigidbody.velocity.magnitude immer positiv ist, 
	//     siehe erste berechnung von avgMoveSpeed)
	// in diesem moment setzt die forwaertsbewegung wieder ein
	
	// wenn wir uns raeckwaerts bewegen muss der einschlag der raeder in die andere richtung gehen
	// kennen wir zbsp aus dem einparkieren ohne servolenkung
	if (avgMoveSpeed < 0.0) {
		// drehe die raeder in die andere richtung
		rotation = -rotation;
	}
	
	carController.Move(rotation, acceleration, acceleration, handbrake);

}