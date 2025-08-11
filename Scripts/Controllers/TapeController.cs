using UnityEngine;

[RequireComponent(typeof(AudioManager))]
public class TapeController : MonoBehaviour, IInteractable
{
	private bool active = true;
	public AudioObject tapeLine;
	private AudioManager audMan;
	public ushort id = 0;

	void Awake()
	{
		audMan = GetComponent<AudioManager>();
	}

	public void Use()
	{
		if (!active) return;
		active = false;
		FlagManager.Instance.SetFlag(15 + id, 1);
		audMan.PlaySound(tapeLine);
	}
}