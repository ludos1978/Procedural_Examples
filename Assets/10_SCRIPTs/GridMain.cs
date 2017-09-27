using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GridMain : MonoBehaviour {

	public Grid grid;

	// Use this for initialization
	void Awake () {
		grid.Initialize();

		Debug.Log(grid);

		grid.Set(3,5,1);

		Debug.Log(grid);

		for ( int x=0; x<grid.sizeX; x++) {
			grid.Set(x, 0, 1);
		}

		grid.GenerateLevel();

		grid.Set(0,5,1);

		grid.GenerateLevel();
	}
	
	// Update is called once per frame
	void Update () {
		
	}
}
