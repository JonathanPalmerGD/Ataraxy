using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class GrapplingHookProj : Projectile 
{
	//Grappling state - Moving, Attached (to environment), Pulling (enemy to you)
	public enum GrapplingState { Firing, Attached, Pulling };
	private GrapplingState hookState = GrapplingState.Firing;
	private Entity pullingEntity = null;
	private GameObject pullingGameObject = null;
	private bool collidedYet = false;
	private LineRenderer lr;
	public float timeRemaining = 5;
	public float hookSpeed = 20;

	public override void Init()
	{
		lr = gameObject.GetComponent<LineRenderer>();
		if (lr == null)
		{
			lr = gameObject.AddComponent<LineRenderer>();
		}

		//lr.material = new Material(Shader.Find("Particles/Additive"));

		lr.SetVertexCount(2);
		lr.SetColors(Creator.BeamColor, Creator.BeamColor);
		lr.SetWidth(.2f, .2f);
		DrawChain();
	}

	public virtual void DrawChain()
	{
		Vector3 firstPos = Creator.Carrier.FirePoints[0].transform.position;
		lr.SetPosition(0, firstPos);
		lr.SetPosition(1, transform.position - transform.forward * transform.localScale.z/2);
		lr.material.mainTextureScale = new Vector2(Vector3.Distance(firstPos, transform.position), 1);
	}

	public virtual void Start() 
	{
		Init();

		ProjVel = 2600;
	}

	public virtual void Update() 
	{
		timeRemaining -= Time.deltaTime;

		if (timeRemaining < 0)
		{
			Retract();
		}

		//Draw line to the creator.
		DrawChain();

		if (hookState == GrapplingState.Firing)
		{
			//Do nothing. We already have our velocity.
		}
		else if (hookState == GrapplingState.Attached)
		{
			HandleRetracted();

			Vector3 pullDir = transform.position - Creator.Carrier.transform.position;
			pullDir.Normalize();

			//I don't think this looks better
			//transform.LookAt(transform.position + pullDir * 5, Vector3.up);

			//Pull the creator
			//Creator.Carrier.ExternalMove(pullDir, 1, ForceMode.VelocityChange);

			Creator.Carrier.rigidbody.velocity = Vector3.zero;
			Creator.Carrier.ExternalMove(pullDir, 2 * hookSpeed, ForceMode.VelocityChange);
		}
		else if (hookState == GrapplingState.Pulling)
		{
			HandleRetracted();

			Vector3 pullDir = Creator.Carrier.transform.position - transform.position;
			pullDir.Normalize();

			//Orients the grappling hook to face away from the player as it retracts.
			transform.LookAt(transform.position - pullDir * 5, Vector3.up);

			//This could let us move linearly towards the player.
			rigidbody.velocity = Vector3.zero;
			rigidbody.AddForce(pullDir * hookSpeed, ForceMode.VelocityChange);
			if (pullingEntity != null)
			{
				//Pull the target
				pullingEntity.transform.position = transform.position + transform.forward * (transform.localScale.z / 2 + pullingEntity.transform.localScale.x / 3);
			}
			else if (pullingGameObject != null)
			{
				if (pullingGameObject.rigidbody != null)
				{
					//Pull the target - Set it inside of us
					pullingGameObject.transform.position = transform.position + transform.forward * (transform.localScale.z / 2);
				}
			}
		}
	}

	void HandleRetracted()
	{
		float distance = Vector3.Distance(transform.position, Creator.Carrier.gameObject.transform.position);

		if (distance < 4)
		{
			//Debug.LogWarning("Projectile collided with same Faction as firing source.\n");
			if (hookState == GrapplingState.Attached || hookState == GrapplingState.Pulling)
			{
				((GrapplingHook)Creator).RemoveProjectile(0);
			}
		}
	}

	void OnDestroy()
	{
		if(Creator != null)
		{
			((GrapplingHook)Creator).ProjectileDestroyed();
		}
	}

	void OnTriggerEnter(Collider collider)
	{
		if (enabled)
		{
			//Debug.Log("Projectile collided with: " + collider.gameObject.name + "\n" + collider.gameObject.tag + " Faction: " + Faction + " " + Damage);
			string cTag = collider.gameObject.tag;
			if (cTag == "Enemy" || cTag == "Player")// || cTag == "Entity" || cTag == "Island")
			{
				Entity collidedEntity = collider.gameObject.GetComponent<Entity>();
				if (collidedEntity != null)
				{
					#region Same Faction Collision
					if (collidedEntity.Faction == Faction)
					{
						//Debug.LogWarning("Projectile collided with same Faction as firing source.\n");
						if (hookState == GrapplingState.Attached || hookState == GrapplingState.Pulling)
						{
							((GrapplingHook)Creator).RemoveProjectile();
						}

						return;
					}
					#endregion
					#region Other Faction Collision
					else
					{
						#region Player Use
						//If the projectile is from the Player
						if (Faction == Allegiance.Player && collidedEntity.Faction == Allegiance.Enemy)
						{
							//Deal damage to the enemy
							collidedEntity.GetComponent<Enemy>().AdjustHealth(-Damage * (1 + Creator.Carrier.Level * .1f));

							//Make ourself a child.
							//gameObject.transform.SetParent(collidedEntity.transform);

							if (collidedEntity.rigidbody != null)
							{
								RetractPullingTarget(collidedEntity);
							}
							else
							{
								BecomeAttached();
							}
							//Apply AbilityEffect to the target.
							for (int i = 0; i < projectileEffects.Count; i++)
							{
								collidedEntity.GetComponent<Enemy>().ApplyAbilityEffect(projectileEffects[i]);
							}
						}
						#endregion
						#region Enemy Use
						//Else if it is from an Enemy
						else if (Faction == Allegiance.Enemy && collidedEntity.Faction == Allegiance.Player)
						{
							//Deal damage to the player
							GameManager.Instance.player.AdjustHealth(-Damage);

							//Grab the player!
							RetractPullingTarget(collidedEntity);

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
						#endregion
					}
					#endregion
				}
			}
			else if (cTag == "Projectile" || cTag == "Checkpoint")
			{
				
			}
			else if (cTag == "Token")
			{
				MultiToken token = collider.gameObject.GetComponent<MultiToken>();
				if (token != null)
				{
					//Pull token.
					RetractPullingTarget(collider.gameObject);
				}
			}
			else if(collider.gameObject != Creator.Carrier.gameObject)
			{
				BecomeAttached();
			}
		}
	}

	/// <summary>
	/// Pulls the target back towards the Carrier.
	/// Destroys hook if the target is immovable.
	/// </summary>
	/// <param name="collidedEntity">Collided entity.</param>
	public void RetractPullingTarget(GameObject collidedObject)
	{
		if(collidedObject.rigidbody != null)
		{
			if (!collidedYet)
			{
				rigidbody.velocity = Vector3.zero;
				collidedObject.rigidbody.velocity = Vector3.zero;

				hookState = GrapplingState.Pulling;
				pullingGameObject = collidedObject;
				collidedYet = true;
				timeRemaining += 5;
			}
		}
		else
		{
			((GrapplingHook)Creator).RemoveProjectile();
		}
	}

	/// <summary>
	/// Pulls the target back towards the Carrier.
	/// Destroys hook if the target is immovable.
	/// </summary>
	/// <param name="collidedEntity">Collided entity.</param>
	public void RetractPullingTarget(Entity collidedEntity)
	{
		if(collidedEntity.rigidbody != null)
		{
			if (!collidedYet)
			{
				//Face what we grabbed towards the Carrier?

				rigidbody.velocity = Vector3.zero;
				collidedEntity.rigidbody.velocity = Vector3.zero;
				//collidedEntity.transform.LookAt(Creator.Carrier.transform.position, Vector3.up);
			
				hookState = GrapplingState.Pulling;
				pullingEntity = collidedEntity;
				collidedYet = true;
				timeRemaining += 5;
			}
		}
		else
		{
			((GrapplingHook)Creator).RemoveProjectile();
		}
	}

	public void Retract()
	{
		collidedYet = true;
		rigidbody.velocity = Vector3.zero;
		hookState = GrapplingState.Pulling;
		hookSpeed = 80;
	}

	/// <summary>
	/// Hook attaches to location and pulls carrier.
	/// Destroys hook if carrier has no rigidbody.
	/// </summary>
	public void BecomeAttached()
	{
		if (!collidedYet)
		{
			if (Creator.Carrier.rigidbody != null)
			{
			
				rigidbody.velocity = Vector3.zero;
				Creator.Carrier.rigidbody.useGravity = false;

				hookState = GrapplingState.Attached;
				collidedYet = true;
				timeRemaining += 5;
			}
			else
			{
				Retract();
			}
		}
	}

	public void StopGrapple()
	{
		if (hookState == GrapplingState.Attached)
		{
			Vector3 pullDir = transform.position - Creator.Carrier.transform.position;
			pullDir.Normalize();

			//Pull the creator
			Creator.Carrier.ExternalMove(pullDir, -1, ForceMode.VelocityChange);
		}
		else if (hookState == GrapplingState.Pulling)
		{
			if (pullingEntity != null)
			{
				if (pullingEntity.rigidbody != null)
				{
					Vector3 pullDir = Creator.Carrier.transform.position - transform.position;
					pullDir.Normalize();
					//Pull the creator
					pullingEntity.ExternalMove(pullDir, -2f, ForceMode.VelocityChange);
				}
			}
		} 
		timeRemaining = 0;
	}

	public virtual void Collide()
	{
		//Stop moving and change state.
		
	}
}
