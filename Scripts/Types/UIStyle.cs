using UnityEngine;

[CreateAssetMenu(fileName = "NewStyle", menuName = "Custom/UI Style")]
public class UIStyle : ScriptableObject
{
	[Header("Checkbox")]
	public Sprite tg_on;
	public Sprite tg_off;
	[Header("Step Progress Bar")]
	public Sprite spb_image;
	public Sprite spb_ghost;
	[Header("Cursor")]
	public Sprite cr_normal;
	public Sprite cr_hover;
	[Header("Other")]
	public UITransitionObject transition;
	[Header("SFX")]
	public AudioObject hoverSound;
}