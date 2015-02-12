﻿using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class GrapplingHookProj : Projectile 
{
	//Grappling state - Moving, Attached (to environment), Pulling (enemy to you)
	public enum GrapplingState { Firing, Attached, Pulling };
	private GrapplingState hookState = GrapplingState.Firing;
	private Entity pullingEntity = null;
	private bool collidedYet = false;
	private LineRenderer lr;

	public override void Init()
	{
		lr = gameObject.GetComponent<LineRenderer>();
		if (lr == null)
		{
			lr = gameObject.AddComponent<LineRenderer>();
		}

		//lr.material = new Material(Shader.Find("Particles/Additive"));

		lr.SetVertexCount(2);
		lr.SetColors(creator.BeamColor, creator.BeamColor);
		lr.SetWidth(.2f, .2f);
		DrawChain();
	}

	public virtual void DrawChain()
	{
		Vector3 firstPos = ((Player)creator.Carrier).FirePoints[0].transform.position;
		lr.SetPosition(0, firstPos);
		lr.SetPosition(1, transform.position);
		lr.material.mainTextureScale = new Vector2(Vector3.Distance(firstPos, transform.position), 1);
	}

	public virtual void Start() 
	{
		Init();

		ProjVel = 1200;
	}

	public virtual void Update() 
	{
		//Draw line to the creator.
		DrawChain();

		if (hookState == GrapplingState.Firing)
		{
			//Do nothing. We already have our velocity.
		}
		else if (hookState == GrapplingState.Attached)
		{
			Vector3 pullDir = transform.position - creator.Carrier.transform.position;
			pullDir.Normalize();
			//Pull the creator
			creator.Carrier.ExternalMove(pullDir, 1, ForceMode.VelocityChange);
		}
		else if (hookState == GrapplingState.Pulling)
		{
			if (pullingEntity != null)
			{
				Vector3 pullDir = creator.Carrier.transform.position - transform.position;
				pullDir.Normalize();
				//Pull the creator
				pullingEntity.ExternalMove(pullDir, 2f, ForceMode.VelocityChange);
				
				//This could let us move linearly towards the player.
				//rigidbody.velocity = Vector3.zero;
				rigidbody.AddForce(pullDir * 2f, ForceMode.VelocityChange);
			}
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
							((GrapplingHook)creator).RemoveProjectile();
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
							collidedEntity.GetComponent<Enemy>().AdjustHealth(-Damage);

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
							#endregion
					}
					#endregion
				}
			}

			else if(cTag == "Projectile")
			{
				//((GrapplingHook)creator).RemoveProjectile();
				enabled = false;
			}
			else
			{
				BecomeAttached();
			}
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
		}
	}

	public void BecomeAttached()
	{
		if (!collidedYet)
		{
			creator.Carrier.rigidbody.useGravity = false;
			rigidbody.velocity = Vector3.zero;
			hookState = GrapplingState.Attached;
			collidedYet = true;
		}
	}

	public void StopGrapple()
	{
		if (hookState == GrapplingState.Attached)
		{
			Vector3 pullDir = transform.position - creator.Carrier.transform.position;
			pullDir.Normalize();
			//Pull the creator
			creator.Carrier.ExternalMove(pullDir, -1, ForceMode.VelocityChange);
		}
		else if (hookState == GrapplingState.Pulling)
		{
			if (pullingEntity != null)
			{
				Vector3 pullDir = creator.Carrier.transform.position - transform.position;
				pullDir.Normalize();
				//Pull the creator
				pullingEntity.ExternalMove(pullDir, -2f, ForceMode.VelocityChange);
			}
		}
	}

	public virtual void Collide()
	{
		//Stop moving and change state.
		
	}
}
