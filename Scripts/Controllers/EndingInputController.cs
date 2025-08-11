using TMPro;
using UnityEngine;

public class EndingInputController : MonoBehaviour, IInteractable
{
	private TMP_Text txt;
	private string currentTxt = " ";
	private string letters = " ADEHLP";
	private int letterIndex = 0;
	private ClassicGameManager gameManager;
	public int id = 0;

	private void Awake()
	{
		txt = GetComponentInChildren<TMP_Text>();
		gameManager = FindObjectOfType<ClassicGameManager>();
		if (gameManager == null)
			Destroy(gameObject);
	}

	private void UpdateText()
	{
		letterIndex++;
		if (letterIndex >= letters.Length)
			letterIndex = 0;

		currentTxt = letters[letterIndex].ToString();
		gameManager.SetCombinationAtIndex(id, currentTxt);
	}

	public void Use()
	{
		UpdateText();
		txt.text = currentTxt;
	}
}
