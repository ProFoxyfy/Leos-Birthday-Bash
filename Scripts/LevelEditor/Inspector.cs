using AYellowpaper.SerializedCollections;
using System.Collections.Generic;
using UnityEngine;

public class Inspector : MonoBehaviour
{
	[SerializedDictionary]
	public SerializedDictionary<string, BaseUIInput> fields = new SerializedDictionary<string, BaseUIInput>();
}