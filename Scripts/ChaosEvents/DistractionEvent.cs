using UnityEngine;
using System.Collections;

public class DistractionEvent : ChaosEvent
{
	public AudioObject leoLine;
	public AudioObject ariLine;
	Leo leo;
	public AudioManager ariSpeaker;

	IEnumerator Undo(ChaosEventManager manager)
	{
		yield return new WaitForSecondsRealtime(leoLine.clip.length);
		leo.frozen = false;
		leo.blind = false;
		leo.canEndGame = true;
		leo.Anger(-0.75f);
		leo.ChangeSprite(false);
		manager.blockChaosEvents = false;

		Destroy(gameObject); // Never make this occur again
	}

    public override void Activate(ChaosEventManager manager)
	{
		StopAllCoroutines();
		manager.blockChaosEvents = true;
		leo = FindObjectOfType<Leo>(true);
		leo.frozen = true;
		leo.canEndGame = false;
		leo.blind = true;
		manager.ShowMessage("Ari is calling...");

		ariSpeaker.transform.position = leo.transform.position + (Vector3.right * 0.5f) + (Vector3.forward * 0.5f);
		leo.ChangeSprite(true);
		leo.audMan.PlaySound(leoLine);
		ariSpeaker.PlaySound(ariLine);
		StartCoroutine(Undo(manager));
	}
}
