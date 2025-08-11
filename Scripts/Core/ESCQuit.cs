using UnityEngine;
using UnityEngine.InputSystem;

public class ESCQuit : MonoBehaviour
{
	InputAction quitAction;

    void Awake()
    {
        quitAction = GameInputManager.Instance.GetAction("Pause");
    }

    void Update()
    {
        if (quitAction.IsPressed())
		{
			Application.Quit();
		}
    }
}
