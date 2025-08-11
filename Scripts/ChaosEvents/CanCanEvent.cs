using UnityEngine;
using System.Collections;

public class CanCanEvent : ChaosEvent
{
	public AudioObject music;
	Leo leo;

	IEnumerator Undo()
	{
		yield return new WaitForSecondsRealtime(music.clip.length);
		leo.blind = false;
		leo.canEndGame = true;
		leo.Anger(-0.94f);
	}

    public override void Activate(ChaosEventManager manager)
	{
		StopAllCoroutines();
		leo = FindObjectOfType<Leo>(true);
		leo.Anger(1f);
		leo.canEndGame = false;
		leo.blind = true;
		manager.ShowMessage("Leo is now extremely silly.");
		MusicManager.Instance.PlayTrack(music);
		StartCoroutine(Undo());
	}
}
