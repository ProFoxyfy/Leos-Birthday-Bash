using System;
using System.Collections.Generic;

[Serializable]
public class TileData
{
    public Dictionary<Direction, bool> walls = new Dictionary<Direction, bool>();
}
