using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class MonkStaff : Weapon
{
	public static int IconIndex = 52;
	public GameObject bladeSlashPrefab;
	Vector3 movementVector;

	public override void Init()
	{
		base.Init();
		bladeSlashPrefab = Resources.Load<GameObject>("StaffThwack");
		Icon = UIManager.Instance.Icons[IconIndex];

		DurSpecialCost = 1;
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
		StaffThwack slash = go.GetComponent<StaffThwack>();
		slash.Init();

		//Slash Edge Extend direction
		Vector3 LeftVector = Vector3.Cross(dir, Vector3.up);

		List<Vector3> slashPoints = new List<Vector3>();

		Vector3 firstPoint = -1 * (slash.transform.position - firePoints[2].transform.position - 2 * LeftVector);
		Vector3 secondPoint = slash.transform.position - firePoints[0].transform.position - (dir * .75f);
		Vector3 thirdPoint = -1 * (slash.transform.position - firePoints[3].transform.position + 2 * LeftVector);

		Debug.Log(firstPoint + "\t\t" + secondPoint + "\t\n" + thirdPoint);

		Quaternion rot = Quaternion.Euler(Random.Range(0, Mathf.PI * 2) + dir.x, 0, Random.Range(0, Mathf.PI * 2) + dir.z);

		//Vector3 quatRot = Quaternion.Euler(Random.Range(0, 360) + dir.x, 0, Random.Range(0, 360) + dir.z) * firstPoint;
		//Vector3 quatRot2 = Quaternion.Euler(Random.Range(0, 360) + dir.x, 0, Random.Range(0, 360) + dir.z) * thirdPoint;
		Vector3 quatRot = Quaternion.Euler(90 + dir.x, 0, 90 + dir.z) * firstPoint;
		Vector3 quatRot2 = Quaternion.Euler(90 + dir.x, 0, 90 + dir.z) * thirdPoint;

		Vector3 diff = quatRot - secondPoint;
		Vector3 opp = secondPoint - diff;
		//Debug.Log("Dir: " + dir + "\nNew: " + quatRot);

		Vector3 dirRelToPivot = firstPoint - secondPoint;
		dirRelToPivot = rot * dirRelToPivot;

		slashPoints.Add(quatRot);
		slashPoints.Add(secondPoint);
		slashPoints.Add(opp);

		//Debug.Log(quatRot + "\t\t" + opp + "\n");

		SetupMeleeProjectile(slash, dir, slashPoints, new Vector2(.5f, .5f));
		//float lungeVel = 20;
		//Vector3 movementDir = Vector3.Cross(dir, Vector3.up);
		//MoveCarrier(movementDir, lungeVel, Vector3.up, 3, true);
	}

	public virtual void AngleCollider(MeleeProjectile proj, Vector3 target, Vector3 axis)
	{
		proj.projectileCollider.transform.LookAt(target, axis);
	}

	public override void UseWeaponSpecial(GameObject target = null, System.Type targType = null, GameObject[] firePoints = null, Vector3 hitPoint = default(Vector3), bool lockOn = false)
	{
		Vector3 firePoint = firePoints[0].transform.position;

		Vector3 dir = hitPoint - firePoint;
		dir.Normalize();

		/*
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
		*/

		float lungeVel = 45;
		Vector3 movementDir = dir;
		movementDir = new Vector3(movementDir.x, 0, movementDir.z);
		movementDir.Normalize();
		//Debug.Log(dir + "\n" + movementDir + "\n");
		MoveCarrier(movementDir, lungeVel, Vector3.up, 15, false);
	}

	public override Vector3 AdjProjectileColliderPosition(MeleeProjectile proj)
	{
		return proj.projectileCollider.transform.forward * 1f;
	}

	#region Static Functions
	public static MonkStaff New()
	{
		MonkStaff w = ScriptableObject.CreateInstance<MonkStaff>();
		w.AbilityName = MonkStaff.GetWeaponName();
		w.Durability = Random.Range(10, 60);
		w.NormalCooldown = 1;
		w.SpecialCooldown = 6;
		w.CdLeft = 0;
		return w;
	}

	static string[] adj = { "Basic", "Bulky", "Hasty", "Deadly", "Steel", "Vampiric", "Anachronic", "Violent", "Nimble", "Strange" };
	static string weaponName = "Monk's Staff";
	public static string GetWeaponName()
	{
		int rndA = Random.Range(0, adj.Length);

		return (adj[rndA] + " " + weaponName);
	}
	#endregion
}
