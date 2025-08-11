using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

[RequireComponent(typeof(Button))]
public class Checkbox : MonoBehaviour
{
	private Button toggle;
	private Image toggleImage;
	private TMP_Text text;
	public string setting;
	[ReadOnly]
	public bool value;

	private void Awake()
	{
		toggle = GetComponent<Button>();
		text = GetComponentInChildren<TMP_Text>();
		toggleImage = GetComponent<Image>();
		UpdateValue();

		toggle.onClick.AddListener(Toggle);
	}

	private void UpdateValue()
	{
		if (setting != "")
			this.SetValue((bool)FlagManager.Instance.GetSetting(setting));
	}

	public void SetValue(bool val)
	{
		this.value = val;
		if (setting != "")
			FlagManager.Instance.SetSetting(setting, val);
		this.toggleImage.sprite = value ? GlobalsManager.Instance.style.tg_on : GlobalsManager.Instance.style.tg_off;
	}

	private void Toggle()
	{
		SetValue(!this.value);
	}
}
