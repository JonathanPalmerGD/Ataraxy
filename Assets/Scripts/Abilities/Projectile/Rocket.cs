﻿using UnityEngine;
using System.Collections;

public class Rocket : Projectile
{
	public GameObject target;

	public bool homing = true;
	public float explosiveDamage;

	public float homingVelocity = 1200;
	public float blastRadius;
	public Vector3 dirToTarget;
	private float fuelRemaining;
	private bool detonateOnAnything = false;
	public Detonator explosive;
	public GameObject body;

	void Start()
	{
		Damage = 5;
		blastRadius = 5;
		explosiveDamage = 1;
		fuelRemaining = 5f;
		body = transform.FindChild("Rocket Body").gameObject;
		//explosive = transform.FindChild("Detonator-Rocket").GetComponent<Detonator>();
	}

	void Update()
	{
		if (fuelRemaining > 0)
		{
			fuelRemaining -= Time.deltaTime;

			if (homing)
			{
				if (target == null)
				{
					fuelRemaining = 0;
				}
				else
				{
					//Update the direction we want to go.
					dirToTarget = target.transform.position - (transform.position + Time.deltaTime * rigidbody.velocity);
					dirToTarget.Normalize();

					//Apply a force in 
					rigidbody.AddForce(dirToTarget * homingVelocity * rigidbody.mass);

					//Debug.Log("Current Speed: " + rigidbody.velocity.magnitude + "\nFuel: " + fuelRemaining);
				}
			}
		}
		else
		{
			rigidbody.useGravity = true;
			rigidbody.drag = .3f;
			gameObject.particleSystem.enableEmission = false;
		}

		//Face the the homing object in the direction it is moving. This gives the illusion of turning.
		transform.LookAt(transform.position + rigidbody.velocity * 3);
	}

	public override void Collide()
	{
		rigidbody.drag += 2;
		Detonator det;
		if (explosive != null)
		{
			det = (Detonator)GameObject.Instantiate(explosive, transform.position, Quaternion.identity);

			det.Explode();
		}
		gameObject.particleSystem.enableEmission = false;
		gameObject.collider.enabled = false;
		body.renderer.enabled = false;
		enabled = false;
		body.SetActive(false);

		Collider[] hitColliders = Physics.OverlapSphere(transform.position, blastRadius);
		int i = 0;
		while (i < hitColliders.Length)
		{
			float distFromBlast = Vector3.Distance(hitColliders[i].transform.position, transform.position);
			float parameterForMessage = -(explosiveDamage * blastRadius / distFromBlast);

			hitColliders[i].gameObject.SendMessage("AdjustHealth", parameterForMessage, SendMessageOptions.DontRequireReceiver);
			i++;
		}
		Destroy(gameObject, 3.0f);
	}
}