using UnityEngine;

public class GummyBear : MonoBehaviour
{
	public AudioObject sound;
	AudioManager audMan;

	void Awake()
	{
		audMan = gameObject.AddComponent<AudioManager>();
		audMan.PlaySound(sound);
		Invoke("Die", 30f);
	}

	void Die()
	{
		Destroy(gameObject);
	}

    void Update()
    {
        transform.position -= Vector3.up * Time.deltaTime;
    }
}
