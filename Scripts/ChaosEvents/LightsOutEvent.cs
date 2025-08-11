using UnityEngine;
using System.Collections;

public class LightsOutEvent : ChaosEvent
{
	EnvironmentController ec;
	public AudioObject leoVoice;
	public AudioObject powerOff;
	AudioManager audMan;
	Leo leo;

	private void Start()
	{
		audMan = gameObject.AddComponent<AudioManager>();
		ec = EnvironmentController.Instance;
	}

	IEnumerator DoLightsOut()
	{
		if (leo == null) yield break;
		audMan.PlayOneShot(powerOff);
		foreach (LightController light in ec.lights)
		{
			if (!light.visible) continue;
			light.visible = false;
			yield return new WaitForSeconds(Random.Range(0.07f, 0.15f));
		}
		leo.audMan.PlayOneShot(leoVoice);
		leo.blind = true;
		yield return new WaitForSeconds(35f);
		leo.blind = false;
		foreach (LightController light in ec.lights)
		{
			if (light.visible) continue;
			light.visible = true;
			yield return new WaitForSeconds(Random.Range(0.1f, 0.2f));
		}
	}

    public override void Activate(ChaosEventManager manager)
	{
		StopAllCoroutines();
		leo = FindObjectOfType<Leo>(true);
		manager.ShowMessage("Oh noes!! Looks like a power outage...");
		StartCoroutine(DoLightsOut());
	}
}
