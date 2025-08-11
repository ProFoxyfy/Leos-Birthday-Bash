using System.Collections;
using System;
using System.Collections.Generic;
using UnityEngine;
using TMPro;

public class MusicPlayerManager : MonoBehaviour
{
    public AudioObject[] tracks;
	public string[] trackNames;
	public string[] trackAuthors;
	public string[] funFacts;
	public GameObject trackPrefab;
	public Transform container;
	public TMP_Text trackName;
	public TMP_Text trackAuthor;
	public TMP_Text funFactText;
	public TMP_Text durationText;
	bool initialized = false;

	void Awake()
	{
		if (initialized) return;
		initialized = true;
		int c = 0;
		foreach (string name in trackNames)
		{
			GameObject track = Instantiate(trackPrefab, container);
			int trackNum = c;
			track.GetComponent<TMP_Text>().text = name;
			TimeSpan t = TimeSpan.FromSeconds(tracks[trackNum].clip.length);
			track.GetComponent<Button>().onClick.AddListener(() => {
				trackName.text = name;
				trackAuthor.text = trackAuthors[trackNum];
				funFactText.text = funFacts[trackNum];
				durationText.text = t.ToString(@"mm\:ss");
				MusicManager.Instance.PlayTrack(tracks[trackNum]);
			});
			c++;
		}
	}

	void OnDisable()
	{
		MusicManager.Instance.StopAll();
	}
}
