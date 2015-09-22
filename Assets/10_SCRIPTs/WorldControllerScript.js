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
	// richtung des rays
	var rayDirection : Vector3 = Vector3.down;
	// speicherplatz f�r das resultat der kollision
	var rayHit : RaycastHit;
	
	// gr�sse des platzieten objektes
	var cubeScale : float = 5;
	// minimaler abstand zwischen den objekten
	var cubeMinSpacing : float = 1;
	// range f�r die platzierung der objekte (-placingRange .. +placingRange)
	var placingRange : float = 250;
	// z�hler f�r anzahl platzierter objekte
	var cubeCount : int = 0;
	
	// 1000 mal
	for (var i=0; i<1000; i++) {
		// bestimme ein zentrum f�r das objekt, zuf�llig in x und y position (-placingRange .. +placingRange)
		raySource = Vector3(Random.Range(-placingRange, placingRange), 100, Random.Range(-placingRange, placingRange));
		// ist das resultat innerhalb des platzer wo fahrzeuge positioniert werden
		if ((-50 < raySource.x && raySource.x < 50) && (-50 < raySource.z && raySource.z < 50)) {
			// f�hre die platzierung nicht aus
		} else {
			// pr�fe ob auf 4 richtungen ob das objekt auf das terrain oder ein anderes objekt trifft
			// zentrum + (gr�sse des objektes+abstand des objektes in x und z koordinate) als startpunkt des rays
			Physics.Raycast(raySource+Vector3(cubeScale+cubeMinSpacing,0,cubeScale+cubeMinSpacing), rayDirection, rayHit);
			// mach nur weiter wenn wir auf das terrain treffen
			if (rayHit.collider.gameObject.name == "Terrain") {
				// wiederhole den schritt f�r
				// -x, 0, z richtung
				Physics.Raycast(raySource+Vector3(-cubeScale+cubeMinSpacing,0,cubeScale+cubeMinSpacing), rayDirection, rayHit);
				if (rayHit.collider.gameObject.name == "Terrain") {
					// wiederhole den schritt f�r
					// x, 0, -z richtung
					Physics.Raycast(raySource+Vector3(cubeScale+cubeMinSpacing,0,-cubeScale+cubeMinSpacing), rayDirection, rayHit);
					if (rayHit.collider.gameObject.name == "Terrain") {
						// wiederhole den schritt f�r
						// -x, 0, -z richtung
						Physics.Raycast(raySource+Vector3(-cubeScale+cubeMinSpacing,0,-cubeScale+cubeMinSpacing), rayDirection, rayHit);
						if (rayHit.collider.gameObject.name == "Terrain") {
							// mache einen ray um die h�he im zentrum zu bestimmen
							Physics.Raycast(raySource, rayDirection, rayHit);
							if (rayHit.collider.gameObject.name == "Terrain") {
								// platziere den cube an dieser position
								CreateCube(rayHit.point);
								cubeCount += 1;
							}
						}
					}
				}
			}
		}
	}
	print("I: WorldControllerScript.Start: placed "+cubeCount+" Cubes");
	
}

function Update () {
}