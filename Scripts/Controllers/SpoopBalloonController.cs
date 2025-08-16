using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SpoopBalloonController : MonoBehaviour
{
	Vector3 direction;
	Rigidbody rb;
	float speed = 2f;
	PlayerManager plr;
	AudioManager audMan;
	public AudioObject noise;

	private void Awake()
	{
		audMan = GetComponent<AudioManager>();
		plr = EnvironmentController.Instance.GetPlayer(0);
		rb = GetComponent<Rigidbody>();
	}

	private void Start()
	{
		audMan.PlaySound(noise);
	}

	private void Update()
	{
		direction = (plr.transform.position - transform.position).normalized;
		rb.linearVelocity = direction * speed;
	}
}
