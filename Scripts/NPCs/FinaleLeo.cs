using UnityEngine;

public class FinaleLeo : BaseNPC
{
	public AudioManager audMan;
	public AudioObject voiceline;
	public AudioObject regretVoiceline;
	bool activated = false;
	PlayerManager plr;
	public AudioObject music;
	public AudioObject regretMusic;
	public AudioObject birthday;

	GameObject creditsScreen;
	GameObject screenInst;

	// Obligatory comment that serves zero purpose

	void Awake()
	{
		creditsScreen = Resources.Load<GameObject>("FinalCredits");
		plr = Object.FindAnyObjectByType<PlayerManager>();
		audMan = GetComponent<AudioManager>();

		screenInst = Instantiate(creditsScreen, null);
		screenInst.SetActive(false);
	}

	void DoTheMusic()
	{
		if (FlagManager.Instance.GetFlag(4) == 1)
			MusicManager.Instance.PlayTrack(regretMusic);
		else
			MusicManager.Instance.PlayTrack(music);

		MusicManager.Instance.FadeIn(1f);
	}

	void DoTheBirthdayMusic()
	{
		MusicManager.Instance.SetVolume(1f);
		MusicManager.Instance.PlayTrack(birthday);
	}

	void RollCredits()
	{
		MusicManager.Instance.FadeOut(2f);


		foreach (LightController light in EnvironmentController.Instance.lights)
		{
			// no credits yet
			// TODO: add them lol
			light.visible = false;	
		}

		if (FlagManager.Instance.GetFlag(4) == 1)
		{
			FlagManager.Instance.SetFlag(4, 0); // Your sins are forgiven
			EnvironmentController.Instance.Invoke("ExitToMenu", 3f);
			return;
		}

		FlagManager.Instance.SetFlag(3, 1); // You did it. Congratulations.
		Invoke("DoTheBirthdayMusic", 2.1f);
		screenInst.SetActive(true);
		Invoke("HideCreditsAndReturn", 32f);
	}

	void HideCreditsAndReturn()
	{
		screenInst.SetActive(false);
		EnvironmentController.Instance.Invoke("ExitToMenu", 2f);
	}

	private void OnTriggerEnter(UnityEngine.Collider other)
	{
		if (activated) return; // This holds the entire ceiling up. Do not remove.
		if (!other.gameObject.CompareTag("Player")) return; // Technically... only the player could collide but meh
		MusicManager.Instance.FadeOut(2f);
		Invoke("DoTheMusic", 2f);
		HUDManager.Instance.Disable();
		plr.frozen = true;
		activated = true;

		// DRY? Nah never heard of it :D
		if (FlagManager.Instance.GetFlag(4) == 1)
		{
			audMan.PlaySound(regretVoiceline);
			Invoke("RollCredits", regretVoiceline.clip.length + 1f);
		}
		else
		{
			audMan.PlaySound(voiceline);
			Invoke("RollCredits", voiceline.clip.length + 1f);
		}
	}
}
