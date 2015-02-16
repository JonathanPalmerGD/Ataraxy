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
		Vector3 firstPos = ((Player)Creator.Carrier).FirePoints[0].transform.position;
		lr.SetPosition(0, firstPos);
		lr.SetPosition(1, transform.position - transform.forward * transform.localScale.z/2);
		lr.material.mainTextureScale = new Vector2(Vector3.Distance(firstPos, transform.position), 1);
	}

	public virtual void Start() 
	{
		Init();

		ProjVel = 1900;
	}

	public virtual void Update() 
	{
		timeRemaining -= Time.deltaTime;

		if (timeRemaining < 0)
		{
			((GrapplingHook)Creator).RemoveProjectile();
		}

		//Draw line to the creator.
		DrawChain();

		if (hookState == GrapplingState.Firing)
		{
			//Do nothing. We already have our velocity.
		}
		else if (hookState == GrapplingState.Attached)
		{
			Vector3 pullDir = transform.position - Creator.Carrier.transform.position;
			pullDir.Normalize();
			//Pull the creator
			Creator.Carrier.ExternalMove(pullDir, 1, ForceMode.VelocityChange);
		}
		else if (hookState == GrapplingState.Pulling)
		{
			if (pullingEntity != null)
			{
				Vector3 pullDir = Creator.Carrier.transform.position - transform.position;
				pullDir.Normalize();
				//Pull the creator
				pullingEntity.ExternalMove(pullDir, 2f, ForceMode.VelocityChange);
				
				//This could let us move linearly towards the player.
				//rigidbody.velocity = Vector3.zero;
				rigidbody.AddForce(pullDir * 2f, ForceMode.VelocityChange);
			}
			else if (pullingGameObject != null)
			{
				if (pullingGameObject.rigidbody != null)
				{
					Vector3 pullDir = Creator.Carrier.transform.position - transform.position;
					pullDir.Normalize();
					//Pull the creator
					//pullingGameObject.rigidbody.AddForce(pullDir * 2f, ForceMode.VelocityChange);
					pullingGameObject.transform.position = transform.position + transform.forward * (transform.localScale.z / 2);

					//This could let us move linearly towards the player.
					//rigidbody.velocity = Vector3.zero;
					rigidbody.AddForce(pullDir * 2f, ForceMode.VelocityChange);
				}
			}
		}
	}

	void OnDestroy()
	{
		((GrapplingHook)Creator).ProjectileDestroyed();
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
								Debug.LogError("Grappling Hook not configured for enemy use\n");
								//Deal damage to the enemy
								GameManager.Instance.player.AdjustHealth(-Damage * (1 + Creator.Carrier.Level * .1f));

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

			else if(cTag == "Projectile")
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
			else
			{
				BecomeAttached();
			}
		}
	}

	public void RetractPullingTarget(GameObject collidedObject)
	{
		if (!collidedYet)
		{
			rigidbody.velocity = Vector3.zero;

			hookState = GrapplingState.Pulling;
			pullingGameObject = collidedObject;
			collidedYet = true;
			timeRemaining += 5;
		}
	}

	public void RetractPullingTarget(Entity collidedEntity)
	{
		if (!collidedYet)
		{
			rigidbody.velocity = Vector3.zero;

			hookState = GrapplingState.Pulling;
			pullingEntity = collidedEntity;
			collidedYet = true;
			timeRemaining += 5;
		}
	}

	public void BecomeAttached()
	{
		if (!collidedYet)
		{
			Creator.Carrier.rigidbody.useGravity = false;
			rigidbody.velocity = Vector3.zero;
			hookState = GrapplingState.Attached;
			collidedYet = true;
			timeRemaining += 5;
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
				Vector3 pullDir = Creator.Carrier.transform.position - transform.position;
				pullDir.Normalize();
				//Pull the creator
				pullingEntity.ExternalMove(pullDir, -2f, ForceMode.VelocityChange);
			}
		} 
		timeRemaining = 0;
	}

	public virtual void Collide()
	{
		//Stop moving and change state.
		
	}
}
