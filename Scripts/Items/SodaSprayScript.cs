using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SodaSprayScript : MonoBehaviour
{
	private Rigidbody rb;
	private float speed = 2f;
	private float lifetime = 30f;

	private void Start()
	{
		rb = GetComponent<Rigidbody>();
		rb.linearVelocity = transform.forward * speed;

		Invoke("Remove", lifetime);
	}

	private void Remove()
	{
		Destroy(this.gameObject);
	}
}
