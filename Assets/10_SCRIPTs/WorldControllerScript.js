#pragma strict

/* funktion zum erstellen eines cubes, mit einer position */
function CreateCube (pos : Vector3) {
	var cube = GameObject.CreatePrimitive(PrimitiveType.Cube);
	//cube.AddComponent(Rigidbody);
	cube.transform.position = pos + Vector3(0,1,0);
	cube.transform.localScale = Vector3(10,10,10);
}

function Start () {
	// startpunkt der kollision
	var raySource : Vector3;
	
	// gruesse des platzieten objektes
	var cubeScale : float = 10;
	// minimaler abstand zwischen den objekten
	var cubeMinSpacing : float = 1;
	// range fuer die platzierung der objekte (-placingRange .. +placingRange)
	var placingRange : float = 250;
	// zuehler fuer anzahl platzierter objekte
	var cubeCount : int = 0;
	
	// 1000 mal
	for (var i=0; i<300; i++) {
		// bestimme ein zentrum fuer das objekt, zufaellig in x und y position (-placingRange .. +placingRange)
		raySource = Vector3(Random.Range(-placingRange, placingRange), 100, Random.Range(-placingRange, placingRange));
		// ist das resultat innerhalb des platzes wo das fahrzeug fährt
		if ((-50 < raySource.x && raySource.x < 50) && (-50 < raySource.z && raySource.z < 50)) {
			// fuehre die platzierung nicht aus
		} else {
			// pruefe ob auf 4 richtungen ob das objekt auf das terrain oder ein anderes objekt trifft
			// zentrum + (gruesse des objektes+abstand des objektes in x und z koordinate) als startpunkt des rays
			if (	PosCheck(raySource+Vector3(  cubeScale+cubeMinSpacing, 0,  cubeScale+cubeMinSpacing))
				&&	PosCheck(raySource+Vector3( -cubeScale+cubeMinSpacing, 0,  cubeScale+cubeMinSpacing))
				&&	PosCheck(raySource+Vector3(  cubeScale+cubeMinSpacing, 0, -cubeScale+cubeMinSpacing))
				&&	PosCheck(raySource+Vector3( -cubeScale+cubeMinSpacing, 0, -cubeScale+cubeMinSpacing))) {
					var pos : Vector3 = GetHeight(raySource);
					// platziere den cube an dieser position
					CreateCube(pos);
					cubeCount += 1;
			}

			// // pruefe ob auf 4 richtungen ob das objekt auf das terrain oder ein anderes objekt trifft
			// // zentrum + (gruesse des objektes+abstand des objektes in x und z koordinate) als startpunkt des rays
			// Physics.Raycast(raySource+Vector3(cubeScale+cubeMinSpacing,0,cubeScale+cubeMinSpacing), rayDirection, rayHit);
			// // mach nur weiter wenn wir auf das terrain treffen
			// if (rayHit.collider.gameObject.name == "Terrain") {
			// 	// wiederhole den schritt fuer
			// 	// -x, 0, z richtung
			// 	Physics.Raycast(raySource+Vector3(-cubeScale+cubeMinSpacing,0,cubeScale+cubeMinSpacing), rayDirection, rayHit);
			// 	if (rayHit.collider.gameObject.name == "Terrain") {
			// 		// wiederhole den schritt fuer
			// 		// x, 0, -z richtung
			// 		Physics.Raycast(raySource+Vector3(cubeScale+cubeMinSpacing,0,-cubeScale+cubeMinSpacing), rayDirection, rayHit);
			// 		if (rayHit.collider.gameObject.name == "Terrain") {
			// 			// wiederhole den schritt fuer
			// 			// -x, 0, -z richtung
			// 			Physics.Raycast(raySource+Vector3(-cubeScale+cubeMinSpacing,0,-cubeScale+cubeMinSpacing), rayDirection, rayHit);
			// 			if (rayHit.collider.gameObject.name == "Terrain") {
			// 				// mache einen ray um die hoehe im zentrum zu bestimmen
			// 				Physics.Raycast(raySource, rayDirection, rayHit);
			// 				if (rayHit.collider.gameObject.name == "Terrain") {
			// 					// platziere den cube an dieser position
			// 					CreateCube(rayHit.point);
			// 					cubeCount += 1;
			// 				}
			// 			}
			// 		}
			// 	}
			// }
		}
	}
	print("I: WorldControllerScript.Start: placed "+cubeCount+" Cubes");
	
}

function PosCheck (sourcePos : Vector3) : boolean {
	// richtung des rays
	var rayDirection : Vector3 = Vector3.down;
	// speicherplatz fuer das resultat der kollision
	var rayHit : RaycastHit;

	Physics.Raycast(sourcePos, rayDirection, rayHit);
	if (rayHit.collider.gameObject.name == "Terrain") {
		return true;
	}
	return false;
}

function GetHeight (sourcePos : Vector3) : Vector3 {
	// richtung des rays
	var rayDirection : Vector3 = Vector3.down;
	// speicherplatz fuer das resultat der kollision
	var rayHit : RaycastHit;

	Physics.Raycast(sourcePos, rayDirection, rayHit);
	if (rayHit.collider.gameObject.name == "Terrain") {
		return rayHit.point;
	}
	// gib einen wert zurück der nicht stoert, besser ware es sauber abzufangen
	return new Vector3(0,100,0);
}

function Update () {
}