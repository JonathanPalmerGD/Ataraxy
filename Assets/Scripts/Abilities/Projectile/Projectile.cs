using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class Projectile : MonoBehaviour 
{
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

	public Weapon creator;

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

		GameObject.Destroy(gameObject);
	}
}
