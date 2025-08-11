using UnityEngine;
using System.Collections;

public class NicoEvent : ChaosEvent
{
	GameObject nicoPrefab;
	public AudioObject scream;

	private void Start()
	{
		nicoPrefab = Resources.Load<GameObject>("NPCs/Nico");
	}

	IEnumerator DoTheNico(GameObject nico)
	{
		AudioManager audMan = nico.GetComponent<AudioManager>();
		yield return new WaitForSecondsRealtime(30f);
		audMan.StopAllSounds();
		audMan.PlaySound(scream);
		yield return new WaitForSecondsRealtime(0.7f);
		Destroy(nico);
	}

    public override void Activate(ChaosEventManager manager)
	{
		manager.ShowMessage("Added Nico");
		GameObject nico = Instantiate(nicoPrefab, EnvironmentController.Instance.GetRandomNavigableTile().position, Quaternion.identity, null);
		StartCoroutine(DoTheNico(nico));
	}
}
