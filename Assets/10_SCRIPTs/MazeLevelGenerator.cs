// by reto.spoerri@zhdk.ch
// MIT License

using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using System.Collections.ObjectModel;

[RequireComponent(typeof(MeshFilter))]
[RequireComponent(typeof(MeshRenderer))]

public class MazeLevelGenerator : MonoBehaviour {
	private int[][] level;
	private Collection<int[]> open = new Collection<int[]> ();
	private List<int> deadEndInt = new List<int> ();
	private List<int> roomInt = new List<int> ();
// temp variables
	private int x;
	private int y;
	private float zPos = 0;

	private int levelSizeX = 1;
	private int levelSizeY = 1;

// instance arrays
	private List<GameObject> models = new List<GameObject> ();
	public GameObject maze0000Prefab;
	public GameObject maze00X0Prefab;
	public GameObject maze0XX0Prefab;
	public GameObject maze0XXXPrefab;
	public GameObject mazeX0X0Prefab;
	public GameObject exitPrefab;

	void Update () {
	}

	void Start () {
		// level size must be uneven number 
		//SceneCreate (17, 17, 0);
	}

	public int[][] SceneCreate (int mazeSizeX, int mazeSizeY, int seed) {
        InitLevel (mazeSizeX, mazeSizeY);
		GenerateMaze (seed);
		GenerateRooms ();
		InstantiateMeshes ();

		// combine the mesh
        if (true) {
            CombineMeshes combineMeshes = GetComponent<CombineMeshes> ();
            combineMeshes.Combine (models);
        }

		// hide all models
		/*foreach (GameObject mdl in models) {
			mdl.SetActive(false);
		}*/

        for (int x = 0; x < levelSizeX; x++) {
			string s = "";
            for (int y= 0; y < levelSizeY; y++) {
				s += level[x][y]+" ";
			}
			Debug.Log(s);
		}

		return level;
	}
	
    public void SetWall (int x, int y, int value) {
        if ((x >= 0) && (x < level.Length) && (y >= 0) && (y < level [0].Length)) {
            level [x] [y] = value;
        }
    }

	public void SceneDestroy () {
		foreach (var model in models) {
			Destroy (model);
		}
		models = new List<GameObject> ();

		level = null;
		
		deadEndInt = new List<int> ();
		roomInt = new List<int> ();
		
	}
	
    public void InitLevel (int mazeSizeX, int mazeSizeY) {
        levelSizeX = mazeSizeX;
        levelSizeY = mazeSizeY;

		level = new int[levelSizeX][];
		for (x = 0; x < levelSizeX; x++) {
			level [x] = new int[levelSizeY];
			for (y = 0; y < levelSizeY; y++) {
				level [x] [y] = 1;
				// wall
			}
		}
	}

	// generate the level-data
	public void GenerateMaze (int seed) {
		// entry (level start)
		level [1] [1] = 0;
		// open
		Random.seed = seed;
    
		// open consists of array with [posX, posY, diffPosX, diffPosY]
		open.Add (new int[] { 2, 1, 1, 0 });
		open.Add (new int[] { 1, 2, 0, 1 });
		deadEndInt.Add (3 * 10000 + 1);
		deadEndInt.Add (1 * 10000 + 3);
		while (open.Count > 0) {
			// pick a random element from walls
			int index = Random.Range (0, open.Count - 1);
			//print("index: "+index);
			int[] location = open [index];
			open.RemoveAt (index);
      
			int[] newLocation = new int[] {
				location [0] + location [2],
				location [1] + location [3]
			};
			int[] currentWall = new int[] { location [0], location [1] };
			int[] oldLocation = new int[] {
				location [0] - location [2],
				location [1] - location [3]
			};
			if ((level [newLocation [0]] [newLocation [1]] == 1) || (Random.Range (0, 10) == 0)) {
				// not in the maze yet or 10% chance
				// mark as open
				level [currentWall [0]] [currentWall [1]] = 0;
				level [newLocation [0]] [newLocation [1]] = 0;
				deadEndInt.Remove (oldLocation [0] * 10000 + oldLocation [1]);
				// check all 4 adjacent positions for possible continuing
				var directions = new int[][] {
					new int[] { 0, 1 },
					new int[] { -1, 0 },
					new int[] {
						1,
						0
					},
					new int[] {
						0,
						-1
					}
				};
				foreach (int[] posDiff in directions) {
					int[] nextLocation = new int[] {
						newLocation [0] + posDiff [0],
						newLocation [1] + posDiff [1]
					};
					// if next location within area
					if (((nextLocation [0] > 0) && (nextLocation [0] < levelSizeX - 1)) && ((nextLocation [1] > 0) && (nextLocation [1] < levelSizeY - 1))) {
						// if next location is a wall
						if (level [nextLocation [0] + posDiff [0]] [nextLocation [1] + posDiff [1]] == 1) {
							open.Add (new int[] {
								nextLocation [0],
								nextLocation [1],
								posDiff [0],
								posDiff [1]
							});
							deadEndInt.Remove ((nextLocation [0] + posDiff [0]) * 10000 + (nextLocation [1] + posDiff [1]));
							deadEndInt.Add ((nextLocation [0] + posDiff [0]) * 10000 + (nextLocation [1] + posDiff [1]));
						}
					}
				}
			} else {
				// this is a dead end
			}
		}
		// open the level exit (level exit)
		level [levelSizeX - 2] [levelSizeY - 2] = 0;
		// open
		level [levelSizeX - 2] [levelSizeY - 1] = 0;
		// open
		// remove dead ends at exit
		deadEndInt.Remove ((levelSizeX - 1) * 10000 + (levelSizeY - 1));
		deadEndInt.Remove ((levelSizeX - 2) * 10000 + (levelSizeY - 1));
		deadEndInt.Remove ((levelSizeX - 1) * 10000 + (levelSizeY - 2));
		deadEndInt.Remove ((levelSizeX - 2) * 10000 + (levelSizeY - 2));
	}

