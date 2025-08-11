using TMPro;
using UnityEngine;

public class EditorDirectionalObject : BaseEditorObject
{
	TMP_Text dirText;

	private void Awake()
	{
		dirText = GetComponentInChildren<TMP_Text>();
	}

	private void Update()
	{
		dirText.text = DirectionData.dirShortNames[this.data.dir];
	}
}