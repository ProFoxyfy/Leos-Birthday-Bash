using System;
using UnityEngine;

[Serializable]
public struct Color3
{
	public float r;
	public float g;
	public float b;

	public Color3(float r, float g, float b)
	{
		this.r = r;
		this.g = g;
		this.b = b;
	}

	public Color3(Color color)
	{
		this.r = color.r;
		this.g = color.g;
		this.b = color.b;
	}

	public readonly Color ToColor()
	{
		return new Color(r, g, b, 1f);
	}
}