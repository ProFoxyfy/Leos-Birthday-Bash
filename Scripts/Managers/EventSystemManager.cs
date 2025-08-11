using UnityEngine;

public class EventSystemManager : MonoBehaviour
{
	void Awake()
	{
		if (FindFirstObjectByType<EventSystemManager>() != this)
			Destroy(gameObject);
		DontDestroyOnLoad(this);
	}
}
