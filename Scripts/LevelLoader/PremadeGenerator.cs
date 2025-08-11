using System;
using System.Collections;
using System.Collections.Generic;
using System.Collections.Specialized;
using System.IO;
using System.Linq;
using Unity.AI.Navigation;
using UnityEngine;
using UnityEngine.AI;
using UnityEngine.Events;

public class PremadeGenerator : MonoBehaviour
{
	LevelData data;
	LevelLoader loader;
	string posterData = "";
	GameObject swingDoorPrefab;
	GameObject doorPrefab;
	GameObject triggerPrefab;
	GameObject obstaclePrefab;
	GameObject itemPrefab;
	EnvironmentController ec;
	public Dictionary<int, GameObject> npcs = new();
	Dictionary<int, Direction> exitDirections = new();
	public LevelStyle style;

	private void Awake()
	{
		npcs.Add(0, Resources.Load<GameObject>("NPCs/HappyLeo"));
		npcs.Add(1, Resources.Load<GameObject>("NPCs/Leo"));

		ec = FindObjectOfType<EnvironmentController>();
		swingDoorPrefab = Resources.Load<GameObject>("WallSD");
		doorPrefab = Resources.Load<GameObject>("WallD");
		itemPrefab = Resources.Load<GameObject>("Item");
		obstaclePrefab = Resources.Load<GameObject>("TileBlocker");
		loader = gameObject.AddComponent<LevelLoader>();
	}

	private void PlaceTile(Vector2Int pos, TileData data)
	{
		ec.PlaceTile(pos, data);
	}

	private TriggerController PlaceTrigger(Vector2Int pos, LevelObject data)
	{
		return ec.PlaceTrigger(pos, data);
	}

	private void PlaceSwingingDoor(LevelObject obj)
	{
		TileController tile = ec.GetTile(obj.position.x, obj.position.y);
		string shortName = DirectionData.dirShortNames[obj.dir];
		Transform wall = tile.transform.Find(shortName);
		GameObject inst = Instantiate(swingDoorPrefab, wall.position, wall.rotation, tile.transform);
	}

	private void PlaceDoor(LevelObject obj)
	{
		TileController tile = ec.GetTile(obj.position.x, obj.position.y);
		string shortName = DirectionData.dirShortNames[obj.dir];
		Transform wall = tile.transform.Find(shortName);
		GameObject inst = Instantiate(doorPrefab, wall.position, wall.rotation, tile.transform);
		DoorScript door = inst.GetComponent<DoorScript>();
		MeshRenderer mr = inst.GetComponent<MeshRenderer>();
		DoorType doorType = obj.type == ObjectType.StaffDoor ? DoorType.Staff : DoorType.Regular;
		door.closeTex = style.doors[doorType][0];
		door.openTex = style.doors[doorType][1];
		mr.material.SetTexture("_OverlayTex", style.doors[doorType][0]);
	}

	private void BlockArea(Pos2 origin, Pos2 end)
	{

		for (int x = end.x; x < origin.x; x++)
		{
			for (int y = origin.y; y < end.y; y++)
			{
				ec.SetTileBlocked(new Pos2(x, y), true);
			}
		}
	}

	private void PlacePoster(PosterData poster)
	{
		TileController ctrl = ec.GetTile(poster.position.x, poster.position.y);
		MeshRenderer wall = ctrl.transform.Find(DirectionData.dirShortNames[poster.dir]).GetComponent<MeshRenderer>();
		string id = poster.id.Trim();
		Texture2D tex = style.posterInfo.posters[id];
		bool isFamilyFriendly = (bool)FlagManager.Instance.GetSetting("familyFriendly");
		if (isFamilyFriendly && style.posterInfo.suggestivePosterIDs.Contains(id))
		{
			tex = style.posterInfo.suggestivePosterReplacements.ContainsKey(id)
				? style.posterInfo.suggestivePosterReplacements[id]
				: style.posterInfo.suggestivePosterFallback;
		}

		if (style.posterInfo.posterMasks.ContainsKey(id))
		{
			wall.material.SetTexture("_MaskTex", style.posterInfo.posterMasks[id]);
		}

		wall.material.SetTexture("_OverlayTex", tex);
	}

	private Dictionary<int, List<TriggerController>> exitWalls = new Dictionary<int, List<TriggerController>>();

	private void SpawnItem(Pos2 pos, ItemType type)
	{
		GameObject inst = Instantiate(itemPrefab, ec.TileToWorldPos(pos, 0.56f), Quaternion.identity);
		ItemController ctrl = inst.GetComponent<ItemController>();
		ctrl.item = type;
		ctrl.Initialize();
	}


	private void SpawnFurniture(string id, LevelObject obj)
	{
		GameObject furnitureObj = Resources.Load<GameObject>("Furniture/" + id);
		TileController ctrl = ec.GetTile(obj.position.x, obj.position.y);
		Vector3 orientation = DirectionData.dirVectorsFurniture[obj.dir]*90f;

		Instantiate(furnitureObj, ec.TileToWorldPos(obj.position, 0f), Quaternion.Euler(orientation), ctrl.transform);
	}

