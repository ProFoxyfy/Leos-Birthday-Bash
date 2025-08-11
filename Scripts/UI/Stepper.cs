using UnityEngine;

public class Stepper : BaseUIInput
{
	public BaseProgressBar progressBar;
	public Button increment;
	public Button decrement;
	public float min;
	public float max;
	public float steps;
	[SerializeField, ReadOnly]
	internal float value;
	private float relativeValue;
	public string setting;
	public bool rawProgress = false;

	private void Awake()
	{
		if (!string.IsNullOrEmpty(setting) && !string.IsNullOrWhiteSpace(setting))
			relativeValue = (float)FlagManager.Instance.GetSetting(setting);
		value = Mathf.Clamp(min + (relativeValue * max), min, max);
		increment.onClick.AddListener(Increment);
		decrement.onClick.AddListener(Decrement);
	}

	private void Start()
	{
		progressBar.progress = Mathf.Floor((rawProgress ? value : relativeValue) * 100f) / 100f;
		progressBar.min = min;
		progressBar.max = max;
		progressBar.Refresh();
	}

	void Increment()
	{
		this.ChangeValue(false);
	}

	void Decrement()
	{
		this.ChangeValue(true);
	}

	void ChangeValue(bool subtract)
	{
		relativeValue = Mathf.Clamp(subtract ? relativeValue - (max / steps) : relativeValue + (max / steps), min, 1f);
		value = Mathf.Clamp(min + (relativeValue*max), min, max);
		progressBar.progress = Mathf.Floor((rawProgress ? value : relativeValue)*100f) / 100f;
		progressBar.Refresh();

		if (!string.IsNullOrEmpty(setting) && !string.IsNullOrWhiteSpace(setting))
			FlagManager.Instance.SetSetting(setting, relativeValue);
	}
}