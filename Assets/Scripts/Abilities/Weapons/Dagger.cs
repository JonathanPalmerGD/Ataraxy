using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class Dagger : Weapon 
{
	public static int IconIndex = 29;
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

		List<Vector3> stabPoints = new List<Vector3>();

		stabPoints.Add(stab.transform.position - firePoints[0].transform.position - dir * 2);
		stabPoints.Add(stab.transform.position - firePoints[0].transform.position - dir);
		stabPoints.Add(stab.transform.position - firePoints[0].transform.position);

		SetupSlash(stab, dir, stabPoints);
	}

	public override void UseWeaponSpecial(GameObject target = null, System.Type targType = null, GameObject[] firePoints = null, Vector3 hitPoint = default(Vector3), bool lockOn = false)
	{
		Vector3 firePoint = firePoints[0].transform.position;

		Vector3 dir = hitPoint - firePoint;

		float lungeVel = 45;
		Vector3 movementDir = dir;
		movementDir += Vector3.up * 15 / lungeVel;
		MoveCarrier(dir, lungeVel, true, true);
		
		
		/*Vector3 firePoint = firePoints[0].transform.position;

		Vector3 dir = hitPoint - firePoint;
		dir.Normalize();

		GameObject go = (GameObject)GameObject.Instantiate(bladeSlashPrefab, firePoint, Quaternion.identity);
		BladeSlash slash = go.GetComponent<BladeSlash>();

		List<Vector3> slashPoints = new List<Vector3>();
		slashPoints.Add(-1 * (slash.transform.position - firePoints[2].transform.position));
		slashPoints.Add(slash.transform.position - firePoints[0].transform.position);
		slashPoints.Add(-1 * (slash.transform.position - firePoints[3].transform.position));

		SetupSlash(slash, dir, slashPoints);*/
	}

	public void SetupSlash(DaggerStab stab, Vector3 velocityDirection, List<Vector3> stabPoints)
	{
		stab.lr.material = new Material(Shader.Find("Particles/Additive"));

		stab.lr.SetVertexCount(3);
		stab.lr.SetColors(BeamColor, BeamColor);
		stab.lr.SetWidth(.5f, .0f);

		stab.stabPoints = new System.Collections.Generic.List<Vector3>();
		
		for (int i = 0; i < stabPoints.Count; i++)
		{
			stab.stabPoints.Add(stabPoints[i]);
		}
		stab.Faction = Faction;
		stab.creator = this;

		stab.rigidbody.AddForce(velocityDirection * stab.ProjVel * stab.rigidbody.mass);

		Vector3 target = stab.transform.position + (velocityDirection * 8);

		stab.stabCollider.transform.LookAt(target, Vector3.Cross(stabPoints[0], stabPoints[1]));

		stab.stabCollider.transform.position -= stab.stabCollider.transform.forward * 1f;
		Destroy(stab.gameObject, 10f);
	}

	
	void Update() 
	{
		//If we're behind our target, change the color of the Icon to red?
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
		return w;
	}

	static string[] adj = { "Basic", "Bulky", "Hasty", "Deadly", "Steel", "Vampiric", "Anachronic", "Violent", "Nimble", "Strange" };
	static string weaponName = "Dagger";
	public static string GetWeaponName()
	{
		int rndA = Random.Range(0, adj.Length);

		return (adj[rndA] + " " + weaponName);
	}
	#endregion
}
