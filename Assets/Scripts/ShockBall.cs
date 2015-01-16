using UnityEngine;
using System.Collections;

public class ShockBall : Projectile
{
	public Detonator explosive;
	private float shockBallGrowthRate = 3f;
	private float maxShockBallSize = 4f;

	void Start()
	{
		Damage = 5;
		ProjVel = 1;
		//body = transform.FindChild("Rocket Body").gameObject;
		explosive = transform.FindChild("Detonator-Tiny").GetComponent<Detonator>();
	}

	void Update()
	{
		if (((SphereCollider)collider).radius < 4)
		{
			((SphereCollider)collider).radius += shockBallGrowthRate * Time.deltaTime / 2;
		}
		if (particleSystem.startSize < 4)
		{
			//particleSystem.startLifetime += shockBallGrowthRate * Time.deltaTime;
			particleSystem.startSize += shockBallGrowthRate * Time.deltaTime;
		}
	}

	public void Shocked()
	{
		//DO THE THING!
		explosive.Explode();
		gameObject.particleSystem.enableEmission = false;
		gameObject.collider.enabled = false;
		Destroy(rigidbody);
		enabled = false;
		Destroy(gameObject, 5.0f);
	}
}
