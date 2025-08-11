using System;

[Serializable]
public class LevelObject
{
	public Pos2 position;
	public ObjectType type;
	public Color3 color;
	public float range;
	public Direction dir;
	public int id;
	public string npc;
}