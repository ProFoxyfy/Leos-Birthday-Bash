using Discord;
using System;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class RichPresenceManager : Singleton<RichPresenceManager>
{
#nullable enable
	const long CLIENT_ID = 0; // nuh uh
	Discord.Discord dc;
	public bool loaded = false;
	private long startTime = 0;

	private Dictionary<int, string?> endingCountImages = new()
	{
		[0] = null,
		[1] = "endings_one",
		[2] = "endings_two",
		[3] = "endings_three",
		[4] = "endings_four",
	};

	private Dictionary<string, string> sceneDetailsStrings = new()
	{
		["MainMenu"] = "In the main menu",
		["Game"] = "In-game: {0} mode",
		["Error"] = "Corrupted save file"
	};

	public void Init()
	{
		// y this no run???
		dc = new(CLIENT_ID, (ulong)CreateFlags.NoRequireDiscord);

		dc.SetLogHook(LogLevel.Debug, (level, message) =>
		{
			Debug.Log(string.Format("RPManager[{0}]: {1}", level, message));
		});

		loaded = true;
		startTime = DateTimeOffset.UtcNow.ToUnixTimeSeconds();
	}

	public void UpdateActivity()
	{
		ActivityManager activityManager = dc.GetActivityManager();
		string detailsStr = "";
		string sceneName = SceneManager.GetActiveScene().name;

		switch (sceneName)
		{
			case "Game":
				detailsStr = sceneDetailsStrings[sceneName];
				switch (GlobalsManager.Instance.currentMode)
				{
					case GameMode.Story:
						detailsStr = string.Format(detailsStr, "Story");
						break;
					case GameMode.Endless:
						detailsStr = string.Format(detailsStr, "Endless");
						break;
					case GameMode.Doubt:
						detailsStr = string.Format(detailsStr, "???");
						break;
					default:
						detailsStr = string.Format(detailsStr, "No");
						break;
				}
				break;
			default:
				detailsStr = sceneDetailsStrings[sceneName];
				break;
		}

		Activity activity = new()
		{
			State = null,
			Details = detailsStr,
			Assets =
			{
				LargeImage = "large_notitle",
				LargeText = CoreInitializer.version,
				SmallImage = endingCountImages[FlagManager.Instance.CountEndings()],
				SmallText = string.Format("{0}/4 Endings", FlagManager.Instance.CountEndings())
			},
			Timestamps =
			{
				Start = startTime
			},
			Instance = false,
			Type = ActivityType.Playing
		};

		activityManager.UpdateActivity(activity, result =>
		{
			Debug.Log("Updated RPC activity -> " + result.ToString());
		});
	}

	private void Update()
	{
		dc.RunCallbacks();
	}

	private void OnApplicationQuit()
	{
		dc.Dispose();
	}
}
