using UnityEngine;
using System.Collections;

public class ShockBall : Projectile
{
	public Detonator explosive;
	private float shockBallGrowthRate = 3f;
	private float maxShockBallSize = 4f;
	public float blastRadius;
	public float explosiveDamage;

	public override void Start()
	{
		Damage = 5;
		ProjVel = 1;
		explosiveDamage = 3;
		blastRadius = 5;
		//explosive = transform.FindChild("Detonator-Shock").GetComponent<Detonator>();
	}

	public override void Update()
	{
		if (((SphereCollider)collider).radius < maxShockBallSize)
		{
			((SphereCollider)collider).radius += shockBallGrowthRate * Time.deltaTime / 2;
			blastRadius += shockBallGrowthRate * Time.deltaTime;
			explosiveDamage += shockBallGrowthRate * Time.deltaTime / 2;
		}
		if (particleSystem.startSize < maxShockBallSize)
		{
			//particleSystem.startLifetime += shockBallGrowthRate * Time.deltaTime;
			particleSystem.startSize += shockBallGrowthRate * Time.deltaTime;
		}
	}

	public void Shocked()
	{
		//DO THE THING!
		Detonator det;
		if (explosive != null)
		{
			det = (Detonator)GameObject.Instantiate(explosive, transform.position, Quaternion.identity);

			det.Explode();
		}
		gameObject.particleSystem.enableEmission = false;
		gameObject.collider.enabled = false;
		Destroy(rigidbody);
		enabled = false;

		Collider[] hitColliders = Physics.OverlapSphere(transform.position, blastRadius);
		int i = 0;
		while (i < hitColliders.Length)
		{
			float distFromBlast = Vector3.Distance(hitColliders[i].transform.position, transform.position);
			float parameterForMessage = -(explosiveDamage * blastRadius / distFromBlast);

			hitColliders[i].gameObject.SendMessage("AdjustHealth", parameterForMessage * Creator.Carrier.DamageAmplification, SendMessageOptions.DontRequireReceiver);
			i++;
		} 


		Destroy(gameObject, 5.0f);
	}
}
