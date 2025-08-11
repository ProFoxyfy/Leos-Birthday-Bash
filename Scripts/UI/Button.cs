using System;
using TMPro;
using UnityEngine;
using UnityEngine.Assertions;
using UnityEngine.Events;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public class Button : MonoBehaviour, IClickable
{
	private TMP_Text text;
	private Image img;

	public bool isTextButton = false;
	public bool isHybrid = false;

	public bool isHoverable { get; set; }
	public bool isEnabled
	{
		get
		{
			return base.enabled;
		}
		set
		{
			base.enabled = value;
		}
	}

	public TextButtonEffect textEffect;
	public ImageButtonEffect imageEffect;

	public Sprite normalSprite;
	public Sprite hoverSprite;
	public Sprite downSprite;

	public Color normalColor;
	public Color hoverColor;
	public Color downColor;

	private AudioManager audMan;

	public bool hoverSound = false;

	public UnityEvent onClick = new UnityEvent();

	public ClickableState state { get; set; }

	public bool usePresetAction = false;

	public MenuPresetAction action;

	public string actionArgument1 = "";
	public string actionArgument2 = "";
	public string actionArgument3 = "";

	[ReadOnlyAttribute]
	public string currentStateDebug = "---";

	private void Awake()
	{
		isHoverable = true;
		TryGetComponent<Image>(out img);
		text = GetComponentInChildren<TMP_Text>();
		audMan = gameObject.AddComponent<AudioManager>();
		if (usePresetAction)
			onClick.AddListener(new UnityAction(DefaultActionClick));
	}

	void UpdateButton()
	{
		if (!isHybrid)
		{
			Assert.IsFalse(isTextButton && text == null, "Text button requires a TMPro text component.");
			Assert.IsFalse(!isTextButton && img == null, "Image button requires an image component.");
		}
		else
			Assert.IsTrue(isHybrid && img != null && text != null, "Hybrid button requires both a TMPro text component and an image component.");
		currentStateDebug = this.state.ToString();
		if (isTextButton || isHybrid)
		{
			switch (textEffect)
			{
				case TextButtonEffect.None:
					break;
				case TextButtonEffect.Underline:
					text.fontStyle = this.state == ClickableState.Hover ? FontStyles.Underline : FontStyles.Normal;
					break;
				case TextButtonEffect.Bold:
					text.fontStyle = this.state == ClickableState.Hover ? FontStyles.Bold : FontStyles.Normal;
					break;
				case TextButtonEffect.Strikethrough:
					text.fontStyle = this.state == ClickableState.Hover ? FontStyles.Strikethrough : FontStyles.Normal;
					break;
			}
		}
		if (!isTextButton || isHybrid)
		{
			switch (imageEffect)
			{
				case ImageButtonEffect.None:
					break;
				case ImageButtonEffect.Sprite:
					switch (state)
					{
						case ClickableState.None:
							img.sprite = normalSprite;
							break;
						case ClickableState.Hover:
							img.sprite = hoverSprite;
							break;
						case ClickableState.Down:
							img.sprite = downSprite;
							break;
					}
					break;
				case ImageButtonEffect.Tint:
					switch (state)
					{
						case ClickableState.None:
							img.color = normalColor;
							break;
						case ClickableState.Hover:
							img.color = hoverColor;
							break;
						case ClickableState.Down:
							img.color = downColor;
							break;
					}
					break;
			}
		}
	}

	private void LoadGame()
	{
		SceneManager.LoadSceneAsync("Game", LoadSceneMode.Single);
	}

	private void DoDoubtRoute()
	{
		bool doCave = FlagManager.Instance.GetFlag(6) == 0;
		GlobalsManager.Instance.currentMode = doCave ? GameMode.Cave : GameMode.Doubt;
		SceneManager.LoadSceneAsync("Game", LoadSceneMode.Single);
	}

	public void DefaultActionClick()
	{
		if (!usePresetAction || action == MenuPresetAction.None) return;
		switch (action)
		{
			case MenuPresetAction.None:
				break;
			case MenuPresetAction.LoadScene:
				SceneManager.LoadScene(actionArgument1, LoadSceneMode.Single);
				break;
			case MenuPresetAction.ExitToMenu:
				EnvironmentController.Instance.ExitToMenu();
				break;
			case MenuPresetAction.LoadGame:
				GameMode targetMode = Enum.Parse<GameMode>(actionArgument1);
				GlobalsManager.Instance.currentMode = targetMode;
				LoadGame();
				break;
			case MenuPresetAction.QuitGame:
				FlagManager.Instance.Save();
				FlagManager.Instance.SaveSettings();
#if UNITY_EDITOR
				UnityEditor.EditorApplication.isPlaying = false;
#else
                Application.Quit();
#endif
				break;
			case MenuPresetAction.DoDoubtConsequence:
				GlobalsManager.Instance.currentMode = GameMode.DoubtConsequence;
				SceneManager.LoadSceneAsync("Game", LoadSceneMode.Single);
				break;
			case MenuPresetAction.BeginDoubtRoute:
				DoDoubtRoute();
				break;
		}
	}

	void SetState(ClickableState newState)
	{
		this.state = newState;
		UpdateButton();
	}

	public void Down()
	{
		SetState(ClickableState.Down);
	}

	public void Up()
	{
		if (state == ClickableState.Down)
			onClick.Invoke();
		SetState(ClickableState.None);
	}

	public void Hover()
	{
		if (hoverSound)
			audMan.PlayOneShot(GlobalsManager.Instance.style.hoverSound);
		SetState(ClickableState.Hover);
	}

	public void Unhover()
	{
		SetState(ClickableState.None);
	}

	private void Update()
	{
		if (!GlobalsManager.Instance.activeCursor.hoveringOverElement || (UnityEngine.Object)GlobalsManager.Instance.activeCursor.lastObject != this)
			this.Unhover();
	}
}

public enum TextButtonEffect
{
	None,
	Underline,
	Bold,
	Strikethrough
}

public enum ImageButtonEffect
{
	None,
	Sprite,
	Tint
}

public enum MenuPresetAction
{
	None,
	LoadScene,
	ExitToMenu,
	QuitGame,
	LoadGame,
	BeginDoubtRoute,
	DoDoubtConsequence
}
