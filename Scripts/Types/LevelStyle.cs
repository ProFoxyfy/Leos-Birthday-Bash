using AYellowpaper.SerializedCollections;
using UnityEngine;

[CreateAssetMenu(fileName = "newLevelStyle", menuName = "Custom/Level Style")]
public class LevelStyle : ScriptableObject
{
	public GamePosterInfo posterInfo;
	public Texture2D wall;
	public Texture2D wallMask;
	public Texture2D ceiling;
	public Texture2D floor;
	public Material skybox;
	public SerializedDictionary<DoorType, Texture2D[]> doors;
}

public enum DoorType
{
	Regular,
	Staff
}
