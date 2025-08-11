using AYellowpaper.SerializedCollections;
using UnityEngine;
using UnityEngine.Diagnostics;

public class BadEndManager : BaseGameManager
{
	public TextAsset levelData;

    PremadeGenerator generator;

    public AudioObject ambience;
	public AudioObject doubtVoice;

    private GameObject doubtPrefab;
    private GameObject leoPrefab;
	private GameObject tapePrefab;

	private PlayerManager plr;
	AudioManager audMan;

    private string formatString = "";

    public override void Init()
    {
		audMan = GetComponent<AudioManager>();
        ec = FindObjectOfType<EnvironmentController>();
        ec.itmData = Resources.Load<ItemData>("BaseItems");
        doubtPrefab = Resources.Load<GameObject>("NPCs/StrongerDoubt");
        leoPrefab = Resources.Load<GameObject>("NPCs/DeadLeo");
		tapePrefab = Resources.Load<GameObject>("Tape");
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

	public SerializedDictionary<int, AudioObject> tapeAudio = new();

	private void PlaceTape(int id, Pos2 position)
	{
		GameObject obj = ec.PlaceMarker(tapePrefab, position, 0.45f);
		TapeController ctrl = obj.GetComponent<TapeController>();
		ctrl.id = (ushort)id;
		ctrl.tapeLine = tapeAudio[id];
	}

	private void Start()
    {
        MusicManager.Instance.PlayTrack(ambience);
		FlagManager.Instance.SetFlag(2, 1);
		FlagManager.Instance.SetFlag(4, 1);
        FlagManager.Instance.Save();
        FlagManager.Instance.SaveSettings();
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
        // spawn leo & doubt
        switch (id)
        {
            case 0:
                ec.PlaceMarker(doubtPrefab, position, 0f);
                break;
            case 1:
                ec.PlaceMarker(leoPrefab, position, 0f);
                break;
			case 2:
				LevelObject dummy = new LevelObject();
				TriggerController trigger = ec.PlaceTrigger(position.ToVec2Int(), dummy);
				trigger.type = TriggerType.CustomTrigger;
				trigger.onEnter.AddListener(() => {
					if (plr.frozen) return;
					plr.frozen = true;
					audMan.PlaySound(doubtVoice);
					Invoke("DoubtFinish", doubtVoice.clip.length + 0.1f);
				});
				break;
            case 3:
                PlaceTape(3, position);
                break;
            default:
                break;
        }
    }

	public override void Broadcast(string msg)
	{}

    public void DoubtFinish()
    {
            // Doubt finished his slander, crash the game
#if UNITY_EDITOR
            UnityEditor.EditorApplication.isPlaying = false;
#else
            Utils.ForceCrash(ForcedCrashCategory.Abort);
#endif
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
