using UnityEngine;

public class MemoryGameManager : BaseGameManager
{
	public TextAsset levelData;
	public TextAsset posterData;

	private int collected = 0;
	private int maximum = 0;
	PremadeGenerator generator;

	public AudioObject music;

	private GameObject objectivePrefab;
	private GameObject leoPrefab;

	private PlayerManager plr;

	private string formatString = "";

	Pos2 leoPos;

	public override void Init()
	{
		ec = EnvironmentController.Instance;
		ec.ambientColor = Color.black;

		ec.itmData = Resources.Load<ItemData>("BaseItems");
		objectivePrefab = Resources.Load<GameObject>("Memory");
		leoPrefab = Resources.Load<GameObject>("NPCs/FinaleLeo");
		balloonPrefab = Resources.Load<GameObject>("Balloon");

		generator = gameObject.AddComponent<PremadeGenerator>();
		generator.style = style;
		generator.Generate(levelData, posterData);
		StyleLevel(true, true);

		// Multiplayer support? God only knows.
		plr = ec.GetPlayer(0);

		formatString = LocalizationManager.Instance.GetLocalizedString("HUD_ObjectiveFormatMem", LangStringType.Menu)[0];
		HUDManager.Instance.objectiveFormat = formatString.Replace("?", "0");
	}

	private void Start()
	{
		MusicManager.Instance.PlayTrack(music);

		// spooky scary tunings
		// send dead memes down your spine
		if (FlagManager.Instance.GetFlag(4) == 1)
			MusicManager.Instance.audioSource.pitch = 0.98f;

		// Increase visibility
		HUDManager.Instance.objectiveText.color = Color.white;
		plr.itemTxt.color = Color.white;

		// Prevent pausing
		HUDManager.Instance.SetPauseEnabled(false);

		// Disable sprinting
		plr.disableSprint = true;
		plr.sprintFOV = 0f;

		// Dim the lights a little bit
		foreach (LightController light in ec.lights)
			light.lightRange *= 0.65f;
	}

	public override void OnObjectiveCollect(ObjectiveController objective)
	{
		collected++;
		HUDManager.Instance.objectiveCount = collected;

		// Pop him in like it's Tuesday
		if (collected == maximum)
			ec.PlaceMarker(leoPrefab, leoPos);
	}

	public override void OnGameTriggerEnter(TriggerType type, int id, TriggerController trigger)
	{
		// I have no reason to keep this :/
		DebugManager.Log("Trigger type: " + type.ToString());
	}

	public override void HandleMarker(int id, Pos2 position)
	{
		switch (id)
		{
			case 0:
				maximum++;
				ec.PlaceMarker(objectivePrefab, position, 0.51f);
				break;
			case 1:
				leoPos = position;
				break;
			default:
				break;
		}
	}

	public override void Broadcast(string msg)
	{
		return;
	}

	private void Update()
	{
		// TODO: Nothing. There is nothing to be done here.
	}

	private void LateUpdate()
	{
		HUDManager.Instance.objectiveFormat = formatString.Replace("?", maximum.ToString());
	}

	public override bool CanBlockExit()
	{
		// Exits? Nope don't exist here
		return false;
	}

	public override void OnTriggerCreate(TriggerType type, int id, TriggerController trigger)
	{
		// I need to implement this so the C# compiler doesn't yell at me
	}
}
