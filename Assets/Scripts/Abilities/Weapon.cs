using UnityEngine;
using System.Collections;

public class Weapon : Ability 
{
	private bool useSpecialCooldown = false;
	public bool UseSpecialCooldown
	{
		get { return useSpecialCooldown; }
		set { useSpecialCooldown = value; }
	}
	private float cooldown;
	public float Cooldown
	{
		get { return cooldown; }
		set { cooldown = value; }
	}
	private float specialCooldown;
	public float SpecialCooldown
	{
		get { return specialCooldown; }
		set { specialCooldown = value; }
	}
	private float cdLeft;
	public float CdLeft
	{
		get { return cdLeft; }
		set { cdLeft = value; }
	}
	private int durability;
	public int Durability
	{
		get { return durability; }
		set { durability = value; }
	}

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

	/// <summary>
	/// Activates the weapon.
	/// </summary>
	/// <param name="durabilityLost">The amount of durability list</param>
	/// <param name="normalCooldown">True if normal cooldown, false for special</param>
	public void UseWeapon(int durabilityLost, bool normalCooldown)
	{
		if (CdLeft <= 0)
		{
			if (Durability >= durabilityLost)
			{
				Durability -= durabilityLost;
				Remainder.text = Durability.ToString();
				UseSpecialCooldown = !normalCooldown;

				if (UseSpecialCooldown)
				{
					CdLeft = SpecialCooldown;
				}
				else
				{
					CdLeft = Cooldown;
				}
			}
		}
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
		//w.Icon = Resources.Load("Atlases/VortexIconAtlas");
		w.durability = Random.Range(10, 60);
		w.Cooldown = 1;
		w.SpecialCooldown = 6;
		w.CdLeft = 0;
		return w;
	}	

	static string[] adj = { "Bulky", "Hasty", "Deadly", "Steel", "Vampiric", "Anachronic", "Violent", "Nimble" };
	static string[] noun = { "Grenade Launcher", "Whip", "Sniper Rifle", "Dagger", "Mace", "Tome" };
	public static string GetWeaponName()
	{
		int rndA = Random.Range(0, adj.Length);
		int rndB = Random.Range(0, noun.Length);

		return (adj[rndA] + " " + noun[rndB]);
	}
	#endregion
}
