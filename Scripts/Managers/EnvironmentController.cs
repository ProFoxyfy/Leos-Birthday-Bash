using AYellowpaper.SerializedCollections;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using Unity.AI.Navigation;
using UnityEngine;
using UnityEngine.SceneManagement;

public class EnvironmentController : Singleton<EnvironmentController>
{
	public GameModeData gameData;

	[Header("Lighting")]
	public Color ambientColor = Color.black;
	public LightBlendMode lightBlendMode = LightBlendMode.Additive;
	[SerializeField, ReadOnly]
	internal Texture2D lightMap;
	private Vector2Int mapSize;
	internal bool stopLightingUpdate = false;

	[Header("Data")]
	public List<TileController> tiles;
	public TileController[,] orderedTiles;
	public SerializedDictionary<Pos2, bool> blockedTiles = new();
	public List<LightController> lights = new();
	public List<BaseNPC> npcs = new();
	public Vector2 plrSpawn;
	public ItemData itmData;

	[Header("Instances")]
	private NavMeshSurface nms;
	public BaseGameManager gameManager;

	private List<PlayerManager> players;

	private GameObject tilePrefab;
	private GameObject triggerPrefab;
	private GameObject exitSignPrefab;

	private Transform tileContainer;
	private Transform lightContainer;
	private Transform triggerContainer;
	private Transform aiContainer;
	private Transform markerContainer;

	private void Awake()
	{
		plrSpawn = Vector2.zero;

		UICamera.Instance.SetOverlay(true);

		Instantiate(gameData.gameManagers[GlobalsManager.Instance.currentMode], null);

		gameManager = FindObjectOfType<BaseGameManager>();

		nms = GetComponent<NavMeshSurface>();
		players = FindObjectsOfType<PlayerManager>().ToList();

		mapSize = new Vector2Int(100, 100);
		this.orderedTiles = new TileController[mapSize.x, mapSize.y];

		tilePrefab = Resources.Load<GameObject>("Tile");
		triggerPrefab = Resources.Load<GameObject>("BaseTileTrigger");
		exitSignPrefab = Resources.Load<GameObject>("ExitSign");

		tileContainer = new GameObject("Tiles").transform;
		tileContainer.parent = transform;
		lightContainer = new GameObject("Lights").transform;
		lightContainer.parent = transform;
		triggerContainer = new GameObject("Triggers").transform;
		triggerContainer.parent = transform;
		aiContainer = new GameObject("AI").transform;
		aiContainer.parent = transform;
		markerContainer = new GameObject("Markers").transform;
		markerContainer.parent = transform;

		gameManager.Init();

		nms.BuildNavMesh();

		lightMap = new Texture2D(mapSize.x + 1, mapSize.y + 1, TextureFormat.RGB24, false);
		lightMap.filterMode = FilterMode.Point;
		this.FillLightmap(this.ambientColor);
		lightMap.Apply(false, false);

		Shader.SetGlobalTexture("_Lightmap", lightMap);

		GetPlayer(0).itmData = this.itmData;
		CharacterController plrChrCtrl = GetPlayer(0).GetComponent<CharacterController>();
		plrChrCtrl.enabled = false;
		GetPlayer(0).transform.localPosition = TileToWorldPos(new Pos2(plrSpawn), 0.25f);
		plrChrCtrl.enabled = true;

		// init light data
		foreach (TileController t in tiles)
		{
			LightData data = new LightData();

			foreach (LightController light in lights)
			{
				int rawDist = GetRawDistance(t.tilePos.x, t.tilePos.y, (int)light.transform.position.x, (int)light.transform.position.z);
				if (rawDist > (int)light.lightRange) continue;
				data.distances.Add(rawDist);
				data.lights.Add(light);
			}

			t.lightData = data;
		}
	}

	private Vector2Int GetTilePosition(Vector3 pos)
	{
		return new Vector2Int((int)pos.x, (int)pos.z);
	}

	private void GetNeighbors(List<TileController> list, TileController origin)
	{
		Vector2Int pos = Vector2Int.zero;
		foreach (Vector2 vector in DirectionData.dirVectors.Values)
		{
			pos.x = origin.tilePos.x + (int)vector.x;
			pos.y = origin.tilePos.y + (int)vector.y;
			list.Add(orderedTiles[pos.x, pos.y]);
		}
	}

	private int GetRawDistance(int aX, int aY, int bX, int bY)
	{
		return Mathf.Abs(aX - bX) + Mathf.Abs(aY - bY);
	}

	private int GetDistance(Vector2Int origin, Vector2Int target)
	{
		int x = Mathf.Abs(origin.x - target.x);
		int y = Mathf.Abs(origin.y - target.y);
		if (x > y)
		{
			return 14 * y + 10 * (x - y);
		}
		return 14 * x + 10 * (y - x);
	}

	public bool IsTileBlocked(Pos2 position)
	{
		if (!blockedTiles.ContainsKey(position)) return false;
		return blockedTiles[position];
	}

	public void ForceUpdateLighting()
	{
		foreach (TileController t in tiles)
		{
			t.UpdateTileLighting();
		}
	}

	public TileController GetTile(int x, int y)
	{
		return this.orderedTiles[x, y];
	}

	public Vector3 TileToWorldPos(Pos2 rawPos, float height)
	{
		return new Vector3(rawPos.x, height, rawPos.y);
	}

	public PlayerManager GetPlayer(int index)
	{
		return players[index];
	}

