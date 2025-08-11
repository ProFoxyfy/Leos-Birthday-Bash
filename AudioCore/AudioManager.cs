using System.Collections.Generic;
using System.Diagnostics;
using UnityEngine;
using UnityEngine.Audio;

[RequireComponent(typeof(AudioSource))]
public class AudioManager : MonoBehaviour
{
	internal AudioSource audioSource;
	public CharacterSubtitleInfo character;
	private SubtitleController lastSubtitle;

	private void Awake()
	{
		audioSource = GetComponent<AudioSource>();
		audioSource.dopplerLevel = 0f;
	}

	public void PlaySound(AudioObject sound)
	{
		if (sound.loop || !sound.positional)
		{
			audioSource.loop = sound.loop;
			audioSource.clip = sound.clip;
			audioSource.pitch = sound.pitch;
			audioSource.maxDistance = sound.radius;
			audioSource.outputAudioMixerGroup = GlobalsManager.Instance.GetMixerGroupFromType(sound.type);
			if (!sound.positional)
				audioSource.spatialBlend = 0f;
			else
				audioSource.spatialBlend = 1f;

			audioSource.Play();
		}
		else
		{
			audioSource.spatialBlend = 1f;
			audioSource.loop = sound.loop;
			audioSource.clip = sound.clip;
			audioSource.pitch = sound.pitch;
			audioSource.maxDistance = sound.radius;
			audioSource.outputAudioMixerGroup = GlobalsManager.Instance.GetMixerGroupFromType(sound.type);
			audioSource.PlayOneShot(audioSource.clip, sound.volume);
		}
		lastSubtitle = SubtitleManager.Instance.CreateSub(sound, this);
	}

	public void PlayOneShot(AudioObject sound)
	{
		audioSource.PlayOneShot(sound.clip, sound.volume);
		SubtitleManager.Instance.CreateSub(sound, this);
	}

	public void StopAllSounds()
	{
		if (lastSubtitle != null)
			Destroy(lastSubtitle.gameObject);
		audioSource.Stop();
	}
}
