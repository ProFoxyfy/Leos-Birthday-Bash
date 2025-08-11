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
		text.text = "Highscore: " + FlagManager.Instance.GetFlag(5);
	}
}
