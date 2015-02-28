using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class GravityStaff : Weapon 
{
	public static int IconIndex = 13;
	Vector3 movementVector;

	public override void Init()
	{
		base.Init();
		Icon = UIManager.Instance.Icons[IconIndex];

		DurCost = 6;
		crosshairIndex = 1;
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

		//float float = 45;
		Vector3 movementDir = dir;
		movementDir = new Vector3(movementDir.x, 0, movementDir.z);

		//Debug.Log(dir + "\n" + movementDir + "\n");
		//MoveCarrier(movementDir, 0, Vector3.up, 1.5f, true);
		MoveCarrier(movementDir, .2f, Vector3.up, 0.45f, true);
	}

	#region Static Functions
	public new static GravityStaff New()
	{
		GravityStaff gs = ScriptableObject.CreateInstance<GravityStaff>();
		gs.AbilityName = GravityStaff.GetWeaponName();
		gs.Durability = Random.Range(140, 200);
		gs.NormalCooldown = 1;
		gs.SpecialCooldown = .08f;
		gs.CdLeft = 0;
		gs.PrimaryDesc = "[Damage]\nA weak laser as a last resort.";
		gs.SecondaryDesc = "[Utility]\nHold: Dampen gravity effects on you.\nUseful for long or high jumps.";
		return gs;
	}

	static string weaponName = "Gravity Staff";
	public new static string GetWeaponName()
	{
		return (weaponName);
	}
	#endregion
}
