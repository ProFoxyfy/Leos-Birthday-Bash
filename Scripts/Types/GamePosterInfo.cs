using AYellowpaper.SerializedCollections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "NewPosterData", menuName = "Custom/Poster Info")]
public class GamePosterInfo : ScriptableObject
{
	public SerializedDictionary<string, Texture2D> posters;
	public Texture2D suggestivePosterFallback;
	public List<string> suggestivePosterIDs;
	public SerializedDictionary<string, Texture2D> suggestivePosterReplacements;
	public SerializedDictionary<string, Texture2D> posterMasks;
}
