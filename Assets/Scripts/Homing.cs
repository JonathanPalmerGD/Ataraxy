using UnityEngine;
using System.Collections;

public class Homing : Projectile 
{
	public GameObject target;

	public bool homing = true;

	public float homingVelocity = 30;
	public Vector3 dirToTarget;
	private float fuelRemaining = 8;
	private bool detonateOnAnything = false;
	public Detonator explosive;
	public GameObject body;

	void Start()
	{
		Damage = 5;
		body = transform.FindChild("Rocket Body").gameObject;
		explosive = transform.FindChild("Detonator-Base").GetComponent<Detonator>();
	}
	
	void Update() 
	{
		if (fuelRemaining > 0)
		{
			fuelRemaining -= Time.deltaTime;

			if (homing)
			{
				//Update the direction we want to go.
				dirToTarget = target.transform.position - (transform.position + Time.deltaTime * rigidbody.velocity);
				dirToTarget.Normalize();

				//Apply a force in 
				rigidbody.AddForce(dirToTarget * homingVelocity * rigidbody.mass);

				//Debug.Log("Current Speed: " + rigidbody.velocity.magnitude + "\nFuel: " + fuelRemaining);
			}
		}
		else
		{
			rigidbody.useGravity = true;
			rigidbody.drag = 0;
		}

		//Face the the homing object in the direction it is moving. This gives the illusion of turning.
		transform.LookAt(transform.position + rigidbody.velocity * 3);
	}

	void OnTriggerEnter(Collider collider)
	{
		if (enabled)
		{
			string cTag = collider.gameObject.tag;
			if (cTag == "Enemy" || cTag == "Player")// || cTag == "Entity" || cTag == "Island")
			{
				Entity atObj = collider.gameObject.GetComponent<Entity>();
				if (atObj != null)
				{
					if (atObj.Faction == Faction)
					{
						//Debug.LogWarning("Projectile collided with same Faction as firing source.\n");
						return;
					}
					else
					{
						//If the projectile is from the Player
						if (Faction == Allegiance.Player && atObj.Faction == Allegiance.Enemy)
						{

							//Deal damage to the enemy
							atObj.GetComponent<Enemy>().AdjustHealth(-Damage);

							//Apply AbilityEffect to the target.
							for (int i = 0; i < projectileEffects.Count; i++)
							{
								atObj.GetComponent<Enemy>().ApplyAbilityEffect(projectileEffects[i]);
							}
						}
						//Else if it is from an Enemy
						else if (Faction == Allegiance.Enemy && atObj.Faction == Allegiance.Player)
						{
							//Deal damage to the enemy
							GameManager.Instance.player.AdjustHealth(-Damage);

							//Apply AbilityEffect to the target.
							for (int i = 0; i < projectileEffects.Count; i++)
							{
								GameManager.Instance.player.ApplyAbilityEffect(projectileEffects[i]);
							}

							//Award experience to the enemy who fired it.
							if (AwardXPOnHit && Shooter != null)
							{
								Shooter.GainExperience(XpBountyOnHit);
							}
						}
					}
				}
			}

			explosive.Explode();
			gameObject.particleSystem.enableEmission = false;
			gameObject.collider.enabled = false;
			enabled = false;
			body.SetActive(false);
			Destroy(gameObject, 3.0f);
		}
	}

	/*void OnDrawGizmos()
	{
		Gizmos.color = Color.black;
		Gizmos.DrawLine(transform.position, transform.position + rigidbody.velocity * 50);
	}*/
}
