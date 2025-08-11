using UnityEngine;
using TMPro;

public class DiaryEntryScript : MonoBehaviour, IInteractable
{
	public GameObject entryUi;
	public TMP_Text entryContent;
	public ushort entryId;

    public void Use()
	{
		gameObject.SetActive(false);
		FlagManager.Instance.SetFlag(entryId + 6, 1);
		EnvironmentController.Instance.gameManager.Broadcast("diaryEntryCollect");
		HUDManager.Instance.Disable();
		HUDManager.Instance.SetPauseEnabled(false);
		HUDManager.Instance.blockGameInput = true;
		entryUi.SetActive(true);

		entryContent.text = LocalizationManager.Instance.GetLocalizedString("DRY_E" + entryId.ToString(), LangStringType.Menu)[0];
	}
}