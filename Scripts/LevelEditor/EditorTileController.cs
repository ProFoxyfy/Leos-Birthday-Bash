using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EditorTileController : MonoBehaviour
{
	private Dictionary<Direction, GameObject> wallSprites = new Dictionary<Direction, GameObject>();
	public TileData data;

	private void Awake()
	{
		data = new TileData();
		wallSprites[Direction.North] = transform.Find("N").gameObject;
		wallSprites[Direction.South] = transform.Find("S").gameObject;
		wallSprites[Direction.West] = transform.Find("W").gameObject;
		wallSprites[Direction.East] = transform.Find("E").gameObject;
	}

	public void SetWallEnabled(Direction direction, bool val)
	{
		data.walls[direction] = val;
	}

	private void LateUpdate()
	{
		foreach (KeyValuePair<Direction, bool> wall in data.walls)
		{
			wallSprites[wall.Key].SetActive(wall.Value);
		}
	}
}