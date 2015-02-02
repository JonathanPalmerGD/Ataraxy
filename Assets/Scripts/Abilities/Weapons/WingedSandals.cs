using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class WingedSandals : Weapon 
{
	public static int IconIndex = 60;
	Vector3 movementVector;

	public override void Init()
	{
		base.Init();
		//daggerStabPrefab = Resources.Load<GameObject>("DaggerStab");
		Icon = UIManager.Instance.Icons[IconIndex];

		DurCost = 1;
		DurSpecialCost = 6;
		NormalCooldown = .5f;
		SpecialCooldown = 12f;
#if CHEAT
		//NormalCooldown = .30f;
		//SpecialCooldown = .5f;
		Durability = 100;
#else
		
#endif
		BeamColor = Color.white;
	}

	public override void UseWeapon(GameObject target = null, System.Type targType = null, GameObject[] firePoints = null, Vector3 hitPoint = default(Vector3), bool lockOn = false)
	{
		Vector3 firePoint = firePoints[0].transform.position;

		Vector3 dir = hitPoint - firePoint;
		//Debug.DrawLine(firePoint, firePoint + dir, Color.black, 6.0f);

		float lungeVel = 2.2f;
		Vector3 movementDir = dir;
		movementDir = new Vector3(movementDir.x, 0, movementDir.z);
		movementDir.Normalize();

		MoveCarrier(movementDir, lungeVel, Vector3.up, 3.5f, false);
	}

	public override void UseWeaponSpecial(GameObject target = null, System.Type targType = null, GameObject[] firePoints = null, Vector3 hitPoint = default(Vector3), bool lockOn = false)
	{
		Vector3 firePoint = firePoints[0].transform.position;

		Vector3 dir = hitPoint - firePoint;

		Vector3 movementDir = dir;
		movementDir = new Vector3(movementDir.x, 0, movementDir.z);

		MoveCarrier(Vector3.zero, 0, Vector3.up, 14f, false);
	}

	public override Vector3 AdjProjectileColliderPosition(MeleeProjectile proj)
	{
		return proj.projectileCollider.transform.forward * 1f;
	}

	#region Static Functions
	public static WingedSandals New()
	{
		WingedSandals ws = ScriptableObject.CreateInstance<WingedSandals>();
		ws.AbilityName = WingedSandals.GetWeaponName();
		ws.Durability = Random.Range(25, 40);
		ws.NormalCooldown = .5f;
		ws.SpecialCooldown = 12f;
		ws.CdLeft = 0;
		ws.PrimaryDesc = "A small flap of the sandal's tiny wings for flying short distances.";
		ws.SecondaryDesc = "A powerful ascending gust.";
		return ws;
	}

	static string[] adj = { "Basic", "Bulky", "Hasty", "Deadly", "Steel", "Vampiric", "Anachronic", "Violent", "Nimble", "Strange" };
	static string weaponName = "Winged Sandals";
	public static string GetWeaponName()
	{
		int rndA = Random.Range(0, adj.Length);

		return (/*adj[rndA] + " " + */weaponName);
	}
	#endregion
}
