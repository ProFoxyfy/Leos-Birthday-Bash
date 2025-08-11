using UnityEngine;
using UnityEngine.Diagnostics;

public class CaveGameManager : BaseGameManager
{
	public TextAsset levelData;

	PremadeGenerator generator;

	public AudioObject ambience;

	private GameObject hopePrefab;
	private GameObject doubtPrefab;

	private PlayerManager plr;

	private string formatString = "";

	public override void Init()
	{
		ec = FindObjectOfType<EnvironmentController>();
		ec.itmData = Resources.Load<ItemData>("BaseItems");
		hopePrefab = Resources.Load<GameObject>("NPCs/Hope");
		doubtPrefab = Resources.Load<GameObject>("NPCs/DoubtCave");
		generator = gameObject.AddComponent<PremadeGenerator>();
		generator.style = style;
		generator.Generate(levelData, null);
		StyleLevel();
		plr = ec.GetPlayer(0);
		formatString = LocalizationManager.Instance.GetLocalizedString("HUD_ObjectiveFormat", LangStringType.Menu)[0];
		HUDManager.Instance.objectiveFormat = formatString.Replace("?", "0");
		HUDManager.Instance.Disable();
		HUDManager.Instance.SetPauseEnabled(false);
	}

	private void Start()
	{
		MusicManager.Instance.PlayTrack(ambience);
	}

	public override void OnObjectiveCollect(ObjectiveController objective)
	{
		return;
	}

	public override void OnGameTriggerEnter(TriggerType type, int id, TriggerController trigger)
	{
		DebugManager.Log("Trigger type: " + type.ToString());
	}

	public override void HandleMarker(int id, Pos2 position)
	{
		// spawn hope & doubt
		switch (id)
		{
			case 0:
				ec.PlaceMarker(hopePrefab, position, 0.51f);
				break;
			case 1:
				ec.PlaceMarker(doubtPrefab, position, 0f);
				break;
			default:
				break;
		}
	}

	public override void Broadcast(string msg)
	{
		if (msg == "doubt")
		{
			// Doubt finished his cameo, switch to doubt mode
			FlagManager.Instance.SetFlag(6, 1);
			FlagManager.Instance.Save();
			FlagManager.Instance.SaveSettings();
#if UNITY_EDITOR
			UnityEditor.EditorApplication.isPlaying = false;
#else
            Utils.ForceCrash(ForcedCrashCategory.Abort);
#endif
		}
	}

	private void LateUpdate()
	{
		HUDManager.Instance.objectiveFormat = formatString.Replace("?", "0");
	}

	public override bool CanBlockExit()
	{
		return false;
	}

	public override void OnTriggerCreate(TriggerType type, int id, TriggerController trigger)
	{
		// don't need this
	}
}
