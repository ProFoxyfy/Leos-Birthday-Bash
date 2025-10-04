using System;
using System.Collections.Generic;
using System.IO;
using UnityEngine;
using UnityEngine.SceneManagement;
using YamlDotNet.Serialization;

public class FlagManager : Singleton<FlagManager>
{
	// flags:
	// 0 - DEAD ending beaten?
	// 1 - HELP ending beaten?
	// 2 - bad ending beaten?
	// 3 - good ending beaten?
	// 4 - regret? (slightly different good ending, after bad ending)
	// 5 - high score [variable]
	// 6 - seen doubt
	// 7 - 15 diary entries seen?
	// 16 - 18 tapes listened?
	// 19 - go to bad end

	public static Dictionary<string, object> settings = new();
	public static Dictionary<string, bool> tempFlags = new();
	public int[] flags = new int[64];

	public static string settingsFile = "settings.dat";
	public static string settingsFilePath;
	public static string saveFile = "save.dat";
	public static string saveFilePath;
	public static string gameVarFile = "config.yaml";
	public static string gameVarFilePath;

	public GameVars gameVars;

	public static Deserializer deserializer = new();

	private string defaultSave;

	public bool error = false;

	[ReadOnly]
	public bool hasLoaded;

	public static readonly Dictionary<string, object> defaultSettings = new()
	{
		["sensitivity"] = 1f,
		["cursorSensitivity"] = 1f,
		["subtitlesEnabled"] = true,
		["language"] = Language.English,
		["resolution"] = 0,
		["fullscreen"] = true,
		["volume_music"] = 1f,
		["volume_sfx"] = 1f,
		["volume_voice"] = 1f,
		["largeUi"] = false,
		["largeCursor"] = false,
		["familyFriendly"] = false,
		["forceJollyMode"] = false,
		["invertY"] = false,
		["lowerSens"] = false
	};

	public int CountEndings()
	{
		int count = 0;

		// also since flags are 0/1 we can just add them together to get the amount of
		// endings finished
		for (int i = 0; i< 4; i++)
			count += GetFlag(i);

		return count;
	}

	private void Awake()
	{
		DontDestroyOnLoad(this);
		for (int i = 0; i < flags.Length; i++)
			defaultSave += "0\n";

		settingsFilePath = Application.persistentDataPath + Path.DirectorySeparatorChar + settingsFile;
		saveFilePath = Application.persistentDataPath + Path.DirectorySeparatorChar + saveFile;
		gameVarFilePath = Application.streamingAssetsPath + Path.DirectorySeparatorChar + gameVarFile;

		gameVars = deserializer.Deserialize<GameVars>(File.ReadAllText(gameVarFilePath));
	}

	public static string StringBetween(string input, string start, string end)
	{
		int startCharIndex = input.IndexOf(start);
		int endCharIndex = input.IndexOf(end) + 1;
		return input.Substring(startCharIndex, endCharIndex - startCharIndex);
	}

	private object DeserializeToType(string input, string type)
	{
		switch (type)
		{
			case "Boolean":
				return input == "True";
			case "String":
				return input;
			case "Int32":
				return int.Parse(input);
			case "Single":
				return float.Parse(input);
			case "Language":
				return Enum.Parse(typeof(Language), input);
			default:
				return null;
		}
	}

	private void SerializeSettings(string targetFile)
	{
		string output = "";
		string path = Application.persistentDataPath + "\\" + targetFile;

		foreach (KeyValuePair<string, object> setting in settings)
		{
			output += "(" + setting.Value.GetType().Name + ")" + setting.Key + ": " + setting.Value.ToString() + "\n";
		}

		File.WriteAllText(path, output);
	}

	private void DeserializeSettings(string targetFile)
	{
		settings.Clear();
		var input = File.ReadAllLines(Application.persistentDataPath + "\\" + targetFile);

		foreach (string line in input)
		{
			string rawType = StringBetween(line, "(", ")");
			string type = rawType.Replace("(", "").Replace(")", "");
			var cleaned = line.Replace(rawType, "");
			var formatted = cleaned.Replace(": ", "|");
			var val = formatted.Split('|');
			settings.Add(val[0], DeserializeToType(val[1], type));
		}
	}

	public void SaveSettings()
	{
		if (error) return;
		SerializeSettings(settingsFile);
	}

	private void ValidateSettings()
	{
		foreach (KeyValuePair<string, object> setting in defaultSettings)
		{
			if (!settings.ContainsKey(setting.Key))
				settings[setting.Key] = setting.Value;
		}
	}

	public void SetFlag(int key, int value)
	{
		flags[key] = value;
	}

	public int GetFlag(int key)
	{
		return flags[key];
	}

	public void LoadSettings()
	{
		if (!File.Exists(settingsFilePath))
			File.Create(settingsFilePath).Close();
		DeserializeSettings(settingsFile);
		ValidateSettings();
	}

	public void SetSetting(string key, object value)
	{
		settings[key] = value;
	}

	public object GetSetting(string key)
	{
		return settings[key];
	}

	public bool SettingExists(string key)
	{
		return settings.ContainsKey(key);
	}

	private void SerializeFlags(string filename)
	{
		var path = Application.persistentDataPath + "\\" + filename;
		var output = "";
		foreach (int flag in flags)
		{
			output += flag.ToString() + "\n";
		}

		File.WriteAllText(path, output);
	}

	private void DeserializeFlags(string filename)
	{
		var path = Application.persistentDataPath + "\\" + filename;
		var input = File.ReadAllLines(path);
		if (input.Length < flags.Length)
		{
			error = true;
			Debug.LogError("[FlagManager] Load error - missing entries");
		}
		for (int i = 0; i < (input.Length + 1); i++)
		{
			if (i >= flags.Length || i >= input.Length || string.IsNullOrEmpty(input[i]))
			{
				continue;
			}
			try
			{
				flags[i] = int.Parse(input[i].Trim());
			}
			catch (Exception e)
			{
				Debug.LogError("[FlagManager] " + e.ToString());
				error = true;
				break;
			}
		}
	}

	public void Save()
	{
		if (error) return;
		SerializeFlags(saveFile);
	}

	public void Load()
	{
		if (!File.Exists(saveFilePath))
			File.WriteAllText(saveFilePath, defaultSave);
		DeserializeFlags(saveFile);
		hasLoaded = true;

		if (error)
			SceneManager.LoadScene("Error");
	}

	private void OnApplicationQuit()
	{
		Save();
		SaveSettings();
	}
}
