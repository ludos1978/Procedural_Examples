using UnityEngine;
using System.Collections;
using System.Collections.Generic;

// neuer typ mit 2 vector informationen
[System.Serializable]
public class PosAndDir {
    public Vector2 pos; // x, y position speichern
    public Vector2 dir; // -1,0,1 / -1,0,1   , z.Bsp links = -1,0 , rechts = 1,0 , oben = 0,1 , unten = 0,-1
}


public class MazeImageGenerate : MonoBehaviour {

    Color openPixel = Color.white;
    Color closedPixel = Color.black;

    public Texture2D tex;
    public Renderer renderer;
    public float octA = 4;
    public float octB = 2;
    public float octC = 1;
    public float scale = 1f;
    public float basePosX = 0;
    public float basePosY = 0;
    public AnimationCurve curve;

    public bool doFindDoors = false;

    public List<PosAndDir> posAndDirList = new List<PosAndDir> ();
    public Color doorColor = new Color(1, 0.8f, 0.8f, 1);

    MazeLevelGenerator mazeLevelGen;

	// Use this for initialization
	void Start () {
        tex = new Texture2D (40, 30);
        tex.filterMode = FilterMode.Point;
        renderer.material.SetTexture ("_MainTex", tex);
        //GenerateNoise ();

        mazeLevelGen = GetComponent<MazeLevelGenerator>();
        mazeLevelGen.InitLevel (41, 31);

        // aktualisiere alle sekunden
        //StartCoroutine (Coroutine ());

        GenerationStep ();
	}
	
    IEnumerator Coroutine () {
        while (true) {
            GenerationStep ();
            yield return new WaitForSeconds(1);
        }
    }

    void GenerationStep () {
        SetAllWalls ();
        //GenerateRoom ();
        GenerateNoise ();
        if (doFindDoors) {
            for (int c = 0; c < 5; c++) {
                FindPossibleDoors ();
                GenerateCorridor ();
            }
        }

        for (int x = 0; x < tex.width; x++) {
            for (int y = 0; y < tex.height; y++) {
                mazeLevelGen.SetWall (x, y, Mathf.RoundToInt(tex.GetPixel(x,y).grayscale));
            }
        }
        mazeLevelGen.InstantiateMeshes ();
    }

    void SetAllWalls () {
        for (int x = 0; x < tex.width; x++) {
            for (int y = 0; y < tex.height; y++) {
                tex.SetPixel (x, y, closedPixel);
            }
        }
        tex.Apply ();
    }

    public int corridorSeed = 13;
    void GenerateCorridor () {
        if (posAndDirList.Count <= 0)
            return;
        
        Random.seed = corridorSeed;
        int index = Random.Range (0, posAndDirList.Count);
        PosAndDir currentPosAndDir = posAndDirList [index];
        posAndDirList.RemoveAt (index);

        Debug.Log ("MazeImageGenerate.GenerateCorridor: " + currentPosAndDir.pos + " " + currentPosAndDir.dir);

        int length = Random.Range (5, 8);
        for (int i = 0; i < length; i++) {
            Vector2 digPosition = currentPosAndDir.pos + currentPosAndDir.dir * i;
            tex.SetPixel ((int)digPosition.x, (int)digPosition.y, openPixel);
        }
        tex.Apply ();
    }

    void GenerateRoom () {
        Random.seed = 0;
        int sizeX = Random.Range (3, 7);
        int sizeY = Random.Range (4, 6);
        int posX = 1;
        int posY = 1;

        Color c = Color.white;
        for (int x = posX; x < posX + sizeX; x++) {
            for (int y = posY; y < posY + sizeY; y++) {
                tex.SetPixel (x, y, openPixel);
            }
        }
        tex.Apply ();
    }

