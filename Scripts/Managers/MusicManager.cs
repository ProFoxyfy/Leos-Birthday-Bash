using TweenX;
using TweenX.EasingStyles;
using UnityEngine;

[RequireComponent(typeof(AudioManager))]
public class MusicManager : Singleton<MusicManager>
{
	AudioManager audMan;
	public bool sampleLoudness = false;
	public float loudness;

	private float[] clipSampleData;
	public float updateStep = 0.02f;
	public int sampleDataLength = 1024;
	private float currentUpdateTime = 0f;
	private XFloat volume = 1f;

	public AudioSource audioSource;
	private TweenManager twMan;

	private void Awake()
	{
		DontDestroyOnLoad(this);
		twMan = gameObject.AddComponent<TweenManager>();
		audMan = GetComponent<AudioManager>();
		audioSource = GetComponent<AudioSource>();
		clipSampleData = new float[sampleDataLength];
	}

	private void Update()
	{
		audioSource.volume = volume;

		if (!sampleLoudness) return;
		currentUpdateTime += Time.deltaTime;
		if (currentUpdateTime >= updateStep)
		{
			currentUpdateTime = 0f;
			audioSource.clip.GetData(clipSampleData, audioSource.timeSamples); //I read 1024 samples, which is about 80 ms on a 44khz stereo clip, beginning at the current sample position of the clip.
			loudness = 0f;
			foreach (var sample in clipSampleData)
			{
				loudness += Mathf.Abs(sample);
			}
			loudness /= sampleDataLength;
		}
	}

	public void FadeIn(float duration)
	{
		twMan.PlayTweenSingle(ref volume, new Tween(duration, new Linear(), 0f, 1f));
	}

	public void SetVolume(float volume)
	{
		this.volume.Set(volume);
	}

	public void FadeOut(float duration)
	{
		twMan.PlayTweenSingle(ref volume, new Tween(duration, new Linear(), 1f, 0f));
	}

	public void PlayTrack(AudioObject track)
	{
		audMan.PlaySound(track);
	}

	public void StopAll()
	{
		audMan.StopAllSounds();
	}
}
