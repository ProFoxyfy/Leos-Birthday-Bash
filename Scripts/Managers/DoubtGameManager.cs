using System.Collections;
using System.Collections.Generic;
using TweenX;
using UnityEngine;
using UnityEngine.Diagnostics;

public class DoubtGameManager : BaseGameManager
{
	public TextAsset levelData;
	public TextAsset posterData;

	public int collected = 0;
	public int maximum = 0;

	Dictionary<int, bool> reachedExits = new Dictionary<int, bool>();
	int exitCount = 0;

	public AudioObject exit1Clip;
	public AudioObject exit2Clip;
	public AudioObject exit3Clip;
	public AudioObject bossLoopIntro;
	public AudioObject bossLoop;
	public AudioObject allPresents;
	public AudioManager doubtSpeaker;
	PremadeGenerator generator;

	private float musicBPM = 170f;
	private float time = 0f;
	private float strength = 0.25f;
	private float abberationStrength = 0f;

	public AudioObject school;

	private GameObject objectivePrefab;
	private GameObject projectilePrefab;
	private GameObject retryScreenPrefab;
	private GameObject retryScreen;

	private ContinueScreen retryScript;

	private PlayerManager plr;

	Doubt npcLeo;

	private float glitchStrength = 0f;

	private string formatString = "";

	private bool finalExit = false;
	private int finalExitId = 0;

	internal int doubtHealth = 8;

	public override void Init()
	{
		ec = EnvironmentController.Instance;
		ec.itmData = Resources.Load<ItemData>("BaseItems");
		objectivePrefab = Resources.Load<GameObject>("Objective");
		projectilePrefab = Resources.Load<GameObject>("Projectile");
		balloonPrefab = Resources.Load<GameObject>("Balloon");

		retryScreenPrefab = Resources.Load<GameObject>("RetryScreen");
		retryScreen = Instantiate(retryScreenPrefab, null);
		retryScreen.SetActive(false);
		retryScript = retryScreen.GetComponentInChildren<ContinueScreen>();

		generator = gameObject.AddComponent<PremadeGenerator>();
		generator.style = style;
		generator.npcs[0] = null;
		generator.npcs[1] = Resources.Load<GameObject>("NPCs/Doubt");
		generator.Generate(levelData, posterData);

		foreach (LightController light in ec.lights)
		{
			if (light.lightColor == Color.white)
			{
				light.lightRange -= 0.3f;
				light.lightColor = new Color(0.7f, 0.7f, 0.7f);
			}
		}

		int targetRedLights = 2;

		for (int i = 0; i < targetRedLights; i++)
			ec.lights[Random.Range(0, ec.lights.Count - 1)].lightColor = Color.red;

		StyleLevel();
		plr = ec.GetPlayer(0);
		formatString = LocalizationManager.Instance.GetLocalizedString("HUD_ObjectiveFormat", LangStringType.Menu)[0];
		HUDManager.Instance.objectiveFormat = formatString.Replace("?", maximum.ToString());
	}

	private void Start()
	{
		MusicManager.Instance.PlayTrack(school);
	}

