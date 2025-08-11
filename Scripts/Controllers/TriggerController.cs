using UnityEngine;
using UnityEngine.Events;

public class TriggerController : MonoBehaviour
{
	public UnityEvent onEnter;
	public TriggerType type;
	///<summary>
	///  Determines the ID to link with if the trigger is comprised of multiple objects.
	///</summary>
	public int id;
	public bool ignoreNpcs = true;
	private PlayerManager plr;
	public Pos2 position;

	private void Awake()
	{
		plr = EnvironmentController.Instance.GetPlayer(0);
		EnvironmentController.Instance.gameManager.OnTriggerCreate(type, this.id, this);
	}

	private void OnTriggerEnter(Collider other)
	{
		if (other.gameObject != plr.gameObject && ignoreNpcs) return;
		if ((!other.gameObject.layer.Equals(LayerMask.NameToLayer("NPC")) || other.gameObject == plr.gameObject) && !ignoreNpcs) return;
		if (this.type != TriggerType.CustomTrigger)
		{
			EnvironmentController.Instance.gameManager.OnGameTriggerEnter(this.type, this.id, this);
		}
		onEnter.Invoke();
	}
}

public enum TriggerType
{
	ExitTrigger,
	ExitWallTrigger,
	CustomTrigger
}