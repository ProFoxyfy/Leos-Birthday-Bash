using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;

public class LevelEditorCamera : MonoBehaviour
{
	Camera cam;
	InputAction move;
	InputAction zoom;
	InputAction moveFaster;

	private void Awake()
	{
		move = GameInputManager.Instance.GetAction("Move");
		zoom = GameInputManager.Instance.GetAction("MouseWheel");
		moveFaster = GameInputManager.Instance.GetAction("Sprint");
		cam = UICamera.Instance.main;
	}

	private void Update()
	{
		Vector2 delta = Vector2.zero;
		bool fast = moveFaster.IsPressed();
		delta = move.ReadValue<Vector2>() * (fast ? 15f : 6f) * Time.deltaTime;
		cam.transform.position += new Vector3(delta.x, delta.y);
		cam.orthographicSize = Mathf.Clamp(cam.orthographicSize + (zoom.ReadValue<float>() * 0.1f), 0.8f, 15f);
		cam.transform.position = new Vector3(cam.transform.position.x, cam.transform.position.y, -1f);
	}
}
