using System.Collections;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.UI;

public class Transition : MonoBehaviour
{
	private UITransitionObject transition;
	private Image maskImage;
	public bool doOnStart = false;
	public bool inOnStart = true;
	private float delay = 0f;
	public UnityEvent onPerform = new UnityEvent();
	public UnityEvent onOut = new UnityEvent();
	[SerializeField]
	private int currentFrame = 0;
	public bool state = false;
	private bool started = false;

	private void Awake()
	{
		transition = GlobalsManager.Instance.style.transition;

		if (!started)
		{
			started = true;
			this.currentFrame = inOnStart ? 0 : transition.transitionFrames.Length - 1;
			this.state = inOnStart;
		}
		this.maskImage = GetComponent<Image>();
		maskImage.pixelsPerUnitMultiplier = 0.8f;
		this.delay = (1f / transition.transitionFrames.Length) * transition.defaultDurationSeconds;
	}

	private void Start()
	{
		if (doOnStart)
			this.Perform(this.inOnStart);
	}

	private IEnumerator DoTransition(bool inOrOut)
	{
		this.state = inOrOut;
		if (!inOrOut)
			this.onOut.Invoke();
		GlobalsManager.Instance.activeCursor.SetCursorVisible(false);
		int start = 0;
		int end = this.transition.transitionFrames.Length;
		int direction = inOrOut ? 1 : -1;
		for (int i = inOrOut ? start : end - 1; inOrOut ? i < end : i > start; i += direction)
		{
			this.currentFrame = i;
			yield return new WaitForSecondsRealtime(this.delay);
		}
		currentFrame = !inOrOut ? start : end - 1;
		Update();
		onPerform.Invoke();
		if (!inOrOut)
			gameObject.SetActive(false);
		GlobalsManager.Instance.activeCursor.SetCursorVisible(true);
	}

	private void Update()
	{
		this.maskImage.sprite = this.transition.transitionFrames[currentFrame];
	}

	public void Perform(bool inOut)
	{
		if (inOut)
		{
			currentFrame = !inOut ? this.transition.transitionFrames.Length - 1 : 0;
			gameObject.SetActive(true);
		}
			
		StartCoroutine(this.DoTransition(inOut));
	}

	public IEnumerator PerformAsync(bool inOut)
	{
		if (inOut)
		{
			currentFrame = !inOut ? this.transition.transitionFrames.Length - 1 : 0;
			gameObject.SetActive(true);
		}

		yield return StartCoroutine(this.DoTransition(inOut));
	}

	public void PerformInstant(bool inOut)
	{
		gameObject.SetActive(inOut);
		currentFrame = inOut ? this.transition.transitionFrames.Length - 1 : 0;
	}
}
