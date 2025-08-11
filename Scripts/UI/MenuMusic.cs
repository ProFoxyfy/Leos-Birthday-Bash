using TweenX;
using TweenX.EasingStyles;
using UnityEngine;

public class MenuMusic : MonoBehaviour
{
	public XFloat volume = 0f;
	AudioManager audMan;
	TweenManager twMan;
	Transition trans;
	public AudioObject music;

	private void Awake()
	{
		trans = GetComponent<Transition>();
		twMan = gameObject.AddComponent<TweenManager>();
		audMan = gameObject.AddComponent<AudioManager>();

		trans.onOut.AddListener(OnOff);
	}

	private void Update()
	{
		audMan.audioSource.volume = volume;
	}

	private void Start()
	{
		twMan.CancelAllTweens();
		volume.Set(1f);
		audMan.PlaySound(music);
	}

	private void OnDisable()
	{
		twMan.CancelAllTweens();
		volume.Set(1f);
	}

	private void OnOff()
	{
		twMan.PlayTweenSingle(ref volume, new Tween(GlobalsManager.Instance.style.transition.defaultDurationSeconds, new Linear(), 1f, 0f));
	}
}
