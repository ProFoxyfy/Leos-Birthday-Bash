using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;

public class PauseMenuManager : MonoBehaviour
{
	public Canvas menu;
	private Transition menuTransition;
	public bool menuOpen = false;
	private InputAction pause;

	private void Awake()
	{
		pause = GameInputManager.Instance.GetAction("Pause");
		menuTransition = menu.GetComponentInChildren<Transition>();
		menuTransition.onPerform.AddListener(Unpause);
	}

	private void Unpause()
	{
		if (menuOpen)
		{
			menuOpen = false;
			menu.gameObject.SetActive(false);
			HUDManager.Instance.Enable();
			Time.timeScale = 1f;
		}
		else
		{
			menuOpen = true;
		}
	}

	private void Update()
	{
		if (!HUDManager.Instance.pauseEnabled)
		{
			if (menuOpen)
			{
				menuOpen = false;
				Unpause();
			}
			return;
		}
		HUDManager.Instance.blockGameInput = menu.gameObject.activeSelf;
		if (menuOpen || menu.gameObject.activeSelf) return;
		if (pause.WasPressedThisFrame())
		{
			Time.timeScale = 0f;
			menu.gameObject.SetActive(true);
			HUDManager.Instance.Disable();
			menuTransition.Perform(true);
		}
	}
}
