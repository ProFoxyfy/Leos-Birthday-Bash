using UnityEngine;

public class RandomAngerEvent : ChaosEvent
{
	Leo leo;

	private void Start()
	{
		leo = FindObjectOfType<Leo>(true);
	}

    public override void Activate(ChaosEventManager manager)
	{
		float amount = Random.Range(-0.05f, 0.05f);
		leo.Anger(amount);
		manager.ShowMessage("Made Leo angrier by " + amount.ToString());
	}
}
