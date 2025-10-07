using UnityEngine;

public class CoreInitializer : MonoBehaviour
{
	public static bool hasLoaded = false;
	public static Vector2 uiResolution = new Vector2(480, 360);
	public const string version = "1.2d";

	private void Awake()
	{
		GlobalsManager.Instance.style = Resources.Load<UIStyle>("DefaultStyle");

		MusicManager.Instance.sampleLoudness = false;
		MusicManager.Instance.SetVolume(1f);
		MusicManager.Instance.audioSource.pitch = 1f;
		
		SubtitleManager.Instance.Init();
		UICamera.Instance.Initialize();

		Shader.SetGlobalFloat("_VertexGlitchStrength", 0f);
		Shader.SetGlobalFloat("_FogEnd", -1f);

		if (!FlagManager.Instance.hasLoaded)
		{
			FlagManager.Instance.LoadSettings();
			FlagManager.Instance.Load();
		}

		if (RichPresenceManager.Instance.loaded)
			RichPresenceManager.Instance.UpdateActivity();

		// BUG: This prevents the CoreInitializer from running in the editor under certain circumstances.
		Language gameLang;
		if (hasLoaded)
		{
			FlagManager.Instance.SaveSettings();
			MusicManager.Instance.StopAll();
			Time.timeScale = 1f;

			gameLang = (Language)FlagManager.Instance.GetSetting("language");
			LocalizationManager.Instance.LoadLanguage(gameLang);
			AudioListenerManager.Instance.MoveListener(Camera.main != null ? Camera.main.transform : UICamera.Instance.main.transform);
			Destroy(gameObject);
			return;
		}
		hasLoaded = true;

		RichPresenceManager.Instance.Init();
		RichPresenceManager.Instance.UpdateActivity();

		FlagManager.Instance.LoadSettings();
		FlagManager.Instance.Load();

		Vector2 res = Vector2Helper.ResToVec2(GlobalsManager.Instance.resolutions[(int)FlagManager.Instance.GetSetting("resolution")]);
		Screen.SetResolution((int)res.x, (int)res.y, Screen.fullScreen);

		GameInputManager.Instance.enabled = true;
		DebugManager.Initialize();

		AudioListenerManager.Instance.MoveListener(Camera.main != null ? Camera.main.transform : UICamera.Instance.main.transform);

		gameLang = (Language)FlagManager.Instance.GetSetting("language");
		LocalizationManager.Instance.LoadLanguage(gameLang);
	}

	private void OnApplicationQuit()
	{
		hasLoaded = false;
		RichPresenceManager.Instance.loaded = false;
	}

	private void OnGUI()
	{
		if (!Application.isEditor) return;
		DebugManager.Render();
	}
}
