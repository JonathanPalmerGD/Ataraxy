using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class Projectile : MonoBehaviour 
{
	protected bool fizzled = false;

	private bool awardXPOnHit = false;
	public bool AwardXPOnHit
	{
		get { return awardXPOnHit; }
		set { awardXPOnHit = value; }
	}
	private float xpBountyOnHit = 5;
	public float XpBountyOnHit
	{
		get { return xpBountyOnHit; }
		set { xpBountyOnHit = value; }
	}

	private Entity shooter;
	public Entity Shooter
	{
		get { return shooter; }
		set { shooter = value; }
	}

	private float damage = 1;
	public float Damage
	{
		get { return damage; }
		set { damage = value; }
	}
	private float projVel = 1500;
	public float ProjVel
	{
		get { return projVel; }
		set { projVel = value; }
	}
	private float projLife = 8;
	public float ProjLife
	{
		get { return projLife; }
		set { projLife = value; }
	}
	public Allegiance Faction;
	/*public Allegiance Faction
	{
		get { return faction; }
		set { faction = value; }
	}*/

	private Weapon creator;
	public Weapon Creator
	{
		get { return creator; }
		set { creator = value; }
	}

	public List<AbilityEffect> projectileEffects;

	public virtual void Init()
	{
		//Debug.Log("Hit");
		////transform.SetParent(GameManager.Instance.gameObject.transform);
	}

	public virtual void Start() 
	{

	}

	public virtual void Update() 
	{
		
	}

	void OnTriggerEnter(Collider collider)
	{
		if (enabled)
		{
			//Debug.Log(name + " - collided with " + collider.name + "\n");
			if (collider.tag == "ProjectileFizzler")
			{
				if(collider.name == "NullShield")
				{
					if (Faction != collider.gameObject.GetComponent<NullShield>().Faction)
					{	
						Fizzle();
					}
				}
			}
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
						//Debug.Log(name + " - Inside Projectile Damage Handling\n");
						//Debug.Log("F" + Faction + " " + atObj.Faction + "\n");
						//If the projectile is from the Player
						if (Faction == Allegiance.Player && atObj.Faction == Allegiance.Enemy)
						{
							ProjectileHitTarget(atObj);
							
							//Deal damage to the enemy
							//atObj.GetComponent<Enemy>().AdjustHealth(-Damage * (1 + Shooter.Level * .1f));

							//Apply AbilityEffect to the target.
							//for (int i = 0; i < projectileEffects.Count; i++)
							//{
							//	atObj.GetComponent<Enemy>().ApplyAbilityEffect(projectileEffects[i]);
							//}
						}
						//Else if it is from an Enemy
						else if (Faction == Allegiance.Enemy && atObj.Faction == Allegiance.Player)
						{
							ProjectileHitTarget(GameManager.Instance.player);
							
							//Deal damage to the enemy
							//GameManager.Instance.player.AdjustHealth(-Damage);

							//Apply AbilityEffect to the target.
							//for (int i = 0; i < projectileEffects.Count; i++)
							//{
							//	GameManager.Instance.player.ApplyAbilityEffect(projectileEffects[i]);
							//}

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
			if (!fizzled)
			{
				Collide();
			}
		}
	}

	public virtual void ProjectileHitTarget(Entity target)
	{
		//Debug.Log("Projectile Hit Target\n");

		float damageAmp = 1.0f;
		float lifeStealPer = 0.0f;
		if (Shooter != null)
		{
			damageAmp = Shooter.DamageAmplification;
			lifeStealPer = Shooter.LifeStealPer;

			Shooter.AdjustHealth(damage * damageAmp * lifeStealPer);
		}
		if (Creator != null)
		{
			damageAmp = Creator.Carrier.DamageAmplification;
			lifeStealPer = Creator.Carrier.LifeStealPer;

			Creator.Carrier.AdjustHealth(damage * damageAmp * lifeStealPer);
		}

		target.AdjustHealth(-Damage * damageAmp);
		
		//Is carrier an NPC
		/*if ((typeof(Creator.Carrier)).IsSubclassOf(typeof(NPC)) || Creator.Carrier == typeof(NPC))
		{

		}*/

		//Apply AbilityEffect to the target.
		for (int i = 0; i < projectileEffects.Count; i++)
		{
			target.ApplyAbilityEffect(projectileEffects[i]);
		}
	}

	public virtual void Fizzle()
	{
		fizzled = true;
		GameObject.Destroy(gameObject, .02f);
	}

	public virtual void Collide()
	{
		GameObject.Destroy(gameObject);
	}
}
