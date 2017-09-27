using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GridMain : MonoBehaviour {

	public Grid grid;

	// Use this for initialization
	void Start () {
		grid.Initialize();

		Debug.Log(grid);

		grid.Set(3,5,1);

		Debug.Log(grid);

		grid.Set(11,-5,3);
		
		Debug.Log(grid);
	}
	
	// Update is called once per frame
	void Update () {
		
	}
}
