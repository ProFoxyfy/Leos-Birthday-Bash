using UnityEngine;

public abstract class BaseItem : MonoBehaviour
{
	public abstract ItemType type { get; }
	public abstract void Use(PlayerManager plr);
	public abstract void UseMachine(PlayerManager plr, VendingMachineType type);
}
