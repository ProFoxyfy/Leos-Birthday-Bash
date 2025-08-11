using UnityEngine.Audio;
using UnityEngine;
using System.Collections.Generic;
using System.Linq;
using System;

public class GlobalsManager : Singleton<GlobalsManager>
{
	public MenuCursor activeCursor;
	public bool firstLaunch = true;


	[SerializeField, ReadOnly]
	private AudioMixerGroup sfxGroup;
	[SerializeField, ReadOnly]
	private AudioMixerGroup voiceGroup;
	[SerializeField, ReadOnly]
	private AudioMixerGroup musicGroup;

	public List<Resolution> resolutions;

	public GameMode currentMode = GameMode.Story;

	public UIStyle style;

	private AudioMixer mixer;

	public AudioMixerGroup GetMixerGroupFromType(SoundType type)
	{
		return type switch
		{
			SoundType.SFX => sfxGroup,
			SoundType.Voice => voiceGroup,
			SoundType.Music => musicGroup,
			_ => null,
		};
	}

	private AudioMixerGroup FindFirstMixerGroup(string hierarchialName)
	{
		return mixer.FindMatchingGroups(hierarchialName)[0];
	}

	private void Awake()
	{
		DontDestroyOnLoad(this);

		resolutions = Screen.resolutions.Select(resolution => new Resolution { width = resolution.width, height = resolution.height }).Distinct().ToList();

		mixer = Resources.Load<AudioMixer>("Audio/Main");

		sfxGroup = FindFirstMixerGroup("Master/SFX");
		voiceGroup = FindFirstMixerGroup("Master/Voice");
		musicGroup = FindFirstMixerGroup("Master/Music");

		Screen.fullScreenMode = FullScreenMode.FullScreenWindow;
	}

	private void LateUpdate()
	{
		FlagManager.tempFlags["jollyMode"] = DateTime.Now.Month == 12 || (bool)FlagManager.Instance.GetSetting("forceJollyMode");
		bool fullscreen = (bool)FlagManager.Instance.GetSetting("fullscreen");
		float vol_music = (float)FlagManager.Instance.GetSetting("volume_music");
		float vol_sfx = (float)FlagManager.Instance.GetSetting("volume_sfx");
		float vol_voice = (float)FlagManager.Instance.GetSetting("volume_voice");

		mixer.SetFloat("Music_Volume", Mathf.Log(vol_music) * 20);
		mixer.SetFloat("SFX_Volume", Mathf.Log(vol_sfx) * 20);
		mixer.SetFloat("Voice_Volume", Mathf.Log(vol_voice) * 20);

		Screen.fullScreen = fullscreen;
	}
}
