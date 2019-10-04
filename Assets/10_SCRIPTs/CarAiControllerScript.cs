using UnityEngine;
using System.Collections;
using UnityStandardAssets.Vehicles.Car;

public class CarAiControllerScript : MonoBehaviour {
/* ki steuerung mit 2 rays zum ausweichen von objekten */

enum STATE {
  CHASE,
  EVADE,
  ROAM
}

private STATE currentState = STATE.ROAM;

//private CarSteeringScript carSteeringScript;
private CarController carController;
private GameObject carBody;

float avgMoveSpeed = 50;

void  Start (){
	print ("I: CarAiControllerScript");

	// gib mir eine referenz auf des fahrzeugsteuerungsscript
	//carSteeringScript = GetComponentInChildren<CarSteeringScript>();
	carController = transform.GetComponentInChildren<CarController>();
	// setze eine zufaellig position
	//transform.position = Vector3(Random.Range(-50, 50),transform.position.y + Random.Range(0, 10),Random.Range(-50, 50));
	// eine refernez auf das fahrzeug (wo der rigidbody angemacht ist)
	carBody = transform.gameObject; //.Find("carBody").gameObject;
}

void  FixedUpdate (){
  bool flee= false;
  if (Input.GetKey ("i")) {
    flee = true;
  }
  bool chase= false;
  if (Input.GetKey ("o")) {
    chase = true;
  }
  bool roam= false;
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

void  Evade (){
}

void  Chase (){
}

void  Roam (){
	float acceleration 	= 0;
	float rotation 		= 0;
	float handbrake 		= 0;
	
	// startposition des raycasts
	Vector3 raySource;
	// richtung des raycasts
	Vector3 rayDirection;
	// laenge des raycasts
	float rayLength = 20.0f;
	
	// entfernung zum objekt auf das der linke strahl trifft
	// als standart wird ein wert verwendet der bei einer kollision niemals auftreten kann
	// (da der raycast mittels der variable rayLength auf die maximale laenge
	//   beschraenkt wird, kann kein graesserer wert auftreten)
	float distanceToLeftCollision = rayLength + 1;
	// die berechnete kollision von strahl wird hier gespeichert
	RaycastHit leftHit;
	// startpunkt des strahls
	// position des fahrzeugs + (umwandlungsmatrix von lokalkoordinaten in weltkooridnaten * vector aus der fahrzeugsicht)
	raySource = carBody.transform.position+(carBody.transform.TransformDirection(new Vector3(-.7f,.6f,0.8f)));
	// rixchtung des strahls
	// umwandlungsmatrix von lokalkoordinaten in weltkooridnaten * normalisierter richtungsvektor in fahrzeugkoodinaten
	rayDirection = carBody.transform.TransformDirection(new Vector3(-1,0,3).normalized);
	// zeichne eine linie die den raycast representiert
	Debug.DrawLine (raySource, raySource+rayDirection*rayLength);
	// aeberpraefe auf kollisionen
	if (Physics.Raycast (raySource, rayDirection, out leftHit, rayLength)) {
		
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
	float distanceToRightCollision = rayLength + 1;
	RaycastHit rightHit;
	raySource = carBody.transform.position + carBody.transform.TransformDirection(new Vector3(.7f,.6f,0.8f)); // * carBody.transform.localToWorldMatrix);
	rayDirection = carBody.transform.localToWorldMatrix * (new Vector3(1,0,3).normalized);
	Debug.DrawLine (raySource, raySource+rayDirection*rayLength);
	if (Physics.Raycast (raySource, rayDirection, out rightHit, rayLength)) {
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
	float turnAngle= 10;
	
	// ------- ZU WEIT WEG -----
		// wenn das fahrzeug weiter als 250 einheiten vom welt-zentrum entfernt ist
		if ((carBody.transform.position - new Vector3(0,carBody.transform.position.y,0)).magnitude > 250) {
			// berechne, aus der fahrzeugausrichtung und position, wo das zentrum der welt ist
			Vector3 centerOfWorldFromMyView = carBody.transform.InverseTransformPoint(0, 0, 0);

			// die x koorinate sagt mir ob das zentrum der welt links (negativ) oder rechts (positiv) von mir ist
			// schraenke diesen wert zwischen -10 und 10 ein (-54.2f wird zu -10, 123.0f wird zu 10.0f, 5.0f bleibt konstanz)
			turnAngle = Mathf.Min(10, Mathf.Max(-10, centerOfWorldFromMyView.x));
			// setze den wert als drehwinkel faer die raeder
			rotation = turnAngle;
		}
	// ------- ZU WEIT WEG -----
	
	// ------- AUSWEICHEN --------
		turnAngle = 10;
		// wenn der linke oder der rechte strahl eine kollision ausgelaest hat
		// (distanz der kollision ungleich dem standartwert von oben)
		if ((distanceToLeftCollision <= rayLength) || (distanceToRightCollision <= rayLength)) {
			// der kleinere wert von linker und rechter kollisionsentfernung
			float minHitDistance= Mathf.Min(distanceToLeftCollision, distanceToRightCollision);
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
	// ------- AUSWEICHEN --------
	
	// ------- RUECKWAERTS FAHREN BEI KOLLSION --------
		// berechne einen wert der die durchschnittsgeschwindigkeit repraesentiert
		avgMoveSpeed = (avgMoveSpeed * 0.9f) + (carBody.GetComponent<Rigidbody>().velocity.magnitude * 0.1f);
		
		/*if (carBody.transform.parent.gameObject.name == "carprefabPlayer") {
			Debug.Log("avgSpeed "+avgMoveSpeed+" vel "+carBody.GetComponent.<Rigidbody>().velocity.magnitude);
		}*/
		// ist die durchschnittsgeschwindigkeit sehr klein aber nicht negativ
		// (nach ungefaehr 3 sekunden gestoppt trifft dies zu)
		// 
		if ((avgMoveSpeed < 0.05f) && (avgMoveSpeed > 0)) {
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
		if (avgMoveSpeed < 0.0f) {
			// drehe die raeder in die andere richtung
			rotation = -rotation;
		}
	// ------- RUECKWAERTS FAHREN BEI KOLLSION --------
	
	carController.Move(rotation, acceleration, acceleration, handbrake);
}
}