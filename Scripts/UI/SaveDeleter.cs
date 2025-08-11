using System.IO;
using UnityEngine;

public class SaveDeleter : MonoBehaviour
{
	public void YeetIt()
	{
		File.Delete(Application.persistentDataPath + Path.PathSeparator + "save.dat");

		for (int i = 0; i < FlagManager.Instance.flags.Length; i++)
			FlagManager.Instance.SetFlag(i, 0);

		Application.Quit();
	}
}
