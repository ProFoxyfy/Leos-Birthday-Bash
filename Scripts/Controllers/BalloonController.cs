using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BalloonController : MonoBehaviour
{
	Vector3 direction;
	float timer = 0f;
	Rigidbody rb;
	float speed = 1.5f;
	LitSprite sprite;

	private void Awake()
	{
		sprite = GetComponentInChildren<LitSprite>();
		rb = GetComponent<Rigidbody>();

		sprite.tint = Color.HSVToRGB(Random.Range(0f, 1f), 1f, 1f);

		RandomDirection();
		timer = 3f;
	}

	private void Update()
	{
		if (timer <= 0f)
		{
			RandomDirection();
			timer = 3f;
		}

		timer -= Time.deltaTime;
		rb.linearVelocity = direction * speed;
	}

	private void RandomDirection()
	{
		direction = new Vector3(Random.Range(-1f, 1f), 0f, Random.Range(-1f, 1f)).normalized;
	}
}
