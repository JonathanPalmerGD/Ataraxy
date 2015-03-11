using UnityEngine;
using System.Collections;

public class ShockRifle : Weapon 
{
	public static int IconIndex = 3;
	public GameObject shockBallPrefab;

	public override void Init()
	{
		base.Init();
		shockBallPrefab = Resources.Load<GameObject>("Projectiles/ShockBall");
		Icon = UIManager.Instance.Icons[IconIndex];

		primaryFirePointIndex = 1;
		specialFirePointIndex = 1;
		crosshairColor = new Color(.7f, .6f, .95f);

		crosshairIndex = 7;
		PrimaryDamage = .4f;
		SpecialDamage = 1;
		DurSpecialCost = 2;
		NormalCooldown = .25f;
		SpecialCooldown = Random.Range(.45f, .65f);
#if CHEAT
		NormalCooldown = .3f;
		SpecialCooldown = 1f;
		Durability = 100;
#else
		//SpecialCooldown = Random.Range(4, 5);
#endif
		BeamColor = new Color(.75f, .14f, .77f);
	}

	public override void UseWeapon(GameObject target = null, System.Type targType = null, GameObject[] firePoints = null, Vector3 targetScanDir = default(Vector3), bool lockOn = false)
	{
		Color[] lrColors = new Color[2];
		lrColors[0] = BeamColor;
		lrColors[1] = BeamColor;
		Vector2 lineSize = new Vector2(.1f, .1f);
		SetupLineRenderer(lrColors, lineSize, NormalCooldown / 2, firePoints, targetScanDir);

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
			if (targType.IsSubclassOf(typeof(Enemy)) || targType == typeof(Enemy))
			{
				{
					//Debug.Log("Used Weapon on Enemy\n");
					Enemy e = target.GetComponent<Enemy>();

					//Check Faction
					if (e.Faction != Faction)
					{
						//Display visual effect

						//Damage the enemy
						e.AdjustHealth(-PrimaryDamage * (1 + Carrier.Level * .1f));
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

	public override void UseWeaponSpecial(GameObject target = null, System.Type targType = null,  GameObject[] firePoints = null, Vector3 targetScanDir = default(Vector3), bool lockOn = false)
	{
		Vector3 firePoint = firePoints[specialFirePointIndex].transform.position;

		GameObject go = (GameObject)GameObject.Instantiate(shockBallPrefab, firePoint, Quaternion.identity);
		ShockBall shock = go.GetComponent<ShockBall>();
		shock.Shooter = Carrier;
		shock.Faction = Faction;
		Vector3 dir = targetScanDir - firePoint;
		dir.Normalize();

		shock.rigidbody.AddForce(dir * shock.ProjVel * shock.rigidbody.mass);

		Destroy(shock, 20);
	}

	#region Static Functions
	public new static ShockRifle New()
	{
		ShockRifle w = ScriptableObject.CreateInstance<ShockRifle>();
		w.AbilityName = ShockRifle.GetWeaponName();
		w.Durability = Random.Range(80, 95);
		w.NormalCooldown = .3f;
		w.SpecialCooldown = 4;
		w.CdLeft = 0;
		w.PrimaryDesc = "[Damage] [Combo]\nA fast firing shocking laser.\nWarning: Laser discharge explodes static orbs.";
		w.SecondaryDesc = "[Damage], [Combo], [Explosive]\nAn growing orb of static.";
		return w;
	}

	static string weaponName = "Shock Rifle";
	public new static string GetWeaponName()
	{
		return (weaponName);
	}
	#endregion
}
