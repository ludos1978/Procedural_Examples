using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Grid : MonoBehaviour {

	List<List<int>> grid;
	public int sizeX = 8;
	public int sizeY = 8;

	// Use this for initialization
	void Start () {
		Initialize();
	}

	void Initialize() {
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
}
