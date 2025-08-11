using System;
using System.IO;
using UnityEngine;
using UnityEngine.InputSystem;
using System.Collections.Generic;
using AYellowpaper.SerializedCollections;
using System.Runtime.Serialization.Formatters.Binary;
using TMPro;

public class LevelEditorPlacement : MonoBehaviour
{
	private const string IMPORTANT_MESSAGE = @"
Heya, Nox here.

This is part of an internal level editor tool actually used for, well
level editing.
While the scene that does contain it is removed, there are leftovers,
and these leftovers are not unused assets, they're just part
of this internal tool.
So if you're datamining, just take note of this.
";

	public GameObject prefab;
	public Transform ghost;
	public Transform selectGhost;
	public Transform parent;
	public GameObject inspector;
	public GameObject objectSettings;

	private BaseEditorObject selectedObject;
	private BaseEditorObject lastSelectedObject;

	[SerializedDictionary]
	public SerializedDictionary<ObjectType, Inspector> inspectors = new SerializedDictionary<ObjectType, Inspector>();

	public Grid grid;
	private Camera cam;
	private MenuCursor cursor;
	private RectTransform cursorTran;
	private InputAction place;
	private InputAction destroy;
	public LevelData data;
	public TMP_Text coordinateTxt;
	private EditorTileController[,] controllers = new EditorTileController[100, 100];
	private LevelEditorTool currentTool = LevelEditorTool.Cursor;
	private bool forceCursor = false;
	private MenuCursor curs;
	public ObjectType currentType = ObjectType.Light;
	private string type = "NSWE";

	public TextBox markerDataBox;

	private int tileMask;
	private int objectMask;
	private int markerMask;

	[SerializedDictionary]
	public SerializedDictionary<ObjectType, GameObject> objectPrefabs = new SerializedDictionary<ObjectType, GameObject>();

	public GameObject markerPrefab;

	private void Awake()
	{
		tileMask = LayerMask.GetMask("EditorTile");
		objectMask = LayerMask.GetMask("EditorObject");
		markerMask = LayerMask.GetMask("EditorMarker");

		curs = GlobalsManager.Instance.activeCursor;
		data = new LevelData();
		cam = UICamera.Instance.main;
		cursor = GlobalsManager.Instance.activeCursor;
		cursorTran = cursor.GetComponent<RectTransform>();

		place = GameInputManager.Instance.GetAction("Click");
		destroy = GameInputManager.Instance.GetAction("ItemUse");

		UpdateInspector(null);

		LevelObject spawn = new LevelObject();
		spawn.type = ObjectType.PlayerSpawn;
		spawn.position = new Pos2(50, 50);
		PlaceObject(spawn);
	}

	private bool IsObstructed(Vector2 pos)
	{
		TileData tile = data.tiles[(int)Mathf.Clamp(pos.x, 0, data.tiles.GetLength(0)), (int)Mathf.Clamp(pos.y, 0, data.tiles.GetLength(1))];
		return tile != null;
	}

	private void UpdateTiling()
	{
		for (int x = 0; x < data.tiles.GetLength(0); x++)
		{
			for (int y = 0; y < data.tiles.GetLength(1); y++)
			{
				EditorTileController tile = controllers[x, y];
				if (tile == null) continue;
				foreach (Direction dir in Enum.GetValues(typeof(Direction)))
				{
					tile.SetWallEnabled(dir, !IsObstructed(new Vector2(x,y) + -DirectionData.dirVectors[dir]));
				}
			}
		}
	}

	public void SetTileType(string type)
	{
		this.type = type;
	}

	private void PlaceObject(LevelObject obj, bool isNew = true)
	{
		Vector3 pos = grid.GetCellCenterWorld(obj.position.ToVec3Int()) - Vector3.one * 0.5f;
		Vector3 actualPos = ((Vector3.one * 100) - pos) - Vector3.one * 50;
		GameObject inst = Instantiate(objectPrefabs[obj.type], new Vector3(actualPos.x, actualPos.y, 0f), Quaternion.identity, parent);
		BaseEditorObject ctrl = inst.GetComponent<BaseEditorObject>();
		ctrl.data = obj;
		if (isNew)
			data.objects.Add(obj);
	}

