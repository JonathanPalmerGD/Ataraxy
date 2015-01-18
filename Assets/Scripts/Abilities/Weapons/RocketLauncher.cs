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
		rocketPrefab = Resources.Load<GameObject>("Rocket");
		Icon = UIManager.Instance.Icons[IconIndex];
		primaryFirePointIndex = 1;
		specialFirePointIndex = 1;
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
		if (targType != null)
		{
			if (targType.IsSubclassOf(typeof(Enemy)) || targType.IsAssignableFrom(typeof(Enemy)))
			{
				Enemy e = target.GetComponent<Enemy>();
				//Check Faction
				if (e.Faction != Faction)
				{
					GameObject go = (GameObject)GameObject.Instantiate(rocketPrefab, firePoint, Quaternion.identity);
					Homing homing = go.GetComponent<Homing>();

					homing.Faction = Faction;

					if (lockOn)
					{
						homing.target = target;
						homing.homing = true;
					}
					else
					{
						homing.target = null;
						homing.homing = false;

						homing.rigidbody.AddForce((hitPoint - firePoint) * homing.ProjVel * homing.rigidbody.mass);
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
			Homing homing = go.GetComponent<Homing>();

			homing.Faction = Faction;

			homing.rigidbody.drag = 0;
			homing.target = null;
			homing.homing = false;

			//Debug.DrawLine(firePoint, hitPoint, Color.red, 5.0f);
			Vector3 dir = hitPoint - firePoint;
			dir.Normalize();

			homing.rigidbody.AddForce((dir * homing.ProjVel * homing.rigidbody.mass));

			Destroy(homing, fuelPerRocket + 8);
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
		return w;
	}

	static string[] adj = { "Basic", "Bulky", "Hasty", "Deadly", "Steel", "Vampiric", "Anachronic", "Violent", "Nimble", "Strange" };
	static string weaponName = "Rocket Launcher";
	public static string GetWeaponName()
	{
		int rndA = Random.Range(0, adj.Length);

		return (adj[rndA] + " " + weaponName);
	}
	#endregion
}
