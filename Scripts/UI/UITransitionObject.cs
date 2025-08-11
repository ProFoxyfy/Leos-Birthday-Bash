using UnityEngine;

[CreateAssetMenu(fileName = "NewTransition", menuName = "Custom/UI Transition")]
public class UITransitionObject : ScriptableObject
{
	public Sprite[] transitionFrames;
	public float defaultDurationSeconds;
}