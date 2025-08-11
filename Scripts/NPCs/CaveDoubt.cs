using UnityEngine;

public class CaveDoubt : MonoBehaviour
{
	AudioManager audMan;
	public AudioObject voiceline;
	private bool triggered = false;

	private void Awake()
	{
		audMan = GetComponent<AudioManager>();
	}

	private void LineEnd()
	{
		EnvironmentController.Instance.gameManager.Broadcast("doubt");
	}

	private void OnTriggerEnter(Collider other)
	{
		if (!other.gameObject.CompareTag("Player") || triggered) return;
		triggered = true;
		FlagManager.Instance.SetFlag(6, 1);
		FlagManager.Instance.Save();
		FlagManager.Instance.SaveSettings();
		EnvironmentController.Instance.GetPlayer(0).frozen = true;
		audMan.PlaySound(voiceline);
		Invoke("LineEnd", voiceline.clip.length);
	}
}