	public override void OnObjectiveCollect(ObjectiveController objective)
	{
		Doubt leo = null;
		if (collected == 0)
		{
			foreach (BaseNPC npc in ec.npcs)
			{
				leo = null;
				npc.gameObject.TryGetComponent<Doubt>(out leo);
				if (leo != null)
					npcLeo = leo;
				npc.gameObject.SetActive(leo != null);
			}
			npcLeo.Anger(0.02f);
		}

		npcLeo.Anger(0.04f);
		npcLeo.Hear(plr.transform.position, 45);

		collected++;

		if (collected == maximum)
			doubtSpeaker.PlaySound(allPresents);

		HUDManager.Instance.objectiveCount = collected;
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
						light.lightColor = Color.white;
						yield return new WaitForSeconds(0.3f);
					}
					lightingStage1Finished = true;
					break;
			}
		}
	}

	private void FinaleLogic()
	{
		if (exitCount == 4) return;
		if (exitCount > 1) return;

		MusicManager.Instance.sampleLoudness = true;
		npcLeo.Anger(0.2f);
		HUDManager.Instance.Break();

		ec.GetPlayer(0).staminaRegenTime = 0;
		ec.GetPlayer(0).staminaRegen = 0f;
		ec.GetPlayer(0).stamina = 0f;
		ec.GetPlayer(0).disableSprint = true;
		ec.GetPlayer(0).walkSpeed = 3.5f;
		ec.GetPlayer(0).runSpeed = 3.5f;
		ec.GetPlayer(0).ChangeFOV(80f);
	}

	private void ResetEnvironment()
	{
		StopAllCoroutines();
		MusicManager.Instance.sampleLoudness = false;
		HUDManager.Instance.Unbreak();
		ec.GetPlayer(0).staminaRegenTime = 2;
		ec.GetPlayer(0).walkSpeed = 1.5f;
		ec.GetPlayer(0).runSpeed = 3f;
		ec.GetPlayer(0).staminaRegen = 0.05f;
		ec.GetPlayer(0).stamina = 100f;
		ec.GetPlayer(0).staminaDecrement = 0f;
		ec.GetPlayer(0).ChangeFOV(60f);
		ec.stopLightingUpdate = false;
	}

	private void ProgressFinaleLighting()
	{
		if (exitCount > 1) return;
		StartCoroutine(FinaleLighting());
	}

	[EditorCools.Button]
	private void DoFinalExit()
	{
		collected = maximum;

		exitCount = 1;
		this.FinaleLogic();
		this.ProgressFinaleLighting();

		reachedExits[3] = true;
		reachedExits[1] = true;
		reachedExits[2] = true;
		generator.BlockExit(3);
		generator.BlockExit(1);
		generator.BlockExit(2);

		exitCount = 3;

		this.ProgressMusic();
		this.ProgressFinaleLighting();
	}

	public override void OnGameTriggerEnter(TriggerType type, int id, TriggerController trigger)
	{
		DebugManager.Log("Trigger type: " + type.ToString());
		switch (type)
		{
			case TriggerType.ExitWallTrigger:
				if (reachedExits.ContainsKey(id)) break;
				if (collected < maximum) break;
				if (finalExit) return;

				npcLeo.Hear(plr.transform.position, 80);

				DebugManager.Log($"Reached exit w/ id {id}, n exits reached: {exitCount}");
				if (exitCount == 3)
				{
					plr.ClearInventory();
					MusicManager.Instance.StopAll();
					finalExitId = id;
					finalExit = true;
					TileController signTile = ec.exitSigns[id];
					npcLeo.rushTarget = signTile.transform;
					npcLeo.state = DoubtState.Rush;
					ResetEnvironment();
					reachedExits[id] = true;
					exitCount++;
					ec.exitLights[id].visible = false;
					break;
				}

				ec.exitLights[id].visible = false;
				exitCount++;
				reachedExits[id] = true;
				this.ProgressMusic();
				this.FinaleLogic();
				this.ProgressFinaleLighting();
				break;
			case TriggerType.ExitTrigger:
				if (reachedExits.ContainsKey(id)) break;
				if (collected < maximum) break;
				exitCount++;
				DebugManager.Log($"Reached final exit w/ id {id}");
				reachedExits[id] = true;
				this.ProgressMusic();
				this.FinaleLogic();
				this.ProgressFinaleLighting();
				break;
		}
	}

	public override void HandleMarker(int id, Pos2 position)
	{
		// we dont need half of this
		switch (id)
		{
			case 0:
				ec.PlaceMarker(objectivePrefab, position, 0.51f);
				maximum++;
				break;
			default:
				break;
		}
	}

	public AudioObject doubtIntro;
	public AudioObject doubtHitFirst;
	public AudioObject doubtHit;

	private bool doExit = false;

	int projectileCount = 0;

	private void SpawnProjectile(Pos2 position)
	{
		projectileCount++;
		Instantiate(projectilePrefab, ec.TileToWorldPos(position, 0.4f), Quaternion.identity, transform);
	}

	private void SpawnProjectileRandom()
	{
		Pos2 position = ec.GetRandomNavigableTilePos();
		SpawnProjectile(position);
	}

	int targetCount = 23;

	IEnumerator RespawnProjectiles()
	{
		while (Time.timeScale > 0f)
		{
			if (projectileCount < targetCount)
				SpawnProjectileRandom();

			yield return new WaitForSeconds(1.5f);
		}
	}

	private void EndIntro()
	{
		npcLeo.state = DoubtState.Chase;
		MusicManager.Instance.PlayTrack(bossLoop);
		StartCoroutine(StartShake());
	}

	public bool cancelShake = false;

	IEnumerator StartShake()
	{
		while (!cancelShake)
		{
			yield return new WaitForSecondsRealtime(60f / musicBPM);
			Shader.SetGlobalVector("_VertexGlitchSeed", new Vector2(Random.Range(-10000, 10000), Random.Range(-10000, 10000)));
			glitchStrength = strength;
		}
	}

	public override void Broadcast(string msg)
	{
		// turned this into a switch statement for my sanity
		switch (msg)
		{
			case "doubtReached":
				plr.disableSprint = true;
				doExit = true;
				Pos2 position = new Pos2(ec.exitSigns[finalExitId].tilePos + Vector2Int.right * 2);

				npcLeo.health = doubtHealth;

				SpawnProjectile(position);
				StartCoroutine(RespawnProjectiles());

				HUDManager.Instance.Disable();
				HUDManager.Instance.SetPauseEnabled(false);
				plr.ClearInventory();

				generator.BlockExit(finalExitId);

				MusicManager.Instance.PlayTrack(this.bossLoopIntro);
				npcLeo.audMan.PlaySound(doubtIntro);

				Vector3 targetPos = ec.TileToWorldPos(new Pos2(ec.exitSigns[finalExitId].tilePos), 0f);

				// Whoever decided the CharacterController prioritizes over .position,
				// I will skin you alive. (in Minecraft)
				plr.controller.enabled = false;
				plr.transform.position = targetPos;
				plr.controller.enabled = true;

				break;
			case "doubtHit":
				if (npcLeo.health == doubtHealth)
				{
					// shut up doubt bla bla thank you
					npcLeo.audMan.StopAllSounds();

					// make him threaten you
					npcLeo.audMan.PlaySound(doubtHitFirst);
					Invoke("EndIntro", doubtHitFirst.clip.length);
					return;
				}

				// Removed duplicate health deduction, that was causing the problem woops

				abberationStrength = 0.3f;
				npcLeo.audMan.PlayOneShot(doubtHit);

				if (npcLeo.health == 0)
				{
					npcLeo.state = DoubtState.Idle;
					// this isn't supposed to happen?
					Utils.ForceCrash(ForcedCrashCategory.Abort);
				}
				break;
			case "projectileDie":
				projectileCount--;
				break;
			case "tagged":
				ec.UnloadLevel();
				retryScreen.SetActive(true);
				retryScript.FadeIn();
				break;
		}
	}

	private void Update()
	{
		glitchStrength = Mathf.Max(0f, glitchStrength - Time.deltaTime);
		abberationStrength = Mathf.Max(0f, abberationStrength - Time.deltaTime * 0.5f);
		Shader.SetGlobalVector("CA_Intense", new Vector2(-abberationStrength, 1f));
		Shader.SetGlobalFloat("CA_Mult", 1f);
		Shader.SetGlobalFloat("CA_Blend", 1f);
		Shader.SetGlobalFloat("_VertexGlitchStrength", glitchStrength);
	}

	private void LateUpdate()
	{
		HUDManager.Instance.objectiveFormat = formatString.Replace("?", maximum.ToString());
	}

	public override bool CanBlockExit()
	{
		return (collected == maximum && exitCount < 4) || doExit;
	}

	public override void OnTriggerCreate(TriggerType type, int id, TriggerController trigger)
	{
		// don't need this
	}
}
