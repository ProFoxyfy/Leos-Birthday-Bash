using UnityEngine;

public class MusicManagerWrapper : MonoBehaviour
{
	private MusicManager musicMan;

	private void Awake()
	{
		musicMan = MusicManager.Instance;
	}

	public void FadeIn(float duration)
	{
		musicMan.FadeIn(duration);
	}

	public void FadeOut(float duration)
	{
		musicMan.FadeOut(duration);
	}

	public void SetVolume(float volume)
	{
		musicMan.SetVolume(volume);
	}

	public void PlayTrack(AudioObject track)
	{
		musicMan.PlayTrack(track);
	}

	public void StopAll()
	{
		musicMan.StopAll();
	}
}