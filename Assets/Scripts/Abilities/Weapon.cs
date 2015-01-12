using UnityEngine;
using System.Collections;

public class Weapon : Ability 
{
	private float cooldown;
	public float Cooldown
	{
		get { return cooldown; }
		set { cooldown = value; }
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

	public void UseWeapon(int durabilityLost)
	{
		if (CdLeft <= 0)
		{
			if (Durability >= durabilityLost)
			{
				Durability -= durabilityLost;
				CdLeft = Cooldown;
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
		return Name + " : " + durability + " uses left";
	}

	#region Static Functions
	public static Weapon New()
	{
		Weapon w = ScriptableObject.CreateInstance<Weapon>();
		w.Name = Weapon.GetWeaponName();
		//w.Icon = Resources.Load("Atlases/VortexIconAtlas");
		w.durability = Random.Range(10, 60);
		w.Cooldown = 1;
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
