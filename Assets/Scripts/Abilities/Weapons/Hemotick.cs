﻿using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class Hemotick : Weapon 
{
	public static int IconIndex = 55;
	public GameObject hazePrefab;
	public float drainTimer = 15f;
	public float drainAmount = .5f;
	public float hazeVelocity = 10;
	public float hazeDur = 1.6f;
	public int tickLevel = 0;
	public Vector3 firePointOffset = Vector3.up;
	
	public override void Init()
	{
		base.Init();
		hazePrefab = Resources.Load<GameObject>("Projectiles/BloodHaze");
		Icon = UIManager.Instance.Icons[IconIndex];
		
		crosshairIndex = 6;
		crosshairColor = new Color(.66f, 0, 0);

		crosshairSize = new Vector2(128,128);
		primaryFirePointIndex = 0;
		specialFirePointIndex = 0;
		PrimaryDamage = 0.20f;
		NormalCooldown = Random.Range(.10f, .15f);
		SpecialCooldown = 3;
		DurSpecialCost = 0;
		hazeDur = .4f;
		Durability = Random.Range(120, 160);
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
		base.UpdateWeapon(time);
	}
	
	public override bool CheckAbility()
	{
		return base.CheckAbility();
	}

	public override void UseWeapon(GameObject target = null, System.Type targType = null, GameObject[] firePoints = null, Vector3 targetScanDir = default(Vector3), bool lockOn = false)
	{
		Vector3 firePoint = firePoints[primaryFirePointIndex].transform.position - (Vector3.up / 2);
		
		GameObject go = (GameObject)GameObject.Instantiate(hazePrefab, firePoint + firePointOffset, Quaternion.identity);
		BloodHaze bh = go.GetComponent<BloodHaze>();
		bh.Shooter = Carrier;
		
		Vector3 dir = targetScanDir - firePoint;
		float dev = 15;
		dir = Quaternion.Euler(Random.Range(-dev, dev), Random.Range(-dev, dev), Random.Range(-dev, dev)) * dir;
		dir.Normalize();
		
		go.transform.LookAt(targetScanDir);
		
		bh.Creator = this;
		bh.rigidbody.AddForce((dir * hazeVelocity * (bh.ProjVel / 20) * bh.rigidbody.mass));
		bh.ProjLife = hazeDur;
		
		bh.Damage = PrimaryDamage;
		bh.deathTimer = bh.ProjLife;
		
		bh.Faction = Faction;
	}

	public override void UseWeaponSpecial(GameObject target = null, System.Type targType = null, GameObject[] firePoints = null, Vector3 targetScanDir = default(Vector3), bool lockOn = false)
	{
		float upperHealthCost = Mathf.Max(tickLevel * 2, Carrier.MaxHealth / 4);
		float lowerHealthCost = Mathf.Min(tickLevel * 2, Carrier.MaxHealth / 4);

		#if UNITY_EDITOR
		lowerHealthCost = Carrier.Health / 10;
		upperHealthCost = Carrier.Health / 8;
		#endif

		#if !UNITY_EDITOR
		if(tickLevel == modifier.Length - 1)
		{
			Carrier.AdjustHealth(-1000);
		}
		else
		{
		#endif
			//Deal random damage to the player.
			Carrier.AdjustHealth(Random.Range(-lowerHealthCost, -upperHealthCost));

			tickLevel++;
			Durability += Mathf.Min(tickLevel * 5, 50);

			PrimaryDamage += 0.03f;
			AbilityName = Hemotick.modifier[(tickLevel >= Hemotick.modifier.Length) ? Hemotick.modifier.Length - 1 : tickLevel] + " " + Hemotick.GetWeaponName();
			HandleVisuals();
		#if !UNITY_EDITOR
		}
		#endif

	}

	#region Static Functions
	public new static Hemotick New()
	{
		Hemotick ht = ScriptableObject.CreateInstance<Hemotick>();
		ht.tickLevel = 0;
		ht.AbilityName = Hemotick.modifier[ht.tickLevel] + " " + Hemotick.GetWeaponName();
		ht.Durability = Random.Range(200, 300);
		ht.PrimaryDesc = "[Damage]\nThe Hemotick expels a damaging blood haze.";
		ht.SecondaryDesc = "[Warning]\nFeed the Hemotick a portion of your health to empower it.\nBe careful, if hungry enough, it might kill you.";
		return ht;
	}

	static string[] modifier = {"Unblooded", "Hungry", "Famished", "Starving", "Voracious", "Craving", "Ravenous", "Stuffed", "Plump", "Gorged", "Monstrous", "World Consuming", "Player Consuming"};
	static string weaponName = "Hemotick";
	public new static string GetWeaponName()
	{
		return (weaponName);
	}
	#endregion
}