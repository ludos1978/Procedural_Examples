using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SetUvOffset : MonoBehaviour {

	public Material uvMat1;
	public Vector2 uvOffset1;
	public Material uvMat2;
	public Vector2 uvOffset2;
	
	// Update is called once per frame
	void Update () {
		string materialSlotname = "_MainTex";
		uvMat1.SetTextureOffset(materialSlotname, uvMat1.GetTextureOffset(materialSlotname) + uvOffset1 * Time.deltaTime);
		uvMat2.SetTextureOffset(materialSlotname, uvMat2.GetTextureOffset(materialSlotname) + uvOffset2 * Time.deltaTime);
	}
}
