using UnityEngine;
using System.Collections;

public class Weapon : Ability
{
	#region Weapon's Faction
	private Allegiance faction;
	public virtual Allegiance Faction
	{
		get { return Allegiance.Neutral; }
	}
	#endregion
	#region Cooldown
	private float cdLeft;
	public float CdLeft
	{
		get { return cdLeft; }
		set { cdLeft = value; }
	}
	private bool useSpecialCooldown = false;
	public bool UseSpecialCooldown
	{
		get { return useSpecialCooldown; }
		set { useSpecialCooldown = value; }
	}
	private float normalCooldown;
	public float NormalCooldown
	{
		get { return normalCooldown; }
		set { normalCooldown = value; }
	}
	private float specialCooldown;
	public float SpecialCooldown
	{
		get { return specialCooldown; }
		set { specialCooldown = value; }
	}
	#endregion
	#region Durability & Use Costs
	private int durability;
	public int Durability
	{
		get { return durability; }
		set { durability = value; }
	}
	private int durCost;
	public int DurCost
	{
		get { return durCost; }
		set { durCost = value; }
	}
	private int durSpecialCost;
	public int DurSpecialCost
	{
		get { return durSpecialCost; }
		set { durSpecialCost = value; }
	}
	#endregion

	#region Weapon Attributes
	private float primaryDamage = 2;
	public float PrimaryDamage
	{
		get { return primaryDamage; }
		set { primaryDamage = value; }
	}
	
	#endregion

	public bool hitscan = true;

	public void UpdateWeapon(float time)
	{
		if (CdLeft > time)
		{
			CdLeft -= time;
		}
		else
		{
			CdLeft = 0;
		}
	}

	public virtual void UseWeapon(GameObject target = null, System.Type targType = null, bool lockOn = false)
	{
		if (targType == typeof(Enemy))
		{
			//Debug.Log("Used Weapon on Enemy\n");
			Enemy e = target.GetComponent<Enemy>();

			//Check Faction
			if (e.Faction != Faction)
			{
				//Display visual effect

				//Damage the enemy
				e.AdjustHealth(-PrimaryDamage);
			}
		}
		else if (targType == typeof(NPC))
		{
			//Debug.Log("Used Weapon on NPC\n");

		}
		//If our targType is null from targetting a piece of terrain or something?
		else
		{
			//Debug.Log("Weapon hitscan'd something else\n");
			//Do something like play a 'bullet hitting metal wall' audio.
		}
	}

	public virtual void UseWeaponSpecial(GameObject target = null, bool lockOn = false)
	{

	}

	public virtual bool HandleDurability(bool specialAttack = false, GameObject target = null, bool lockOn = false)
	{
		//If we're off cooldown
		if (CdLeft <= 0)
		{
			//If this is a special attack, use the special cost, otherwise normal cost.
			int curCost = specialAttack ? DurSpecialCost : DurCost;

			if (Durability >= curCost)
			{
				//Reduce Durability, update our text
				Durability -= curCost;
				Remainder.text = Durability.ToString();

				//Mark whether we should use the special cooldown or not.
				UseSpecialCooldown = specialAttack;
				if (UseSpecialCooldown)
				{
					CdLeft = SpecialCooldown;
				}
				else
				{
					CdLeft = NormalCooldown;
				}

				//Say that the weapon successfully fired.
				return true;
			}
		}
		return false;
	}

	public override bool CheckAbility()
	{
		if (durability <= 0)
		{
			return true;
		}
		return false;
	}

	public override string GetInfo()
	{
		return AbilityName + " : " + durability + " uses left";
	}

	#region Static Functions
	public static Weapon New()
	{
		Weapon w = ScriptableObject.CreateInstance<Weapon>();
		w.AbilityName = Weapon.GetWeaponName();
		//w.Icon = Resources.Load("Atlases/AtaraxyIconAtlas");
		w.durability = Random.Range(10, 60);
		w.NormalCooldown = 1;
		w.SpecialCooldown = 6;
		w.CdLeft = 0;
		return w;
	}	

	static string[] adj = { "Basic", "Bulky", "Hasty", "Deadly", "Steel", "Vampiric", "Anachronic", "Violent", "Nimble" };
	static string[] noun = { "Pistol", "Grenade Launcher", "Whip", "Sniper Rifle", "Dagger", "Mace", "Tome" };
	public static string GetWeaponName()
	{
		int rndA = Random.Range(0, adj.Length);
		int rndB = Random.Range(0, noun.Length);

		return (adj[rndA] + " " + noun[rndB]);
	}
	#endregion
}
