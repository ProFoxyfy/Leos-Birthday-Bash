using UnityEngine;

public class ObjectiveController : MonoBehaviour, IInteractable
{
	public AudioManager audMan;

	private void Awake()
	{
		audMan = GetComponent<AudioManager>();
	}

	public void Use()
	{
		EnvironmentController.Instance.GetPlayer(0).stamina = 100f;
		EnvironmentController.Instance.gameManager.OnObjectiveCollect(this);
		gameObject.SetActive(false);
	}
}
