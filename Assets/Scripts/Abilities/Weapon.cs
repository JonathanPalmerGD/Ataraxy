﻿using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class Weapon : Ability
{
	#region Weapon's Faction & Bearer
	public Allegiance Faction;
	/*private GameObject weaponBearer;
	public GameObject WeaponBearer
	{
		get { return weaponBearer; }
		set { weaponBearer = value; }
	}*/
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
	#region Indexes (Fire Point, Icon)
	public int primaryFirePointIndex = 1;
	public int specialFirePointIndex = 1;
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
	#region Description
	private string primaryDesc = "A weak laser";
	public string PrimaryDesc
	{
		get { return primaryDesc; }
		set { primaryDesc = value; }
	}
	private string secondaryDesc = "A strong laser";
	public string SecondaryDesc
	{
		get { return secondaryDesc; }
		set { secondaryDesc = value; }
	}
	#endregion

	public virtual void UpdateWeapon(float time)
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

	public virtual void UseWeapon(GameObject target = null, System.Type targType = null, GameObject[] firePoints = null, Vector3 hitPoint = default(Vector3), bool lockOn = false)
	{
		Color[] lrColors = new Color[2];
		lrColors[0] = BeamColor;
		lrColors[1] = Color.grey;
		Vector2 lineSize = new Vector2( .1f, .1f );
		SetupLineRenderer(lrColors, lineSize, .3f, firePoints, hitPoint);

		if (targType != null)
		{
			if (targType.IsSubclassOf(typeof(Enemy)) || targType.IsAssignableFrom(typeof(Enemy)))
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
			if (targType.IsSubclassOf(typeof(NPC)) || targType.IsAssignableFrom(typeof(NPC)))
			{
				//Debug.Log("Used Weapon on NPC\n");

			}
		}
		//If our targType is null from targetting a piece of terrain or something?
		else
		{
			//Debug.Log("Weapon hitscan'd something else\n");
			//Do something like play a 'bullet hitting metal wall' audio.
		}
	}

	public virtual void UseWeaponSpecial(GameObject target = null, System.Type targType = null, GameObject[] firePoints = null, Vector3 hitPoint = default(Vector3), bool lockOn = false)
	{
		Color[] lrColors = new Color[2];
		lrColors[0] = Color.red;
		lrColors[1] = Color.red;
		Vector2 lineSize = new Vector2(.2f, .2f);
		SetupLineRenderer(lrColors, lineSize, 1f, firePoints, hitPoint);

		if (targType != null)
		{
			if (targType.IsSubclassOf(typeof(Enemy)) || targType.IsAssignableFrom(typeof(Enemy)))
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
			else if (targType != null && targType.IsAssignableFrom(typeof(NPC)))
			{
				//Debug.Log("Used Weapon on NPC\n");

			}
		}
		//If our targType is null from targetting a piece of terrain or something?
		else
		{
			//Debug.Log("Weapon hitscan'd something else\n");
			//Do something like play a 'bullet hitting metal wall' audio.
		}
	}

	public virtual void SetupLineRenderer(Color[] colors, Vector2 lineSize, float time, GameObject[] firePoints = null, Vector3 hitPoint = default(Vector3))
	{
		if (hitPoint != default(Vector3) && firePoints.Length > 0)
		{
			LineRenderer lr = Carrier.gameObject.GetComponent<LineRenderer>();
			if (lr == null)
			{
				lr = Carrier.gameObject.AddComponent<LineRenderer>();
			}

			lr.material = new Material(Shader.Find("Particles/Additive"));

			lr.SetVertexCount(2);
			lr.SetColors(colors[0], colors[1]);
			lr.SetWidth(lineSize.x, lineSize.y);
			lr.SetPosition(0, firePoints[0].transform.position);
			lr.SetPosition(1, hitPoint);
			Destroy(lr, time);
		}
	}

	/// <summary>
	/// Used by weapon children to move the carrier in some direction
	/// </summary>
	/// <param name="movementDir">The normalized direction of movement</param>
	/// <param name="movementVel">The velocity to move at</param>
	/// <param name="secondDir">A second direction for movement, such as a slight upward boost</param>
	/// <param name="secondVel">The velocity of the second direction for movement.</param>
	/// <param name="additiveMovement">Whether to add the velocity to current velocity, or replace it entirely.</param>
	public virtual void MoveCarrier(Vector3 movementDir, float movementVel, Vector3 secondDir, float secondVel, bool additiveMovement = false, bool dampenVertical = true)
	{
		movementDir.Normalize();

		if (Carrier.gameObject.tag == "Player")
		{
			//Get player's character controller?
			//CharacterMotor charMotor = WeaponBearer.GetComponent<CharacterMotor>();

			if (additiveMovement)
			{
				//Vector3 updatedVelocity = charMotor.movement.velocity;
				Vector3 updatedVelocity = Carrier.gameObject.rigidbody.velocity;
				Vector3 gainedVelocity = (movementDir * movementVel) + (secondDir * secondVel);
				updatedVelocity += gainedVelocity;// new Vector3(gainedVelocity.x, gainedVelocity.y / 2, gainedVelocity.z);
				Carrier.gameObject.rigidbody.velocity = updatedVelocity;
			}
			else
			{
				Vector3 updatedVelocity = Vector3.zero;
				Vector3 gainedVelocity = (movementDir * movementVel) + (secondDir * secondVel);
				updatedVelocity += gainedVelocity;
				Carrier.gameObject.rigidbody.velocity = updatedVelocity;
			}
		}
		else
		{
			//Move the enemy?
		}
		#region [OLD] CharacterMotor Player
		/*if (WeaponBearer.tag == "Player")
		{
			//Get player's character controller?
			CharacterMotor charMotor = WeaponBearer.GetComponent<CharacterMotor>();


			if (additiveMovement)
			{
				Vector3 updatedVelocity = charMotor.movement.velocity;
				Debug.Log(updatedVelocity + "\t" + 
					movementDir + " * " + movementVel + "=" + (movementDir * movementVel) + "\n" + 
					secondDir + "*" + secondVel+  "=" + (secondDir * secondVel) + "\t\t\t" + 
					((movementDir * movementVel) + (secondDir * secondVel)));
				updatedVelocity += (movementDir * movementVel) + (secondDir * secondVel);
				charMotor.SetVelocity(updatedVelocity);
			}
			else
			{
				
				Vector3 updatedVelocity = (movementDir * movementVel) + (secondDir * secondVel);
				Debug.Log(updatedVelocity + "\t" +
					movementDir + " * " + movementVel + "=" + (movementDir * movementVel) + "\n" +
					secondDir + "*" + secondVel + "=" + (secondDir * secondVel) + "\t\t\t" +
					((movementDir * movementVel) + (secondDir * secondVel)));
				charMotor.SetVelocity(updatedVelocity);
			}
		}
		else
		{
			//Move the enemy?
		}*/
		#endregion
	}

	/// <summary>
	/// Initializes the Melee Projectile within the world space.
	/// </summary>
	/// <param name="proj">Reference to the already instantiated melee projectile.</param>
	/// <param name="velocityDirection">The direction that the projectile will move with it's Projectile.ProjVel</param>
	/// <param name="linePoints">The points that the LineRenderer should display.</param>
	/// <param name="lineWidth">The thickness of the line (x = start, y = end)</param>
	/// <param name="specialVelocity">For when a projectile needs a separate velocity for special attacks.</param>
	public virtual void SetupMeleeProjectile(MeleeProjectile proj, Vector3 velocityDirection, List<Vector3> linePoints, Vector2 lineWidth, float specialVelocity = 0)
	{
		proj.lr.material = new Material(Shader.Find("Particles/Additive"));

		//This could be changed to take linePoints.Count.
		proj.lr.SetVertexCount(3);
		//Beam Color is a characteristic of a weapon, as the weapon doesn't usually change color.
		proj.lrColor = BeamColor;
		proj.lr.SetWidth(lineWidth.x, lineWidth.y);

		proj.lrPoints = new List<Vector3>();
		for (int i = 0; i < linePoints.Count; i++)
		{
			proj.lrPoints.Add(linePoints[i]);
		}

		//Friendly fire stub for determining who should and shouldn't be affected.
		proj.Faction = Faction;
		//My Weapon's are Scriptable Objects. Some projectiles might influence their parent weapon by refunding health or ammo.
		proj.creator = this;
		
		//Give the projectile velocity. Melee projectiles generally have drag to slow them down quickly.
		if (specialVelocity != 0)
		{
			proj.rigidbody.AddForce(velocityDirection * specialVelocity * proj.rigidbody.mass);
		}
		else
		{
			proj.rigidbody.AddForce(velocityDirection * proj.ProjVel * proj.rigidbody.mass);
		}

		//Target is ahead of the projectile (the 8 hard coded value is just enough to always be in front of the projectile
		Vector3 target = proj.transform.position + (velocityDirection * 8);

		//Orients the collider to align with the two end points while facing the target.
		AngleCollider(proj, target, Vector3.Cross(linePoints[0], linePoints[2]));

		//Repositions the collider. AdjProjColliderPos is overloaded differently by different weapons.
		proj.projectileCollider.transform.position -= AdjProjectileColliderPosition(proj);

		//Fallback setup destruction. Most projectiles handle their own removal. This would ideally be changed to a pooling projectile pattern.
		Destroy(proj.gameObject, 10f);
	}

	/// <summary>
	/// Tilts the collider to orient with the upward axis, generally through the use of cross product on the end points.
	/// </summary>
	/// <param name="target">The direction the projectile will face</param>
	/// <param name="axis">The direction that will be considered as 'Up'</param>
	public virtual void AngleCollider(MeleeProjectile proj, Vector3 target, Vector3 axis)
	{
		proj.projectileCollider.transform.LookAt(target, axis);
	}

	/// <summary>
	/// Moves the collider forward or backward slightly to align with an individual weapon.
	/// Overloaded by each child as necessary. Should be replaced as a characteristic of the weapon.
	/// </summary>
	/// <param name="proj"></param>
	/// <returns></returns>
	public virtual Vector3 AdjProjectileColliderPosition(MeleeProjectile proj)
	{
		return proj.projectileCollider.transform.forward * .4f;
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
		w.Durability = Random.Range(30, 60);
		w.NormalCooldown = 1;
		w.SpecialCooldown = 6;
		w.CdLeft = 0;
		w.PrimaryDesc = "[Damage]\nDefault Hitscan Laser";
		w.SecondaryDesc = "[Damage]\nA deadly laser.";
		return w;
	}	

	static string[] adj = { "Basic", "Bulky", "Hasty", "Deadly", "Steel", "Vampiric", "Anachronic", "Violent", "Nimble" };
	static string[] noun = { "Default Weapon" };
	public static string GetWeaponName()
	{
		int rndA = Random.Range(0, adj.Length);
		int rndB = Random.Range(0, noun.Length);

		return (adj[rndA] + " " + noun[rndB]);
	}
	#endregion
}
