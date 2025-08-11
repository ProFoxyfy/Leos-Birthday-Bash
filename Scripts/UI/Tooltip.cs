using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class Tooltip : MonoBehaviour
{
	private RectTransform background;
	private TMP_Text text;
	private Image img;
	private RectTransform cursor;
	public bool show = false;
	[ReadOnly]
	public string content = "";

	private void Awake()
	{
		cursor = GlobalsManager.Instance.activeCursor.GetComponent<RectTransform>();
		background = GetComponent<RectTransform>();
		img = GetComponent<Image>();
		text = GetComponentInChildren<TMP_Text>();
	}

	private void Update()
	{
		bool isLarge = (bool)FlagManager.Instance.GetSetting("largeCursor");
		Vector2 offset = new Vector2(isLarge ? 20 : 14, isLarge ? 32 : 18);
		background.anchoredPosition = cursor.anchoredPosition - new Vector2(
			(-background.sizeDelta.x / 2f) - offset.x,
			(background.sizeDelta.y / 2f) + offset.y
		);

		Vector2 maxPosition = new(
			UIScaler.baseResolution.x - (background.sizeDelta.x / 2f),
			UIScaler.baseResolution.y - (background.sizeDelta.y / 2f)
		);
		background.anchoredPosition = new Vector2(
			Mathf.Clamp(background.anchoredPosition.x, 0, maxPosition.x),
			Mathf.Clamp(background.anchoredPosition.y, -maxPosition.y, 0)
		);
		Vector2 textSize = text.GetPreferredValues(text.text);
		background.sizeDelta = textSize + new Vector2(6, 4);
	}

	private void LateUpdate()
	{
		background.localPosition = new Vector3(background.localPosition.x, background.localPosition.y, show ? 0 : -9999);
		text.text = content;
	}
}
