using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class GrapplingHookProj : Projectile 
{
	//Grappling state - Moving, Attached (to environment), Pulling (enemy to you)
	public enum GrapplingState { Firing, Attached, Pulling };
	private GrapplingState hookState = GrapplingState.Firing;
	private Entity pullingEntity = null;

	public virtual void Init()
	{

	}

	public virtual void Start() 
	{
		//ColliderName = "StabCollider";
		base.Init();

		Damage = 1;
		//ProjVel = 1200;
		//movementDecay = 0f;
		//visualDecay = .35f;
		//rigidbody.drag = 8;
	}

	public virtual void Update() 
	{
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
				pullingEntity.ExternalMove(pullDir, 15f, ForceMode.VelocityChange);
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
					if (collidedEntity.Faction == Faction)
					{
						Debug.LogWarning("Projectile collided with same Faction as firing source.\n");
						return;
					}
					else
					{
						//Debug.Log("F" + Faction + " " + atObj.Faction + "\n");
						//If the projectile is from the Player
						if (Faction == Allegiance.Player && collidedEntity.Faction == Allegiance.Enemy)
						{
							//Deal damage to the enemy
							collidedEntity.GetComponent<Enemy>().AdjustHealth(-Damage);

							//Make ourself a child.
							//gameObject.transform.SetParent(collidedEntity.transform);

							rigidbody.velocity = Vector3.zero;

							hookState = GrapplingState.Pulling;
							pullingEntity = collidedEntity;

							//Apply AbilityEffect to the target.
							for (int i = 0; i < projectileEffects.Count; i++)
							{
								collidedEntity.GetComponent<Enemy>().ApplyAbilityEffect(projectileEffects[i]);
							}
						}
						//Else if it is from an Enemy
						else if (Faction == Allegiance.Enemy && collidedEntity.Faction == Allegiance.Player)
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

			else if(cTag == "Projectile")
			{
				//Destroy ourself
				enabled = false;
			}
			else
			{
				rigidbody.velocity = Vector3.zero;
				hookState = GrapplingState.Attached;
			}


			//Clean up the bullet. This should be updated to add it to an object pool
			Collide();
		}
	}

	public virtual void Collide()
	{
		//Stop moving and change state.
		
	}
}
