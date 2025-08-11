using UnityEngine;

public static class Vector2Helper
{
	public static Vector2Int ToVec2Int(Vector2 vector)
	{
		return new Vector2Int((int)vector.x, (int)vector.y);
	}

	public static Vector3 ToVec3(Vector2 vector)
	{
		return new Vector3(vector.x, vector.y);
	}

	public static Vector3 ToVec3YX(Vector2 vector)
	{
		return new Vector3(vector.y, vector.x);
	}

	public static Vector3 ToVec3nYX(Vector2 vector)
	{
		return new Vector3(-vector.y, vector.x);
	}

	public static Vector2 Vec3ToVec2(Vector3 vector)
	{
		return (Vector2)vector;
	}

	public static Vector2 ResToVec2(Resolution res)
	{
		return new Vector2(res.width, res.height);
	}
}
