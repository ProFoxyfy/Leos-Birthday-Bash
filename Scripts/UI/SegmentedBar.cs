using UnityEngine;
using UnityEngine.UI;

public class SegmentedBar : BaseProgressBar
{
	private Image bar;
	private Image ghost;
	public int segments;
	private float segmentSize;

	private void Awake()
	{
		ghost = transform.parent.GetComponent<Image>();
		bar = GetComponent<Image>();
		ghost.sprite = GlobalsManager.Instance.style.spb_ghost;
		bar.sprite = GlobalsManager.Instance.style.spb_image;
		segmentSize = (1f / segments) * bar.sprite.texture.width;
	}

	public override void Refresh()
	{
		float segmentAmount = Mathf.RoundToInt(this.progress * segments) / (float)segments;
		bar.fillAmount = segmentAmount;
	}
}
