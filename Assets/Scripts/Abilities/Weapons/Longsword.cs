using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class Longsword : Weapon
{
	public static int IconIndex = 35;
	public GameObject bladeSlashPrefab;
	Vector3 movementVector;

	public override void Init()
	{
		base.Init();
		bladeSlashPrefab = Resources.Load<GameObject>("BladeSlash");
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
		BladeSlash slash = go.GetComponent<BladeSlash>();

		List<Vector3> slashPoints = new List<Vector3>();
		slashPoints.Add(-1*(slash.transform.position - firePoints[2].transform.position));
		slashPoints.Add(slash.transform.position - firePoints[0].transform.position);
		slashPoints.Add(-1*(slash.transform.position - firePoints[3].transform.position));

		SetupSlash(slash, dir, slashPoints);
		float lungeVel = 20;
		Vector3 movementDir = Vector3.Cross(dir, Vector3.up);
		MoveCarrier(movementDir, lungeVel, Vector3.up, 3, true);
	}

	public override void UseWeaponSpecial(GameObject target = null, System.Type targType = null, GameObject[] firePoints = null, Vector3 hitPoint = default(Vector3), bool lockOn = false)
	{
		Vector3 firePoint = firePoints[0].transform.position;

		Vector3 dir = hitPoint - firePoint;
		dir.Normalize();

		GameObject go = (GameObject)GameObject.Instantiate(bladeSlashPrefab, firePoint, Quaternion.identity);
		BladeSlash slash = go.GetComponent<BladeSlash>();

		List<Vector3> slashPoints = new List<Vector3>();
		slashPoints.Add(-1*(slash.transform.position - firePoints[1].transform.position));
		slashPoints.Add(slash.transform.position - firePoints[0].transform.position);
		slashPoints.Add(-1*(slash.transform.position - firePoints[4].transform.position));

		SetupSlash(slash, dir, slashPoints);

		float lungeVel = 20;
		Vector3 movementDir = -Vector3.Cross(dir, Vector3.up);
		MoveCarrier(movementDir, lungeVel, Vector3.up, 3, true);
	}

	public void SetupSlash(BladeSlash slash, Vector3 velocityDirection, List<Vector3> slashPoints)
	{
		slash.lr.material = new Material(Shader.Find("Particles/Additive"));

		slash.lr.SetVertexCount(3);
		slash.lr.SetColors(BeamColor, BeamColor);
		slash.lr.SetWidth(.2f, .6f);

		slash.slashPoints = new System.Collections.Generic.List<Vector3>();

		for (int i = 0; i < slashPoints.Count; i++)
		{
			slash.slashPoints.Add(slashPoints[i]);
		}
		slash.Faction = Faction;
		slash.creator = this;

		slash.rigidbody.AddForce(velocityDirection * slash.ProjVel * slash.rigidbody.mass);

		Vector3 target = slash.transform.position + (velocityDirection * 8);

		slash.slashCollider.transform.LookAt(target, Vector3.Cross(slashPoints[0], slashPoints[2]));

		slash.slashCollider.transform.position -= slash.slashCollider.transform.forward * .4f;
		Destroy(slash.gameObject, 10f);
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
		return w;
	}

	static string[] adj = { "Basic", "Bulky", "Hasty", "Deadly", "Steel", "Vampiric", "Anachronic", "Violent", "Nimble", "Strange" };
	static string weaponName = "Longsword";
	public static string GetWeaponName()
	{
		int rndA = Random.Range(0, adj.Length);

		return (adj[rndA] + " " + weaponName);
	}
	#endregion
}
