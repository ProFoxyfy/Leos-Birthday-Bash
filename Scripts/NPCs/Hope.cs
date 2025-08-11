using UnityEngine;

public class Hope : MonoBehaviour
{
	PlayerManager plr;
	AudioManager audMan;
	public AudioObject voiceline;

	private void Awake()
	{
		audMan = GetComponent<AudioManager>();
		plr = EnvironmentController.Instance.GetPlayer(0);
	}

	private void Unfreeze()
	{
		plr.frozen = false;
		gameObject.SetActive(false);
	}

	void Start()
	{
		plr.frozen = true;
		audMan.PlaySound(voiceline);
		Invoke("Unfreeze", voiceline.clip.length);
	}
}