	public void BlockExit(int id)
	{
		Direction dir = exitDirections[id];
		foreach (TriggerController t in exitWalls[id])
		{
			GameObject blocker;
			Transform tile = ec.GetTile(t.position.x, t.position.y).transform;
			if (!tile.Find(obstaclePrefab.name))
				blocker = Instantiate(obstaclePrefab, ec.TileToWorldPos(t.position, 0f), Quaternion.identity, ec.GetTile(t.position.x, t.position.y).transform);
			else
				blocker = tile.Find(obstaclePrefab.name).gameObject;
			blocker.SetActive(ec.gameManager.CanBlockExit());

			Vector2Int n = Vector2Helper.ToVec2Int(t.position.ToVec2Int() - DirectionData.dirVectors[dir]);
			TileController ct = ec.GetTile(n.x, n.y);
			ec.SetTileBlocked(new Pos2(ct.tilePos), ec.gameManager.CanBlockExit());
			GameObject wall = ct.transform.Find(DirectionData.dirShortNamesInverse[dir]).gameObject;
			wall.SetActive(ec.gameManager.CanBlockExit());
		}
	}

	public void Generate(TextAsset level, TextAsset posters)
	{
		RenderSettings.skybox = style.skybox;
		data = loader.LoadFromAsset(level);
		posterData = posters != null ? posters.text : null;

		PlayerManager plr = ec.GetPlayer(0);

		for (int x = 0; x < data.tiles.GetLength(0); x++)
		{
			for (int y = 0; y < data.tiles.GetLength(1); y++)
			{
				TileData tile = data.tiles[x,y];
				if (tile == null) continue;
				PlaceTile(new Vector2Int(x, y), tile);
			}
		}

		if (posterData != null)
		{
			foreach (string entry in posterData.Split('\n'))
			{
				if (string.IsNullOrEmpty(entry.Trim())) continue;
				string[] entryData = entry.Split(':');
				int x = 0;
				int y = 0;
				string id = entryData[3];
				Direction dir = Direction.North;

				int.TryParse(entryData[0], out x);
				int.TryParse(entryData[1], out y);

				dir = DirectionData.nameToDir[entryData[2]];

				Pos2 pos = new Pos2(x, y);

				PosterData poster = new PosterData(pos, id, dir);
				PlacePoster(poster);
			}
		}

		foreach (LevelObject obj in data.objects)
		{
			switch (obj.type)
			{
				case ObjectType.Light:
					ec.PlaceLight(obj.color.ToColor(), obj.range, obj.position);
					break;
				case ObjectType.PlayerSpawn:
					ec.plrSpawn = obj.position.ToVec2();
					break;
				case ObjectType.SwingDoor:
					PlaceSwingingDoor(obj);
					break;
				case ObjectType.Door or ObjectType.StaffDoor:
					PlaceDoor(obj);
					break;
				case ObjectType.ExitTrigger:
					PlaceTrigger(obj.position.ToVec2Int(), obj);
					Instantiate(obstaclePrefab, ec.TileToWorldPos(obj.position, 0f), Quaternion.identity, ec.GetTile(obj.position.x, obj.position.y).transform);
					ec.SetTileBlocked(obj.position, true);
					break;
				case ObjectType.ExitWall:
					if (!exitWalls.ContainsKey(obj.id))
						exitWalls[obj.id] = new List<TriggerController>();
					Vector2 newPos = obj.position.ToVec2Int() - DirectionData.dirVectors[obj.dir];
					TriggerController trig = PlaceTrigger(Vector2Helper.ToVec2Int(newPos), obj);
					exitWalls[obj.id].Add(trig);
					exitDirections[obj.id] = obj.dir;
					trig.onEnter.AddListener(() =>
					{
						BlockExit(obj.id);
					});
					break;
				case ObjectType.ExitTriggerSwingDoor:
					PlaceTrigger(obj.position.ToVec2Int(), obj);
					Instantiate(obstaclePrefab, ec.TileToWorldPos(obj.position, 0f), Quaternion.identity, ec.GetTile(obj.position.x, obj.position.y).transform);
					PlaceSwingingDoor(obj);
					ec.SetTileBlocked(obj.position, true);
					break;
				case ObjectType.NPC:
					ec.SpawnNPC(npcs[0], obj.position);
					ec.SpawnNPC(npcs[1], obj.position).SetActive(false);
					break;
				case ObjectType.Furniture:
					string id = "";
					ec.SetTileBlocked(obj.position, true);
					switch (obj.id)
					{
						case 1:
							id = "ClassTable";
							break;
						case 2:
							id = "Table";
							break;
					}

					SpawnFurniture(id, obj);
					break;
			}
		}

		Pos2 blockOrigin = Pos2.one;
		Pos2 blockEnd = Pos2.one;

		foreach (LevelMarker marker in data.markers)
		{
			// NOTE: 254 and 255 are RESERVED for blocking a single area from NPCs.
			// 8-20 are reserved for items.
			switch (marker.data)
			{
				// items
				case 8:
					SpawnItem(marker.position, ItemType.SodyPop);
					break;
				case 9:
					SpawnItem(marker.position, ItemType.ChocolateKiss);
					break;
				// NPC blocked area
				case 254:
					blockOrigin = marker.position;
					break;
				case 255:
					blockEnd = marker.position;
					break;
				default:
					break;
			}

			ec.gameManager.HandleMarker(marker.data, marker.position);
		}
		BlockArea(blockOrigin, blockEnd);
	}
}
