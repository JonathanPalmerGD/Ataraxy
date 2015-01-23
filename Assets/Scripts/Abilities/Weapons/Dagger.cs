using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class Dagger : Weapon 
{
	public static int IconIndex = 44;
	public GameObject daggerStabPrefab;
	Vector3 movementVector;

	public override void Init()
	{
		base.Init();
		daggerStabPrefab = Resources.Load<GameObject>("DaggerStab");
		Icon = UIManager.Instance.Icons[IconIndex];

		DurSpecialCost = 5; 
		NormalCooldown = .30f;
		SpecialCooldown = 2f;
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
		dir.Normalize();

		GameObject go = (GameObject)GameObject.Instantiate(daggerStabPrefab, firePoint, Quaternion.identity);
		DaggerStab stab = go.GetComponent<DaggerStab>();

		stab.Init();

		List<Vector3> stabPoints = new List<Vector3>();

		stabPoints.Add(stab.transform.position - firePoints[0].transform.position - dir * 2);
		stabPoints.Add(stab.transform.position - firePoints[0].transform.position - dir);
		stabPoints.Add(stab.transform.position - firePoints[0].transform.position);

		SetupMeleeProjectile(stab, dir, stabPoints, new Vector2(.5f, .0f));
	}

	public override void UseWeaponSpecial(GameObject target = null, System.Type targType = null, GameObject[] firePoints = null, Vector3 hitPoint = default(Vector3), bool lockOn = false)
	{
		Vector3 firePoint = firePoints[0].transform.position;

		Vector3 dir = hitPoint - firePoint;

		float lungeVel = 45;
		Vector3 movementDir = dir;
		movementDir = new Vector3(movementDir.x, 0, movementDir.z);
		movementDir.Normalize();
		//Debug.Log(dir + "\n" + movementDir + "\n");
		MoveCarrier(movementDir, lungeVel, Vector3.up, 3, true);
	}

	public override Vector3 AdjProjectileColliderPosition(MeleeProjectile proj)
	{
		return proj.projectileCollider.transform.forward * 1f;
	}

	#region Static Functions
	public static Dagger New()
	{
		Dagger w = ScriptableObject.CreateInstance<Dagger>();
		w.AbilityName = Dagger.GetWeaponName();
		w.Durability = Random.Range(10, 60);
		w.NormalCooldown = 1;
		w.SpecialCooldown = 6;
		w.CdLeft = 0;
		w.PrimaryDesc = "A quick stab.\nBackstabs coming soon!";
		w.SecondaryDesc = "A quick dash forward.\nUseful for getaways or crossing length gaps.";
		return w;
	}

	static string[] adj = { "Basic", "Bulky", "Hasty", "Deadly", "Steel", "Vampiric", "Anachronic", "Violent", "Nimble", "Strange" };
	static string weaponName = "Dagger";
	public static string GetWeaponName()
	{
		int rndA = Random.Range(0, adj.Length);

		return (/*adj[rndA] + " " + */weaponName);
	}
	#endregion
}
