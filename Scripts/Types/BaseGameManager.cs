using UnityEngine;
using static UnityEngine.Rendering.DebugUI.MessageBox;

public abstract class BaseGameManager : MonoBehaviour
{
	internal GameObject balloonPrefab;
	internal EnvironmentController ec;
	public LevelStyle style;

	internal void SpawnBalloons()
	{
		int count = 30;

		for (int i = 0; i < count; i++)
		{
			Vector3 pos = ec.GetRandomNavigableTile().position;

			// Magic numbers go brrrr
			pos.y = 0.4f;

			// Balloon? Baboon?
			GameObject clone = Instantiate(balloonPrefab, pos, Quaternion.identity, transform);
		}
	}
	internal void StyleLevel(bool hideCeilings = false, bool useWallMask = false)
	{
		// Defensive programming is a must in 2025
		// Thank God I made this a habit
		if (style == null) return;

		foreach (MeshRenderer renderer in ec.GetComponentsInChildren<MeshRenderer>(true))
		{
			if (renderer.gameObject.name.Contains("Floor"))
				renderer.material.SetTexture("_MainTex", style.floor);
			else if (renderer.gameObject.name.Contains("Ceiling"))
			{
				if (hideCeilings)
				{
					// The ceiling can go eat my shorts
					renderer.enabled = false;
					continue;
				}

				renderer.material.SetTexture("_MainTex", style.ceiling);
			}
			else
			{
				if (renderer.gameObject.name.Contains("EndingInput") || !renderer.material.shader.name.Contains("Tile") || renderer.material.name.Contains("Furniture"))
					// If it's anything but a wall, then we mind our business
					continue;
				renderer.material.SetTexture("_MainTex", style.wall);
				if (useWallMask)
					// This is used only once. I don't care.
					renderer.material.SetTexture("_MaskTex", style.wallMask);
			}
		}
	}

	public abstract void Broadcast(string msg);
	public abstract void Init();
	public abstract void OnObjectiveCollect(ObjectiveController objective);
	public abstract void OnGameTriggerEnter(TriggerType type, int id, TriggerController trigger);
	public abstract void OnTriggerCreate(TriggerType type, int id, TriggerController trigger);
	public abstract bool CanBlockExit();
	public abstract void HandleMarker(int id, Pos2 position);
}
