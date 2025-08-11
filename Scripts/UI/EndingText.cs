using TMPro;
using UnityEngine;

public class EndingText : MonoBehaviour
{
	TMP_Text text;
	Button btn;
	int count = 0;
	int maximum = 4;

	private void Awake()
	{
		btn = GetComponent<Button>();
		text = GetComponent<TMP_Text>();
	}

	private void Start()
	{
		// cleaned this up :D
		// also since flags are 0/1 we can just add them together to get the amount of
		// endings finished
		for (int i = 0; i < 4; i++)
			count += FlagManager.Instance.GetFlag(i);

		int didHelp = FlagManager.Instance.GetFlag(1);
		int didDead = FlagManager.Instance.GetFlag(0);

		bool hasAllDiaryEntries = true;
		for (int i = 7; i < 15; i++) // Check if all diary entries have been seen
		{
			if (FlagManager.Instance.GetFlag(i) == 0)
			{
				hasAllDiaryEntries = false;
				break;
			}
		}

		bool hasAllTapes = FlagManager.Instance.GetFlag(16) == 1 && FlagManager.Instance.GetFlag(17) == 1;

		// future-proofed :D
		text.text = "Endings: " + count.ToString() + "/" + maximum.ToString();

		bool readyForTrueEndings = didHelp == 1 && didDead == 1 && hasAllDiaryEntries && hasAllTapes;
		bool gameFinished = count == maximum;
		text.color = gameFinished ? Color.green : readyForTrueEndings ? Color.red : Color.white;

		btn.enabled = readyForTrueEndings || gameFinished;
	}
}
