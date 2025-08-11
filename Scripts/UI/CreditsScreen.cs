using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class CreditsScreen : MonoBehaviour
{
	public Sprite[] pages;
	public float[] duration;
	RawImage img;
	Image slide;
	private int currentPage = 0;
	public float speed = 1f;

	private void Awake()
	{
		img = GetComponent<RawImage>();
		slide = GetComponentInChildren<Image>();
	}

	private void OnEnable()
	{
		StartCoroutine(DoSequence());
	}

	private void Update()
	{
		img.material.SetFloat("_Offset", Time.unscaledTime * speed);
	}

	IEnumerator DoSequence()
	{
		currentPage = 0;
		while (currentPage < pages.Length && gameObject.activeSelf)
		{
			if (currentPage > pages.Length - 1)
				yield break;
			slide.sprite = pages[currentPage];
			yield return new WaitForSeconds(duration[currentPage]);
			currentPage++;
		}
	}
}
