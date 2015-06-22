using UnityEngine;
using System.Collections;

public class Rocket : Projectile
{
	public GameObject target;

	public bool homing = true;
	public float explosiveDamage;

	public float homingVelocity;
	public float blastRadius;
	public float fuelRemaining;
	public Vector3 dirToTarget;
	//private bool detonateOnAnything = false;
	public Detonator explosive;
	public GameObject body;

	public AudioSource rocketThrust;

	public override void Start()
	{
		body = transform.FindChild("Rocket Body").gameObject;
		rocketThrust = AudioManager.Instance.MakeSource("Rocket_Thrust", transform.position, transform);
		rocketThrust.minDistance = 9;

		rocketThrust.loop = true;

		rocketThrust.Play();
	}

	public override void Update()
	{
		if (fuelRemaining > 0)
		{
			fuelRemaining -= Time.deltaTime;

			if (homing)
			{
				if (target == null || !target.activeInHierarchy)
				{
					if (rocketThrust != null)
					{
						rocketThrust.Stop();
					}

					fuelRemaining = 0;
				}
				else
				{
					//Update the direction we want to go.
					dirToTarget = target.transform.position - (transform.position + Time.deltaTime * GetComponent<Rigidbody>().velocity);
					dirToTarget.Normalize();

					//Apply a force in 
					GetComponent<Rigidbody>().AddForce(dirToTarget * homingVelocity * GetComponent<Rigidbody>().mass);

					//Debug.Log("Current Speed: " + rigidbody.velocity.magnitude + "\nFuel: " + fuelRemaining);
				}
			}
		}
		else
		{
			if (rocketThrust != null)
			{
				rocketThrust.Stop();
			}
			GetComponent<Rigidbody>().useGravity = true;
			GetComponent<Rigidbody>().drag = .3f;
			gameObject.GetComponent<ParticleSystem>().enableEmission = false;
		}

		//Face the the homing object in the direction it is moving. This gives the illusion of turning.
		transform.LookAt(transform.position + GetComponent<Rigidbody>().velocity * 3);
	}

	public override void ProjectileHitTarget(Entity target)
	{

	}

	public override void Collide()
	{
		GetComponent<Rigidbody>().drag += 2;
		Detonator det;
		if (explosive != null)
		{
			det = (Detonator)GameObject.Instantiate(explosive, transform.position, Quaternion.identity);

			det.Explode();
		}

		if (rocketThrust != null)
		{
			rocketThrust.Stop();
		}

		AudioSource rocketAud = AudioManager.Instance.MakeSourceAtPos("Rocket_Explosion", transform.position);
		rocketAud.minDistance = 9;
		rocketAud.Play();
		
		gameObject.GetComponent<ParticleSystem>().enableEmission = false;
		gameObject.GetComponent<Collider>().enabled = false;
		body.GetComponent<Renderer>().enabled = false;
		enabled = false;
		body.SetActive(false);

		Collider[] hitColliders = Physics.OverlapSphere(transform.position, blastRadius);
		int i = 0;
		while (i < hitColliders.Length)
		{
			float distFromBlast = Vector3.Distance(hitColliders[i].transform.position, transform.position);
			float parameterForMessage = -(explosiveDamage * blastRadius / distFromBlast);

			//Debug.Log("Dealing Damage to : " + hitColliders[i].name + "\t" + parameterForMessage + "\n");
			hitColliders[i].gameObject.SendMessage("AdjustHealth", parameterForMessage * Creator.Carrier.DamageAmplification, SendMessageOptions.DontRequireReceiver);
			i++;
		}
		Destroy(gameObject, 3.0f);
	}
}