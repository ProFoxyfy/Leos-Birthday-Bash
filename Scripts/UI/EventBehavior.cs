using UnityEngine;
using UnityEngine.Events;

public class EventBehavior : MonoBehaviour
{
	public UnityEvent OnAwake = new UnityEvent();
	public UnityEvent OnStart = new UnityEvent();

	private void Awake()
	{
		OnAwake.Invoke();
	}

	void Start()
	{
		OnStart.Invoke();
	}
}
