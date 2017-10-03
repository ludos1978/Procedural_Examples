using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GridMain : MonoBehaviour {

	public Grid grid;

	public bool loadFromTexture = false;
	public Texture2D tex;

	// Use this for initialization
	void Awake () {
		grid.Initialize(16, 16);

		Debug.Log(grid);

		grid.Set(3,5,1);

		Debug.Log(grid);

		for ( int x=0; x<grid.sizeX; x++) {
			grid.Set(x, 0, 1);
		}

		grid.GenerateLevel();

		grid.Set(0,5,1);

		grid.GenerateLevel();

		if (loadFromTexture) {
			for (int x=0; x<tex.width; x++) {
				for (int y=0; y<tex.height; y++) {
					bool pixelState = (tex.GetPixel(x,y).r > 0.5f);
					if (pixelState)
						grid.Set(x,y, 1);
					else
						grid.Set(x,y, 0);
				}
			}
		}

		grid.GenerateLevel();
	}
	
	// Update is called once per frame
	void Update () {
		
	}
}
