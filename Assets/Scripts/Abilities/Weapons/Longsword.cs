using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class Longsword : Weapon
{
	public static int IconIndex = 39;
	public GameObject bladeSlashPrefab;
	Vector3 movementVector;

	public override void Init()
	{
		base.Init();
		bladeSlashPrefab = Resources.Load<GameObject>("Projectiles/BladeSlash");
		Icon = UIManager.Instance.Icons[IconIndex];

		crosshairIndex = 7;
		DurSpecialCost = 1;
		NormalCooldown = Random.Range(.45f, .55f);
		SpecialCooldown = NormalCooldown;
#if CHEAT
		NormalCooldown = .5f;
		SpecialCooldown = .5f;
		Durability = 100;
#else
		
#endif
		BeamColor = Color.white;
	}

	public override void UseWeapon(GameObject target = null, System.Type targType = null, GameObject[] firePoints = null, Vector3 targetScanDir = default(Vector3), bool lockOn = false)
	{
		Vector3 firePoint = firePoints[0].transform.position;

		Vector3 dir = targetScanDir - firePoint;
		//Debug.DrawLine(Carrier.transform.position + Vector3.up * 10, Carrier.transform.position + Vector3.up * 10 + dir, Color.magenta, 5.0f);
		dir.Normalize();

		GameObject go = (GameObject)GameObject.Instantiate(bladeSlashPrefab, firePoint, Quaternion.identity);
		BladeSlash slash = go.GetComponent<BladeSlash>();
		slash.Init();
		slash.Shooter = Carrier;

		//Slash Edge Extend direction
		Vector3 LeftVector = Vector3.Cross(dir, Vector3.up);

		List<Vector3> slashPoints = new List<Vector3>();
		slashPoints.Add(-1 * (slash.transform.position - firePoints[2].transform.position - 2 * LeftVector));
		slashPoints.Add(slash.transform.position - firePoints[0].transform.position);
		slashPoints.Add(-1 * (slash.transform.position - firePoints[3].transform.position + 2 * LeftVector));

		SetupMeleeProjectile(slash, dir, slashPoints, new Vector2(.2f, .6f));
		//float lungeVel = 20;
		//Vector3 movementDir = Vector3.Cross(dir, Vector3.up);
		//MoveCarrier(movementDir, lungeVel, Vector3.up, 3, true);
	}

	public override void UseWeaponSpecial(GameObject target = null, System.Type targType = null, GameObject[] firePoints = null, Vector3 targetScanDir = default(Vector3), bool lockOn = false)
	{
		Vector3 firePoint = firePoints[0].transform.position;

		Vector3 dir = targetScanDir - firePoint;
		dir.Normalize();

		GameObject go = (GameObject)GameObject.Instantiate(bladeSlashPrefab, firePoint, Quaternion.identity);
		BladeSlash slash = go.GetComponent<BladeSlash>();
		slash.Init();
		slash.Shooter = Carrier;

		//Slash Edge Extend direction
		Vector3 LeftVector = Vector3.Cross(dir, Vector3.up);

		List<Vector3> slashPoints = new List<Vector3>();
		slashPoints.Add(-1 * (slash.transform.position - firePoints[1].transform.position + 2 * LeftVector));
		slashPoints.Add(slash.transform.position - firePoints[0].transform.position);
		slashPoints.Add(-1 * (slash.transform.position - firePoints[4].transform.position - 2 * LeftVector));

		SetupMeleeProjectile(slash, dir, slashPoints, new Vector2( .2f, .6f));

		//float lungeVel = 20;
		//Vector3 movementDir = -Vector3.Cross(dir, Vector3.up);
		//MoveCarrier(movementDir, lungeVel, Vector3.up, 3, true);
	}

	#region Static Functions
	public new static Longsword New()
	{
		Longsword w = ScriptableObject.CreateInstance<Longsword>();
		w.AbilityName = Longsword.GetWeaponName();
		w.Durability = Random.Range(10, 60);
		w.NormalCooldown = 1;
		w.SpecialCooldown = 6;
		w.CdLeft = 0;
		w.PrimaryDesc = "[Damage]\nA left-to-right slash.";
		w.SecondaryDesc = "[Damage]\nA right-to-left slash.\nStrangely similar to the primary fire...";
		return w;
	}

	static string weaponName = "Longsword";
	public new static string GetWeaponName()
	{
		return (weaponName);
	}
	#endregion
}
