using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TileController : MonoBehaviour
{
	public Color lightColor = Color.white;
	private EnvironmentController ec;
	private int mask;
	private Vector3 xyVector = new Vector3(1, 1, 0);
	public Vector2Int tilePos;
	public LightData lightData;

	private void Awake()
	{
		mask = LayerMask.GetMask("Walls");
		ec = EnvironmentController.Instance;

		foreach (Transform t in GetComponentsInChildren<Transform>())
		{
			if (t == transform) continue;

			t.localScale += xyVector * 0.0005f;

			if (t.name.Contains("Wall"))
				t.gameObject.layer = LayerMask.NameToLayer("Walls");
			else if (t.name == "Ceiling")
				t.gameObject.layer = LayerMask.NameToLayer("Ceiling");
			else if (t.name == "Floor")
				t.gameObject.layer = LayerMask.NameToLayer("Floor");
		}
	}

	public void UpdateTileLighting()
	{
		this.lightColor = Color.black;

		for (int i = 0; i < lightData.lights.Count; i++)
		{
			if (!lightData.lights[i].visible) continue;
			switch (ec.lightBlendMode)
			{
				case (LightBlendMode.Greatest):
					this.lightColor.r = Mathf.Max(this.lightColor.r, lightData.lights[i].lightColor.r * (1f - Mathf.Clamp01((float)lightData.distances[i] / (float)lightData.lights[i].lightRange)));
					this.lightColor.g = Mathf.Max(this.lightColor.g, lightData.lights[i].lightColor.g * (1f - Mathf.Clamp01((float)lightData.distances[i] / (float)lightData.lights[i].lightRange)));
					this.lightColor.b = Mathf.Max(this.lightColor.b, lightData.lights[i].lightColor.b * (1f - Mathf.Clamp01((float)lightData.distances[i] / (float)lightData.lights[i].lightRange)));
					break;
				case (LightBlendMode.Additive):
					this.lightColor += lightData.lights[i].lightColor * (1f - Mathf.Clamp01((float)lightData.distances[i] / (float)lightData.lights[i].lightRange));
					break;
				case (LightBlendMode.Cumulative):
					this.lightColor += lightData.lights[i].lightColor * (1f - Mathf.Clamp01((float)lightData.distances[i] / (float)lightData.lights[i].lightRange)) * (Color.white - this.lightColor);
					break;
			}
		}

		this.lightColor.r = Mathf.Lerp(this.ec.ambientColor.r, 1f, this.lightColor.r);
		this.lightColor.g = Mathf.Lerp(this.ec.ambientColor.g, 1f, this.lightColor.g);
		this.lightColor.b = Mathf.Lerp(this.ec.ambientColor.b, 1f, this.lightColor.b);
	}
}
