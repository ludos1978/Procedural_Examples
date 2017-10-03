using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[System.Serializable]
public class IntegerPosition {
	[SerializeField]
	public int posX;
	[SerializeField]
	public int posY;

	public override string ToString() {
		return (posX+"/"+posY);
	}
}

public class Grid : MonoBehaviour {

	public List<List<int>> grid;
	[HideInInspector]
	public int sizeX = 8;
	[HideInInspector]
	public int sizeY = 8;

	public GameObject[] levelPrefabs;

	public List<GameObject> levelObjectInstances;

	/// <summary>
	/// initialize the array with 0's
	/// </summary>
	public void Initialize(int newSizeX, int newSizeY, int defaultValue = 0) {
		sizeX = newSizeX;
		sizeY = newSizeY;
		grid = new List<List<int>>();
		for (int x=0; x<sizeX; x++) {
			grid.Add(new List<int>());
			for (int y=0; y<sizeY; y++) {
				grid[x].Add(defaultValue);
			}
		}
	}

	/// <summary>
	/// return a string with the contents of the array
	/// </summary>
	public override string ToString() {
		string output = "";
		for (int x=0; x<sizeX; x++) {
			for (int y=0; y<sizeY; y++) {
				output += grid[x][y] + " ";
			}
			output += "\n";
		}
		return output;
	}

	/// <summary>
	/// check if x and y within the bounds of the array, if so set the value
	/// </summary>
	public void Set(int x, int y, int value) {
		if (((x >= 0) && (x < sizeX)) && ((y >= 0) && (y < sizeY))) {
			grid[x][y] = value;
		}
		else {
			Debug.LogError("Set: invalid input values "+x+" "+y+" "+value);
		}
	}
	public void Set(IntegerPosition pos, int value) {
		Set(pos.posX, pos.posY, value);
	}

	/// <summary>
	/// check if x and y within the bounds of the array, if so return the value, if not return -1
	/// </summary>
	public int Get(int x, int y) {
		if (((x >= 0) && (x < sizeX)) && ((y >= 0) && (y < sizeY))) {
			return grid[x][y];
		}
		else {
			Debug.LogError("Get: invalid input values "+x+" "+y);
			return -1;
		}
	}
	public int Get(IntegerPosition pos) {
		return Get(pos.posX, pos.posY);
	}

	/// <summary>
	/// Generates the level using the 3d models in the levelPrefabs array
	/// </summary>
	public void GenerateLevel() {
		// delete all preexisting instances of level elements
		if (levelObjectInstances != null) {
			for (int id=0; id<levelObjectInstances.Count; id++) {
				Destroy(levelObjectInstances[id]);
			}
		}
		levelObjectInstances = new List<GameObject>();

		// generate all level elements
		for (int x=0; x<sizeX; x++) {
			for (int y=0; y<sizeY; y++) {
				int id = Get(x,y);
				// haben wir genuegend modelle zum darstellen
				if ((id >= 0) && (id < levelPrefabs.Length)) {
					GameObject goPrefab = levelPrefabs[id];
					GameObject goInstance = Instantiate(goPrefab, new Vector3(x,0,y), Quaternion.identity, transform);
					goInstance.SetActive(true);
					levelObjectInstances.Add(goInstance);
				}
			}
		}
	}
}
