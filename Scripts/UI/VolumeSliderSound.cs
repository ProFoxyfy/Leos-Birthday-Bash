using System.Collections.Generic;
using UnityEngine;

[RequireComponent(typeof(AudioManager))]
public class VolumeSliderSound : MonoBehaviour
{
	public AudioObject sound;
	public List<Button> triggers = new List<Button>();
	private AudioManager audMan;

	private void Awake()
	{
		audMan = GetComponent<AudioManager>();
		foreach (Button btn in triggers)
		{
			btn.onClick.AddListener(Meow);
		}
	}

	public void Meow()
	{
		audMan.PlaySound(sound);
	}
}
