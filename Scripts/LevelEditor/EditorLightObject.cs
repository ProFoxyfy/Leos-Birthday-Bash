using UnityEngine;

public class EditorLightObject : BaseEditorObject
{
	private SpriteRenderer sprite;

	private void Awake()
	{
		data.type = ObjectType.Light;
		sprite = GetComponent<SpriteRenderer>();
	}
	private void Update()
	{
		sprite.color = data.color.ToColor();
	}
}