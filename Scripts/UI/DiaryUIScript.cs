using UnityEngine;
using UnityEngine.InputSystem;

public class DiaryUIScript : MonoBehaviour
{
	InputAction clickAction;

	void Start()
	{
		clickAction = GameInputManager.Instance.GetAction("Click");
	}

    public void Hide()
    {
		HUDManager.Instance.Enable();
		HUDManager.Instance.blockGameInput = false;
		gameObject.SetActive(false);
    }

    void Update()
    {
		if (clickAction.WasPressedThisFrame())
			this.Hide();
    }
}
