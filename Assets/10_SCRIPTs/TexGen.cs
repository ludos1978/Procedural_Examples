using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TexGen : MonoBehaviour {

	public Texture2D tex;
	public Material mat;

	// Use this for initialization
	void Start () {
		tex = new Texture2D(512, 512);
		for (int x=0; x<512; x++) {
			for (int y=0; y<512; y++) {
				float v = Mathf.PerlinNoise(x*0.03f, y*0.03f);
				tex.SetPixel(x, y, new Color(v,v,v));
			}
		}
		tex.Apply();
		mat.mainTexture = tex;
	}
}
