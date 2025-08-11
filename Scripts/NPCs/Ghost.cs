using UnityEngine;

public class Ghost : MonoBehaviour
{
	private void OnTriggerEnter(Collider other)
	{
		if (!other.gameObject.CompareTag("Player"))
			return;
		EnvironmentController.Instance.gameManager.Broadcast("ghost");
		Destroy(gameObject);
	}
}
