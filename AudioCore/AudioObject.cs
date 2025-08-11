using UnityEngine;

[CreateAssetMenu(fileName = "NewSound", menuName = "Custom/Sound Object")]
public class AudioObject : ScriptableObject
{
	public AudioClip clip;
	public SoundType type = SoundType.SFX;
	public bool loop = false;
	public string subtitleKey;
	public float[] events;
	public float radius = 10f;
	public float volume = 1f;
	public float pitch = 1f;
	public bool positional = true;
	public bool encrypted = false;
}

public enum SoundType
{
	SFX = 0,
	Voice = 1,
	Music = 2
}