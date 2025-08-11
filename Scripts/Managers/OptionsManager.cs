using AYellowpaper.SerializedCollections;
using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;

public class OptionsManager : MonoBehaviour
{
	public TMP_Text pageDisplay;
	public Button next;
	public Button prev;
	private int currentPage;
	[SerializedDictionary]
	public SerializedDictionary<int, GameObject> pages;
	[SerializedDictionary]
	public SerializedDictionary<int, string> pageNames;

	private void Awake()
	{
		currentPage = 1;
		UpdatePages();

		next.onClick.AddListener(Next);
		prev.onClick.AddListener(Previous);
	}

	private void UpdatePages()
	{
		if (currentPage > pages.Count)
			currentPage = 1;
		else if (currentPage < 1)
			currentPage = pages.Count;

		pageDisplay.text = LocalizationManager.Instance.GetLocalizedString(pageNames[currentPage], LangStringType.Menu)[0];

		foreach (GameObject obj in pages.Values)
		{
			obj.SetActive(false);
		}

		pages[currentPage].SetActive(true);
	}

	private void Next()
	{
		currentPage++;
		UpdatePages();
	}

	private void Previous()
	{
		currentPage--;
		UpdatePages();
	}
}
