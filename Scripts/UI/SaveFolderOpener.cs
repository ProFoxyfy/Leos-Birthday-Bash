using UnityEngine;

public class SaveFolderOpener : MonoBehaviour
{
    public void Open()
	{
		Application.OpenURL("file://" + Application.persistentDataPath);
	}
}
