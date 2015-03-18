using UnityEngine;
using System.Collections;

public class GlacialDisk : Projectile
{
	public Detonator explosive;
	public float shatterRadius;
	public float explosiveDamage;
	public bool shattered = false;

	public override void Start()
	{
		ProjVel = 1;
	}

	public override void Update()
	{
		/*if (((SphereCollider)collider).radius < maxShockBallSize)
		{
			((SphereCollider)collider).radius += shockBallGrowthRate * Time.deltaTime / 2;
			blastRadius += shockBallGrowthRate * Time.deltaTime;
			explosiveDamage += shockBallGrowthRate * Time.deltaTime / 2;
		}
		if (particleSystem.startSize < maxShockBallSize)
		{
			//particleSystem.startLifetime += shockBallGrowthRate * Time.deltaTime;
			particleSystem.startSize += shockBallGrowthRate * Time.deltaTime;
		}*/
	}

	public override void Collide()
	{
		CreateExplosion(true);

		((GlacialSling)Creator).RemoveDisk(this);
 		base.Collide();
	}


	void OnDestroy()
	{
		if (Creator != null)
		{
			((GlacialSling)Creator).RemoveDisk(this);
		}
	}

	private void CreateExplosion(bool impact = false)
	{
		if (!shattered)
		{
			shattered = true;
			//DO THE THING!
			Detonator det;
			if (explosive != null)
			{
				det = (Detonator)GameObject.Instantiate(explosive, transform.position, Quaternion.identity);

				if (impact)
				{
					det.size = det.size / 2;
				}

				det.Explode();
			}
			gameObject.particleSystem.enableEmission = false;
			gameObject.collider.enabled = false;
			gameObject.renderer.enabled = false;

			if (rigidbody)
			{
				Destroy(rigidbody);
			}
			enabled = false;

			if (!impact)
			{
				Collider[] hitColliders = Physics.OverlapSphere(transform.position, shatterRadius);
				int i = 0;
				while (i < hitColliders.Length)
				{
					float distFromBlast = Vector3.Distance(hitColliders[i].transform.position, transform.position);
					float parameterForMessage = -(explosiveDamage * shatterRadius / distFromBlast);

					hitColliders[i].gameObject.SendMessage("AdjustHealth", parameterForMessage, SendMessageOptions.DontRequireReceiver);
					i++;
				}
			}


			Destroy(gameObject, 5.0f);
		}
	}

	public void Shatter()
	{
		CreateExplosion(false);
	}
}
