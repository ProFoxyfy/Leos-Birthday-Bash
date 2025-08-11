using AYellowpaper.SerializedCollections;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using TMPro;
using TweenX;
using TweenX.EasingStyles;
using TweenX.EasingStyles.Advanced;
using UnityEngine;
using static UnityEngine.EventSystems.EventTrigger;

public class ClassicGameManager : BaseGameManager
{
	public TextAsset level;
	public TextAsset posterData;
	PremadeGenerator generator;
	Dictionary<int, bool> reachedExits = new Dictionary<int, bool>();
	int exitCount = 0;

	public AudioObject school;
	public AudioObject schoolDecember;
	public AudioObject exit1Clip;
	public AudioObject exit2Clip;
	public AudioObject exit3Clip;
	public AudioObject winAmbience;
	public AudioObject combinationCorrectSound;
	public AudioObject helpAmbience;
	public AudioObject allPresentsVoice;
	public AudioManager allPresentsSpeaker;
	private XFloat fogEnd = new XFloat(-1f);
	private IEasingFunction fogEase = new Linear();
	private TweenManager twMan;
	private GameObject objectivePrefab;

	public Material finaleSkybox;

	private GameObject keypadPrefab;
	private GameObject friend;
	private GameObject ghostPrefab;
	private GameObject entryUiPrefab;
	private GameObject entryObject;
	private GameObject tapePrefab;
	private AudioManager audMan;
	private float glitchStrength = 0f;

	private Dictionary<Pos2, Direction> endingWalls = new Dictionary<Pos2, Direction>();
	private Dictionary<Pos2, Direction> combinationWalls = new Dictionary<Pos2, Direction>();

	private string formatString = "";

	public int collected = 0;
	public int maximum = 0;

	[SerializeField, ReadOnly]
	private string currentCombination = "    ";

	private PlayerManager plr;

	public Pos2 ghostPos;

	public List<DiaryEntry> diaryEntries = new();
	public List<GameObject> diaryEntryObjects = new();
	public TMP_Text diaryUiContent;
	public GameObject diaryUi;

	Leo npcLeo;

	private string ReplaceAtPosition(string self, int position, string newValue)
	{
		return self.Remove(position, newValue.Length).Insert(position, newValue);
	}

	public void SetCombinationAtIndex(int index, string character)
	{
		currentCombination = ReplaceAtPosition(currentCombination, index - 1, character);
	}

	public override void Init()
	{
		twMan = gameObject.AddComponent<TweenManager>();
		audMan = gameObject.AddComponent<AudioManager>();
		ec = Object.FindAnyObjectByType<EnvironmentController>();
		ec.itmData = Resources.Load<ItemData>("BaseItems");
		objectivePrefab = Resources.Load<GameObject>("Objective");
		balloonPrefab = Resources.Load<GameObject>("Balloon");
		ghostPrefab = Resources.Load<GameObject>("Ghost");
		friend = Resources.Load<GameObject>("HELP");
		keypadPrefab = Resources.Load<GameObject>("EndingInput");
		entryObject = Resources.Load<GameObject>("DiaryPiece");
		entryUiPrefab = Resources.Load<GameObject>("DiaryEntryUI");
		tapePrefab = Resources.Load<GameObject>("Tape");
		generator = gameObject.AddComponent<PremadeGenerator>();
		generator.style = style;
		generator.Generate(level, posterData);
		StyleLevel();
		SpawnBalloons();
		plr = ec.GetPlayer(0);

		GameObject diaryUiInstance = Instantiate(entryUiPrefab, null);
		diaryUi = diaryUiInstance.transform.Find("DiaryEntry").gameObject;
		diaryUiContent = diaryUi.transform.Find("Background").gameObject.GetComponentInChildren<TMP_Text>();

		formatString = LocalizationManager.Instance.GetLocalizedString("HUD_ObjectiveFormat", LangStringType.Menu)[0];
		HUDManager.Instance.objectiveFormat = formatString.Replace("?", maximum.ToString());
	}

	private void Start()
	{
		if (FlagManager.tempFlags["jollyMode"])
			MusicManager.Instance.PlayTrack(schoolDecember);
		else
			MusicManager.Instance.PlayTrack(school);
	}

