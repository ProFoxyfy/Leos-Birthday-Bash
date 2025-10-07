using NUnit.Framework;
using System.Collections.Generic;
using UnityEngine;

public class FunMenuManager : MonoBehaviour
{
	public Button leoAdd;
	public Button leoRemove;
	public Button leoAnger;
	public Button leoCalm;

	public Button nicoAdd;
	public Button nicoRemove;

	public Button plrWP;
	public Button plrWM;

	public Button plrRP;
	public Button plrRM;

	public Button presentP;
	public Button presentM;

	public Checkbox noClip;
	public Checkbox infItems;

	public Checkbox blindLeo;

	public GameObject funMenuButton;

	EnvironmentController ec;
	PlayerManager plr;
	CharacterController plrChar;

	float defaultRadius = 0.25f;

	GameObject leoPrefab;
	GameObject nicoPrefab;

	List<Leo> leos = new();
	List<CrazyNico> nicos = new();

	private void Awake()
	{
		funMenuButton.SetActive(FlagManager.Instance.GetFlag(3) == 1);

		// AAA
		ec = EnvironmentController.Instance;
		plr = ec.GetPlayer(0);
		plrChar = plr.GetComponent<CharacterController>();

		leos.Add(Object.FindAnyObjectByType<Leo>(FindObjectsInactive.Include));

		leoPrefab = Resources.Load<GameObject>("NPCs/Leo");
		nicoPrefab = Resources.Load<GameObject>("NPCs/Nico");

		leoAdd.onClick.AddListener(() =>
		{
			Pos2 position = ec.GetRandomNavigableTilePos();
			leos.Add(ec.SpawnNPC(leoPrefab, position).GetComponent<Leo>());
		});

		leoRemove.onClick.AddListener(() =>
		{
			foreach (Leo leo in leos)
				Destroy(leo.gameObject);
		});

		leoAnger.onClick.AddListener(() =>
		{
			foreach (Leo leo in leos)
				leo.Anger(0.05f);
		});

		leoCalm.onClick.AddListener(() =>
		{
			foreach (Leo leo in leos)
				leo.Anger(-0.05f);
		});

		nicoAdd.onClick.AddListener(() =>
		{
			nicos.Add(ec.SpawnNPC(nicoPrefab, ec.GetRandomNavigableTilePos()).GetComponent<CrazyNico>());
		});

		nicoRemove.onClick.AddListener(() =>
		{
			foreach (CrazyNico nico in nicos)
				Destroy(nico.gameObject);
		});

		plrWP.onClick.AddListener(() =>
		{
			plr.walkSpeed += 0.5f;
		});

		plrWM.onClick.AddListener(() =>
		{
			plr.walkSpeed -= 0.5f;
		});

		plrRP.onClick.AddListener(() =>
		{
			plr.runSpeed += 0.5f;
		});

		plrRM.onClick.AddListener(() =>
		{
			plr.runSpeed -= 0.5f;
		});

		presentP.onClick.AddListener(() =>
		{
			switch (GlobalsManager.Instance.currentMode)
			{
				case GameMode.Story:
					((ClassicGameManager)ec.gameManager).collected++;
					break;
				case GameMode.Endless:
					((EndlessGameManager)ec.gameManager).collected++;
					break;
				case GameMode.Doubt:
					((DoubtGameManager)ec.gameManager).collected++;
					break;
				default:
					break;
			}
		});

		presentM.onClick.AddListener(() =>
		{
			switch (GlobalsManager.Instance.currentMode)
			{
				case GameMode.Story:
					((ClassicGameManager)ec.gameManager).collected--;
					break;
				case GameMode.Endless:
					((EndlessGameManager)ec.gameManager).collected--;
					break;
				case GameMode.Doubt:
					((DoubtGameManager)ec.gameManager).collected--;
					break;
				default:
					break;
			}
		});
	}

	private void Update()
	{
		if (noClip.value)
			plrChar.radius = float.PositiveInfinity;
		else
			plrChar.radius = defaultRadius;

		plr.infItems = infItems.value;
	}

	private void LateUpdate()
	{
		// inefficient but meh
		foreach (Leo leo in leos)
		{
			if (leo == null) continue;
			leo.canEndGameFun = !noClip.value;
			leo.blindFun = blindLeo.value;
		}
	}
}