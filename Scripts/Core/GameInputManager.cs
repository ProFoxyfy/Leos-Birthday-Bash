using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;

public class GameInputManager : Singleton<GameInputManager>
{
	public Dictionary<string, InputAction> actions = new Dictionary<string, InputAction>();
	public InputActionAsset playerInput;
	private InputActionMap gameMap;

	private void Awake()
	{
		DontDestroyOnLoad(this);
		playerInput = Resources.Load<InputActionAsset>("DefaultInput");
		gameMap = playerInput.FindActionMap("Game");
		foreach (InputAction action in gameMap.actions)
		{
			actions.Add(action.name, action);
		}
		gameMap = playerInput.FindActionMap("UI");
		foreach (InputAction action in gameMap.actions)
		{
			actions.Add(action.name, action);
		}
	}

	private void Update()
	{
		Cursor.lockState = CursorLockMode.Locked;
		Cursor.visible = false;
	}

	public InputAction GetAction(string name)
	{
		return actions[name];
	}
}
