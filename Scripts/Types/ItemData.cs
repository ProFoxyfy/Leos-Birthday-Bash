using AYellowpaper.SerializedCollections;
using System;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "NewItemData", menuName = "Custom/Item Data", order = 1)]
public class ItemData : ScriptableObject
{
	internal Dictionary<ItemType, Type> itemScripts = new Dictionary<ItemType, Type>()
	{
		[ItemType.SodyPop] = typeof(ITM_SodyPop),
		[ItemType.ChocolateKiss] = typeof(ITM_ChocoKiss)
	};
	[SerializedDictionary]
	public SerializedDictionary<ItemType, Sprite> smallSprite;
	[SerializedDictionary]
	public SerializedDictionary<ItemType, Sprite> sprite;
	[SerializedDictionary]
	public SerializedDictionary<ItemType, string> nameKeys;
}
