using UnityEngine;

public class HappyLeo : BaseNPC
{
	public AudioManager audMan;
	public AudioObject howdy;
	public Sprite jollyLeo;

    // brain :O

	void Awake()
	{
		if (FlagManager.tempFlags["jollyMode"])
			GetComponentInChildren<SpriteRenderer>().sprite = jollyLeo;
		audMan = GetComponent<AudioManager>();
	}

	void Start()
	{
		audMan.PlaySound(howdy);
	}
}
