using TMPro;
using UnityEngine;

public class EditorMarker : MonoBehaviour
{
	public LevelMarker data;
	private TMP_Text text;

	private void Awake()
	{
		this.text = GetComponentInChildren<TMP_Text>();
	}

	private void Update()
	{
		this.text.text = data.data.ToString();
	}
}
