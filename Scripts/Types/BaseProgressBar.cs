using UnityEngine;

public abstract class BaseProgressBar : MonoBehaviour
{
	public float progress;
	public float min;
	public float max;
	public abstract void Refresh();
}