// by reto.spoerri@zhdk.ch
// MIT License

using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using System.Collections.ObjectModel;

[RequireComponent(typeof(MeshFilter))]
[RequireComponent(typeof(MeshRenderer))]
[RequireComponent(typeof(CombineMeshes))]
public class MazeMeshGenerator : MonoBehaviour {
    // level data
	private int[][] level;

    [HideInInspector]
	public int levelSizeX = 1;
    [HideInInspector]
    public int levelSizeY = 1;

    private float zPos = 0;

// instance arrays
	private List<GameObject> models = new List<GameObject> ();
	public GameObject maze0000Prefab;
	public GameObject maze00X0Prefab;
	public GameObject maze0XX0Prefab;
	public GameObject maze0XXXPrefab;
	public GameObject mazeX0X0Prefab;
	public GameObject exitPrefab;

    public void InitLevel (int mazeSizeX, int mazeSizeY) {
        levelSizeX = mazeSizeX;
        levelSizeY = mazeSizeY;

        level = new int[levelSizeX][];
        for (int x = 0; x < levelSizeX; x++) {
            level [x] = new int[levelSizeY];
            for (int y = 0; y < levelSizeY; y++) {
                level [x] [y] = 1;
                // wall
            }
        }
    }

    public int GetWall (int x, int y) {
        if (level == null) {
            Debug.Log ("MazeLEvelGenerator.GetWall: level not initialized");
            return -1;
        }

        if ((x >= 0) && (x < level.Length) && (y >= 0) && (y < level [0].Length)) {
            return level [x] [y];
        }
        return -1;
    }

    public void SetWall (int x, int y, int value) {
        if (level == null) {
            Debug.Log ("MazeLEvelGenerator.SetWall: level not initialized");
            return;
        }

        if ((x >= 0) && (x < level.Length) && (y >= 0) && (y < level [0].Length)) {
            level [x] [y] = value;
        }
    }

    public string ToString () {
        string logMessage = "";
        for (int x = 0; x < levelSizeX; x++) {
            for (int y= 0; y < levelSizeY; y++) {
                logMessage += level[x][y]+" ";
            }
            logMessage += "\n";
        }
        return logMessage;
    }

    public void GenerateMeshes () {
        if (level == null) {
            Debug.Log ("MazeLEvelGenerator.GenerateLevel: level not initialized");
            return;
        }
        InstantiateMeshes ();

        // combine the mesh
        if (true) {
            CombineMeshes combineMeshes = GetComponent<CombineMeshes> ();
            combineMeshes.Combine (models);
        }
    }

	public void SceneDestroy () {
		foreach (var model in models) {
			Destroy (model);
		}
		models = new List<GameObject> ();
		//level = null;
	}
	
