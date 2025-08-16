using TMPro;
using UnityEngine;

public class VersionText : MonoBehaviour
{
	TMP_Text label;

	private void Awake()
	{
		label = GetComponent<TMP_Text>();
	}

	void Start()
	{
		label.text = CoreInitializer.version + '!';
	}
}
