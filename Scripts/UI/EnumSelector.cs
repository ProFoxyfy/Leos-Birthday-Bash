using System;
using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;

public class EnumSelector : BaseUIInput
{
	public Button next;
	public Button prev;
	[ReadOnly]
	public int value;
	public TMP_Text display;

	private void Awake()
	{
		value = (int)FlagManager.Instance.GetSetting("language");
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
		if (value >= (int)Language.Null)
			value = 0;
		else if (value < 0)
			value = ((int)Language.Null) - 1;

		FlagManager.Instance.SetSetting("language", (Language)this.value);
		display.text = ((Language)this.value).ToString();
	}
}