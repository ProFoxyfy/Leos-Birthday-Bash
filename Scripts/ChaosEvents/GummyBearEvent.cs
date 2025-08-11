using UnityEngine;
using System.Collections;

public class GummyBearEvent : ChaosEvent
{
	GameObject bearPrefab;

	private void Start()
	{
		bearPrefab = Resources.Load<GameObject>("Rain");
	}

	IEnumerator Rain()
	{
		for (int i = 0; i < 180; i++)
		{
			for (int j = 0; j < 5; j++)
			{
				Instantiate(bearPrefab, EnvironmentController.Instance.GetRandomNavigableTile().position + Vector3.up * 3f, Quaternion.identity, transform);
			}
			yield return new WaitForSeconds(Random.Range(0.07f, 0.15f));
		}
	}

    public override void Activate(ChaosEventManager manager)
	{
		manager.ShowMessage("It's raining... gummy bears?");
		StartCoroutine(Rain());
	}
}
