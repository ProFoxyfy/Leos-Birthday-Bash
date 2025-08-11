using System.Collections.Generic;
using System.IO;
using UnityEngine;
using YamlDotNet.Serialization;

public class LocalizationManager : Singleton<LocalizationManager>
{
    private Dictionary<Language, string> languageFilenames = new Dictionary<Language, string>()
    {
        [Language.English] = "language_en.yaml",
        [Language.German] = "language_de.yaml",
        [Language.Debug] = "language_null.yaml",
        [Language.Null] = ""
    };

    private Dictionary<Language, GameLanguage> loadedLanguages = new Dictionary<Language, GameLanguage>();
    [SerializeField, ReadOnly]
    private Language currentLanguage;
    private Deserializer deserializer;

    private void Awake()
    {
        DontDestroyOnLoad(this);
        deserializer = new Deserializer();
    }

    public void LoadLanguage(Language language)
    {
        if (!languageFilenames.ContainsKey(language))
        {
            Debug.LogWarning("[LocalizationManager] Warning: Language '" + language.ToString() + "'" + " does not have a filename set.");
            return;
        }

        string path = Application.streamingAssetsPath + "\\" + languageFilenames[language];
        if (!File.Exists(path))
        {
            Debug.LogWarning("[LocalizationManager] Warning: language does not exist. Path: " + path);
            return;
        }
        GameLanguage lang = deserializer.Deserialize<GameLanguage>(File.ReadAllText(path));

        loadedLanguages[language] = lang;
        currentLanguage = language;
    }

    private bool stringExists(string id, LangStringType type)
    {
        if (currentLanguage == Language.Null || !loadedLanguages.ContainsKey(currentLanguage)) return false;

        GameLanguage current = loadedLanguages[currentLanguage];
        switch (type)
        {
            case LangStringType.Metadata:
                return current.metadata.ContainsKey(id);
            case LangStringType.Subtitle:
                return current.subtitles.ContainsKey(id);
            case LangStringType.Menu:
                return current.menu.ContainsKey(id);
            default:
                return false;
        }
    }

    public string[] GetLocalizedString(string id, LangStringType type, bool encrypted = false)
    {
        if (!stringExists(id, type)) return new string[1] { id };
        GameLanguage current = loadedLanguages[currentLanguage];

        string[] output;

        switch (type)
        {
            case LangStringType.Metadata:
                output = new string[1] { current.metadata[id] };
                break;
            case LangStringType.Subtitle:
                output = current.subtitles[id];
                break;
            case LangStringType.Menu:
                output = new string[1] { current.menu[id] };
                break;
            default:
                output = new string[1] { id };
                break;
        }

        string[] result = new string[output.Length];

        if (encrypted)
        {
            for (int i = 0; i < output.Length; i++)
            {
                result[i] = TextCipher.Decrypt(output[i]);
            }
        }
        else
            result = output;

        return result;
    }

    public void UnloadLanguage(Language language)
    {
        if (!loadedLanguages.ContainsKey(language))
        {
            Debug.LogWarning("[LocalizationManager] Warning: Attempt to unload null language.");
            return;
        }
        loadedLanguages.Remove(language);

        currentLanguage = Language.Null;
    }
}

class GameLanguage
{
    public Dictionary<string, string> metadata;
    public Dictionary<string, string[]> subtitles;
    public Dictionary<string, string> menu;
}

public enum LangStringType
{
    Metadata = 0,
    Subtitle = 1,
    Menu = 2
}

public enum Language
{
    English = 0,
    German = 1,
    Debug = 2,
    Null = 3
}
