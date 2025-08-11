using UnityEngine;

public class MenuAssignCamera : MonoBehaviour
{
	private void Awake()
	{
		this.GetComponent<Canvas>().worldCamera = UICamera.Instance.main;
	}
}
