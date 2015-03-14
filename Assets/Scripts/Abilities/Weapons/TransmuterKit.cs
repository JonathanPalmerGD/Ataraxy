using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class TransmuterKit : Weapon 
{
	public static int IconIndex = 21;
	public GameObject hazePrefab;
	public Vector3 firePointOffset = Vector3.up;
	
	public override void Init()
	{
		base.Init();
		hazePrefab = Resources.Load<GameObject>("Projectiles/BloodHaze");
		Icon = UIManager.Instance.Icons[IconIndex];
		
		crosshairIndex = 0;
		crosshairColor = new Color(.3f, 1f, .3f);

		crosshairSize = new Vector2(128,128);
		primaryFirePointIndex = 0;
		specialFirePointIndex = 0;
		PrimaryDamage = 0.35f;
		NormalCooldown = Random.Range(.15f, .25f);
		SpecialCooldown = 15;
		DurSpecialCost = 20;
		Durability = Random.Range(55, 95);
		
		#if UNITY_EDITOR
		SpecialCooldown = .5f;
		#endif

		#if CHEAT
		Durability = 100;
		#else
		#endif
		BeamColor = new Color(.65f, .65f, .65f);
	}

	public override void UpdateWeapon(float time)
	{
		if (IconUI != null)
		{
			IconUI.color = new Color(.3f, 1, .3f, IconUI.color.a);
		}
		base.UpdateWeapon(time);
	}
	
	public override bool CheckAbility()
	{
		return base.CheckAbility();
	}

	public override void UseWeapon(GameObject target = null, System.Type targType = null, GameObject[] firePoints = null, Vector3 targetScanDir = default(Vector3), bool lockOn = false)
	{
		//The amount it heals should be amplified at low health.
		float percentageMissing = 1 - (Carrier.Health / Carrier.MaxHealth);
		float healingAmplification = (.5f + percentageMissing) * (.5f + percentageMissing) * (.5f + percentageMissing);

		Carrier.AdjustHealth(PrimaryDamage * healingAmplification + PrimaryDamage / 5);
		
		//Apply any item effects to the carrier - if this is a poisonous kit, you get poisoned using it!
		//Carrier.ApplyAbilityEffect()
	}

	public override void UseWeaponSpecial(GameObject target = null, System.Type targType = null, GameObject[] firePoints = null, Vector3 targetScanDir = default(Vector3), bool lockOn = false)
	{
		//The core goal for this effect is a very powerful, very random effect.
		int selector = 1; //Random.Range(0, 15);
		switch(selector)
		{
			case 0:
				//Effect 0: Bouncy Balls
				Debug.Log("Effect 0:\nThe gift of bouncy balls!");
				break;

			case 1:
				//Effect 1: Restore to full health
				Debug.Log("Effect 1:\nGain max health & full heal");
				Carrier.MaxHealth += 4;
				Carrier.AdjustHealth(Carrier.MaxHealth);
				break;

			case 2:
				//Effect 2: Gain a bunch of experience
				Debug.Log("Effect 2:\nGain wisdom");
				Carrier.GainExperience(Carrier.XPNeeded * 1.4f);
				break;

			case 3:
				//Effect 3: Gain random weapon.
				Debug.Log("Effect 3:\nGain random weapon.");

				break;
			case 4:

				break;
			case 5:

				break;
			case 6:

				break;
			case 7:

				break;
			case 8:

				break;

				break;
			case 10:

				break;
			case 11:

				break;
			case 12:

				break;
			case 13:

				break;
			case 14:

				break;
		}
	}

	public override void UpdateCrosshair(Crosshair crosshair, Vector3 contactPoint = default(Vector3))
	{
		if (contactPoint != default(Vector3) && Vector3.SqrMagnitude(contactPoint - Carrier.transform.position) < 3300)
		{
			crosshair.CrosshairColor = specialCrosshairColor;
		}
		else
		{
			crosshair.CrosshairColor = crosshairColor;
		}

		crosshair.CrosshairIndex = crosshairIndex;
		crosshair.SetCrosshairSize(crosshairSize);
	}

	public override bool HandleDurability(bool specialAttack = false, GameObject target = null, bool lockOn = false)
	{
		if (specialAttack)
		{
			if (Durability >= DurSpecialCost)
			{
				return base.HandleDurability(specialAttack, target, lockOn);
			}
			else
			{
				UseSpecialCooldown = true;
				CdLeft = SpecialCooldown;
				Durability = 0;
				Remainder.text = Durability.ToString();
				return true;
			}
		}
		else
		{
			//Don't let the player use the item at full health.
			if (Carrier.Health / Carrier.MaxHealth == 1)
			{
				return false;
			}

			//Otherwise we'll check the normal conditions.
			return base.HandleDurability(specialAttack, target, lockOn);
		}
	}

	#region Static Functions
	public new static TransmuterKit New()
	{
		TransmuterKit tk = ScriptableObject.CreateInstance<TransmuterKit>();
		tk.AbilityName = TransmuterKit.GetWeaponName();
		tk.Durability = Random.Range(2, 3);
		tk.PrimaryDesc = "[Healing]\nHold: Transmute internal bleeding into NOT internal bleeding!\nMore Effective the more internal bleeding!";
		tk.SecondaryDesc = "[?????]\nThere is no telling what this might do.\nAt very least it'll have a long cooldown and use plenty of ammo.";
		return tk;
	}

	static string weaponName = "Transmuter's Kit";
	public new static string GetWeaponName()
	{
		return (weaponName);
	}
	#endregion
}
