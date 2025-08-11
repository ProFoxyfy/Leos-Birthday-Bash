using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Security.Cryptography;
using TMPro;
using UnityEngine;

public class ResolutionSelector : BaseUIInput
{
	public Button next;
	public Button prev;
	[ReadOnly]
	public int value;
	public TMP_Text display;
	private void Awake()
	{
		value = (int)FlagManager.Instance.GetSetting("resolution");
		UpdateDisplay();

		next.onClick.AddListener(Next);
		prev.onClick.AddListener(Prev);
	}

	private void Next()
	{
		value++;
		UpdateDisplay();
	}

	private void Prev()
	{
		value--;
		UpdateDisplay();
	}

	private void UpdateDisplay()
	{
		if (value >= GlobalsManager.Instance.resolutions.Count)
			value = 0;
		else if (value < 0)
			value = GlobalsManager.Instance.resolutions.Count - 1;

		Vector2 res = Vector2Helper.ResToVec2(GlobalsManager.Instance.resolutions[value]);
		FlagManager.Instance.SetSetting("resolution", value);
		Screen.SetResolution((int)res.x, (int)res.y, Screen.fullScreen);
		display.text = string.Concat(res.x.ToString(), "x", res.y.ToString());
	}
}