using UnityEngine;
using System.Collections;

public class RocketLauncher : Weapon 
{
	public static int IconIndex = 43;
	public GameObject rocketPrefab;
	public float homingVelocity;
	public float fuelPerRocket;
	public float rocketBlastRadius;

	public override void Init()
	{
		base.Init();
		rocketPrefab = Resources.Load<GameObject>("Projectiles/Rocket");
		Icon = UIManager.Instance.Icons[IconIndex];

		crosshairIndex = 5;
		crosshairColor = Color.red;
		specialCrosshairColor = Color.yellow;
		PrimaryDamage = 4.5f;
		SpecialDamage = 3.5f;
		fuelPerRocket = 5;
		rocketBlastRadius = 2;
		homingVelocity = 30;

		primaryFirePointIndex = 1;
		specialFirePointIndex = 1;
		NormalCooldown = Random.Range(.65f, 1.4f);
#if CHEAT
		NormalCooldown = .7f;
		Durability = 100;
#else
		SpecialCooldown = Random.Range(4, 5);
#endif
		BeamColor = new Color(Random.Range(0.0f, 1.0f), Random.Range(0.0f, 1.0f), Random.Range(0.0f, 1.0f));
	}

	public override void UseWeapon(GameObject target = null, System.Type targType = null, GameObject[] firePoints = null, Vector3 targetScanDir = default(Vector3), bool lockOn = false)
	{
		Vector3 firePoint = firePoints[primaryFirePointIndex].transform.position;
		if (targType != null)
		{
			//Debug.Log(targType.IsSubclassOf(typeof(Enemy)).ToString() + "  " + targType == typeof(Enemy).ToString());
			//Debug.Log((targType == typeof(NPC)).ToString());
				
			if (targType.IsSubclassOf(typeof(Enemy)) || targType == typeof(Enemy))
			{
				Enemy e = target.GetComponent<Enemy>();
				//Check Faction
				if (e.Faction != Faction)
				{
					GameObject go = (GameObject)GameObject.Instantiate(rocketPrefab, firePoint, Quaternion.identity);
					Rocket rocket = go.GetComponent<Rocket>();
					rocket.Shooter = Carrier;

					rocket.Faction = Faction;

					rocket.explosiveDamage = SpecialDamage;
					rocket.blastRadius = rocketBlastRadius;
					rocket.fuelRemaining = fuelPerRocket;
					rocket.homingVelocity = homingVelocity;

					if (lockOn)
					{
						rocket.target = target;
						rocket.homing = true;
					}
					else
					{
						rocket.target = null;
						rocket.homing = false;

						rocket.rigidbody.AddForce((targetScanDir - firePoint) * rocket.ProjVel * rocket.rigidbody.mass);
					}
				}
			}
			else if (targType.IsSubclassOf(typeof(NPC)) || targType == typeof(NPC))
			{
				Debug.Log("Used Weapon on NPC\n");
				FireRocket(firePoint, targetScanDir);

			}
		}
		//If our targType is null from targetting a piece of terrain or something?
		else
		{
			FireRocket(firePoint, targetScanDir);
		}
	}

	public void FireRocket(Vector3 firePoint = default(Vector3), Vector3 targetScanDir = default(Vector3))
	{
		//Debug.Log("Weapon hitscan'd something else\n");
		//Do something like play a 'bullet hitting metal wall' audio.
		GameObject go = (GameObject)GameObject.Instantiate(rocketPrefab, firePoint, Quaternion.identity);
		Rocket rocket = go.GetComponent<Rocket>();

		rocket.Faction = Faction;
		rocket.Shooter = Carrier;

		rocket.rigidbody.drag = 0;
		rocket.target = null;
		rocket.homing = false;

		rocket.explosiveDamage = PrimaryDamage;
		rocket.blastRadius = rocketBlastRadius;
		rocket.fuelRemaining = fuelPerRocket;
		rocket.homingVelocity = homingVelocity;

		//Debug.DrawLine(firePoint, targetScanDir, Color.red, 5.0f);
		Vector3 dir = targetScanDir - firePoint;
		dir.Normalize();

		rocket.rigidbody.AddForce((dir * rocket.ProjVel * rocket.rigidbody.mass));

		Destroy(rocket, fuelPerRocket + 8);
	}

	#region Static Functions
	public new static RocketLauncher New()
	{
		RocketLauncher w = ScriptableObject.CreateInstance<RocketLauncher>();
		w.AbilityName = RocketLauncher.GetWeaponName();
		w.Durability = Random.Range(10, 60);
		w.NormalCooldown = 1;
		w.SpecialCooldown = 6;
		w.CdLeft = 0;
		w.PrimaryDesc = "[Damage], [Explosive]\nAn explosive rocket. Homing if you fire when targetting an enemy.";
		w.SecondaryDesc = "[Damage]\nNot yet implemented!";
		return w;
	}

	static string weaponName = "Rocket Launcher";
	public new static string GetWeaponName()
	{
		return (weaponName);
	}
	#endregion
}
