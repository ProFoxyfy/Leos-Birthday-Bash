public interface IClickable
{
	public bool isHoverable { get; set; }
	public bool isEnabled { get; set; }
	ClickableState state { get; set; }
	void Down();
	void Up();
	void Hover();
	void Unhover();
}

public enum ClickableState
{
	None,
	Hover,
	Down
}