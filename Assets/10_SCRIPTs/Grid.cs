using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Grid : MonoBehaviour {

	List<List<int>> grid;
	public int sizeX = 8;
	public int sizeY = 8;

	public void Initialize() {
		grid = new List<List<int>>();
		for (int x=0; x<sizeX; x++) {
			grid.Add(new List<int>());
			for (int y=0; y<sizeY; y++) {
				grid[x].Add(0);
			}
		}
	}

	public override string ToString() {
		string output = "";
		for (int x=0; x<sizeX; x++) {
			for (int y=0; y<sizeY; y++) {
				output += grid[x][y] + " ";
			}
			output += "\n";
		}
		return output;
	}

	public void Set(int x, int y, int value) {

		if (((x >= 0) && (x < sizeX)) && ((y >= 0) && (y < sizeY))) {
			grid[x][y] = value;
		}
		else {
			Debug.LogError("invalid input values "+x+" "+y+" "+value);
		}
	}
}
