using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GridMazeGen : MonoBehaviour {

	public Grid grid;

	List<IntegerPosition> openPositions;

	void Start () {
		Regenerate();
	}

	void Update () {
		if (Input.GetKeyDown(KeyCode.Space)) {
			Regenerate();
		}
	}

	// Use this for initialization
	void Regenerate () {
		grid.Initialize(33,	33, 1);

		openPositions = new List<IntegerPosition>();

		// IntegerPosition intPos = new IntegerPosition();
		// intPos.posX = 1;
		// intPos.posY = 1;
		IntegerPosition startPos = new IntegerPosition() {posX=15, posY=15};
		openPositions.Add( startPos );
		grid.Set(startPos, 0);

		int failsafeCounter =0;
		while ((openPositions.Count > 0) && (failsafeCounter < 200)) {
			failsafeCounter += 1;

			int currentPositionIndex = Random.Range(0, openPositions.Count); // later should be a random value
			IntegerPosition currentPos = openPositions[currentPositionIndex];



			IntegerPosition randomDirectionOffset = new IntegerPosition() {posX=0, posY=0};
			int randomDirection = Random.Range(0,4); // gives 0 .. 3
			switch (randomDirection) {
				case 0: // rechts , x+1, y
					randomDirectionOffset.posX = 1;
					break;
				case 1: // unten , x, y-1
					randomDirectionOffset.posY = -1;
					break;
				case 2: // rechts , x-1, y
					randomDirectionOffset.posX = -1;
					break;
				case 3: // rechts , x, y+1
					randomDirectionOffset.posY = 1;
					break;
			}

			IntegerPosition middlePointPos = new IntegerPosition() {
				posX=currentPos.posX + randomDirectionOffset.posX, 
				posY=currentPos.posY + randomDirectionOffset.posY};
			IntegerPosition nextPointPos = new IntegerPosition() {
				posX=currentPos.posX + randomDirectionOffset.posX * 2, 
				posY=currentPos.posY + randomDirectionOffset.posY * 2};
			
			Debug.Log("checkign "+nextPointPos);

			int middlePoint = grid.Get(middlePointPos);
			int nextPoint = grid.Get(nextPointPos);

			if (nextPoint == 1) {
				// feld frei
				grid.Set(nextPointPos, 0);
				grid.Set(middlePointPos, 0);

				openPositions.Add(nextPointPos);

				openPositions.RemoveAt(currentPositionIndex);
			}
		}

		grid.GenerateLevel();
	}
}
