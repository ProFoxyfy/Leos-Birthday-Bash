using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SequentialTransition : MonoBehaviour
{
	public List<Transition> transitions;
	public List<bool> states;
	public List<bool> instant;
	public bool delay = false;

	private IEnumerator Execute()
	{
		for (int i = 0; i < transitions.Count; i++)
		{
			if (instant[i])
				transitions[i].PerformInstant(states[i]);
			else
				yield return transitions[i].PerformAsync(states[i]);

			if (delay)
				yield return new WaitForSecondsRealtime(1f);
		}
	}

	public void Perform()
	{
		StartCoroutine(Execute());
	}
}
