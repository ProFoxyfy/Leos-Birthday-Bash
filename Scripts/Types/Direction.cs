using System;
using System.Collections.Generic;
using UnityEngine;

[Serializable]
public enum Direction
{
	North,
	South,
	West,
	East
}

public static class DirectionData
{
	public static readonly Dictionary<Direction, Vector2> dirVectors = new Dictionary<Direction, Vector2>()
	{
		[Direction.North] = Vector2.up,
		[Direction.South] = Vector2.down,
		[Direction.West] = Vector2.left,
		[Direction.East] = Vector2.right,
	};
	public static readonly Dictionary<Direction, Vector2> dirVectorsSwapped = new Dictionary<Direction, Vector2>()
	{
		[Direction.North] = Vector2.up,
		[Direction.South] = Vector2.down,
		[Direction.West] = Vector2.right,
		[Direction.East] = Vector2.left,
	};
	public static readonly Dictionary<Direction, Vector3> dirVectorsFurniture = new Dictionary<Direction, Vector3>()
	{
		[Direction.North] = Vector3.zero,
		[Direction.South] = new Vector3(0f, 2f, 0f),
		[Direction.West] = new Vector3(0f, -1f, 0),
		[Direction.East] = new Vector3(0f, 1f, 0f)
	};
	public static readonly Dictionary<Direction, string> dirShortNames = new Dictionary<Direction, string>()
	{
		[Direction.North] = "N",
		[Direction.South] = "S",
		[Direction.West] = "W",
		[Direction.East] = "E",
	};
	public static readonly Dictionary<Direction, string> dirShortNamesInverse = new Dictionary<Direction, string>()
	{
		[Direction.North] = "S",
		[Direction.South] = "N",
		[Direction.West] = "E",
		[Direction.East] = "W",
	};
	public static readonly Dictionary<string, Direction> nameToDir = new Dictionary<string, Direction>()
	{
		["N"] = Direction.North,
		["S"] = Direction.South,
		["W"] = Direction.West,
		["E"] = Direction.East
	};
}