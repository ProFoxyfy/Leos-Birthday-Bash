using System.Collections;
using TweenX;
using TweenX.EasingStyles;
using TweenX.EasingStyles.Advanced;
using UnityEngine;
using UnityEngine.SceneManagement;

public enum DoubtState
{
	Normal,
	Rush,
	Idle,
	Chase
}

public class Doubt : BaseNPC
{
	float tempAnger = 0f;
	float tempAngerCalmDown = 0.06f;
	float anger = 0f;

	float speed = 9.3f;
	float slapDelay = 1.5f;
	float slapDuration = 0.2f;
	float time = 0f;
	float slapTime = 0f;

	public AnimationCurve slapDelayCurve = new AnimationCurve();
	public float slapDelayScale = 2.6f;

	byte currentNoiseValue = 0;

	int layerMask;
	int blockerMask;

	bool jumpscare = false;

	public AudioManager audMan;

	public AudioObject glitchVoiceline;

	public DoubtState state;

	Transform player;
	PlayerManager playerMan;

	internal bool blind = false;
	internal bool canEndGame = true;

	internal bool doExitRush = false;

	public Transform rushTarget;

	private BehaviorEvaluator evaluator;

	private XFloat musicSpeed = 1f;
	private TweenManager twMan;

	private void Start()
	{
		twMan = gameObject.AddComponent<TweenManager>();
		TopNode topNode = new TopNode();
		Node chaseNode = new ChaseNode(this);
		Node wanderNode = new WanderNode(this);

		NOTDecorator notGate = new NOTDecorator();

		Connection topNoiseYes = new ConditionalConnection(topNode, chaseNode, null, 1, NoiseAboveZero);
		Connection topNoiseNo = new ConditionalConnection(topNode, wanderNode, notGate, 1, NoiseAboveZero);
		Connection chaseFinish = new ConditionalConnection(chaseNode, topNode, null, 1, DestinationReached);
		Connection wanderFinish = new ConditionalConnection(wanderNode, topNode, null, 1, DestinationReached);

		topNode.connections.Add(topNoiseYes);
		topNode.connections.Add(topNoiseNo);
		chaseNode.connections.Add(chaseFinish);
		wanderNode.connections.Add(wanderFinish);

		evaluator = new BehaviorEvaluator(topNode, new HighestPrioritySelector());

		evaluator.Init();

		layerMask = LayerMask.GetMask("Walls");
		blockerMask = LayerMask.GetMask("NPCBlocker");
		player = FindObjectOfType<PlayerManager>().transform;
		playerMan = EnvironmentController.Instance.GetPlayer(0);
		audMan = GetComponent<AudioManager>();
	}

	private bool NoiseAboveZero()
	{
		return currentNoiseValue > 0;
	}

	private bool DestinationReached()
	{
		return GetDistanceFromDestination() < 0.5f;
	}

	public void Anger(float amount)
	{
		anger += amount;
	}

	public void TempAnger(float amount)
	{
		tempAnger += amount;
	}

	public void Hear(Vector3 position, byte value)
	{
		if (currentNoiseValue > value) return;
		currentNoiseValue = value;
		nextTarget = position;
	}

	private Vector3 ZeroY(Vector3 vector)
	{
		return new Vector3(vector.x, 0f, vector.z);
	}

	public bool CanReach(Vector3 position, int mask)
	{
		RaycastHit hit;
		Vector3 direction = position - transform.position;
		return !Physics.Raycast(transform.position, direction, out hit, direction.magnitude, mask, QueryTriggerInteraction.Ignore);
	}

	private float GetDistanceFromDestination()
	{
		float dist = Mathf.Abs(Vector3.Distance(ZeroY(transform.position), ZeroY(target)));
		if (dist < 1.5f && !CanReach(target, blockerMask))
		{
			return 0f;
		}
		return dist;
	}

	public int health = 15;
	public float rushSpeed = 10f;
	private bool reached = false;

	IEnumerator Stun()
	{
		this.state = DoubtState.Idle;
		yield return new WaitForSecondsRealtime(1f);
		this.state = DoubtState.Chase;
	}

