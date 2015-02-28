﻿using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class BoundingStaff : Weapon 
{
	public static int IconIndex = 37;
	Vector3 movementVector;

	public override void Init()
	{
		base.Init();
		Icon = UIManager.Instance.Icons[IconIndex];

		DurCost = 4;
		crosshairIndex = 4;
		DurSpecialCost = 1;
		NormalCooldown = .9f;
		SpecialCooldown = .08f;
#if CHEAT
		//NormalCooldown = .30f;
		//SpecialCooldown = .5f;
		Durability = 100;
#else
		
#endif
		BeamColor = Color.white;
	}

	public override void UseWeaponSpecial(GameObject target = null, System.Type targType = null, GameObject[] firePoints = null, Vector3 targetScanDir = default(Vector3), bool lockOn = false)
	{
		Vector3 firePoint = firePoints[0].transform.position;

		Vector3 dir = targetScanDir - firePoint;
		//Debug.DrawLine(GameManager.Instance.playerGO.transform.position, GameManager.Instance.playerGO.transform.position + dir * 30, Color.green, .8f);

		Vector3 movementDir = dir;
		movementDir = new Vector3(movementDir.x, 0, movementDir.z);
		movementDir.Normalize();

		//Carrier.gameObject.GetComponent<Controller>().speedMultiplier = 3f;

		//MoveCarrier(movementDir, 1.8f, Vector3.up, 0.05f, true);
		Carrier.ExternalMove(movementDir, 100.5f, ForceMode.Acceleration);
	}

	#region Static Functions
	public new static BoundingStaff New()
	{
		BoundingStaff bs = ScriptableObject.CreateInstance<BoundingStaff>();
		bs.AbilityName = BoundingStaff.GetWeaponName();
		bs.Durability = Random.Range(80, 99);
		bs.NormalCooldown = 1;
		bs.SpecialCooldown = .08f;
		bs.CdLeft = 0;
		bs.PrimaryDesc = "[Damage]\nA weak laser as a last resort.";
		bs.SecondaryDesc = "[Utility]\nHold: Invoke the God of Travel to boost your forward speed.\nUseful for long.";
		return bs;
	}

	static string[] adj = { "Basic", "Bulky", "Hasty", "Deadly", "Steel", "Vampiric", "Anachronic", "Violent", "Nimble", "Strange" };
	static string weaponName = "Bounding Staff";
	public new static string GetWeaponName()
	{
		//int rndA = Random.Range(0, adj.Length);

		return (/*adj[rndA] + " " + */weaponName);
	}
	#endregion
}
