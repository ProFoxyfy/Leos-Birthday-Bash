using TMPro;

public class TextProgress : BaseProgressBar
{
	private TMP_Text text;

	private void Awake()
	{
		text = GetComponent<TMP_Text>();
	}

	public override void Refresh()
	{
		text.text = this.progress.ToString();
	}
}