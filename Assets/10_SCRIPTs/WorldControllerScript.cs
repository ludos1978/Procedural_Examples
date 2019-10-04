// Converted from UnityScript to C# at http://www.M2H.nl/files/js_to_c.php - by Mike Hergaarden
// Do test the code! You usually need to change a few small bits.

using UnityEngine;
using System.Collections;

public class WorldControllerScript : MonoBehaviour {


/* funktion zum erstellen eines cubes, mit einer position */
void  CreateCube ( Vector3 pos  ){
	GameObject cube= GameObject.CreatePrimitive(PrimitiveType.Cube);
	//cube.AddComponent<Rigidbody>();
	cube.transform.position = pos + new Vector3(0,1,0);
	cube.transform.localScale = new Vector3(10,10,10);
}

void  Start (){
	// startpunkt der kollision
	Vector3 raySource;
	
	// gruesse des platzieten objektes
	float cubeScale = 10;
	// minimaler abstand zwischen den objekten
	float cubeMinSpacing = 1;
	// range fuer die platzierung der objekte (-placingRange .. +placingRange)
	float placingRange = 250;
	// zuehler fuer anzahl platzierter objekte
	int cubeCount = 0;
	
	// 1000 mal
	for (int i=0; i<300; i++) {
		// bestimme ein zentrum fuer das objekt, zufaellig in x und y position (-placingRange .. +placingRange)
		raySource = new Vector3(Random.Range(-placingRange, placingRange), 100, Random.Range(-placingRange, placingRange));
		// ist das resultat innerhalb des platzes wo das fahrzeug fährt
		if ((-50 < raySource.x && raySource.x < 50) && (-50 < raySource.z && raySource.z < 50)) {
			// fuehre die platzierung nicht aus
		} else {
			// pruefe ob auf 4 richtungen ob das objekt auf das terrain oder ein anderes objekt trifft
			// zentrum + (gruesse des objektes+abstand des objektes in x und z koordinate) als startpunkt des rays
			if (	PosCheck(raySource+new Vector3(  cubeScale+cubeMinSpacing, 0,  cubeScale+cubeMinSpacing))
				&&	PosCheck(raySource+new Vector3( -cubeScale+cubeMinSpacing, 0,  cubeScale+cubeMinSpacing))
				&&	PosCheck(raySource+new Vector3(  cubeScale+cubeMinSpacing, 0, -cubeScale+cubeMinSpacing))
				&&	PosCheck(raySource+new Vector3( -cubeScale+cubeMinSpacing, 0, -cubeScale+cubeMinSpacing))) {
					Vector3 pos = GetHeight(raySource);
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

bool PosCheck ( Vector3 sourcePos  ){
	 // richtung des rays
	Vector3 rayDirection = Vector3.down;
	// speicherplatz fuer das resultat der kollision
	RaycastHit rayHit;

	Physics.Raycast(sourcePos, rayDirection, out rayHit);
	if (rayHit.collider.gameObject.name == "Terrain") {
		return true;
	}
	return false;
}

Vector3 GetHeight ( Vector3 sourcePos  ){
	// richtung des rays
	Vector3 rayDirection = Vector3.down;
	// speicherplatz fuer das resultat der kollision
	RaycastHit rayHit;

	Physics.Raycast(sourcePos, rayDirection, out rayHit);
	if (rayHit.collider.gameObject.name == "Terrain") {
		return rayHit.point;
	}
	// gib einen wert zurück der nicht stoert, besser ware es sauber abzufangen
	return new Vector3(0,100,0);
}

void  Update (){
}
}