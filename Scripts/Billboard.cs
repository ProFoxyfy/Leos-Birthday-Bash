using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Billboard : MonoBehaviour
{
	Camera cam;

	void Start()
	{
		cam = Camera.main;
	}

	void LateUpdate()
	{
		if (transform.parent != null && transform.parent.CompareTag("Player"))
		{
			Vector3 rotation = cam.transform.rotation.eulerAngles;
			Vector3 result = new Vector3(0f, rotation.y, 0f);
			transform.localRotation = Quaternion.identity;
		}
		else
		{
			transform.rotation = cam.transform.rotation;
		}
	}
}