	private void PlaceMarker(LevelMarker obj, bool isNew = true)
	{
		Vector3 pos = grid.GetCellCenterWorld(obj.position.ToVec3Int()) - Vector3.one * 0.5f;
		Vector3 actualPos = ((Vector3.one * 100) - pos) - Vector3.one * 50;
		GameObject inst = Instantiate(markerPrefab, new Vector3(actualPos.x, actualPos.y, 0f), Quaternion.identity, parent);
		EditorMarker ctrl = inst.GetComponent<EditorMarker>();
		ctrl.data = obj;
		if (isNew)
			data.markers.Add(obj);
	}

	private void UpdateObjectFromInspector(ref LevelObject obj)
	{
		Inspector inspec = inspectors[obj.type];
		switch (obj.type)
		{
			case ObjectType.Light:
				float.TryParse(((TextBox)inspec.fields["Range"]).currentText, out obj.range);
				Color temp;
				ColorUtility.TryParseHtmlString('#' + ((TextBox)inspec.fields["Color"]).currentText, out temp);
				obj.color = new Color3(temp);
				break;
			case ObjectType.SwingDoor or ObjectType.Door or ObjectType.StaffDoor:
				string val = ((TextBox)inspec.fields["Direction"]).currentText;
				if (!DirectionData.nameToDir.ContainsKey(val))
					break;
				Direction tempDir = DirectionData.nameToDir[val];
				obj.dir = tempDir;
				break;
			case ObjectType.ExitWall or ObjectType.Furniture:
				string val2 = ((TextBox)inspec.fields["Direction"]).currentText;
				if (!DirectionData.nameToDir.ContainsKey(val2))
					break;
				Direction tempDir2 = DirectionData.nameToDir[val2];
				obj.dir = tempDir2;
				int.TryParse(((TextBox)inspec.fields["ID"]).currentText, out obj.id);
				break;
			case ObjectType.ExitTriggerSwingDoor:
				string val3 = ((TextBox)inspec.fields["Direction"]).currentText;
				if (!DirectionData.nameToDir.ContainsKey(val3))
					break;
				Direction tempDir3 = DirectionData.nameToDir[val3];
				obj.dir = tempDir3;
				int.TryParse(((TextBox)inspec.fields["ID"]).currentText, out obj.id);
				break;
			case ObjectType.ExitTrigger:
				int.TryParse(((TextBox)inspec.fields["ID"]).currentText, out obj.id);
				break;
			default:
				break;
		}
	}

	private void UpdateInspector(BaseEditorObject obj)
	{
		if (obj == null || obj.data.type == ObjectType.PlayerSpawn)
		{
			inspector.SetActive(false);
			return;
		}
		if (lastSelectedObject == obj)
		{
			UpdateObjectFromInspector(ref obj.data);
			return;
		}
		lastSelectedObject = obj;
		inspector.SetActive(true);

		Inspector current = null;

		foreach (KeyValuePair<ObjectType, Inspector> inspector in inspectors)
		{
			if (inspector.Key != obj.data.type)
				inspector.Value.gameObject.SetActive(false);
			else
			{
				current = inspector.Value;
				inspector.Value.gameObject.SetActive(true);
			}
		}

		switch (obj.data.type)
		{
			case ObjectType.Light:
				((TextBox)current.fields["Range"]).SetText(obj.data.range.ToString());
				((TextBox)current.fields["Color"]).SetText(ColorUtility.ToHtmlStringRGB(obj.data.color.ToColor()));
				break;
			case ObjectType.SwingDoor or ObjectType.Door or ObjectType.StaffDoor:
				((TextBox)current.fields["Direction"]).SetText(DirectionData.dirShortNames[obj.data.dir]);
				break;
			case ObjectType.ExitWall or ObjectType.ExitTriggerSwingDoor or ObjectType.Furniture:
				((TextBox)current.fields["Direction"]).SetText(DirectionData.dirShortNames[obj.data.dir]);
				((TextBox)current.fields["ID"]).SetText(obj.data.id.ToString());
				break;
			case ObjectType.ExitTrigger:
				((TextBox)current.fields["ID"]).SetText(obj.data.id.ToString());
				break;
			default:
				Debug.LogWarning("[LevelEditor] WARNING: Unknown type fed into inspector. Type: " + obj.data.type.ToString());
				break;
		}
	}

	private void ApplyTileType(ref TileData tile)
	{
		if (type == "")
		{
			foreach (Direction dir in Enum.GetValues(typeof(Direction)))
			{
				tile.walls[dir] = false;
			}
			return;
		}
		foreach (KeyValuePair<Direction, string> dir in DirectionData.dirShortNames)
		{
			if (type.Contains(dir.Value))
				tile.walls[dir.Key] = true;
			else
				tile.walls[dir.Key] = false;
		}
	}

