using System.IO;
using System.Runtime.Serialization.Formatters.Binary;
using UnityEngine;

public class LevelLoader : MonoBehaviour
{
	public LevelData LoadFromPath(string path)
	{
		FileStream file = new FileStream(path, FileMode.Open, FileAccess.Read, FileShare.ReadWrite);
		BinaryFormatter bf = new BinaryFormatter();
		LevelData data;
		data = (LevelData)bf.Deserialize(file);
		file.Close();
		return data;
	}

	public LevelData LoadFromAsset(TextAsset asset)
	{
		BinaryFormatter bf = new();
		LevelData data;
		MemoryStream stream = new MemoryStream(asset.bytes);
		data = (LevelData)bf.Deserialize(stream);
		stream.Close();
		return data;
	}

	public LevelData LoadFromStream(FileStream stream)
	{
		BinaryFormatter bf = new BinaryFormatter();
		LevelData data;
		data = (LevelData)bf.Deserialize(stream);
		stream.Close();
		return data;
	}
}
