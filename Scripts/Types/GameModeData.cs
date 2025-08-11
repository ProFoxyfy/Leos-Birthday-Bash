using AYellowpaper.SerializedCollections;
using UnityEngine;

[CreateAssetMenu(fileName = "New Game Mode Data", menuName = "Custom/Game Mode Data")]
public class GameModeData : ScriptableObject
{
	public SerializedDictionary<GameMode, GameObject> gameManagers;
}
