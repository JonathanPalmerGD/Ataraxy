using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class Rapier : Weapon 
{
	public static int IconIndex = 54;
	public GameObject daggerStabPrefab;
	Vector3 movementVector;

	public override void Init()
	{
		base.Init();
		daggerStabPrefab = Resources.Load<GameObject>("Projectiles/RapierStab");
		Icon = UIManager.Instance.Icons[IconIndex];


		AbilityName = Rapier.GetWeaponName();
		SetupDurability(10, 60);
		PrimaryDesc = "[Damage]\nA thin but powerful melee stab.";
		SecondaryDesc = "[Damage], [Utility]\nA dashing lunge combined with a powerful stab!";

		PrimaryDamage = 3;
		crosshairIndex = 3;
		SpecialDamage = 9;
		DurSpecialCost = 5;
		NormalCooldown = .80f;
		SpecialCooldown = 6f;
#if CHEAT
		//NormalCooldown = .30f;
		//SpecialCooldown = .5f;
		Durability = 100;
#else

#endif
		BeamColor = new Color(.7f, .45f, .35f);
	}

	public override void UseWeapon(GameObject target = null, System.Type targType = null, GameObject[] firePoints = null, Vector3 targetScanDir = default(Vector3), bool lockOn = false)
	{
		Vector3 firePoint = firePoints[0].transform.position;

		Vector3 dir = targetScanDir - firePoint;
		dir.Normalize();

		GameObject go = (GameObject)GameObject.Instantiate(daggerStabPrefab, firePoint, Quaternion.identity);
		RapierStab stab = go.GetComponent<RapierStab>();

		stab.Init();
		stab.Shooter = Carrier;
		stab.Damage = PrimaryDamage;

		List<Vector3> stabPoints = new List<Vector3>();

		stabPoints.Add(stab.transform.position - firePoints[0].transform.position - dir * 2);
		stabPoints.Add(stab.transform.position - firePoints[0].transform.position);
		stabPoints.Add(stab.transform.position - firePoints[0].transform.position + dir * 2);

		SetupMeleeProjectile(stab, dir, stabPoints, new Vector2(.25f, .0f));
	}

	public override void UseWeaponSpecial(GameObject target = null, System.Type targType = null, GameObject[] firePoints = null, Vector3 targetScanDir = default(Vector3), bool lockOn = false)
	{
		Vector3 firePoint = firePoints[0].transform.position;

		Vector3 dir = targetScanDir - firePoint;

		float lungeVel = 20;
		Vector3 movementDir = dir;
		//The rapier doesn't lose the Y component so you can lunge upwards.
		//movementDir = new Vector3(movementDir.x, 0, movementDir.z);
		movementDir.Normalize();
		//Debug.Log(dir + "\n" + movementDir + "\n");
		MoveCarrier(movementDir, lungeVel, Vector3.up, 3, true);


		dir.Normalize();

		GameObject go = (GameObject)GameObject.Instantiate(daggerStabPrefab, firePoint, Quaternion.identity);
		RapierStab stab = go.GetComponent<RapierStab>();

		stab.Init();
		stab.Shooter = Carrier;

		List<Vector3> stabPoints = new List<Vector3>();

		stab.ProjVel = stab.ProjVel * 4;
		stab.rigidbody.drag -= 2;
		stab.Damage = SpecialDamage;

		stabPoints.Add(stab.transform.position - firePoints[0].transform.position - dir * 2);
		stabPoints.Add(stab.transform.position - firePoints[0].transform.position);
		stabPoints.Add(stab.transform.position - firePoints[0].transform.position + dir * 2);

		SetupMeleeProjectile(stab, dir, stabPoints, new Vector2(.25f, .0f));
	}

	public override Vector3 AdjProjectileColliderPosition(MeleeProjectile proj)
	{
		return proj.projectileCollider.transform.forward * 0f;
	}

	#region Static Functions
	public new static Rapier New()
	{
		Rapier w = ScriptableObject.CreateInstance<Rapier>();

		w.AbilityName = Rapier.GetWeaponName();
		w.SetupDurability(10, 60);
		w.PrimaryDesc = "[Damage]\nA thin but powerful melee stab.";
		w.SecondaryDesc = "[Damage], [Utility]\nA dashing lunge combined with a powerful stab!";
		return w;
	}

	static string weaponName = "Rapier";
	public new static string GetWeaponName()
	{
		return (weaponName);
	}
	#endregion
}
