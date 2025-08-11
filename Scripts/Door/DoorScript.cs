using System.Collections;
using System.Collections.Generic;
using Unity.AI.Navigation;
using UnityEngine;
using UnityEngine.InputSystem;

public class DoorScript : MonoBehaviour, IInteractable
{
	public AudioObject openSnd;
	public AudioObject closeSnd;
	public Texture2D openTex;
	public Texture2D closeTex;
	private AudioManager audMan;
	private MeshRenderer mesh;
	private Collider coll;
	private float openTime = 3f;
	private float time = 0f;
	private bool isOpen = false;
	private bool lastState = false;
	private int npcLayer = 0;
	private Leo leo;

	private void Awake()
	{
		npcLayer = LayerMask.NameToLayer("NPC");
		npcLayer = LayerMask.NameToLayer("NPC");
		coll = GetComponent<Collider>();
		mesh = GetComponent<MeshRenderer>();
		audMan = GetComponent<AudioManager>();
		leo = FindObjectOfType<Leo>(true);
	}

	private void SetOpen(bool open, bool isPlayer = false)
	{
		time = open ? openTime : 0f;
		isOpen = open;

		if (leo == null)
			leo = FindObjectOfType<Leo>();
		if (!isPlayer || leo == null) return;
		leo.Hear(transform.position, 32);
	}

	public void Use()
	{
		this.SetOpen(true, true);
	}

	private void OnCollisionEnter(Collision collision)
	{
		if (collision.collider.CompareTag("Player")) return;
		if (collision.gameObject.layer != npcLayer) return;
		this.SetOpen(true);
	}

	private void Update()
	{
		time = Mathf.Max(0, time - Time.deltaTime);
		mesh.material.SetTexture("_OverlayTex", isOpen ? openTex : closeTex);
		coll.isTrigger = isOpen;

		if (isOpen != lastState)
			audMan.PlaySound(isOpen ? openSnd : closeSnd);

		lastState = isOpen;

		if (time <= 0f)
			isOpen = false;
	}
}
