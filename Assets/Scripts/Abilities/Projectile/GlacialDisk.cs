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
		Damage = 1;
		ProjVel = 1;
		explosiveDamage = 3;
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
		((GlacialSling)Creator).RemoveDisk(this);
 		base.Collide();
	}

	public void Shatter()
	{
		if (!shattered)
		{
			shattered = true;
			//DO THE THING!
			Detonator det;
			if (explosive != null)
			{
				det = (Detonator)GameObject.Instantiate(explosive, transform.position, Quaternion.identity);

				det.Explode();
			}
			gameObject.particleSystem.enableEmission = false;
			gameObject.collider.enabled = false;
			gameObject.renderer.enabled = false;

			Destroy(rigidbody);
			enabled = false;

			Collider[] hitColliders = Physics.OverlapSphere(transform.position, shatterRadius);
			int i = 0;
			while (i < hitColliders.Length)
			{
				float distFromBlast = Vector3.Distance(hitColliders[i].transform.position, transform.position);
				float parameterForMessage = -(explosiveDamage * shatterRadius / distFromBlast);

				hitColliders[i].gameObject.SendMessage("AdjustHealth", parameterForMessage, SendMessageOptions.DontRequireReceiver);
				i++;
			}


			Destroy(gameObject, 5.0f);
		}
	}
}
