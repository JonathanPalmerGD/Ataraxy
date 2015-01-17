using UnityEngine;
using System.Collections;

public class ShockRifle : Weapon 
{
	public static int IconIndex = 13;
	public GameObject shockBallPrefab;

	public override void Init()
	{
		base.Init();
		shockBallPrefab = Resources.Load<GameObject>("ShockBall");
		Icon = UIManager.Instance.Icons[IconIndex];

		primaryFirePointIndex = 1;
		specialFirePointIndex = 1;

		PrimaryDamage = 1;
		SpecialDamage = 4;
		DurSpecialCost = 2;
		SpecialCooldown = Random.Range(3, 4);
#if CHEAT
		NormalCooldown = .3f;
		SpecialCooldown = 1f;
		Durability = 100;
#else
		SpecialCooldown = Random.Range(4, 5);
#endif
		BeamColor = new Color(.75f, .14f, .77f);
	}

	public override void UseWeapon(GameObject target = null, System.Type targType = null, GameObject[] firePoints = null, Vector3 hitPoint = default(Vector3), bool lockOn = false)
	{
		Color[] lrColors = new Color[2];
		lrColors[0] = BeamColor;
		lrColors[1] = BeamColor;
		Vector2 lineSize = new Vector2(.1f, .1f);
		SetupLineRenderer(lrColors, lineSize, .3f, firePoints, hitPoint);

		if (target != null)
		{
			ShockBall sb = target.GetComponent<ShockBall>();
			if (sb != null)
			{
				//Detonate the shock ball.
				sb.Shocked();
			}
		}
		if (targType != null)
		{
			if (targType.IsSubclassOf(typeof(Enemy)) || targType.IsAssignableFrom(typeof(Enemy)))
			{
				{
					//Debug.Log("Used Weapon on Enemy\n");
					Enemy e = target.GetComponent<Enemy>();

					//Check Faction
					if (e.Faction != Faction)
					{
						//Display visual effect

						//Damage the enemy
						e.AdjustHealth(-PrimaryDamage);
					}
				}
			}
			else if (targType != null && targType.IsAssignableFrom(typeof(NPC)))
			{
			}
		}
		//If our targType is null from targetting a piece of terrain or something?
		else
		{
			//Debug.Log("Weapon hitscan'd something else\n");
			//Do something like play a 'bullet hitting metal wall' audio.
		}
	}

	public override void UseWeaponSpecial(GameObject target = null, System.Type targType = null,  GameObject[] firePoints = null, Vector3 hitPoint = default(Vector3), bool lockOn = false)
	{
		Vector3 firePoint = firePoints[specialFirePointIndex].transform.position;

		GameObject go = (GameObject)GameObject.Instantiate(shockBallPrefab, firePoint, Quaternion.identity);
		ShockBall shock = go.GetComponent<ShockBall>();

		shock.Faction = Faction;
		Vector3 dir = hitPoint - firePoint;
		dir.Normalize();

		shock.rigidbody.AddForce(dir * shock.ProjVel * shock.rigidbody.mass);

		Destroy(shock, 20);
	}

	#region Static Functions
	public static ShockRifle New()
	{
		ShockRifle w = ScriptableObject.CreateInstance<ShockRifle>();
		w.AbilityName = ShockRifle.GetWeaponName();
		w.Durability = Random.Range(10, 60);
		w.NormalCooldown = 1;
		w.SpecialCooldown = 6;
		w.CdLeft = 0;
		return w;
	}

	static string[] adj = { "Basic", "Bulky", "Hasty", "Deadly", "Steel", "Vampiric", "Anachronic", "Violent", "Nimble", "Strange" };
	static string weaponName = "Shock Rifle";
	public static string GetWeaponName()
	{
		int rndA = Random.Range(0, adj.Length);

		return (adj[rndA] + " " + weaponName);
	}
	#endregion
}