	private void Update()
	{
		Vector3 worldPos = cursorTran.position;
		Vector3Int pos = new Vector3Int(Mathf.RoundToInt(worldPos.x), Mathf.RoundToInt(worldPos.y));
		Vector3 gridPos = grid.GetCellCenterWorld(pos) - Vector3.one * 0.5f;
		Vector3 actualGridPos = ((Vector3.one * 100) - pos) - Vector3.one * 50;
		coordinateTxt.text = $"Coordinate: {actualGridPos.x}, {actualGridPos.y}";
		ghost.position = gridPos;

		selectGhost.gameObject.SetActive(selectedObject != null);
		if (selectedObject != null)
			selectGhost.transform.position = selectedObject.transform.position;

		int mask;
		switch (currentTool)
		{
			case LevelEditorTool.Tile:
				mask = tileMask;
				break;
			case LevelEditorTool.Object or LevelEditorTool.Cursor:
				mask = objectMask;
				break;
			case LevelEditorTool.Marker:
				mask = markerMask;
				break;
			default:
				mask = 0;
				break;
		}

		RaycastHit2D currentObj = Physics2D.Raycast(gridPos, Vector2.zero, 0, mask);

		forceCursor = curs.hoveringOverElement;

		ghost.gameObject.SetActive(!forceCursor && currentTool != LevelEditorTool.Cursor);

		UpdateInspector(selectedObject);

		switch (currentTool)
		{
			case LevelEditorTool.Cursor:
				if (curs.hoveringOverElement)
					return;
				// Check if mouse is down and we're over an object
				// Also dragging objects :D
				if (!place.IsPressed()) break;
				if (currentObj.collider == null
					&& selectedObject != null
					&& Mathf.Abs(Vector3.Distance(
						curs.transform.position,
						selectedObject.transform.position
						)) > 99.5f
					)
				{
					selectedObject = null;
					break;
				}
				if (selectedObject != null && currentObj.collider == null)
				{
					selectedObject.data.position = new Pos2((int)actualGridPos.x, (int)actualGridPos.y);
					selectedObject.transform.position = gridPos;
				}
				else
				{
					if (currentObj.collider == null) break;
					BaseEditorObject beo = currentObj.collider.gameObject.GetComponent<BaseEditorObject>();
					selectedObject = beo;
				}
				break;
			case LevelEditorTool.Tile:
				if (!ghost.gameObject.activeSelf)
					return;
				selectedObject = null;
				if (place.IsPressed() && (currentObj.collider == null))
				{
					GameObject inst = Instantiate(prefab, gridPos, Quaternion.identity, parent);
					EditorTileController ctrl = inst.GetComponent<EditorTileController>();
					ApplyTileType(ref ctrl.data);
					controllers[(int)actualGridPos.x, (int)actualGridPos.y] = ctrl;
					data.tiles[(int)actualGridPos.x, (int)actualGridPos.y] = ctrl.data;

					//UpdateTiling();
				}
				else if (destroy.IsPressed() && (currentObj.collider != null))
				{
					Destroy(currentObj.collider.gameObject);
					data.tiles[(int)actualGridPos.x, (int)actualGridPos.y] = null;
					controllers[(int)actualGridPos.x, (int)actualGridPos.y] = null;

					//UpdateTiling();
				}
				break;
			case LevelEditorTool.Object:
				if (!ghost.gameObject.activeSelf)
					return;
				selectedObject = null;
				if (place.WasPressedThisFrame() && (currentObj.collider == null))
				{
					LevelObject obj = new LevelObject();
					obj.type = currentType;
					obj.position = new Pos2(actualGridPos);

					obj.color = new Color3(1, 1, 1);
					obj.range = 2f;
					obj.npc = "None";
					this.PlaceObject(obj);
				}
				else if (destroy.WasPressedThisFrame() && (currentObj.collider != null))
				{
					BaseEditorObject obj;
					currentObj.collider.gameObject.TryGetComponent<BaseEditorObject>(out obj);
					if (obj.data.type == ObjectType.PlayerSpawn) return;

					foreach (LevelObject i in data.objects)
					{
						if (i.position == obj.data.position)
						{
							data.objects.Remove(i);
							break;
						}
					}

					Destroy(currentObj.collider.gameObject);
				}
				break;
			case LevelEditorTool.Marker:
				if (!ghost.gameObject.activeSelf)
					return;
				selectedObject = null;
				if (place.WasPressedThisFrame() && (currentObj.collider == null))
				{
					LevelMarker obj2 = new();
					obj2.position = new Pos2(actualGridPos);
					obj2.data = 0;
					byte.TryParse(markerDataBox.currentText, out obj2.data);
					this.PlaceMarker(obj2);
				}
				else if (destroy.WasPressedThisFrame() && (currentObj.collider != null))
				{
					EditorMarker obj2;
					currentObj.collider.gameObject.TryGetComponent<EditorMarker>(out obj2);

					foreach (LevelMarker i2 in data.markers)
					{
						if (i2.position == obj2.data.position)
						{
							data.markers.Remove(i2);
							break;
						}
					}

					Destroy(currentObj.collider.gameObject);
				}
				break;
		}
	}

