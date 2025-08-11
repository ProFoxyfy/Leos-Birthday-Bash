using System.Threading.Tasks;
using UnityEngine;

public class Leo : BaseNPC
{
	float tempAnger = 0f;
	float tempAngerCalmDown = 0.06f;
	float anger = 0f;

	public Sprite jollySprite;
	public Sprite happySprite;
	public Sprite angrySprite;
	public Sprite happySpriteJolly;

	float speed = 7f;
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

	public AudioObject slap;

	public AudioManager audMan;
	public AudioObject[] jumpscares;

	Transform player;

	internal bool blind = false;
	internal bool canEndGame = true;
	internal bool frozen = false;

	private BehaviorEvaluator evaluator;

	public void ChangeSprite(bool happy)
	{
		if (happy)
			GetComponentInChildren<SpriteRenderer>().sprite = FlagManager.tempFlags["jollyMode"] ? happySpriteJolly : happySprite;
		else
			GetComponentInChildren<SpriteRenderer>().sprite = FlagManager.tempFlags["jollyMode"] ? jollySprite : angrySprite;
	}

	private void Start()
	{
		if (FlagManager.tempFlags["jollyMode"])
			GetComponentInChildren<SpriteRenderer>().sprite = jollySprite;
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

	private void Update()
	{
		if (jumpscare)
		{
			Time.timeScale = 0;
			agent.isStopped = true;
			agent.speed = 0;
			return;
		}

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

		transform.position = new Vector3(transform.position.x, 0f, transform.position.z);
	}

	private void LateUpdate()
	{
		if (jumpscare) return;

		if (frozen)
		{
			agent.SetDestination(transform.position);
			return;
		}

		agent.SetDestination(target);
		if (time >= slapDelay)
		{
			audMan.PlaySound(this.slap);
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
		jumpscare = true;
		HUDManager.Instance.pauseEnabled = false;
		audMan.PlayOneShot(jumpscares[Random.Range(0, jumpscares.Length - 1)]);
		EnvironmentController.Instance.stopLightingUpdate = true;
	}
}
