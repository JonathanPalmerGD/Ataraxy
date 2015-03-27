using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class TransmuterKit : Weapon 
{
	public static int IconIndex = 21;
	public GameObject hazePrefab;
	public GameObject waspPrefab;
	public GameObject beholderPrefab;
	public Vector3 firePointOffset = Vector3.up;
	public float gravityTimer;
	
	public override void Init()
	{
		base.Init();
		hazePrefab = Resources.Load<GameObject>("Projectiles/BloodHaze");
		waspPrefab = Resources.Load<GameObject>("Enemies/Ire Wasps");
		beholderPrefab = Resources.Load<GameObject>("Enemies/Beholder");
		Icon = UIManager.Instance.Icons[IconIndex];

		AbilityName = TransmuterKit.GetWeaponName();
		Durability = Random.Range(2, 3);
		PrimaryDesc = "[Healing]\nHold: Transmute internal bleeding into NOT internal bleeding!\nMore Effective the more internal bleeding!";
		SecondaryDesc = "[?????]\nThere is no telling what this might do.\nAt very least it'll have a long cooldown and use plenty of ammo.";

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
		DurSpecialCost = 0;
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
		
		Vector3 firePoint = firePoints[0].transform.position;

		Modifier newMod;
		Vector3 dir = targetScanDir - firePoint;
		dir.Normalize();

		//The core goal for this effect is a very powerful, very random effect.
		int selector = Random.Range(0, 15);
		Debug.Log(selector + "\n");
		switch(selector)
		{
			case 0:
				//Effect 0: Bouncy Balls
				break;

			case 1:
				//Effect 1: Restore to full health
				Carrier.MaxHealth += 4;
				Carrier.AdjustHealth(Carrier.MaxHealth);
				break;

			case 2:
				//Effect 2: Gain a bunch of experience
				Carrier.GainExperience(Carrier.XPNeeded * 1.4f);
				break;

			case 3:
				//Effect 3: Gain random weapon.
				((Player)Carrier).SetupAbility(LootManager.NewWeapon());
				break;
			case 4:
				IreWasps iWasp = ((GameObject)GameObject.Instantiate(waspPrefab, (Carrier.transform.position + dir * 20 + Vector3.up * 8), Quaternion.identity)).GetComponent<IreWasps>();
				iWasp.belowStage = false;
				
				break;
			case 5:
				Beholder behold = ((GameObject)GameObject.Instantiate(beholderPrefab, (Carrier.transform.position + dir * 20 + Vector3.up * 8), Quaternion.identity)).GetComponent<Beholder>();
				behold.belowStage = false;

				break;
			case 6:
				((Player)Carrier).SetupAbility(LootManager.NewWeapon("GrapplingHook"));
				break;
			case 7:
				Vector3 movementDir = dir;
				movementDir = new Vector3(movementDir.x, 0, movementDir.z);

				MoveCarrier(movementDir, 35, Vector3.up, 45f, false);
				break;
			case 8:
				((Player)Carrier).AdjustActiveDurability(DurSpecialCost * 2 + 1);
				SpecialCooldown = .75f;
				break;
			case 9:
				Carrier.AdjustHealth(-Carrier.Health / 2);
				break;
			case 10:
				Physics.gravity = Constants.gravity;
				Physics.gravity = new Vector3(0, Physics.gravity.y / 2, 0);
				break;
			case 11:
				Physics.gravity = Constants.gravity;
				Physics.gravity = new Vector3(0, Physics.gravity.y - 3, 0);
				break;
			case 12:
				//Try to create a new cluster
				Cluster nearest = TerrainManager.Instance.FindNearestCluster(Carrier.transform.position);

					if (nearest != null)
					{
						TerrainManager.Instance.CreateNewCluster(nearest);
					}
					else
					{
						//Otherwise Refund
						((Player)Carrier).AdjustActiveDurability(DurSpecialCost);
						SpecialCooldown = .75f;
					}
				break;
			case 13:
				behold = ((GameObject)GameObject.Instantiate(beholderPrefab, (Carrier.transform.position + dir * 20 + Vector3.up * 8), Quaternion.identity)).GetComponent<Beholder>();
				behold.belowStage = false;
				newMod = ModifierManager.Instance.GainNewModifier(behold.Level, "Elite");
				newMod.Init();
				behold.GainModifier(newMod);
				newMod = ModifierManager.Instance.GainNewModifier(behold.Level, "Kamikaze");
				newMod.Init();
				behold.GainModifier(newMod);
				behold.UpdateLevelUI();
				break;
			case 14:
				iWasp = ((GameObject)GameObject.Instantiate(waspPrefab, (Carrier.transform.position + dir * 20 + Vector3.up * 8), Quaternion.identity)).GetComponent<IreWasps>();
				iWasp.belowStage = false;
				iWasp = ((GameObject)GameObject.Instantiate(waspPrefab, (Carrier.transform.position + dir * 25 + Vector3.up * 12), Quaternion.identity)).GetComponent<IreWasps>();
				iWasp.belowStage = false;
				iWasp = ((GameObject)GameObject.Instantiate(waspPrefab, (Carrier.transform.position + dir * 30 + Vector3.up * 6), Quaternion.identity)).GetComponent<IreWasps>();
				iWasp.belowStage = false;
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
