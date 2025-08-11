using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CrazyNico : BaseNPC
{
	int blockerMask;
	public AudioObject rush;
	float timer = 3f;

	void Awake()
	{
		GetComponent<AudioManager>().PlaySound(rush);
		agent = GetComponent<UnityEngine.AI.NavMeshAgent>();
		blockerMask = LayerMask.GetMask("NPCBlocker");
		target = EnvironmentController.Instance.GetRandomNavigableTile().position;
	}

    // Update is called once per frame
    void Update()
    {
		timer -= Time.deltaTime;
		if (timer > 0) return;
		timer = 3f;
        target = EnvironmentController.Instance.GetRandomNavigableTile().position;
    }

    public bool CanReach(Vector3 position, int mask)
    {
        RaycastHit hit;
        Vector3 direction = position - transform.position;
        return !Physics.Raycast(transform.position, direction, out hit, direction.magnitude, mask, QueryTriggerInteraction.Ignore);
    }

    private Vector3 ZeroY(Vector3 vector)
    {
        return new Vector3(vector.x, 0f, vector.z);
    }

    private float GetDistanceFromDestination()
    {
        float dist = Mathf.Abs(Vector3.Distance(ZeroY(transform.position), ZeroY(target)));
        if (dist < 1.5f && !CanReach(target, blockerMask))
        {
            return 0f;
        }
        return dist;
    }

	void LateUpdate()
	{
		agent.SetDestination(target);
	}
}
