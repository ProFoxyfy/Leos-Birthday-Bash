using UnityEngine;

public class AbsurdEvent : ChaosEvent
{
	public AudioObject confettiSound;
	AudioManager audMan;

	void Awake()
	{
		audMan = gameObject.AddComponent<AudioManager>();
	}

    public override void Activate(ChaosEventManager manager)
	{
		int num = Random.Range(1,8);
		switch (num)
		{
			case 1:
				manager.ShowMessage("Nothing... happened... Quite a peculiar predicament. Toodles!");
				break;
			case 2:
				manager.ShowMessage("Confetti.");
				audMan.PlaySound(confettiSound);
				break;
			case 3:
				manager.ShowMessage("Deleting game... just kidding :)");
				break;
			case 4:
				manager.ShowMessage("// POLICE ASSAULT IN PROGRESS // nevermind guys retreat");
				break;
			case 5:
				manager.ShowMessage("You've been selected for an audit! ...you're suspiciously average.");
				break;
			case 6:
				manager.ShowMessage("cakecakecakecakecakecakecakecakecakecakecakecakecakecakecakecakecakecakecakecakecakecakecakecakecakecakecakecakecakecakecakecakecakecakecakecakecakecakecakecakecakecakecakecakecakecakecakecakecakecakecakecakecakecakecakecakecakecakecakecakecakecakecakecakecakecakecakecakecakecakecakecakecakecakecakecakecakecakecakecakecakecakecakecakecakecakecakecakecakecakecakecakecakecakecakecakecakecakecakecakecakecakecakecakecakecakecakecakecakecakecakecakecakecakecakecakecakecakecakecakecakecakecakecakecake");
				break;
			case 7:
				manager.ShowMessage("Added Yin. <color=red>NullReferenceException!</color>");
				break;
		}
	}
}
