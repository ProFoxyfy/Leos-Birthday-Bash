using System;
using System.Collections.Generic;

[Serializable]
public class LevelData
{
	public TileData[,] tiles = new TileData[100, 100];
	public List<LevelObject> objects = new List<LevelObject>();
	public List<LevelMarker> markers = new List<LevelMarker>();
}