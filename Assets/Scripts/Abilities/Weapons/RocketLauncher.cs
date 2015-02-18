using UnityEngine;
using System.Collections;

public class RocketLauncher : Weapon 
{
	public static int IconIndex = 43;
	public GameObject rocketPrefab;
	public float homingVelocity;
	public float fuelPerRocket;

	public override void Init()
	{
		base.Init();
		rocketPrefab = Resources.Load<GameObject>("Projectiles/Rocket");
		Icon = UIManager.Instance.Icons[IconIndex];

		crosshairIndex = 5;
		crosshairColor = Color.red;
		specialCrosshairColor = Color.yellow;

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
			if (targType.IsSubclassOf(typeof(Enemy)) || targType.IsAssignableFrom(typeof(Enemy)))
			{
				Enemy e = target.GetComponent<Enemy>();
				//Check Faction
				if (e.Faction != Faction)
				{
					GameObject go = (GameObject)GameObject.Instantiate(rocketPrefab, firePoint, Quaternion.identity);
					Rocket rocket = go.GetComponent<Rocket>();
					rocket.Shooter = Carrier;

					rocket.Faction = Faction;

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
			else if (targType == typeof(NPC))
			{
				//Debug.Log("Used Weapon on NPC\n");

			}
		}
		//If our targType is null from targetting a piece of terrain or something?
		else
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

			//Debug.DrawLine(firePoint, targetScanDir, Color.red, 5.0f);
			Vector3 dir = targetScanDir - firePoint;
			dir.Normalize();

			rocket.rigidbody.AddForce((dir * rocket.ProjVel * rocket.rigidbody.mass));

			Destroy(rocket, fuelPerRocket + 8);
		}
	}

	#region Static Functions
	public static RocketLauncher New()
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

	static string[] adj = { "Basic", "Bulky", "Hasty", "Deadly", "Steel", "Vampiric", "Anachronic", "Violent", "Nimble", "Strange" };
	static string weaponName = "Rocket Launcher";
	public static string GetWeaponName()
	{
		int rndA = Random.Range(0, adj.Length);

		return (/*adj[rndA] + " " +*/ weaponName);
	}
	#endregion
}
