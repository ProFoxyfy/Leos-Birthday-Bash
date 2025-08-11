using UnityEngine;

public class AudioListenerManager : Singleton<AudioListenerManager>
{
    private Transform listener;

    private void Awake()
    {
        DontDestroyOnLoad(this);
        this.gameObject.name = "AC_Listener";
        if (!GetComponent<AudioListener>())
        {
            gameObject.AddComponent<AudioListener>();
        }
    }

    private void Update()
    {
        transform.position = listener.position;
        transform.rotation = listener.rotation;
    }

    public void MoveListener(Transform newParent)
    {
        listener = newParent;
    }
}
