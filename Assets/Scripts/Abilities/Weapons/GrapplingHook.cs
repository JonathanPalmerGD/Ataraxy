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
		PrimaryDamage = 5;
		NormalCooldown = Random.Range(.65f, 1.4f);
		SpecialCooldown = Random.Range(1.3f, 2);
		DurSpecialCost = 0;
		#if CHEAT
		NormalCooldown = .7f;
		Durability = 100;
		#else
		#endif
		BeamColor = new Color(.65f, .65f, .65f);
	}

	public override void UpdateWeapon(float time)
	{
		//If we're ready, make our UI white.
		if(weaponState == GrapplingHookWeaponState.Ready)
		{
			IconUI.color = new Color(1, 1, 1);
		}
		//If we're not ready, make our UI grey.
		if(weaponState == GrapplingHookWeaponState.Busy)
		{
			IconUI.color = new Color(.7f, .3f, .3f);
		}
		base.UpdateWeapon(time);
	}

	public override bool CheckAbility()
	{
		if (weaponState == GrapplingHookWeaponState.Busy)
		{
			return false;
		}
		else
		{
			return base.CheckAbility();
		}
	}

	public override void UseWeapon(GameObject target = null, System.Type targType = null, GameObject[] firePoints = null, Vector3 hitPoint = default(Vector3), bool lockOn = false)
	{
		Vector3 firePoint = firePoints[primaryFirePointIndex].transform.position;

		GameObject go = (GameObject)GameObject.Instantiate(hookPrefab, firePoint + Vector3.up, Quaternion.identity);
		currentProjectile = go.GetComponent<GrapplingHookProj>();

		Vector3 dir = hitPoint - firePoint;
		dir.Normalize();

		currentProjectile.creator = this;
		currentProjectile.rigidbody.AddForce((dir * currentProjectile.ProjVel * currentProjectile.rigidbody.mass));

		currentProjectile.Damage = PrimaryDamage;

		currentProjectile.Faction = Faction;

		currentProjectile.timeRemaining = 5;

		weaponState = GrapplingHookWeaponState.Busy;
	}

	public override void UseWeaponSpecial(GameObject target = null, System.Type targType = null, GameObject[] firePoints = null, Vector3 hitPoint = default(Vector3), bool lockOn = false)
	{
		//Destroy our projectile.
		RemoveProjectile(0f);
	}

	public override bool HandleDurability(bool specialAttack = false, GameObject target = null, bool lockOn = false)
	{
		if (specialAttack)
		{
			if (weaponState == GrapplingHookWeaponState.Ready)
			{
				return false;
			}
			return base.HandleDurability(specialAttack, target, lockOn);
		}
		
		//If we aren't ready to fire, we will return false.
		if (weaponState == GrapplingHookWeaponState.Busy)
		{
			return false;
		}
		//Otherwise we'll check the normal conditions.
		return base.HandleDurability(specialAttack, target, lockOn);

	}

	public void RemoveProjectile(float destroyTime = .5f)
	{
		//If we're busy
		if (weaponState == GrapplingHookWeaponState.Busy)
		{
			Carrier.rigidbody.useGravity = true;
			//Set us to ready.
			weaponState = GrapplingHookWeaponState.Ready;
			//Clean up our weapon
			Destroy(currentProjectile.gameObject, destroyTime);
		}
	}
	
	#region Static Functions
	public static GrapplingHook New()
	{
		GrapplingHook g = ScriptableObject.CreateInstance<GrapplingHook>();
		g.AbilityName = GrapplingHook.GetWeaponName();
		g.Durability = Random.Range(6, 17);
		g.NormalCooldown = 1;
		g.SpecialCooldown = 0;
		g.CdLeft = 0;
		g.PrimaryDesc = "Fires a hook and chain that latches onto the first piece of terrain or enemy it hits.\nEnemies are pulled towards you.\nYou are pulled towards terrain at a ridiculous speed.\nExercise Caution.";
		g.SecondaryDesc = "Retracts the current hook instantaneously.\nOnly one hook can be active at a time.";
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
