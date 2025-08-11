using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ProjectileController : MonoBehaviour
{
	Rigidbody rb;
	LitSprite sprite;
	Vector3 velocity;
	PlayerManager plr;
	private bool beenThrown = false;

	private void Awake()
	{
		plr = EnvironmentController.Instance.GetPlayer(0);
		rb = GetComponent<Rigidbody>();
		sprite = GetComponentInChildren<LitSprite>();
		velocity = Vector3.zero;
	}

	private void Die()
	{
		EnvironmentController.Instance.gameManager.Broadcast("projectileDie");
		Destroy(gameObject);
	}

	private void OnTriggerEnter(Collider other)
	{
		if (other.CompareTag("Leo") && beenThrown)
		{
			FindObjectOfType<Doubt>().Hit();
			Die();
			return;
		}
		else if (other.CompareTag("Player") && plr.projectile == null)
		{
			sprite.tint = new Color(1, 1, 1, 0.5f);
			plr.projectile = this;
		}
	}

	private Vector3 NoXZ(Vector3 vector)
	{
		return new Vector3(vector.x, 0f, vector.z);
	}

	private void Update()
	{
		rb.isKinematic = true;
		if (plr.projectile == this && !beenThrown)
			transform.position = plr.transform.position + Vector3.Scale(NoXZ(plr.cam.transform.forward.normalized) * 0.3f, new Vector3(1f, 0.3f, 1f));
		if (!beenThrown) return;
		transform.position += NoXZ(transform.forward) * 4f * Time.deltaTime;
	}

	public void Throw()
	{
		if (beenThrown) return;
		this.beenThrown = true;
		transform.SetParent(null);
		sprite.tint = Color.white;
		transform.eulerAngles = plr.cam.transform.eulerAngles;
		Invoke("Die", 15f);
	}
}
