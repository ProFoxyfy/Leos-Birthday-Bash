using UnityEngine;
using UnityEngine.UI;

public static class UIScaler
{
	public static Vector2 baseResolution = new Vector2(480, 360);
	public static float aspectRatio = baseResolution.x / baseResolution.y;

	public static float GetScale()
	{
		float ratio = GetRatioY();

		float closestWhole = Mathf.FloorToInt(ratio);
		// Scale down by one because it is absolutely
		// fucking guaranteed that we are going to end
		// up on a scale too large. Unless the user
		// decides that a larger UI is better.
		closestWhole = Mathf.Max(1, closestWhole - 1);
		if ((bool)FlagManager.Instance.GetSetting("largeUi"))
			closestWhole += 0.5f;

		return (float)closestWhole;
	}

	public static float GetRatioY()
	{
		float yRatio = Screen.height / baseResolution.y;

		return yRatio;
	}
}