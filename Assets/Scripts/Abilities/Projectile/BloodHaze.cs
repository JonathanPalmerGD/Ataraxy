using UnityEngine;
using System.Collections;

public class BloodHaze : Projectile
{
	public float deathTimer;
	public override void Start()
	{
		ProjVel = 1;
	}

	public override void Update()
	{
		deathTimer -= Time.deltaTime;
		if(deathTimer <= 0)
		{
			rigidbody.velocity = Vector3.zero;
			particleSystem.Stop();
			GameObject.Destroy(gameObject, 2.0f);
			enabled = false;
		}
	}

	public override void Collide()
	{

	}
}
