using UnityEngine;

public class LitSprite : MonoBehaviour
{
	SpriteRenderer sprite;
	EnvironmentController ec;
	public Color tint = Color.white;

	private void Awake()
	{
		ec = EnvironmentController.Instance;
		sprite = GetComponent<SpriteRenderer>();
	}

	private void LateUpdate()
	{
		Vector3 roundedPosition = new Vector3(Mathf.RoundToInt(transform.position.x), 1f, Mathf.RoundToInt(transform.position.z));
		Vector2Int tilePos = new Vector2Int(Mathf.RoundToInt(roundedPosition.x), Mathf.RoundToInt(roundedPosition.z));
		Color lightmapColor = ec.GetLightmapPixel(tilePos);
		sprite.color = lightmapColor * tint;
	}
}