	private void InstantiateMeshes () {
        // destroy old level
        if (models != null) {
            SceneDestroy ();
        }

        // make new clean object list
        models = new List<GameObject> ();

        Debug.Log ("MazeLevelGenerator.InstantiateMeshes");
		Quaternion rot = Quaternion.identity;
		GameObject levelElement = null;
		GameObject fxLayerLevelElement = null;
		for (int x = 0; x < levelSizeX - 1; x++) {
			for (int y = 0; y < levelSizeY - 1; y++) {
				int level00 = level [x] [y];
				int level01 = level [x] [y + 1];
				int level10 = level [x + 1] [y];
				int level11 = level [x + 1] [y + 1];
        
				rot = Quaternion.identity;
				switch (level00 + level01 + level10 + level11) {
				case 0:
					rot.eulerAngles = new Vector3 (270, 0, 0);
					levelElement = (GameObject)Instantiate (maze0000Prefab, new Vector3 ((y - 1) * 2, zPos, (x - 1) * 2), rot);
					break;
				case 1:
					if (level00 != 0) {
						rot.eulerAngles = new Vector3 (270, 180, 0);
					} else if (level01 != 0) {
						rot.eulerAngles = new Vector3 (270, 90, 0);
					} else if (level10 != 0) {
						rot.eulerAngles = new Vector3 (270, 270, 0);
					} else if (level11 != 0) {
						rot.eulerAngles = new Vector3 (270, 0, 0);
					} else {
						print ("E: unhandled case 1 rotation" + level00 + " " + level01 + " " + level10 + " " + level11);
					}
					levelElement = (GameObject)Instantiate (maze00X0Prefab, new Vector3 ((y - 1) * 2, zPos, (x - 1) * 2), rot);
					break;
				case 2:
					if (level00 != 0 && level01 != 0) {
						rot.eulerAngles = new Vector3 (270, 90, 0);
					} else if (level01 != 0 && level11 != 0) {
						rot.eulerAngles = new Vector3 (270, 0, 0);
					} else if (level11 != 0 && level10 != 0) {
						rot.eulerAngles = new Vector3 (270, 270, 0);
					} else if (level10 != 0 && level00 != 0) {
						rot.eulerAngles = new Vector3 (270, 180, 0);
					} else {
//            print("E: unhandled case 2 rotation: "+level00+" "+level01+" "+level10+" "+level11);
						if (level00 != 0 && level11 != 0) {
							rot.eulerAngles = new Vector3 (270, 0, 0);
						} else if (level01 != 0 && level10 != 0) {
							rot.eulerAngles = new Vector3 (270, 90, 0);
						} else {
							print ("E: unhandled case 2 rotation: " + level00 + " " + level01 + " " + level10 + " " + level11);
						}
						levelElement = (GameObject)Instantiate (maze0XX0Prefab, new Vector3 ((y - 1) * 2, zPos, (x - 1) * 2), rot);
						break;
					}
					levelElement = (GameObject)Instantiate (mazeX0X0Prefab, new Vector3 ((y - 1f) * 2f, zPos, (x - 1f) * 2f), rot);
					break;
				case 3:
					if (level00 != 0 && level01 != 0 && level10 != 0) {
						rot.eulerAngles = new Vector3 (270, 270, 0);
					} else if (level01 != 0 && level10 != 0 && level11 != 0) {
						rot.eulerAngles = new Vector3 (270, 90, 0);
					} else if (level10 != 0 && level11 != 0 && level00 != 0) {
						rot.eulerAngles = new Vector3 (270, 0, 0);
					} else if (level11 != 0 && level00 != 0 && level01 != 0) {
						rot.eulerAngles = new Vector3 (270, 180, 0);
					} else {
						print ("E: unhandled case 3 rotation" + level00 + " " + level01 + " " + level10 + " " + level11);
					}
					levelElement = (GameObject)Instantiate (maze0XXXPrefab, new Vector3 ((y - 1) * 2, zPos, (x - 1) * 2), rot);
					break;
				}
				levelElement.transform.parent = transform;
				levelElement.gameObject.isStatic = true;
				models.Add (levelElement);
				// duplicate element on layer 1
			}
		}
		rot = Quaternion.identity;
		// create model
		rot.eulerAngles = new Vector3 (270, 270, 0);
		levelElement = (GameObject)Instantiate (exitPrefab, new Vector3 ((levelSizeY - 1) * 2 + 7, zPos, (levelSizeX - 1) * 2 - 5), rot);
		levelElement.transform.parent = transform;
		models.Add (levelElement);
		// create model
		rot.eulerAngles = new Vector3 (270, 180, 0);
		levelElement = (GameObject)Instantiate (exitPrefab, new Vector3 ((levelSizeY - 1) * 2 + 3, zPos - 3, (levelSizeX - 1) * 2 + 11), rot);
		levelElement.transform.parent = transform;
		models.Add (levelElement);
		// create model
		rot.eulerAngles = new Vector3 (270, 90, 0);
		levelElement = (GameObject)Instantiate (exitPrefab, new Vector3 ((levelSizeY - 1) * 2 - 13, zPos - 6, (levelSizeX - 1) * 2 + 7), rot);
		levelElement.transform.parent = transform;
		models.Add (levelElement);
		// create model
		rot.eulerAngles = new Vector3 (270, 0, 0);
		levelElement = (GameObject)Instantiate (exitPrefab, new Vector3 ((levelSizeY - 1) * 2 - 9, zPos - 9, (levelSizeX - 1) * 2 - 9), rot);
		levelElement.transform.parent = transform;
		models.Add (levelElement);
	}

	// converter functions for location's
	public int[] WorldToGridPos (Vector3 worldPos) {
		return (new int[] {
			(int)(Mathf.Round (((worldPos.z + 1) / 2.0f) + 1)),
			(int)(Mathf.Round (((worldPos.x + 1) / 2.0f) + 1))
		});
	}

	public Vector3 GridToWorldPos (int gridX, int gridY) {
		return (new Vector3 ((gridY - 1) * 2 - 1, 0, (gridX - 1) * 2 - 1));
	}

	public int GetLevel (Vector3 worldPos) {
		int[] pos = WorldToGridPos (worldPos);
		return (level [pos [0]] [pos [1]]);
	}

	public bool CheckExited (Vector3 worldPos) {
		int[] gridPos = WorldToGridPos (worldPos);
		if ((gridPos [0] > levelSizeX) || (gridPos [1] > levelSizeY)) {
			return true;
		}
		return false;
	}
  
}
