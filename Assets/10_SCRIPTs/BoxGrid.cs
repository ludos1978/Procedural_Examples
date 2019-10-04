using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BoxGrid : MonoBehaviour {
	public int maxSizeX = 8;
	public int maxSizeY = 8;

	public int defaultValue = 0;

	public List<GameObject> prefabObjects;

	public List<List<int>> grid;

	public void Start() {
		Initalize();
		DrawLine(0, 0, 7, 0, 1);
		DrawLine(0, 0, 0, 7, 1);
		DrawLine(0, 7, 7, 7, 1);
		DrawLine(7, 0, 7, 7, 1);

		Noise();

		Print();
		Generate();
	}

	public void DrawLine (int sourceX, int sourceY, int targetX, int targetY, int value) {
		Debug.Log(" " + sourceX + " " + sourceY + " " + targetX + " " + targetY + " " + value);
		int diffX = targetX - sourceX;
		int diffY = targetY - sourceY;

		int steps = Mathf.Max(Mathf.Abs(diffX), Mathf.Abs(diffY));

		if (steps == 0)
			return;

		float stepX = (float)diffX / steps;
		float stepY = (float)diffY / steps;

		for (int s = 0; s <= steps; s++) {
			int posX = sourceX + (int)(s * stepX);
			int posY = sourceY + (int)(s * stepY);
			Debug.Log("set " + posX + " " + posY + " " + value+" "+stepX+" "+stepY);
			grid[posX][posY] = value;
		}
	}

	public void Noise () {
		float scale = 0.3f;
		for (int x = 0; x < maxSizeX; x++) {
            for (int y = 0; y < maxSizeY; y++) {
				float value = Mathf.PerlinNoise(x * scale, y * scale);
				grid[x][y] = Mathf.RoundToInt(value);
				//Debug.Log("val " + value);
            }
        }
	}

	public void Initalize() {
		grid = new List<List<int>>();
		for (int x = 0; x < maxSizeX; x++) {
			grid.Add(new List<int>());
			for (int y = 0; y < maxSizeY; y++) {
				grid[x].Add(defaultValue);
            }
		}
	}

	public void Print() {
		string log = "";
		for (int x = 0; x < maxSizeX; x++) {
            for (int y = 0; y < maxSizeY; y++) {
				log += ", " + grid[x][y];
            }
			log += "\n";
        }
		Debug.Log(log);
	}

	public void Generate () {
		for (int x = 0; x < maxSizeX; x++) {
            for (int y = 0; y < maxSizeY; y++) {
				int gridValue = grid[x][y];
				Instantiate(prefabObjects[gridValue], new Vector3(x,0,y), Quaternion.identity, transform);
            }
        }
	}
}
