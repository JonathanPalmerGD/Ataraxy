using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class GrapplingHookProj : Projectile 
{
	//Grappling state - Moving, Attached, Pulling
	public enum GrapplingState { Firing, Attached, Pulling };
	private GrapplingState hookState = GrapplingState.Firing;
	private Entity pullingEntity = null;

	public virtual void Init()
	{
		//Debug.Log("Hit");
		////transform.SetParent(GameManager.Instance.gameObject.transform);
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
			Vector3 pullDir = creator.Carrier.transform.position - transform.position;
			//Pull the creator
			creator.Carrier.ExternalMove(pullDir, 50, ForceMode.Force);
		}
		else if (hookState == GrapplingState.Pulling)
		{
			//Return to the creator.
			Vector3 pullDir = creator.Carrier.transform.position - transform.position;
			
			//Move in the direction of the creator
			rigidbody.velocity = pullDir * 3;

			//Pull whatever we're attached to if able.
			if (pullingEntity != null)
			{
				pullingEntity.ExternalMove(pullDir, 50, ForceMode.Force);
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
				Entity atObj = collider.gameObject.GetComponent<Entity>();
				if (atObj != null)
				{
					if (atObj.Faction == Faction)
					{
						Debug.LogWarning("Projectile collided with same Faction as firing source.\n");
						return;
					}
					else
					{
						//Debug.Log("F" + Faction + " " + atObj.Faction + "\n");
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

			//Clean up the bullet. This should be updated to add it to an object pool
			Collide();
		}
	}

	public virtual void Collide()
	{
		//Stop moving and change state.
		
	}
}
