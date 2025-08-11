using UnityEngine;
using System.Collections;

public class SlipperyEvent : ChaosEvent
{
	PlayerManager plr;

	private void Start()
	{
		plr = EnvironmentController.Instance.GetPlayer(0);
	}

	IEnumerator DoSlippery()
	{
		plr.slippery = true;
		yield return new WaitForSeconds(30f);
		plr.slippery = false;
	}

    public override void Activate(ChaosEventManager manager)
	{
		StopAllCoroutines();
		manager.ShowMessage("Woops! Someone didn't mop properly... Get ready to be slippery!");
		StartCoroutine(DoSlippery());
	}
}
