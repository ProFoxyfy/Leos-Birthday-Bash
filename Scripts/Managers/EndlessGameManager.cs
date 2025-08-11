using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EndlessGameManager : BaseGameManager
{
	public TextAsset levelData;
	public TextAsset posterData;

	private int collected = 0;
	private float anger = 0.004f;
	PremadeGenerator generator;

	public AudioObject school;
	public AudioObject schoolDecember;
	public AudioObject respawnSound;

	private GameObject objectivePrefab;

	private PlayerManager plr;

	Leo npcLeo;

	private string formatString = "";

	public override void Init()
	{
		ec = EnvironmentController.Instance;
		ec.itmData = Resources.Load<ItemData>("BaseItems");
		objectivePrefab = Resources.Load<GameObject>("Objective");
		balloonPrefab = Resources.Load<GameObject>("Balloon");
		generator = gameObject.AddComponent<PremadeGenerator>();
		generator.style = style;
		generator.Generate(levelData, posterData);
		StyleLevel();
		SpawnBalloons();
		plr = ec.GetPlayer(0);
		formatString = LocalizationManager.Instance.GetLocalizedString("HUD_ObjectiveFormat", LangStringType.Menu)[0];
		HUDManager.Instance.objectiveFormat = formatString.Replace("?", "0");
	}

	private void Start()
	{
		if (FlagManager.tempFlags["jollyMode"])
			MusicManager.Instance.PlayTrack(schoolDecember);
		else
			MusicManager.Instance.PlayTrack(school);
	}

	IEnumerator Respawn(ObjectiveController objective)
	{
		yield return new WaitForSeconds(60f * 2f);
		objective.gameObject.SetActive(true);
		objective.audMan.PlaySound(respawnSound);
	}

	public override void OnObjectiveCollect(ObjectiveController objective)
	{
		Leo leo = null;
		if (collected == 0)
		{
			MusicManager.Instance.StopAll();
			foreach (BaseNPC npc in ec.npcs)
			{
				leo = null;
				npc.gameObject.TryGetComponent<Leo>(out leo);
				if (leo != null)
					npcLeo = leo;
				npc.gameObject.SetActive(leo != null);
			}
			npcLeo.Anger(0.05f);
		}

		npcLeo.Anger(-0.07f);

		StartCoroutine(Respawn(objective));

		collected++;

		if (collected > FlagManager.Instance.GetFlag(5))
			FlagManager.Instance.SetFlag(5, collected);

		HUDManager.Instance.objectiveCount = collected;
	}

	public override void OnGameTriggerEnter(TriggerType type, int id, TriggerController trigger)
	{
		DebugManager.Log("Trigger type: " + type.ToString());
	}

	public override void HandleMarker(int id, Pos2 position)
	{
		// we dont need half of this
		switch (id)
		{
			case 0:
				ec.PlaceMarker(objectivePrefab, position, 0.51f);
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
		if (npcLeo == null) return;
		npcLeo.Anger(anger * Time.deltaTime);
	}

	private void LateUpdate()
	{
		HUDManager.Instance.objectiveFormat = formatString.Replace("?", FlagManager.Instance.GetFlag(5).ToString());
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
