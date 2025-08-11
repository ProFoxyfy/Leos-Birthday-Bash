using UnityEngine;

[RequireComponent(typeof(AudioManager))]
public class AudioTest : MonoBehaviour
{
    private AudioManager audMan;
    public AudioObject sound;

    void Start()
    {
        audMan = GetComponent<AudioManager>();
        audMan.PlaySound(sound);
    }
}
