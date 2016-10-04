using UnityEngine;
using System.Collections;

public class OffsetShader : MonoBehaviour {

	// Use this for initialization
	void Start () {
	
	}
	
	// Update is called once per frame
	void Update () {
        GetComponent<Renderer> ().sharedMaterial.SetTextureOffset ("_MainTex", Vector2.one * Time.time);
	}
}
