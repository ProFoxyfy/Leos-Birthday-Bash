using System;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class SubtitleManager : Singleton<SubtitleManager>
{
	private GameObject subtitlePrefab;
	private GameObject container;
	private CanvasScaler scaler;
	private Canvas canvas;

	public void Init()
	{
		// Honestly i can't find a better way to do this ;-;
	}

	private void Awake()
	{
		subtitlePrefab = Resources.Load<GameObject>("SubtitlePrefab");
		scaler = GetComponent<CanvasScaler>();
		if (scaler == null)
		{
			canvas = gameObject.AddComponent<Canvas>();
			canvas.renderMode = RenderMode.ScreenSpaceOverlay;
			canvas.pixelPerfect = false;
			canvas.sortingOrder = 0;
			canvas.targetDisplay = 0;
			canvas.vertexColorAlwaysGammaSpace = true;

			scaler = gameObject.AddComponent<CanvasScaler>();
			scaler.uiScaleMode = CanvasScaler.ScaleMode.ScaleWithScreenSize;
			scaler.referenceResolution = UIScaler.baseResolution;
			scaler.screenMatchMode = CanvasScaler.ScreenMatchMode.Expand;
			scaler.referencePixelsPerUnit = 100;
		}
		container = this.gameObject;
	}

	private void LateUpdate()
	{
		canvas.enabled = Time.timeScale > 0f;
	}

	public SubtitleController CreateSub(AudioObject sound, AudioManager origin)
	{
		if (sound.subtitleKey == "") return null;
		GameObject sub = Instantiate(subtitlePrefab, container.transform);
		SubtitleController sc = sub.GetComponent<SubtitleController>();
		var str = LocalizationManager.Instance.GetLocalizedString(sound.subtitleKey, LangStringType.Subtitle, sound.encrypted);
		sc.content = str;
		sc.events = sound.events;

		sc.color = origin.character.subtitleColor;
		sc.duration = sound.clip.length * (1f + (1f - sound.pitch));
		sc.distance = sound.radius;
		sc.radius = scaler.referenceResolution.y / 2.5f;
		sc.origin = origin.transform;
		sc.loop = sound.loop;
		sc.hasPosition = sound.positional;

		if (!sound.loop)
			Destroy(sub, sc.duration);

		sc.Initialize();
		return sc;
	}
}
