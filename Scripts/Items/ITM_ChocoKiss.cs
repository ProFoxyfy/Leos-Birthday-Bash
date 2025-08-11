using UnityEngine;

public class ITM_ChocoKiss : BaseItem
{
	public override ItemType type
	{
		get
		{
			return ItemType.ChocolateKiss;
		}
	}

	public override void Use(PlayerManager plr)
	{
		plr.stamina = 100f;
		Destroy(this);
	}

	public override void UseMachine(PlayerManager plr, VendingMachineType type) { }
}
