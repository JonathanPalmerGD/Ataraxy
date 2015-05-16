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
	public float assignedHookSpeed = 20;
	public Vector3 firePointOffset = Vector3.up;
	
	public override void Init()
	{
		base.Init();
		hookPrefab = Resources.Load<GameObject>("Projectiles/Grappling Hook");
		Icon = UIManager.Instance.Icons[IconIndex];

		AbilityName = GrapplingHook.GetWeaponName();
		SetupDurability(23, 35);
		PrimaryDesc = "Fires a hook and chain that latches onto the first piece of terrain, token or enemy it hits.\nEnemies & tokens are pulled towards you.\nYou are pulled towards terrain at a ridiculous speed.\nExercise Caution.";
		SecondaryDesc = "[Utility]\nAn arcing grapple throw useful for recovering from a downward plummet.";

		crosshairIndex = 7;
		crosshairColor = new Color(.3f, .3f, .3f);
		specialCrosshairColor = new Color(.3f, 1f, .3f);

		primaryFirePointIndex = 0;
		specialFirePointIndex = 0;
		PrimaryDamage = 0.4f;
		NormalCooldown = Random.Range(.55f, .8f);
		SpecialCooldown = NormalCooldown;
		//SpecialCooldown = Random.Range(.45f, .65f);
		DurSpecialCost = 1;
		primaryAudio = "Grappling_Fire";
		HasAudio = true;
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
			if (IconUI != null)
			{
				IconUI.color = new Color(1, 1, 1, IconUI.color.a);
			}
		}
		//If we're not ready, make our UI grey.
		if(weaponState == GrapplingHookWeaponState.Busy)
		{
			if (IconUI != null)
			{
				IconUI.color = new Color(.7f, .3f, .3f, IconUI.color.a);
			}
		}
		base.UpdateWeapon(time);
	}

	public override void UpdateCrosshair(Crosshair crosshair, Vector3 contactPoint = default(Vector3))
	{
		if (contactPoint != default(Vector3) && Vector3.SqrMagnitude(contactPoint - Carrier.transform.position) < 3300)
		{
			crosshair.CrosshairColor = specialCrosshairColor;
		}
		else
		{
			crosshair.CrosshairColor = crosshairColor;
		}

		crosshair.CrosshairIndex = crosshairIndex;
		crosshair.SetCrosshairSize(crosshairSize);
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

	public override void UseWeapon(GameObject target = null, System.Type targType = null, GameObject[] firePoints = null, Vector3 targetScanDir = default(Vector3), bool lockOn = false)
	{
		if (weaponState == GrapplingHookWeaponState.Ready)
		{
			Vector3 firePoint = firePoints[primaryFirePointIndex].transform.position;

			GameObject go = (GameObject)GameObject.Instantiate(hookPrefab, firePoint + firePointOffset, Quaternion.identity);
			currentProjectile = go.GetComponent<GrapplingHookProj>();
			currentProjectile.Shooter = Carrier;

			Vector3 dir = targetScanDir - firePoint;
			dir.Normalize();

			AudioSource hookAud = AudioManager.Instance.MakeSource(primaryAudio, Carrier.transform.position, Carrier.transform);
			hookAud.minDistance = 5;
			hookAud.Play();
			
			currentProjectile.PlayChainCoroutine();
			
			go.transform.LookAt(targetScanDir);

			currentProjectile.Creator = this;

			currentProjectile.rigidbody.AddForce((dir * assignedHookSpeed * Carrier.ProjSpeedAmp * (currentProjectile.ProjVel / 20) * currentProjectile.rigidbody.mass));

			currentProjectile.Damage = PrimaryDamage;

			currentProjectile.Faction = Faction;

			currentProjectile.timeRemaining = 2f;

			weaponState = GrapplingHookWeaponState.Busy;
		}
		else
		{
			currentProjectile.Retract();
		}
	}

	public override void UseWeaponSpecial(GameObject target = null, System.Type targType = null, GameObject[] firePoints = null, Vector3 targetScanDir = default(Vector3), bool lockOn = false)
	{
		//Destroy our projectile.
		if (weaponState == GrapplingHookWeaponState.Ready)
		{
			Vector3 firePoint = firePoints[primaryFirePointIndex].transform.position;

			GameObject go = (GameObject)GameObject.Instantiate(hookPrefab, firePoint + firePointOffset, Quaternion.identity);
			currentProjectile = go.GetComponent<GrapplingHookProj>();
			currentProjectile.Shooter = Carrier;

			Vector3 dir = targetScanDir - firePoint;
			dir.Normalize();
			dir = new Vector3(dir.x, .8f, dir.z);
			dir.Normalize();
			go.transform.LookAt(dir * 10000);

			currentProjectile.Creator = this;
			currentProjectile.rigidbody.useGravity = true;

			currentProjectile.rigidbody.AddForce((dir * assignedHookSpeed * Carrier.ProjSpeedAmp * (currentProjectile.ProjVel / 20) * currentProjectile.rigidbody.mass));

			currentProjectile.Damage = PrimaryDamage;

			currentProjectile.Faction = Faction;

			currentProjectile.timeRemaining = 3f;

			weaponState = GrapplingHookWeaponState.Busy;
		}
		else
		{
			currentProjectile.Retract();
		}
	}

	public override bool HandleDurability(bool specialAttack = false, GameObject target = null, bool lockOn = false)
	{
		/*if (specialAttack)
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
		}*/
		//Otherwise we'll check the normal conditions.
		return base.HandleDurability(specialAttack, target, lockOn);
	}

	public void RemoveProjectile(float destroyTime = .0f)
	{
		//If we're busy
		if (weaponState == GrapplingHookWeaponState.Busy)
		{
			//Set us to ready.
			weaponState = GrapplingHookWeaponState.Ready;
			//Clean up our weapon
			Destroy(currentProjectile.gameObject, destroyTime);
		}
	}

	public void ProjectileDestroyed()
	{
		if (Carrier != null && Carrier.rigidbody != null)
		{
			Carrier.rigidbody.useGravity = true;
		}
		//Set us to ready.
		weaponState = GrapplingHookWeaponState.Ready;

		currentProjectile = null;
	}
	
	#region Static Functions
	public new static GrapplingHook New()
	{
		GrapplingHook g = ScriptableObject.CreateInstance<GrapplingHook>();
		g.AbilityName = GrapplingHook.GetWeaponName();
		g.SetupDurability(23, 35);
		g.PrimaryDesc = "Fires a hook and chain that latches onto the first piece of terrain, token or enemy it hits.\nEnemies & tokens are pulled towards you.\nYou are pulled towards terrain at a ridiculous speed.\nExercise Caution.";
		g.SecondaryDesc = "[Utility]\nAn arcing grapple throw useful for recovering from a downward plummet.";
		return g;
	}
	
	static string weaponName = "Grappling Hook";
	public new static string GetWeaponName()
	{		
		return (weaponName);
	}
	#endregion
}
