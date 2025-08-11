using UnityEngine;

public class LeoRelocateEvent : ChaosEvent
{
	Leo leo;
	EnvironmentController ec;
	PlayerManager plr;

	void Start()
	{
		ec = EnvironmentController.Instance;
		plr = ec.GetPlayer(0);
	}

	Vector3 findSuitablePosition()
	{
		Transform tile = ec.GetRandomNavigableTile();
		if ((tile.position - plr.transform.position).magnitude < 10)
		{
			return findSuitablePosition();
		}
		return tile.position;
	}

    public override void Activate(ChaosEventManager manager)
	{
		leo = FindObjectOfType<Leo>();
		if (leo == null) return;
		manager.ShowMessage("Leo has been relocated to a random location.");
		leo.transform.position = findSuitablePosition();
	}
}
