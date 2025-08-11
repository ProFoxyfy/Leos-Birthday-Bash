using UnityEngine;

public class EditorBasicObject : BaseEditorObject
{
	[SerializeField]
	private ObjectType type;

	private void Awake()
	{
		data.type = type;
	}
}