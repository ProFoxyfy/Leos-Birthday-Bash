using System.Collections;
using TMPro;
using TweenX;
using TweenX.EasingStyles;
using UnityEditor;
using UnityEngine;
using UnityEngine.InputSystem;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public class ContinueScreen : MonoBehaviour
{
	// I couldve DRYed this out but
	// forgot woops
	public TweenManager titleTM;
	public TweenManager infoTM;
	public TweenManager glowTM;

	AudioManager audMan;

	TMP_Text title;
	TMP_Text info;
	Image glow;

	public XFloat titleTX = 0f;
	public XFloat infoTX = 0f;
	public XFloat glowTX = 0f;
	float titleT = 0f;
	float infoT = 0f;
	float glowT = 0f;

	Color origGlowColor;

	InputAction yesAction;
	InputAction noAction;

	public AudioObject music;

	const float max = 16f;

	Tween tween = new(1f, new Linear(), 0f, max);
	Tween tweenOut = new(1f, new Linear(), max, 0f);

	public void FadeIn()
	{
		titleTM.useUnscaled = true;
		infoTM.useUnscaled = true;
		glowTM.useUnscaled = true;

		EnvironmentController.Instance.UnloadLevel();
		EnvironmentController.Instance.FillLightmap(Color.black);

		audMan.PlaySound(music);

		titleTM.PlayTweenSingle(ref titleTX, tween);
		infoTM.PlayTweenSingle(ref infoTX, tween);
		glowTM.PlayTweenSingle(ref glowTX, tween);
	}

	public void FadeOut()
	{
		titleTM.PlayTweenSingle(ref titleTX, tweenOut);
		infoTM.PlayTweenSingle(ref infoTX, tweenOut);
		glowTM.PlayTweenSingle(ref glowTX, tweenOut);
	}

	private void Awake()
	{
		audMan = gameObject.AddComponent<AudioManager>();

		title = titleTM.gameObject.GetComponent<TMP_Text>();
		info = infoTM.gameObject.GetComponent<TMP_Text>();
		glow = glowTM.gameObject.GetComponent<Image>();

		yesAction = GameInputManager.Instance.GetAction("Yes");
		noAction = GameInputManager.Instance.GetAction("Pause");

		origGlowColor = glow.color;
	}

	IEnumerator Restart(float delay)
	{
		yield return new WaitForSecondsRealtime(delay);
		SceneManager.LoadScene("Game");
	}

	IEnumerator DoBadEnd(float delay)
	{
		yield return new WaitForSecondsRealtime(delay);
		FlagManager.Instance.SetFlag(19, 1);
#if UNITY_EDITOR
		EditorApplication.isPlaying = false;
#else
			Application.Quit();
#endif
	}

	private void Update()
	{
		titleT = Mathf.Round(titleTX);
		infoT = Mathf.Round(infoTX);
		glowT = Mathf.Round(glowTX);

		title.alpha = titleT / max;
		info.alpha = infoT / max;
		glow.color = new Color(origGlowColor.r, origGlowColor.g, origGlowColor.b, glowT / max);

		if (titleT != max) return;

		if (yesAction.WasPressedThisFrame())
		{
			FadeOut();
			StartCoroutine(Restart(tweenOut.duration));
		}
		else if (noAction.WasPressedThisFrame())
		{
			FadeOut();
			StartCoroutine(DoBadEnd(tweenOut.duration));
		}
	}
}
