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

		DurSpecialCost = 1;
		SpecialCooldown = NormalCooldown;
#if CHEAT
		NormalCooldown = .5f;
		SpecialCooldown = .5f;
		Durability = 100;
#else
		
#endif
		BeamColor = Color.white;
	}

	public override void UseWeapon(GameObject target = null, System.Type targType = null, GameObject[] firePoints = null, Vector3 hitPoint = default(Vector3), bool lockOn = false)
	{
		Vector3 firePoint = firePoints[0].transform.position;

		Vector3 dir = hitPoint - firePoint;
		dir.Normalize();

		GameObject go = (GameObject)GameObject.Instantiate(bladeSlashPrefab, firePoint, Quaternion.identity);
		BladeSlash slash = go.GetComponent<BladeSlash>();
		slash.Init();

		//Slash Edge Extend direction
		Vector3 LeftVector = Vector3.Cross(dir, Vector3.up);

		List<Vector3> slashPoints = new List<Vector3>();
		slashPoints.Add(-1 * (slash.transform.position - firePoints[2].transform.position - 2 * LeftVector));
		slashPoints.Add(slash.transform.position - firePoints[0].transform.position);
		slashPoints.Add(-1 * (slash.transform.position - firePoints[3].transform.position + 2 * LeftVector));

		SetupMeleeProjectile(slash, dir, slashPoints, new Vector2(.2f, .6f));
		float lungeVel = 20;
		Vector3 movementDir = Vector3.Cross(dir, Vector3.up);
		//MoveCarrier(movementDir, lungeVel, Vector3.up, 3, true);
	}

	public override void UseWeaponSpecial(GameObject target = null, System.Type targType = null, GameObject[] firePoints = null, Vector3 hitPoint = default(Vector3), bool lockOn = false)
	{
		Vector3 firePoint = firePoints[0].transform.position;

		Vector3 dir = hitPoint - firePoint;
		dir.Normalize();

		GameObject go = (GameObject)GameObject.Instantiate(bladeSlashPrefab, firePoint, Quaternion.identity);
		BladeSlash slash = go.GetComponent<BladeSlash>();
		slash.Init();

		//Slash Edge Extend direction
		Vector3 LeftVector = Vector3.Cross(dir, Vector3.up);

		List<Vector3> slashPoints = new List<Vector3>();
		slashPoints.Add(-1 * (slash.transform.position - firePoints[1].transform.position + 2 * LeftVector));
		slashPoints.Add(slash.transform.position - firePoints[0].transform.position);
		slashPoints.Add(-1 * (slash.transform.position - firePoints[4].transform.position - 2 * LeftVector));

		SetupMeleeProjectile(slash, dir, slashPoints, new Vector2( .2f, .6f));

		float lungeVel = 20;
		Vector3 movementDir = -Vector3.Cross(dir, Vector3.up);
		//MoveCarrier(movementDir, lungeVel, Vector3.up, 3, true);
	}

	#region Static Functions
	public static Longsword New()
	{
		Longsword w = ScriptableObject.CreateInstance<Longsword>();
		w.AbilityName = Longsword.GetWeaponName();
		w.Durability = Random.Range(10, 60);
		w.NormalCooldown = 1;
		w.SpecialCooldown = 6;
		w.CdLeft = 0;
		w.PrimaryDesc = "A left-to-right slash.";
		w.SecondaryDesc = "A right-to-left slash.\nStrangely similar to the primary fire...";
		return w;
	}

	static string[] adj = { "Basic", "Bulky", "Hasty", "Deadly", "Steel", "Vampiric", "Anachronic", "Violent", "Nimble", "Strange" };
	static string weaponName = "Longsword";
	public static string GetWeaponName()
	{
		int rndA = Random.Range(0, adj.Length);

		return (/*adj[rndA] + " " + */weaponName);
	}
	#endregion
}
