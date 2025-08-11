using UnityEngine;
using TMPro;
using TweenX;
using TweenX.EasingStyles;
using TweenX.EasingStyles.Advanced;
using System.Collections;

public class ChaosEventManager : MonoBehaviour
{
	private int chaosMinimumTime = 35;
	private int chaosMaxTime = 160;
	public bool blockChaosEvents = false;
	public ChaosEvent[] events;
	private float currentTime;
	private EnvironmentController ec;
	public TMP_Text eventText;
	public float textYPos = -48;
	private XFloat yPos;
	private IEasingFunction easingStyle;
	private TweenManager twMan;
	public AudioObject notification;
	AudioManager audMan;

	private void Awake()
	{
		audMan = gameObject.AddComponent<AudioManager>();
		yPos = -textYPos;
		easingStyle = new CubicInOut();
		twMan = gameObject.AddComponent<TweenManager>();
		currentTime = chaosMinimumTime;
		ec = EnvironmentController.Instance;
	}

	IEnumerator DoText()
	{
		audMan.PlayOneShot(notification);
		twMan.PlayTweenSingle(ref yPos, new Tween(3f, easingStyle, -textYPos, textYPos));
		yield return new WaitForSeconds(8f);
		twMan.PlayTweenSingle(ref yPos, new Tween(3f, easingStyle, textYPos, -textYPos));
	}

	public void ShowMessage(string text)
	{
		eventText.text = text;
		StartCoroutine(DoText());
	}

	public void DoRandomEvent()
	{
		if (blockChaosEvents) return;
		ChaosEvent selected = events[Random.Range(0, events.Length)];
		if (selected == null)
		{
			DoRandomEvent();
			return;
		}
		selected.Activate(this);
	}

    void Update()
    {
		eventText.rectTransform.anchoredPosition = new Vector2(eventText.rectTransform.anchoredPosition.x, yPos);
        if (currentTime > 0)
			currentTime -= Time.deltaTime;

		if (currentTime <= 0)
		{
			currentTime = (int)Random.Range(chaosMinimumTime, chaosMaxTime);
			DoRandomEvent();
		}
    }
}
