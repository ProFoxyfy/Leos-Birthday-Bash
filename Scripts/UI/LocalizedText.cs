using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;

public class LocalizedText : MonoBehaviour
{
	private TMP_Text text;
	public string key;
	public LangStringType type;

	private void Awake()
	{
		text = GetComponent<TMP_Text>();
		text.text = LocalizationManager.Instance.GetLocalizedString(key, type)[0];
	}
}
