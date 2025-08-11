using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SwingingDoorScript : MonoBehaviour
{
	public Texture2D closed;
	public Texture2D open;

	private Leo leo;

	private AudioManager audMan;
	public AudioObject openSnd;

	private MeshRenderer mr;

	private float openDuration = 3f;
	private float openTimer = 0f;
	private bool isOpen = false;
	private bool isPlayerNoise = false;

	private void Awake()
	{
		leo = FindObjectOfType<Leo>();
		mr = GetComponent<MeshRenderer>();
		audMan = GetComponent<AudioManager>();
	}

	void SetOpen(bool value)
	{
		isOpen = value;
		mr.material.SetTexture("_OverlayTex", value ? open : closed);
		if (value)
			audMan.PlaySound(openSnd);

		if (leo == null)
			leo = FindObjectOfType<Leo>();
		if (value && leo && isPlayerNoise)
			leo.Hear(transform.position, 30);
	}

	private void Update()
	{
		if (openTimer > 0f)
			openTimer -= Time.deltaTime;
		if (openTimer > 0 && !isOpen)
			SetOpen(true);
		else if (openTimer <= 0 && isOpen)
			SetOpen(false);
	}

	private void OnTriggerStay(Collider other)
	{
		if (other.CompareTag("Player"))
			isPlayerNoise = true;
		else
			isPlayerNoise = false;
		openTimer = openDuration;
	}
}
