using UnityEngine;
using System.Collections;
using System.Collections.ObjectModel;
using System.Collections.Generic;

public class MazeGenerator : MonoBehaviour {
    private int[][] level;
    private Collection<int[]> open = new Collection<int[]> ();
    private List<int> deadEndInt = new List<int> ();
    private List<int> roomInt = new List<int> ();

    MazeLevelGenerator levelGenerator;

    private int levelSizeX = 1;
    private int levelSizeY = 1;

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

    public void Clear () {
        deadEndInt = new List<int> ();
        roomInt = new List<int> ();
    }
        
	// Use this for initialization
	void Start () {
        levelGenerator = GetComponent<MazeLevelGenerator> ();
        InitLevel (15, 15);
        GenerateMaze (0);
        GenerateRooms ();

        levelGenerator.InitLevel (15, 15);
        for (int x = 0; x < levelSizeX; x++) {
            for (int y = 0; y < levelSizeY; y++) {
                levelGenerator.SetWall (x, y, level[x][y]);
            }
        }
        levelGenerator.InstantiateMeshes ();
	}
	
	// Update is called once per frame
	void Update () {
	
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
            for (int x = roomPosX; x < roomPosX + roomSizeX; x++) {
                for (int y = roomPosY; y < roomPosY + roomSizeY; y++) {
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


    // get positions for pickups etc...
    public Collection<Vector3> GetEntryLocations () {
        Collection<Vector3> openSpaces = new Collection<Vector3> ();
        for (int x = 0; x < 4; x++) {
            for (int y = 0; y < 4; y++) {
                // is free && is not at startlocation && is not possibly far away to walk (3/3)
                if ((level [x] [y] == 0) && !((x == 1) && (y == 1)) && !((x == 3) && (y == 3))) {
                    openSpaces.Add (levelGenerator.GridToWorldPos (x, y));
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
            openSpaces.Add (levelGenerator.GridToWorldPos (x, y));
        }
        return openSpaces;
    }

    public Collection<Vector3> GetDeadEndLocations () {
        Collection<Vector3> openSpaces = new Collection<Vector3> ();
        for (int rId=0; rId<deadEndInt.Count; rId++) {
            int location = deadEndInt [rId];
            int y = (int)(Mathf.Floor (location) % 10000.0f);
            int x = (int)(Mathf.FloorToInt (location / 10000.0f));
            openSpaces.Add (levelGenerator.GridToWorldPos (x, y));
        }
        return openSpaces;
    }

}

