using UnityEngine;
using TMPro;
using UnityEngine.UI;

public class GalleryManager : MonoBehaviour
{
	public Sprite[] pictures;
	public string[] authors;
	public string[] names;
	public Image picture;
	public TMP_Text name;
	public TMP_Text author;
	public Button forward;
	public Button back;
	private int index = 0;

	void UpdatePicture()
	{
		name.text = $"\"{names[index]}\"";
		author.text = authors[index];
		picture.sprite = pictures[index];
	}

	void ChangeSelection(int increment)
	{
		int tempIndex = index + increment;

		tempIndex = tempIndex >= pictures.Length ? 0 : tempIndex;
		tempIndex = tempIndex < 0 ? pictures.Length - 1 : tempIndex;

		index = tempIndex;
		UpdatePicture();
	}

    void Awake()
    {
		back.onClick.AddListener(() => { ChangeSelection(-1); });
		forward.onClick.AddListener(() => { ChangeSelection(1); });
        UpdatePicture();
    }
}