	public void ChangeTool(string tool)
	{
		currentTool = Enum.Parse<LevelEditorTool>(tool);
	}

	private void Reload()
	{
		foreach (Transform child in parent.GetComponentsInChildren<Transform>())
		{
			if (child == parent) continue;
			Destroy(child.gameObject);
		}
		for (int x = 0; x < data.tiles.GetLength(0); x++)
		{
			for (int y = 0; y < data.tiles.GetLength(1); y++)
			{
				if (data.tiles[x,y] == null) continue;
				Vector3Int pos = new Vector3Int(x, y);
				Vector3 wp = (Vector3.one * 50) - (grid.GetCellCenterWorld(pos) - Vector3.one * 0.5f);
				GameObject obj = Instantiate(prefab, wp, Quaternion.identity, parent);
				controllers[x, y] = obj.GetComponent<EditorTileController>();
				controllers[x, y].data = data.tiles[x,y];
			}
		}

		if (data.objects == null)
			data.objects = new List<LevelObject>();
		if (data.markers == null)
			data.markers = new List<LevelMarker>();

		// Now that we have a proper implementation of LevelObject
		// that isn't entirely cursed, loading them in is very easy
		foreach (LevelObject obj in data.objects)
		{
			this.PlaceObject(obj, false);
		}

		foreach (LevelMarker marker in data.markers)
		{
			this.PlaceMarker(marker, false);
		}

		//UpdateTiling();
	}

	public void SerializeLevel(string path)
	{
		FileStream file = new FileStream(path, FileMode.OpenOrCreate, FileAccess.Write, FileShare.Read);
		BinaryFormatter bf = new BinaryFormatter();
		bf.Serialize(file, data);
		file.Close();
	}

	public void SerializeLevelDefaultPath()
	{
		string docs = Environment.GetFolderPath(Environment.SpecialFolder.MyDocuments);

		if (!Directory.Exists(docs + @"\My Games"))
			Directory.CreateDirectory(docs + @"\My Games");
		if (!Directory.Exists(docs + @"\My Games\Leo's Birthday Bash"))
			Directory.CreateDirectory(docs + @"\My Games\Leo's Birthday Bash");
		if (!Directory.Exists(docs + @"\My Games\Leo's Birthday Bash\LevelEditor"))
			Directory.CreateDirectory(docs + @"\My Games\Leo's Birthday Bash\LevelEditor");

		string path = docs + @"\My Games\Leo's Birthday Bash\LevelEditor\level.dat";
		SerializeLevel(path);
	}

	public void DeserializeLevel(string path)
	{
		FileStream file = new FileStream(path, FileMode.Open, FileAccess.Read, FileShare.ReadWrite);
		BinaryFormatter bf = new BinaryFormatter();
		this.data = (LevelData)bf.Deserialize(file);
		file.Close();
		Reload();
	}

	public void DeserializeLevelDefaultPath()
	{
		string docs = Environment.GetFolderPath(Environment.SpecialFolder.MyDocuments);
		string path = docs + @"\My Games\Leo's Birthday Bash\LevelEditor\level.dat";
		DeserializeLevel(path);
	}

	public void ChangeObjectType(string t)
	{
		this.currentType = (ObjectType)Enum.Parse(typeof(ObjectType), t);
	}
}

public enum LevelEditorTool
{
	Cursor = 0,
	Tile = 1,
	Object = 2,
	Marker = 3
}

[Serializable]
public enum ObjectType
{
	Light,
	PlayerSpawn,
	SwingDoor,
	ExitTrigger,
	ExitTriggerSwingDoor,
	ExitWall,
	NPC,
	Door,
	StaffDoor,
	Furniture
}
