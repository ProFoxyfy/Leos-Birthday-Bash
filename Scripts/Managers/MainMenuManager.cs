using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class MainMenuManager : MonoBehaviour
{
	public Transition warningScreen;
	public Transition menuScreen;
	public EventBehavior menuEvent;
	public AudioObject menuTrack;
	public AudioObject leoMenu;
	AudioManager audMan;

	private void Awake()
	{
		audMan = GetComponent<AudioManager>();
		if (GlobalsManager.Instance.firstLaunch)
		{
			GlobalsManager.Instance.firstLaunch = false;

			if (FlagManager.Instance.GetFlag(19) == 1)
			{
				FlagManager.Instance.SetFlag(19, 0);
				GlobalsManager.Instance.currentMode = GameMode.DoubtConsequence;
				SceneManager.LoadScene("Game");
				return;
			}

			warningScreen.Perform(true);
		}
		else
			menuScreen.Perform(true);

		menuEvent.OnStart.AddListener(OnMenuAwake);
	}

	IEnumerator DelayedPlay()
	{
		yield return new WaitForSecondsRealtime(0.1f);
		MusicManager.Instance.PlayTrack(menuTrack);
		yield return new WaitForSecondsRealtime(menuTrack.clip.length - 1.8f);
		audMan.PlaySound(leoMenu);
	}

	private void OnMenuAwake()
	{
		MusicManager.Instance.StopAll();
		MusicManager.Instance.SetVolume(1f);
		StartCoroutine(DelayedPlay());
	}
}
