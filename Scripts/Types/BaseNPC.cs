using UnityEngine;
using UnityEngine.AI;

public class BaseNPC : MonoBehaviour
{
	public NavMeshAgent agent;
	public Vector3 target;
	public Vector3 nextTarget;
	private Vector3 interactionVelocity = Vector3.zero;
	private bool isInInteraction = false;
	private float interactionFailSave = 0f;
	public bool destinationReached = false;

	void Awake()
	{
		agent = GetComponent<NavMeshAgent>();
	}

	private void OnTriggerStay(Collider other)
	{
		if (other == null || other.attachedRigidbody == null) return;
		this.isInInteraction = true;
		this.interactionFailSave = 1f;
		this.interactionVelocity = other.attachedRigidbody.linearVelocity;
	}

	private void OnTriggerExit(Collider other)
	{
		this.isInInteraction = false;
	}

	internal void UpdateInteractions()
	{
		if (interactionFailSave <= 0f)
			this.isInInteraction = false;
		if (!this.isInInteraction) return;
		if (interactionFailSave > 0)
			interactionFailSave -= Time.deltaTime;
		this.agent.velocity = interactionVelocity;
	}
}
