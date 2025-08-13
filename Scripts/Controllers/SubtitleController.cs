using System.Collections;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class SubtitleController : MonoBehaviour
{
	public string[] content;
	public float[] events;
	public Color color;

	public bool loop = false;

	public Transform origin;
	public Transform cam;

	public TMP_Text text;
	public Image bg;

	public RectTransform rectTransform;

	public bool hasPosition = false;
	public float radius;
	public float distance;
	public float duration = 0f;

	public float timer = 0f;
	private float elapsedTime = 0f;
	private int nextIndex = 0;

	private Vector3 anchoredPos;
	private Vector3 localScale;

	private bool hidden;

	private void Awake()
	{
		if (Camera.main == null) return;
		cam = Camera.main.transform;
	}

	public void Initialize()
	{
		this.text.text = this.content[0];
		this.text.color = this.color;

		if (this.hasPosition)
			this.PositionSubtitle();
		else
			this.PositionInQueue();

		this.Hide(!(bool)FlagManager.Instance.GetSetting("subtitlesEnabled"));
	}

	private void Update()
	{
		if (content.Length > 1 && timer <= 0f && nextIndex < content.Length)
		{
			timer = events[nextIndex] - elapsedTime;
			elapsedTime = events[nextIndex];
			this.text.text = content[nextIndex];
			nextIndex++;
		}
		else if (timer > 0f)
			timer -= Time.unscaledDeltaTime;

		if (this.hasPosition && origin == null)
			Destroy(gameObject);

		if (this.hasPosition)
			this.PositionSubtitle();
		else
			this.PositionInQueue();
	}

	private void Hide(bool hide)
	{
		if (hide && !this.hidden)
		{
			this.text.enabled = false;
			this.bg.enabled = false;
			this.hidden = true;
			return;
		}
		if (!hide && this.hidden)
		{
			this.text.enabled = true;
			this.bg.enabled = true;
			this.hidden = false;
		}
	}

	private void PositionInQueue()
	{
		this.anchoredPos.x = 0f;
		this.anchoredPos.y = -100f;
		this.anchoredPos.z = 0f;
		this.rectTransform.anchoredPosition = this.anchoredPos;
		this.rectTransform.localScale = Vector3.one;
	}

	private void PositionSubtitle()
	{
		if (this.origin != null && this.cam != null)
		{
			float num = Mathf.Atan2(this.cam.position.z - this.origin.position.z, this.cam.position.x - this.origin.position.x) * 57.29578f + this.cam.eulerAngles.y + 180f;
			this.anchoredPos.x = Mathf.Cos(num * 0.017453292f) * this.radius;
			this.anchoredPos.y = Mathf.Sin(num * 0.017453292f) * this.radius;
			this.anchoredPos.z = 0f;
			this.rectTransform.anchoredPosition = this.anchoredPos;
			float num2 = Mathf.Max(1f - Vector3.Distance(this.cam.position, this.origin.position) / this.distance, 0f);
			this.localScale.x = num2;
			this.localScale.y = num2;
			this.localScale.z = 1f;
			this.rectTransform.localScale = this.localScale;
		}
	}
}
