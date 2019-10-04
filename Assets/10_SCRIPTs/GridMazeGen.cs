using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GridMazeGen : MonoBehaviour {

	public Grid grid;

	public List<IntegerPosition> openPositions;

	void Start () {
		Regenerate();
	}

	public bool generating = false;

	void Update () {
		// generate visual 3d model
		if (Input.GetKeyDown(KeyCode.Space)) {
			grid.GenerateLevel();
		}

        // neues labyrinth starten
		if (Input.GetKeyDown(KeyCode.Return)) {
			Regenerate();
		}

		// generierungsprozess starten & stoppen
		if (Input.GetKeyDown(KeyCode.Backspace)) {
			generating = !generating;
		}

		if (generating) {
			GenerateStep();
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

		//int failsafeCounter = 0;
		//while ((openPositions.Count > 0) && (failsafeCounter < 2000)) {
		//	failsafeCounter += 1;
		//	GenerateStep();
		//}

		//grid.GenerateLevel();
	}

	bool IsClosed(IntegerPosition currentPos) {
		int p0 = grid.Get(currentPos + new IntegerPosition( 0,  2));
		int p1 = grid.Get(currentPos + new IntegerPosition( 2,  0));
		int p2 = grid.Get(currentPos + new IntegerPosition( 0, -2));
		int p3 = grid.Get(currentPos + new IntegerPosition(-2,  0));
		return (
			((p0 == 0) || (p0 == -1)) &&
			((p1 == 0) || (p1 == -1)) &&
			((p2 == 0) || (p2 == -1)) &&
			((p3 == 0) || (p3 == -1)));
	}

	void GenerateStep () {
        int currentPositionIndex = Random.Range(0, openPositions.Count); // later should be a random value
        IntegerPosition currentPos = openPositions[currentPositionIndex];

		if (IsClosed(currentPos)) {
            openPositions.RemoveAt(currentPositionIndex);
			Debug.Log("is closed " + currentPositionIndex);
			return;
        }

		int randomDirection = Random.Range(0, 4); // gives 0 .. 3

        // besser waere eine liste von moeglichen wegen zu generieren und davon einen zufaellig auszuwaehlen...
        IntegerPosition randomDirectionOffset = new IntegerPosition() { posX = 0, posY = 0 };
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
            posX = currentPos.posX + randomDirectionOffset.posX,
            posY = currentPos.posY + randomDirectionOffset.posY
        };
        IntegerPosition nextPointPos = new IntegerPosition() {
            posX = currentPos.posX + randomDirectionOffset.posX * 2,
            posY = currentPos.posY + randomDirectionOffset.posY * 2
        };

        Debug.Log("checking " + nextPointPos);

        int middlePoint = grid.Get(middlePointPos);
        int nextPoint = grid.Get(nextPointPos);

        if (nextPoint == 1) {
            // feld frei
            grid.Set(nextPointPos, 0);
            grid.Set(middlePointPos, 0);

			openPositions.Add(nextPointPos);
        }
	}
}
