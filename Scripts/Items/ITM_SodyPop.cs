using UnityEngine;

public class ITM_SodyPop : BaseItem
{
	public override ItemType type
	{
		get
		{
			return ItemType.SodyPop;
		}
	}

	public override void Use(PlayerManager plr)
	{
		Vector3 properPosition = new Vector3(plr.transform.position.x, 0.4f, plr.transform.position.z);
		GameObject.Instantiate(Resources.Load<GameObject>("SodaSpray"), properPosition, Quaternion.Euler(0, Camera.main.transform.rotation.eulerAngles.y, 0));
		Destroy(this);
	}

	public override void UseMachine(PlayerManager plr, VendingMachineType type) { }
}
