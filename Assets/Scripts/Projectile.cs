using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class Projectile : MonoBehaviour 
{
	private bool awardXPOnHit = false;
	private float xpBountyOnHit = 5;

	private Entity shooter;
	public Entity Shooter
	{
		get { return shooter; }
		set { shooter = value; }
	}

	private int damage = -1;
	public int Damage
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
	private Allegiance faction;
	public Allegiance Faction
	{
		get { return faction; }
		set { faction = value; }
	}

	public List<AbilityEffect> projectileEffects;

	void Start() 
	{
	}
	
	void Update() 
	{
		
	}

	void OnTriggerEnter(Collider collider)
	{
		//Debug.Log("Projectile collided with: " + collider.gameObject.name + "\n" + collider.gameObject.tag);
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
					//If the projectile is from the Player
					if (Faction == Allegiance.Player && atObj.Faction == Allegiance.Enemy)
					{
					
						//Deal damage to the enemy
						atObj.GetComponent<Enemy>().AdjustHealth(damage);

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
						GameManager.Instance.player.AdjustHealth(damage);

						//Apply AbilityEffect to the target.
						for (int i = 0; i < projectileEffects.Count; i++)
						{
							GameManager.Instance.player.ApplyAbilityEffect(projectileEffects[i]);
						}

						//Award experience to the enemy who fired it.
						if (awardXPOnHit && shooter != null)
						{
							shooter.GainExperience(xpBountyOnHit);
						}
					}
				}
			}
		}

		//Clean up the bullet. This should be updated to add it to an object pool
		GameObject.Destroy(gameObject);
	}
}