	public void PlaceTile(Vector2Int p, TileData data)
	{
		Vector2Int pos = p;
		GameObject n = Instantiate(tilePrefab, new Vector3(pos.x, 0f, pos.y), Quaternion.identity, tileContainer);
		n.name = $"{pos.x}, {pos.y}";
		TileController t = n.GetComponent<TileController>();
		t.tilePos = pos;
		foreach (KeyValuePair<Direction, string> entry in DirectionData.dirShortNames)
		{
			n.transform.Find(entry.Value).gameObject.SetActive(data.walls[entry.Key]);
		}
		this.tiles.Add(t);
		this.orderedTiles[pos.x, pos.y] = t;
	}

	public GameObject PlaceMarker(GameObject prefab, Pos2 pos, float height = 0f)
	{
		return Instantiate(prefab, new Vector3(pos.x, height, pos.y), Quaternion.identity, markerContainer);
	}

	public GameObject SpawnNPC(GameObject prefab, Pos2 position)
	{
		if (prefab == null) return null;
		GameObject obj = Instantiate(prefab, TileToWorldPos(position, 0f), Quaternion.identity, aiContainer);
		this.npcs.Add(obj.GetComponent<BaseNPC>());
		return obj;
	}

	public Dictionary<int, LightController> exitLights = new();
	public Dictionary<int, TileController> exitSigns = new();

	public TriggerController PlaceTrigger(Vector2Int pos, LevelObject data)
	{
		GameObject newTrigger = Instantiate(triggerPrefab, new Vector3(pos.x, 0.5f, pos.y), Quaternion.identity, triggerContainer);
		TriggerController ctrl = newTrigger.GetComponent<TriggerController>();
		switch (data.type)
		{
			case ObjectType.ExitTrigger or ObjectType.ExitTriggerSwingDoor:
				ctrl.type = TriggerType.ExitTrigger;
				LightController light = this.PlaceLight(new Color(0.55f, 1f, 0.4f), 2f, data.position);
				light.gameObject.name = "ExitLight";
				exitLights.Add(data.id, light);

				Pos2 p = new Pos2(
					(int)(data.position.x + (DirectionData.dirVectorsSwapped[data.dir].x * 2)),
					(int)(data.position.y - (DirectionData.dirVectorsSwapped[data.dir].y * 2))
				);

				TileController frontTile = GetTile(p.x, p.y);
				exitSigns[data.id] = frontTile;
				GameObject sign = Instantiate(exitSignPrefab, frontTile.transform.position, Quaternion.identity, frontTile.transform);
				break;
			case ObjectType.ExitWall:
				ctrl.type = TriggerType.ExitWallTrigger;
				break;
		}
		ctrl.position = data.position;
		ctrl.id = data.id;
		return ctrl;
	}

	public void UnloadLevel()
	{
		Destroy(tileContainer.gameObject);
	}

	public void ExitToMenu()
	{
		UICamera.Instance.SetOverlay(false);
		SceneManager.LoadScene(0);
	}

	public LightController PlaceLight(Color color, float range, Pos2 pos, float intensity = 1f, bool shadows = false)
	{
		GameObject temp = new GameObject("Light");
		temp.transform.position = new Vector3(pos.x, 1f, pos.y);
		temp.transform.parent = lightContainer;
		LightController ctrl = temp.AddComponent<LightController>();
		ctrl.visible = true;
		ctrl.shadows = shadows;
		ctrl.lightColor = color;
		ctrl.lightRange = range * 3;
		ctrl.lightIntensity = intensity;
		lights.Add(ctrl);
		return ctrl;
	}

	private void FixedUpdate()
	{
		lightMap.Apply(false, false);
		if (stopLightingUpdate) return;

		foreach (TileController t in tiles)
		{
			Vector2Int pos = t.tilePos;
			t.UpdateTileLighting();
			lightMap.SetPixel(pos.x, pos.y, t.lightColor);
		}
	}

	public void SetLightmapPixel(Vector2Int position, Color color)
	{
		lightMap.SetPixel(position.x, position.y, color);
	}

	public void FillLightmap(Color color)
	{
		for (int x = 0; x < lightMap.width; x++)
		{
			for (int y = 0; y < lightMap.height; y++)
			{
				this.SetLightmapPixel(new Vector2Int(x, y), color);
			}
		}
	}

	public Color GetLightmapPixel(Vector2Int position)
	{
		return this.lightMap.GetPixel(position.x, position.y);
	}

	public Transform GetRandomTile()
	{
		return tiles[Random.Range(0, tiles.Count)].transform;
	}

	public Transform GetRandomNavigableTile()
	{
		Vector2Int pos = GetRandomTilePos();
		if (IsTileBlocked(new Pos2(pos)))
			return GetRandomNavigableTile();
		return this.GetTile(pos.x, pos.y).transform;
	}

	public Pos2 GetRandomNavigableTilePos()
	{
		Transform tile = GetRandomNavigableTile();
		TileController ctrl = tile.GetComponent<TileController>();
		return new Pos2(ctrl.tilePos);
	}

	public Vector2Int GetRandomTilePos()
	{
		return tiles[Random.Range(0, tiles.Count)].tilePos;
	}

	public void SetTileBlocked(Pos2 position, bool blocked)
	{
		this.blockedTiles[position] = blocked;
	}

	private void OnApplicationQuit()
	{
		Shader.SetGlobalFloat("_VertexGlitchStrength", 0f);
	}
}

public enum LightBlendMode
{
	Additive,
	Cumulative,
	Greatest
}
