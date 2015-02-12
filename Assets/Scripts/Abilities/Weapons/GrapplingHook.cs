using UnityEngine;
using System.Collections;

public class GrapplingHook : Weapon 
{
	public static int IconIndex = 23;
	public GameObject hookPrefab;
	public GrapplingHookProj currentProjectile;
	public float pullVelocity;
	public enum GrapplingHookWeaponState { Ready, Busy }
	public GrapplingHookWeaponState weaponState = GrapplingHookWeaponState.Ready;
	
	public override void Init()
	{
		base.Init();
		hookPrefab = Resources.Load<GameObject>("Projectiles/Grappling Hook");
		Icon = UIManager.Instance.Icons[IconIndex];
		primaryFirePointIndex = 0;
		specialFirePointIndex = 0;
		NormalCooldown = Random.Range(.65f, 1.4f);
		#if CHEAT
		NormalCooldown = .7f;
		Durability = 100;
		#else
		SpecialCooldown = Random.Range(4, 5);
		#endif
		BeamColor = new Color(Random.Range(0.0f, 1.0f), Random.Range(0.0f, 1.0f), Random.Range(0.0f, 1.0f));
	}
	
	public override void UseWeapon(GameObject target = null, System.Type targType = null, GameObject[] firePoints = null, Vector3 hitPoint = default(Vector3), bool lockOn = false)
	{
		Vector3 firePoint = firePoints[primaryFirePointIndex].transform.position;

		GameObject go = (GameObject)GameObject.Instantiate(hookPrefab, firePoint, Quaternion.identity);
		currentProjectile = go.GetComponent<GrapplingHookProj>();

		Vector3 dir = hitPoint - firePoint;
		dir.Normalize();

		currentProjectile.creator = this;
		currentProjectile.rigidbody.AddForce((dir * currentProjectile.ProjVel * currentProjectile.rigidbody.mass));

		currentProjectile.Faction = Faction;

		weaponState = GrapplingHookWeaponState.Busy;

		Destroy(go, 15);
	}

	public void RemoveProjectile()
	{
		//If we're busy
		if (weaponState == GrapplingHookWeaponState.Busy)
		{
			Carrier.rigidbody.useGravity = true;
			//Clean up our weapon
			Destroy(currentProjectile.gameObject, .5f);
			//Set us to ready.
			weaponState = GrapplingHookWeaponState.Ready;
		}
	}
	
	#region Static Functions
	public static GrapplingHook New()
	{
		GrapplingHook g = ScriptableObject.CreateInstance<GrapplingHook>();
		g.AbilityName = GrapplingHook.GetWeaponName();
		g.Durability = Random.Range(10, 60);
		g.NormalCooldown = 1;
		g.SpecialCooldown = 6;
		g.CdLeft = 0;
		g.PrimaryDesc = "This is a grappling hook.";
		g.SecondaryDesc = "Not yet implemented!";
		return g;
	}
	
	static string[] adj = { "Basic", "Bulky", "Hasty", "Deadly", "Steel", "Vampiric", "Anachronic", "Violent", "Nimble", "Strange" };
	static string weaponName = "Grappling Hook";
	public static string GetWeaponName()
	{
		int rndA = Random.Range(0, adj.Length);
		
		return (/*adj[rndA] + " " +*/ weaponName);
	}
	#endregion
}
