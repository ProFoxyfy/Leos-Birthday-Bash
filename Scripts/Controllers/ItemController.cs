using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ItemController : MonoBehaviour, IInteractable
{
	public ItemType item;
	private PlayerManager plr;
	private SpriteRenderer sprite;
	private EnvironmentController ec;

	private void Awake()
	{
		sprite = GetComponent<SpriteRenderer>();
		ec = EnvironmentController.Instance;
		plr = ec.GetPlayer(0);
	}

	public void Initialize()
	{
		sprite.sprite = ec.itmData.sprite[item];
	}

	public void Use()
	{
		plr.GiveItem(ec.itmData.itemScripts[item], this);
	}
}
