using TMPro;
using UnityEngine;
using UnityEngine.InputSystem;
using UnityEngine.InputSystem.Utilities;

[RequireComponent(typeof(Button))]
public class TextBox : BaseUIInput
{
	private TMP_Text text;
	private Button button;
	[SerializeField, ReadOnly]
	private bool sinkInput = false;
	private string validCharacterKeysNumeric = "1234567890";
	private string validCharacterKeysDecimal = "1234567890,";
	private string validCharacterKeysAlphanumeric = "1234567890abcdefghijkmnopqrstuvwxyzABCDEFGHIJKMNOPQRSTUVWXYZ";
	public TextBoxMode mode;
	public int limit;
	private InputAction delete;
	private MenuCursor cursor;
	public string currentText { get; private set; }

	private void Awake()
	{
		cursor = GlobalsManager.Instance.activeCursor;
		delete = GameInputManager.Instance.GetAction("Backspace");
		currentText = "";
		text = GetComponentInChildren<TMP_Text>();
		button = GetComponent<Button>();
		button.onClick.AddListener(this.ToggleFocusState);

		InputSystem.onAnyButtonPress.Call(ctrl => KeyPress(ctrl));
	}

	private string GetValidChars()
	{
		switch (mode)
		{
			case TextBoxMode.Numeric:
				return validCharacterKeysNumeric;
			case TextBoxMode.Alphanumeric:
				return validCharacterKeysAlphanumeric;
			case TextBoxMode.Decimal:
				return validCharacterKeysDecimal;
			default:
				return "";
		}
	}

	private void KeyPress(InputControl key)
	{
		if (!sinkInput) return;
		if (!GetValidChars().Contains(key.displayName)) return;
		if (currentText.Length >= limit) return;
		currentText += key.displayName;
	}

	public void ToggleFocusState()
	{
		sinkInput = !sinkInput;
	}

	public void SetText(string txt)
	{
		this.currentText = txt;
	}

	private void Update()
	{
		text.text = currentText;
		sinkInput = sinkInput && cursor.hoveringOverElement;
		if (!sinkInput) return;
		if (delete.WasPressedThisFrame() && currentText.Length > 0) currentText = currentText.Remove(currentText.Length - 1, 1);
	}
}

public enum TextBoxMode
{
	Numeric,
	Alphanumeric,
	Decimal
}