	public void GenerateRooms () {
		int freePlaces = Mathf.FloorToInt ((levelSizeX * levelSizeY) / 10);
    
		while (freePlaces > 0) {
			int roomSizeX = Mathf.Max (1, Mathf.Min (4, (levelSizeX / 8))) * 2 + 1;
			//Random.Range(1, 3) * 2;
			int roomSizeY = Mathf.Max (1, Mathf.Min (4, (levelSizeY / 8))) * 2 + 1;
			//Random.Range(1, 3) * 2;
			int roomPosX = (Mathf.FloorToInt (Random.Range (1, levelSizeX - roomSizeX - 1) / 2) + 1) * 2 - 1;
			int roomPosY = (Mathf.FloorToInt (Random.Range (1, levelSizeY - roomSizeY - 1) / 2) + 1) * 2 - 1;
//      Debug.Log ("I: GenerateRooms: " + roomSizeX + " " + roomSizeY + " at " + roomPosX + " " + roomPosY);
			for (x = roomPosX; x < roomPosX + roomSizeX; x++) {
				for (y = roomPosY; y < roomPosY + roomSizeY; y++) {
					level [x] [y] = 0;
					// remove dead ends that are next to a room
					deadEndInt.Remove (x * 10000 + y);
					deadEndInt.Remove (x * 10000 + (y - 1));
					deadEndInt.Remove (x * 10000 + (y + 1));
					deadEndInt.Remove ((x - 1) * 10000 + y);
					deadEndInt.Remove ((x + 1) * 10000 + y);
					// add romm to roomPosition
					roomInt.Remove (x * 10000 + y);
					roomInt.Add (x * 10000 + y);
					freePlaces -= 1;
				}
			}
		}
	}

	public void InstantiateMeshes () {
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

	/*public void CombineMeshes () {
		MeshFilter[] meshFilters = GetComponentsInChildren<MeshFilter>();
		CombineInstance[] combine = new CombineInstance[meshFilters.Length];
		int i = 0;
		while (i < meshFilters.Length) {
			combine[i].mesh = meshFilters[i].sharedMesh;
			combine[i].transform = meshFilters[i].transform.localToWorldMatrix;
			meshFilters[i].gameObject.active = false;
			i++;
		}
		transform.GetComponent<MeshFilter>().mesh = new Mesh();
		transform.GetComponent<MeshFilter>().mesh.CombineMeshes(combine);
		transform.gameObject.active = true;
	}*/

	// get positions for pickups etc...
	public Collection<Vector3> GetEntryLocations () {
		Collection<Vector3> openSpaces = new Collection<Vector3> ();
		for (int x = 0; x < 4; x++) {
			for (int y = 0; y < 4; y++) {
				// is free && is not at startlocation && is not possibly far away to walk (3/3)
				if ((level [x] [y] == 0) && !((x == 1) && (y == 1)) && !((x == 3) && (y == 3))) {
					openSpaces.Add (GridToWorldPos (x, y));
				}
			}
		}
		return openSpaces;
	}

	public Collection<Vector3> GetRoomLocations () {
		Collection<Vector3> openSpaces = new Collection<Vector3> ();
		for (int rId=0; rId<roomInt.Count; rId++) {
			int location = roomInt [rId];
			int y = (int)(Mathf.Floor (location) % 10000.0f);
			int x = (int)(Mathf.FloorToInt (location / 10000.0f));
			openSpaces.Add (GridToWorldPos (x, y));
		}
		return openSpaces;
	}

	public Collection<Vector3> GetDeadEndLocations () {
		Collection<Vector3> openSpaces = new Collection<Vector3> ();
		for (int rId=0; rId<deadEndInt.Count; rId++) {
			int location = deadEndInt [rId];
			int y = (int)(Mathf.Floor (location) % 10000.0f);
			int x = (int)(Mathf.FloorToInt (location / 10000.0f));
			openSpaces.Add (GridToWorldPos (x, y));
		}
		return openSpaces;
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
