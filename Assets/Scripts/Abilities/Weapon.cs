using UnityEngine;
using System.Collections;

public class Weapon : Ability
{
	#region Weapon's Faction & Bearer
	public Allegiance Faction;
	private GameObject weaponBearer;
	public GameObject WeaponBearer
	{
		get { return weaponBearer; }
		set { weaponBearer = value; }
	}
	private Color beamColor;
	public Color BeamColor
	{
		get { return beamColor; }
		set { beamColor = value; }
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
	private int durCost = 1;
	public int DurCost
	{
		get { return durCost; }
		set { durCost = value; }
	}
	private int durSpecialCost = 5;
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
	private float specialDamage = 8;
	public float SpecialDamage
	{
		get { return specialDamage; }
		set { specialDamage = value; }
	}

	#endregion

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

	public override void Init()
	{
		base.Init();
		NormalCooldown = Random.Range(.01f, .7f);
		SpecialCooldown = Random.Range(4, 16);
		Icon = UIManager.Instance.Icons[Random.Range(1, UIManager.Instance.Icons.Length)];
		BeamColor = new Color(Random.Range(0.0f, 1.0f), Random.Range(0.0f, 1.0f), Random.Range(0.0f, 1.0f));
	}

	public virtual void UseWeapon(GameObject target = null, System.Type targType = null, Vector3 firePoint = default(Vector3), Vector3 hitPoint = default(Vector3), bool lockOn = false)
	{
		if (hitPoint != default(Vector3) && firePoint != default(Vector3))
		{
			LineRenderer lr = WeaponBearer.GetComponent<LineRenderer>();
			if (lr == null)
			{
				lr = WeaponBearer.AddComponent<LineRenderer>();
			}

			lr.material = new Material(Shader.Find("Particles/Additive"));

			lr.SetVertexCount(2);
			lr.SetColors(BeamColor, Color.grey);
			lr.SetWidth(.1f, .1f);
			lr.SetPosition(0, firePoint);
			lr.SetPosition(1, hitPoint);
			Destroy(lr, .3f);
		}


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

	public virtual void UseWeaponSpecial(GameObject target = null, System.Type targType = null, Vector3 firePoint = default(Vector3), Vector3 hitPoint = default(Vector3), bool lockOn = false)
	{
		if (hitPoint != default(Vector3) && firePoint != default(Vector3))
		{
			LineRenderer lr = WeaponBearer.GetComponent<LineRenderer>();
			if (lr == null)
			{
				lr = WeaponBearer.AddComponent<LineRenderer>();
			}

			lr.material = new Material(Shader.Find("Particles/Additive"));

			lr.SetVertexCount(2);
			lr.SetColors(Color.red, Color.red);
			lr.SetWidth(.2f, .2f);
			lr.SetPosition(0, firePoint);
			lr.SetPosition(1, hitPoint);
			Destroy(lr, 1f);
		}


		if (targType == typeof(Enemy))
		{
			//Debug.Log("Used Weapon on Enemy\n");
			Enemy e = target.GetComponent<Enemy>();

			//Check Faction
			if (e.Faction != Faction)
			{
				//Display visual effect



				//Damage the enemy
				e.AdjustHealth(-SpecialDamage);
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
		w.Durability = Random.Range(10, 60);
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