	private void UpdateHudObjectiveCount()
	{
		HUDManager.Instance.objectiveCount = collected;
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
		}

		npcLeo.Anger(0.05f);
		npcLeo.Hear(plr.transform.position, 45);

		collected++;

		if (collected == maximum)
			allPresentsSpeaker.PlaySound(allPresentsVoice);
		UpdateHudObjectiveCount();
	}

	private void ProgressMusic()
	{
		switch (exitCount)
		{
			case 0:
				break;
			case 1:
				MusicManager.Instance.PlayTrack(this.exit1Clip);
				break;
			case 2:
				MusicManager.Instance.PlayTrack(this.exit2Clip);
				break;
			case 3:
				MusicManager.Instance.PlayTrack(this.exit3Clip);
				break;
			case 4:
				MusicManager.Instance.PlayTrack(this.winAmbience);
				break;
		}
	}

	private bool lightingStage1Finished = false;

	IEnumerator FinaleLighting()
	{
		while (exitCount < 4)
		{
			switch (exitCount)
			{
				case 2 or 3:
					ec.stopLightingUpdate = true;
					Vector2Int tilePos = ec.GetRandomTilePos();
					Color lColor = ec.GetLightmapPixel(tilePos);
					Color targetColor = exitCount == 2 ? Color.red : new Color(0.7f, 0f, 0f, 1f);
					while (lColor == targetColor)
					{
						tilePos = ec.GetRandomTilePos();
						lColor = ec.GetLightmapPixel(tilePos);
						yield return new WaitForEndOfFrame();
					}
					ec.SetLightmapPixel(ec.GetRandomTilePos(), targetColor);
					yield return new WaitForSeconds(0.002f);
					break;
				case 4:
					yield break;
				default:
					if (lightingStage1Finished) yield return new WaitForEndOfFrame();
					foreach (LightController light in EnvironmentController.Instance.lights)
					{
						if (light.name == "ExitLight" || light.lightColor != Color.white) continue;
						light.lightColor = new Color(1f, 0.1f, 0.1f);
						yield return new WaitForSeconds(0.3f);
					}
					lightingStage1Finished = true;
					break;
			}
		}
	}

	private void SpawnFriend(int count)
	{
		for (int i = 0; i < count; i++)
		{
			Vector3 pos = ec.GetRandomNavigableTile().position;
			pos.y = 0.4f;
			GameObject clone = Instantiate(friend, pos, Quaternion.identity, transform);
		}
	}

	private void ProgressFinaleLighting()
	{
		if (exitCount > 1) return;
		StartCoroutine(FinaleLighting());
	}

	private void FinaleLogic()
	{
		if (exitCount == 4)
		{
			// Reset HUD & music flags
			MusicManager.Instance.sampleLoudness = false;
			HUDManager.Instance.Unbreak();
			HUDManager.Instance.SetPauseEnabled(false);
			// Reset player variables
			ec.GetPlayer(0).staminaRegenTime = 2;
			ec.GetPlayer(0).walkSpeed = 1.5f;
			ec.GetPlayer(0).runSpeed = 3f;
			ec.GetPlayer(0).staminaRegen = 0.05f;
			ec.GetPlayer(0).stamina = 100f;
			ec.GetPlayer(0).staminaDecrement = 0f;
			ec.GetPlayer(0).ChangeFOV(60f);
			// Modify lighting to be moody
			ec.ambientColor = Color.black;
			ec.lightBlendMode = LightBlendMode.Greatest;
			ec.stopLightingUpdate = false;
			RenderSettings.skybox = finaleSkybox;

			// Dim lights that aren't exit lights
			foreach (LightController light in ec.lights)
			{
				if (light.name == "ExitLight")
					light.visible = false;
				else if (light.lightColor == new Color(1f, 0.1f, 0.1f))
					light.lightColor = new Color(0.3f, 0.3f, 0.3f);
			}

			// Update lighting to reverse the ending effect
			ec.ForceUpdateLighting();

			RevealWalls(endingWalls);
			RevealDiaryEntries(3);

			// There are no more diary entries to collect and not all tapes have been listened to;
			// spawn tapes.
			if (diaryEntryObjects.Count == 0 && (FlagManager.Instance.GetFlag(16) == 0 || FlagManager.Instance.GetFlag(17) == 0))
				RevealTapes();

			twMan.PlayTweenSingle(ref fogEnd, new Tween(6f, fogEase, 25f, 1000));

			foreach (BaseNPC npc in ec.npcs)
				Destroy(npc.gameObject);

			return;
		}
		if (exitCount > 1) return;
		MusicManager.Instance.sampleLoudness = true;
		npcLeo.Anger(0.2f);
		HUDManager.Instance.Break();
		Shader.SetGlobalColor("_FogColor", Color.red);
		twMan.PlayTweenSingle(ref fogEnd, new Tween(3f, fogEase, 1000f, 25f));

		ec.GetPlayer(0).staminaRegenTime = 0;
		ec.GetPlayer(0).staminaRegen = 0f;
		ec.GetPlayer(0).stamina = 0f;
		ec.GetPlayer(0).walkSpeed = 3.5f;
		ec.GetPlayer(0).runSpeed = 3.5f;
		ec.GetPlayer(0).ChangeFOV(80f);
	}

	private void RevealWalls(Dictionary<Pos2, Direction> walls)
	{
		foreach (KeyValuePair<Pos2, Direction> wall in walls)
		{
			TileController tile = ec.GetTile(wall.Key.x, wall.Key.y);
			Transform target = tile.transform.Find(DirectionData.dirShortNames[wall.Value]);
			target.gameObject.SetActive(false);
		}
	}

	private DiaryEntry? PickFreeDiaryPiece(Dictionary<ushort, bool> blacklist, ref int safeguard)
	{
		// Check if there's no more to spawn
		if (blacklist.Values.All(value => value)) return null;
		if (safeguard > 32) return null;

		// Recurse if it's already taken
		ushort chosenId = (ushort)Random.Range(1, diaryEntries.Count);
		if (blacklist[chosenId])
		{
			safeguard++;
			return PickFreeDiaryPiece(blacklist, ref safeguard);
		}

		DiaryEntry entry = diaryEntries.Find(t => t.id == chosenId);
		blacklist[chosenId] = true;
		return entry;
	}

	private void SpawnDiaryEntry(DiaryEntry entry)
	{
		GameObject entryObj = ec.PlaceMarker(entryObject, entry.position, 0.51f);
		DiaryEntryScript entryScript = entryObj.GetComponent<DiaryEntryScript>();
		entryScript.entryContent = diaryUiContent;
		entryScript.entryUi = diaryUi;
		entryScript.entryId = entry.id;
		diaryEntryObjects.Add(entryObj);
	}

	private void RevealDiaryEntries(int totalToReveal)
	{
		Dictionary<ushort, bool> blacklist = new();
		for (ushort i = 7; i < 15; i++) // There was an off by one error here that I never noticed
			blacklist[(ushort)(i - 6)] = FlagManager.Instance.GetFlag((int)i) == 1;

		// Technically no more need but...
		// Why not?
		int safeguard = 0;

		// Spawn diary entries and break if there's no more
		for (int i = 0; i < totalToReveal; i++)
		{
			DiaryEntry? entry = PickFreeDiaryPiece(blacklist, ref safeguard);
			if (!entry.HasValue) break;
			SpawnDiaryEntry(entry.Value);
		}

		// Change objective text
		maximum = diaryEntryObjects.Count;
		formatString = LocalizationManager.Instance.GetLocalizedString("HUD_ObjectiveFormatDiary", LangStringType.Menu)[0].Replace("?", maximum.ToString());
		collected = 0;
		UpdateHudObjectiveCount();
	}

	private bool finalExit = false;

	public override void OnGameTriggerEnter(TriggerType type, int id, TriggerController trigger)
	{
		DebugManager.Log("Trigger type: " + type.ToString());

		switch (type)
		{
			case TriggerType.ExitWallTrigger:
				// Stop if we've already reached this exit or we don't have enough
				if (reachedExits.ContainsKey(id)) break;
				if (collected < maximum) break;
				DebugManager.Log($"Reached exit w/ id {id}, n exits reached: {exitCount}");

				// Leo should hear if the player has hit an exit
				npcLeo.Hear(plr.transform.position, 80);

				// If we're at 3 exits, then the next one is going to be the final one
				if (exitCount > 2)
				{
					finalExit = true;
					break;
				}

				// Hide the exit light, and track this exit
				ec.exitLights[id].visible = false;
				exitCount++;
				reachedExits[id] = true;

				// Progress finale
				this.ProgressMusic();
				this.FinaleLogic();
				this.ProgressFinaleLighting();
				break;
			case TriggerType.ExitTrigger:
				// Ditto
				if (reachedExits.ContainsKey(id)) break;
				if (collected < maximum) break;

				// Track exit & progress finale
				exitCount++;
				DebugManager.Log($"Reached final exit w/ id {id}");
				reachedExits[id] = true;
				this.ProgressMusic();
				this.FinaleLogic();
				this.ProgressFinaleLighting();
				break;
		}
	}

	private void PlaceKeypad(int id, Pos2 position)
	{
		// uhh
		Direction dir = Direction.West;
		Vector3 offset = Vector2Helper.ToVec3(DirectionData.dirVectors[dir] * 0.005f);
		TileController tile = ec.GetTile(position.x, position.y);
		Transform wall = tile.transform.Find(DirectionData.dirShortNames[dir]);
		GameObject inst = Instantiate(keypadPrefab, wall.position + offset, wall.rotation, tile.transform);
		EndingInputController ctrl = inst.GetComponent<EndingInputController>();
		ctrl.id = id;
	}

	public SerializedDictionary<int, AudioObject> tapeAudio = new();
	private Dictionary<int, Pos2> tapes = new();

	private void PlaceTape(int id, Pos2 position)
	{
		GameObject obj = ec.PlaceMarker(tapePrefab, position, 0.45f);
		TapeController ctrl = obj.GetComponent<TapeController>();
		ctrl.id = (ushort)id;
		ctrl.tapeLine = tapeAudio[id];
	}

	private void RevealTapes()
	{
		foreach (int id in tapes.Keys)
			PlaceTape(id, tapes[id]);
	}

	public override void HandleMarker(int id, Pos2 position)
	{
		// Markers are special objects that
		// are handled by the individual
		// game manager instead of the
		// generator. As such
		// it is particularly useful for
		// game events.
		switch (id)
		{
			// Objectives
			case 0:
				ec.PlaceMarker(objectivePrefab, position, 0.51f);
				maximum++;
				break;

			// Keypads
			case 1:
				PlaceKeypad(1, position);
				break;
			case 2:
				PlaceKeypad(2, position);
				break;
			case 3:
				PlaceKeypad(3, position);
				break;
			case 4:
				PlaceKeypad(4, position);
				break;

			// Reveal walls
			case 5:
				endingWalls.Add(position, Direction.South);
				break;
			case 6:
				combinationWalls.Add(position, Direction.East);
				break;

			// Position marker for the ghost of the HELP ending
			case 7:
				ghostPos = position;
				break;

			// Tapes (technically USB sticks)
			case 30:
				tapes[1] = position;
				break;
			case 31:
				tapes[2] = position;
				break;

			default:
				break;
		}

		// Check if the id is in range for diary entries and spawn
		if (id >= 21 && id <= 29)
		{
			ushort num = (ushort)(id - 21);
			DiaryEntry entry = new();
			entry.position = position;
			entry.id = num;
			diaryEntries.Add(entry);
		}
	}

	private List<string> validCombinations = new()
	{
		"DEAD",
		"HELP"
	};

	private bool disableEnding = false;

	IEnumerator FriendSpawn()
	{
		// This code spawns exactly 600 friends
		yield return new WaitForSecondsRealtime(5f);
		SpawnFriend(20);
		yield return new WaitForSecondsRealtime(10f);
		SpawnFriend(30);
		yield return new WaitForSecondsRealtime(30f);
		SpawnFriend(50);
		yield return new WaitForSecondsRealtime(20f);
		int i = 500;
		while (i > 0)
		{
			i--;
			SpawnFriend(1);
			yield return new WaitForSecondsRealtime(0.01f);
		}
		yield return new WaitForSecondsRealtime(10f);
#if UNITY_EDITOR
		UnityEditor.EditorApplication.isPlaying = false;
#else
		// hehe
		while (true) {}
#endif
	}

	private void SpawnGhost()
	{
		Instantiate(ghostPrefab, ec.TileToWorldPos(ghostPos, 0.4f), Quaternion.identity);
	}

	public Texture2D helpPoster;

	IEnumerator DoTheWalls()
	{
		// Make it all hell
		foreach (MeshRenderer mr in ec.transform.GetComponentsInChildren<MeshRenderer>())
		{
			if (!mr.material.shader.name.Contains("Tile")) continue;
			mr.material.SetTexture("_MainTex", helpPoster);
			mr.material.SetTexture("_OverlayTex", helpPoster);
			yield return new WaitForSecondsRealtime(0.02f);
		}
		yield return new WaitForSeconds(20f);

		// Make sure the fog is coming
		twMan.PlayTweenSingle(ref fogEnd, new Tween(5f, new Linear(), fogEnd, 0f));
		yield return new WaitForSeconds(7f);
#if UNITY_EDITOR
		UnityEditor.EditorApplication.isPlaying = false;
#else
		Application.Quit();
#endif
	}

	private void InitEnding()
	{
		disableEnding = true; // Make sure we can't get it twice
		RevealWalls(combinationWalls);
		audMan.PlaySound(combinationCorrectSound);
		HUDManager.Instance.SetPauseEnabled(false); // Now needed due to tapes
		HUDManager.Instance.Disable();

		// Get rid of diary entries
		foreach (GameObject obj in diaryEntryObjects)
			Destroy(obj);

		switch (currentCombination)
		{
			case "DEAD":
				FlagManager.Instance.SetFlag(0, 1);
				FlagManager.Instance.Save();
				FlagManager.Instance.SaveSettings();
				ec.stopLightingUpdate = true;
				ec.FillLightmap(new Color(0.05f, 0.05f, 0.05f));
				ec.ambientColor = new Color(0.05f, 0.05f, 0.05f);
				StartCoroutine(FriendSpawn());
				foreach (LightController light in ec.lights)
					light.visible = false;

				break;
			case "HELP":
				MusicManager.Instance.StopAll();
				MusicManager.Instance.PlayTrack(helpAmbience);
				FlagManager.Instance.SetFlag(1, 1);
				FlagManager.Instance.Save();
				FlagManager.Instance.SaveSettings();
				SpawnGhost();
				ec.stopLightingUpdate = true;
				Shader.SetGlobalColor("_FogColor", Color.black);
				fogEnd.Set(12f);
				ec.FillLightmap(new Color(0.3f, 0f, 0f));
				ec.ambientColor = new Color(0f, 0f, 0.2f);
				foreach (LightController light in ec.lights)
				{
					light.visible = false;
				}
				break;
		}
	}

	public override void Broadcast(string msg)
	{
		switch (msg)
		{
			case "ghost":
				StartCoroutine(DoTheWalls());
				break;
			case "diaryEntryCollect":
				collected++;
				UpdateHudObjectiveCount();
				HUDManager.Instance.objectiveFormat = LocalizationManager.Instance.GetLocalizedString("HUD_ObjectiveFormatDiary", LangStringType.Menu)[0].Replace("?", maximum.ToString());
				break;
		}
	}

	private void Update()
	{
		Shader.SetGlobalFloat("_FogEnd", fogEnd);
		glitchStrength = Mathf.Max(0f, glitchStrength -= Time.deltaTime);
		if (exitCount > 0 && exitCount < 4)
		{
			plr.fovOffset = Mathf.Lerp(plr.fovOffset, MusicManager.Instance.loudness * 100f, Time.unscaledDeltaTime * 10);
			Shader.SetGlobalFloat("_VertexGlitchStrength", glitchStrength);
		}
		else
		{
			Shader.SetGlobalFloat("_VertexGlitchStrength", 0f);
			glitchStrength = 0f;
			plr.fovOffset = 0f;
		}

		if (validCombinations.Contains(currentCombination) && !disableEnding)
			InitEnding();
	}

	private void LateUpdate()
	{
		HUDManager.Instance.objectiveFormat = formatString.Replace("?", maximum.ToString());
	}

	public override bool CanBlockExit()
	{
		return exitCount < 4 && !finalExit && collected >= maximum;
	}

	public override void OnTriggerCreate(TriggerType type, int id, TriggerController trigger)
	{
		// empty for now
	}
}
