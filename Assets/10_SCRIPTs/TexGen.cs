using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TexGen : MonoBehaviour {

	public Texture2D tex;
	public Material mat;

	// Use this for initialization
	void Update () {
		tex = new Texture2D(64, 64);
		for (int x=0; x<64; x++) {
			for (int y=0; y<64; y++) {
				float v = Mathf.PerlinNoise(x*0.03f+Time.time*0.1f, y*0.03f+Time.time*0.03f);
				tex.SetPixel(x, y, new Color(v,v,v));
			}
		}
		tex.Apply();
		mat.mainTexture = tex;
	}
}
