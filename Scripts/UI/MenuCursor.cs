using System;
using System.Collections.Generic;
using System.Globalization;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.InputSystem;
using UnityEngine.UI;

public class MenuCursor : MonoBehaviour
{
	private RectTransform rect;
	private Camera cam;
	private GraphicRaycaster gr;
	private CanvasScaler scaler;
	private EventSystem es;

	public IClickable lastObject;

	private bool mouseDown = false;
	private InputAction mouseAction;

	private bool cursorActive = true;

	public bool alternateMouseLock = false;

	private Sprite normalSprite;
	private Sprite handSprite;
	private Image img;
	private Vector2 actualPosition = Vector2.zero;

	private Tooltip tip;

	private RectTransform canvas;

	private Vector2 res;
	private Vector2 center;

	public bool hoveringOverElement;

	private float sensitivity = 1f;

	private void Awake()
	{
		normalSprite = GlobalsManager.Instance.style.cr_normal;
		handSprite = GlobalsManager.Instance.style.cr_hover;

		canvas = GetComponentInParent<Canvas>().GetComponent<RectTransform>();
		scaler = canvas.GetComponent<CanvasScaler>();
		cam = UICamera.Instance.main;
		res = CoreInitializer.uiResolution;

		mouseAction = GameInputManager.Instance.GetAction("Click");

		es = EventSystem.current;
		gr = FindAnyObjectByType<GraphicRaycaster>(FindObjectsInactive.Include);
		cam = UICamera.Instance.main;
		rect = GetComponent<RectTransform>();
		img = GetComponent<Image>();

		UpdateCenter();
		UnityEngine.Cursor.lockState = CursorLockMode.Locked;
		this.actualPosition = center;

		tip = FindAnyObjectByType<Tooltip>(FindObjectsInactive.Include);

		GlobalsManager.Instance.activeCursor = this;
	}

	private void Update()
	{
		if (!cursorActive) return;
		this.img.enabled = cursorActive;

		Vector2 delta = GameInputManager.Instance.GetAction("Mouse").ReadValue<Vector2>();
		this.actualPosition += delta * sensitivity;

		bool isLarge = (bool)FlagManager.Instance.GetSetting("largeCursor");
		rect.localScale = Vector2.one * (isLarge ? 1.5f : 1f);

		if (alternateMouseLock)
			this.actualPosition = new Vector2(Mathf.Clamp(this.actualPosition.x, 0, canvas.sizeDelta.x), Mathf.Clamp(this.actualPosition.y, -canvas.sizeDelta.y, 0));
		else
			this.actualPosition = new Vector2(Mathf.Clamp(this.actualPosition.x, 0, res.x), Mathf.Clamp(this.actualPosition.y, -res.y, 0));

		this.actualPosition = new Vector2(this.actualPosition.x, this.actualPosition.y);
		this.rect.anchoredPosition = new Vector2(Mathf.Round(actualPosition.x), Mathf.Round(actualPosition.y));
		this.rect.localPosition = new Vector3(this.rect.localPosition.x, this.rect.localPosition.y, 0f);
	}

	private void LateUpdate()
	{
		if (!cursorActive) return;
		UpdateCenter();
		mouseDown = mouseAction.IsPressed();

		PointerEventData data = new PointerEventData(es);
		data.position = cam.WorldToScreenPoint(this.rect.position);

		List<RaycastResult> results = new List<RaycastResult>();

		gr.Raycast(data, results);

		bool interactableFound = false;
		IClickable component = null;
		TooltipElement tooltipText = null;

		foreach (RaycastResult result in results)
		{
			if (result.gameObject.TryGetComponent<IClickable>(out component) || result.gameObject.TryGetComponent<TooltipElement>(out tooltipText))
			{
				interactableFound = true;
				break;
			}
		}

		if (results.Count > 0 && interactableFound)
		{
			GameObject hoveredObject = results[0].gameObject;
			//hoveredObject.TryGetComponent<IClickable>(out component);
			//hoveredObject.TryGetComponent<TooltipElement>(out tooltipText);
			if (component != null && component.isEnabled)
			{
				hoveringOverElement = true;
				lastObject = component;

				if (mouseDown && lastObject.state == ClickableState.Hover)
					lastObject.Down();
				else if (!mouseDown && lastObject.state == ClickableState.Down)
					lastObject.Up();
				else if (!mouseDown && lastObject.state == ClickableState.None)
					lastObject.Hover();
			}
			if (tooltipText != null && tip != null)
			{
				tip.show = true;
				tip.content = tooltipText.txt;
			}
		}
		else
		{
			if (tip != null)
				tip.show = false;
			hoveringOverElement = false;
			if (lastObject != null)
				lastObject.Unhover();
			lastObject = null;
		}

		img.sprite = hoveringOverElement && component != null && component.isHoverable ? handSprite : normalSprite;
		img.rectTransform.pivot = img.sprite.pivot / img.sprite.texture.height;
	}

	private void UpdateCenter()
	{
		center = new Vector2(res.x / 2, -res.y / 2);

		center = new Vector2(Mathf.FloorToInt(center.x), Mathf.FloorToInt(center.y));
		sensitivity = (float)FlagManager.Instance.GetSetting("cursorSensitivity");

		if ((bool)FlagManager.Instance.GetSetting("lowerSens"))
			sensitivity *= Convert.ToSingle(FlagManager.Instance.gameVars.variables["lowerSensMult"], CultureInfo.InvariantCulture);
	}

	public void SetCursorEnabled(bool value)
	{
		if (!value)
		{
			this.actualPosition = center;
			this.rect.position = this.actualPosition;
		}
		gameObject.SetActive(value);
	}

	public void SetCursorVisible(bool value)
	{
		if (!value)
		{
			this.actualPosition = center;
			this.rect.position = this.actualPosition;
		}
		this.cursorActive = value;
	}
}
