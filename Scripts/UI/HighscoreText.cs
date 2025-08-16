using TMPro;
using UnityEngine;

public class HighscoreText : MonoBehaviour
{
	TMP_Text text;

	private void Awake()
	{
		text = GetComponent<TMP_Text>();
	}

	void Start()
	{
		// oops i did it again silly me
		text.text = string.Format(LocalizationManager.Instance.GetLocalizedString("MSL_HighscoreFormat", LangStringType.Menu)[0], FlagManager.Instance.GetFlag(5));
	}
}
