using UnityEngine;

public class TextShake : MonoBehaviour
{
	Vector2 origPos;
	public float strength = 3f;

	private void Awake()
	{
		origPos = transform.localPosition;
	}

	private void Update()
	{
		transform.localPosition = origPos + new Vector2(Random.Range(-strength, strength), Random.Range(-strength, strength));
	}
}
