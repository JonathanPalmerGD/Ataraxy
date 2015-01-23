using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class GravityStaff : Weapon 
{
	public static int IconIndex = 13;
	//public GameObject daggerStabPrefab;
	Vector3 movementVector;

	public override void Init()
	{
		base.Init();
		//daggerStabPrefab = Resources.Load<GameObject>("DaggerStab");
		Icon = UIManager.Instance.Icons[IconIndex];

		DurCost = 6;
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

	public override void UseWeaponSpecial(GameObject target = null, System.Type targType = null, GameObject[] firePoints = null, Vector3 hitPoint = default(Vector3), bool lockOn = false)
	{
		Vector3 firePoint = firePoints[0].transform.position;

		Vector3 dir = hitPoint - firePoint;

		//float float = 45;
		Vector3 movementDir = dir;
		movementDir = new Vector3(movementDir.x, 0, movementDir.z);
		
		//Debug.Log(dir + "\n" + movementDir + "\n");
		MoveCarrier(movementDir, 0, Vector3.up, 1.5f, true);
	}

	public override Vector3 AdjProjectileColliderPosition(MeleeProjectile proj)
	{
		return proj.projectileCollider.transform.forward * 1f;
	}

	#region Static Functions
	public static GravityStaff New()
	{
		GravityStaff gs = ScriptableObject.CreateInstance<GravityStaff>();
		gs.AbilityName = GravityStaff.GetWeaponName();
		gs.Durability = Random.Range(140, 200);
		gs.NormalCooldown = 1;
		gs.SpecialCooldown = .08f;
		gs.CdLeft = 0;
		gs.PrimaryDesc = "A weak laser as a last resort.";
		gs.SecondaryDesc = "Hold: Dampen gravity effects on you.\nUseful for long or high jumps.";
		return gs;
	}

	static string[] adj = { "Basic", "Bulky", "Hasty", "Deadly", "Steel", "Vampiric", "Anachronic", "Violent", "Nimble", "Strange" };
	static string weaponName = "Gravity Staff";
	public static string GetWeaponName()
	{
		int rndA = Random.Range(0, adj.Length);

		return (/*adj[rndA] + " " + */weaponName);
	}
	#endregion
}
