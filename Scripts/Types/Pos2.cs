using System;
using UnityEngine;

[Serializable]
public struct Pos2
{
	public int x;
	public int y;

	public static Pos2 negativeOne = new Pos2(-1, -1);
	public static Pos2 one = new Pos2(1, 1);

	public Pos2(int x, int y)
	{
		this.x = x;
		this.y = y;
	}

	public Pos2(Vector3 vector)
	{
		this.x = Mathf.RoundToInt(vector.x);
		this.y = Mathf.RoundToInt(vector.y);
	}

	public Pos2(Vector2Int vector)
	{
		this.x = vector.x;
		this.y = vector.y;
	}

	public Vector3 ToVec3()
	{
		return new Vector3((float)x, (float)y);
	}

	public Vector2 ToVec2()
	{
		return new Vector2((float)x, (float)y);
	}

	public Vector2Int ToVec2Int()
	{
		return new Vector2Int(x, y);
	}

	public Vector3Int ToVec3Int()
	{
		return new Vector3Int(x, y);
	}

	public override string ToString()
	{
		return $"{this.x}, {this.y}";
	}

	public static bool operator !=(Pos2 left, Pos2 right) => left.x != right.x && left.y != right.y;
	public static bool operator ==(Pos2 left, Pos2 right) => left.x == right.x && left.y == right.y;

#nullable enable
	public override int GetHashCode()
	{
		return (this.x*53) + (this.y*13);
	}

	public override bool Equals(object obj)
	{
		if (obj == null || !(obj is Pos2))
			return false;
		return this.GetHashCode() == obj.GetHashCode();
	}
}