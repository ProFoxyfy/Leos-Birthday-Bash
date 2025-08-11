using UnityEngine;

public class MusicEvent : ChaosEvent
{
	public AudioObject music;

    public override void Activate(ChaosEventManager manager)
	{
		manager.ShowMessage("You are now forced to listen to horrible music.");
		MusicManager.Instance.PlayTrack(music);
	}
}
