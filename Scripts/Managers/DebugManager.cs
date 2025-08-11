using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;

public static class DebugManager
{
	internal static List<string> log = new List<string>();
	internal static InputAction action;

	public static void Initialize()
	{
		log.Clear();
		action = GameInputManager.Instance.GetAction("DebugMenu");
	}

	public static void Log(string msg)
	{
		log.Add(msg);
	}

	public static void Render()
	{
		if (!Application.isEditor)
			return;
		if (!action.IsPressed())
		{
			Cursor.lockState = CursorLockMode.Locked;
			return;
		}
		Cursor.lockState = CursorLockMode.None;
		GUI.Box(new Rect(32, 32, 500, 200), "Debug Log");
		string str = "";

		for (int i = Mathf.Max(0, log.Count - 6); i < log.Count; i++)
		{
			str += log[i] + "\n";
		}

		GUI.Label(new Rect(32, 48, 500, 200), str);
	}
}
