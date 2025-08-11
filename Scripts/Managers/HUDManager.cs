using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class HUDManager : Singleton<HUDManager>
{
	public float staminaMeterValue = 0f;
	public Slider staminaMeter;
	public Image handImage;
	public TMP_Text objectiveText;
	public GameObject breakEffect;
	private CanvasScaler scaler;
	public bool pauseEnabled = true;
	public bool blockGameInput = false;
	public bool interactable;
	public int objectiveCount = 0;
	public string objectiveFormat = "{0}";

	void Update()
	{
		scaler = GetComponent<CanvasScaler>();
		staminaMeter.value = staminaMeterValue;
		handImage.enabled = this.interactable;
		objectiveText.text = string.Format(objectiveFormat, objectiveCount);
	}

	private void LateUpdate()
	{
		scaler.scaleFactor = UIScaler.GetScale();
	}

	public void SetPauseEnabled(bool value)
	{
		pauseEnabled = value;
	}

	public void Disable()
	{
		GetComponent<Canvas>().gameObject.SetActive(false);
	}

	public void Enable()
	{
		GetComponent<Canvas>().gameObject.SetActive(true);
	}

	public void Break()
	{
		this.breakEffect.SetActive(true);
	}

	public void Unbreak()
	{
		this.breakEffect.SetActive(false);
	}
}