	IEasingFunction ease = new CubicIn();

	public void Hit()
	{
		if (this.state == DoubtState.Idle) return;
		EnvironmentController.Instance.gameManager.Broadcast("doubtHit");
		health--;
		if (health <= 1)
		{
			audMan.PlaySound(glitchVoiceline);
			playerMan.frozen = true;
			state = DoubtState.Idle;
			EnvironmentController.Instance.ambientColor = Color.black;
			StartCoroutine(AllOfTheLights());
			((DoubtGameManager)EnvironmentController.Instance.gameManager).cancelShake = true;
			twMan.PlayTweenSingle(ref musicSpeed, new Tween(8f, ease, 1f, 0f));
			Invoke("InitFinale", 9f);
			return;
		}
		StartCoroutine(Stun());
	}

	// You'd think this'd be really long. It is not.
	private void InitFinale()
	{
		GlobalsManager.Instance.currentMode = GameMode.Memory;
		SceneManager.LoadScene("Game");
	}

	// ALL OF THE LIGHTS BABY
	IEnumerator AllOfTheLights()
	{
		foreach (LightController light in EnvironmentController.Instance.lights)
		{
			light.visible = false;
			yield return new WaitForSecondsRealtime(0.1f);
		}

		EnvironmentController.Instance.FillLightmap(Color.black);
	}

	private void Update()
	{
		if (jumpscare)
		{
			Time.timeScale = 0;
			agent.isStopped = true;
			agent.speed = 0;
			return;
		}

		this.canEndGame = state == DoubtState.Normal || state == DoubtState.Chase;
		MusicManager.Instance.audioSource.pitch = musicSpeed;

		switch (state)
		{
			case DoubtState.Normal:
				break;
			case DoubtState.Rush:
				target = this.rushTarget.position;
				agent.speed = rushSpeed;
				if (GetDistanceFromDestination() < 0.4f && !reached)
				{
					reached = true;
					EnvironmentController.Instance.gameManager.Broadcast("doubtReached");
				}
				break;
			case DoubtState.Chase:
				target = player.position;
				agent.speed = 3f + ((10f - (0.9f * health)) / 5f);
				playerMan.walkSpeed = 3f + ((10f - health) / 3f);
				break;
			case DoubtState.Idle:
				agent.speed = 0;
				break;
		}

		if (state != DoubtState.Normal)
			return;

		if (CanReach(player.position, layerMask) && !blind)
		{
			currentNoiseValue = 127;
			nextTarget = player.position;
		}

		anger = Mathf.Min(anger, 1);

		destinationReached = DestinationReached();

		evaluator.Tick();

		if (destinationReached && currentNoiseValue > 0)
			currentNoiseValue = 0;

		tempAnger = Mathf.Max(tempAnger - (tempAngerCalmDown * Time.deltaTime), 0f);
		slapDelay = slapDelayCurve.Evaluate(anger + tempAnger) * slapDelayScale;

		// PhysX is deaf apparently
		transform.position = new Vector3(transform.position.x, 0f, transform.position.z);
	}

	private void LateUpdate()
	{
		if (jumpscare) return;
		agent.SetDestination(target);
		if (state != DoubtState.Normal)
			return;
		if (time >= slapDelay)
		{
			slapTime = slapDuration;
			time = 0f;
		}
		else
			time += Time.deltaTime;
		slapTime = Mathf.Max(slapTime - Time.deltaTime, 0);

		this.agent.speed = slapTime > 0 ? speed : 0f;
		if (slapTime <= 0)
		{
			this.agent.ResetPath();
			this.agent.velocity = Vector3.zero;
		}

		base.UpdateInteractions();
	}

	private void OnTriggerEnter(Collider other)
	{
		if (!canEndGame) return;
		if (jumpscare) return;
		if (!other.transform.CompareTag("Player")) return;
		MusicManager.Instance.StopAll();
		HUDManager.Instance.pauseEnabled = false;
		jumpscare = true;
		EnvironmentController.Instance.stopLightingUpdate = true;
	}
}
