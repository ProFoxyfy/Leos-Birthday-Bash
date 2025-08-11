using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TooltipElement : MonoBehaviour
{
	public string key;
	[ReadOnly]
	public string txt;

	private void Awake()
	{
		txt = LocalizationManager.Instance.GetLocalizedString(key, LangStringType.Menu)[0];
	}
}