    void GenerateNoise () {
        // verhindere den absturz des scripts (zero division error)
        float sum = (octA + octB + octC);
        if (sum == 0) {
            return;
        }

        
        Color c = Color.white;
        for (int x = 0; x < tex.width; x++) {
            for (int y = 0; y < tex.height; y++) {
                // perlin noise generieren
                float v = Mathf.PerlinNoise(basePosX+x/16f*scale, basePosY+y/16f*scale) * octA/sum +
                    Mathf.PerlinNoise(basePosX+x/8f*scale, basePosY+y/8f*scale) * octB/sum +
                    Mathf.PerlinNoise(basePosX+x/4f*scale, basePosY+y/4f*scale) * octC/sum;

                // farbkorrektur
                v = curve.Evaluate(v);

                // farbe auf textur anwenden
                c = new Color (v, v, v, 1);
                tex.SetPixel (x, y, c);
            }
        }

        tex.Apply ();
    }

    void FindPossibleDoors () {
        posAndDirList = new List<PosAndDir> ();
        
        for (int x = 1; x < tex.width-1; x++) {
            for (int y = 1; y < tex.height-1; y++) {
                // offene position auf (x,y)     und     geschlossene position auf (x+1, y)
                if (GetPositionOpen (new Vector2 (x, y)) && !GetPositionOpen(new Vector2(x+1, y)) && 
                    GetPositionOpen (new Vector2 (x, y+1)) && !GetPositionOpen(new Vector2(x+1, y+1)) && 
                    GetPositionOpen (new Vector2 (x, y-1)) && !GetPositionOpen(new Vector2(x+1, y-1)) ) {
                    PosAndDir curPosAndDir = new PosAndDir () {
                        pos = new Vector2 (x, y),
                        dir = new Vector2 (1, 0)
                    };
                    posAndDirList.Add (curPosAndDir);
                }
                if (GetPositionOpen (new Vector2 (x, y)) && !GetPositionOpen(new Vector2(x-1, y)) && 
                    GetPositionOpen (new Vector2 (x, y+1)) && !GetPositionOpen(new Vector2(x-1, y+1)) && 
                    GetPositionOpen (new Vector2 (x, y-1)) && !GetPositionOpen(new Vector2(x-1, y-1))) {
                    PosAndDir curPosAndDir = new PosAndDir () {
                        pos = new Vector2 (x, y),
                        dir = new Vector2 (-1, 0)
                    };
                    posAndDirList.Add (curPosAndDir);
                }
                if (GetPositionOpen (new Vector2 (x, y)) && !GetPositionOpen(new Vector2(x, y+1)) && 
                    GetPositionOpen (new Vector2 (x+1, y)) && !GetPositionOpen(new Vector2(x+1, y+1)) && 
                    GetPositionOpen (new Vector2 (x-1, y)) && !GetPositionOpen(new Vector2(x-1, y+1))) {
                    PosAndDir curPosAndDir = new PosAndDir () {
                        pos = new Vector2 (x, y),
                        dir = new Vector2 (0, 1)
                    };
                    posAndDirList.Add (curPosAndDir);
                }
                if (GetPositionOpen (new Vector2 (x, y)) && !GetPositionOpen(new Vector2(x, y-1)) && 
                    GetPositionOpen (new Vector2 (x+1, y)) && !GetPositionOpen(new Vector2(x+1, y-1)) && 
                    GetPositionOpen (new Vector2 (x-1, y)) && !GetPositionOpen(new Vector2(x-1, y-1))) {
                    PosAndDir curPosAndDir = new PosAndDir () {
                        pos = new Vector2 (x, y),
                        dir = new Vector2 (0, -1)
                    };
                    posAndDirList.Add (curPosAndDir);
                }
            }
        }

        // faerbe moegliche tuerpositionen rot ein
        foreach (PosAndDir posAndDir in posAndDirList) {
            tex.SetPixel ((int)posAndDir.pos.x, (int)posAndDir.pos.y, doorColor);
        }
        tex.Apply ();
    }

    bool GetPositionOpen(Vector2 pos) {
        if ((pos.x >= 0) && (pos.y >= 0) && (pos.x < tex.width) && (pos.y < tex.height)) {
            return (tex.GetPixel ((int)pos.x, (int)pos.y).grayscale > 0.5f);
        }
        return false;
    }
}
