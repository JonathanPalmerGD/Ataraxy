using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class GlacialSling : Weapon 
{
	public static int IconIndex = 27;
	public GameObject glacialDiskPrefab;
	public List<GlacialDisk> gDisks;
	public float shatterRadius;

	public override void Init()
	{
		base.Init();
		gDisks = new List<GlacialDisk>();
		glacialDiskPrefab = Resources.Load<GameObject>("Projectiles/GlacialDisk");
		Icon = UIManager.Instance.Icons[IconIndex];

		primaryFirePointIndex = 1;
		specialFirePointIndex = 1;
		crosshairColor = new Color(.6f, .6f, .9f);

		crosshairIndex = 4;
		PrimaryDamage = .3f;
		SpecialDamage = 2.7f;
		DurSpecialCost = 2;
		NormalCooldown = .20f;
		shatterRadius = 5;
		SpecialCooldown = Random.Range(6f, 8f);
#if CHEAT
		NormalCooldown = .3f;
		SpecialCooldown = 1f;
		Durability = 100;
#else
		//SpecialCooldown = Random.Range(4, 5);
#endif
		BeamColor = new Color(.5f, .5f, 1f);
	}

	public override void UpdateWeapon(float time)
	{
		if (IconUI != null)
		{
			IconUI.color = new Color(.5f, .5f, 1f, IconUI.color.a);
		}
		base.UpdateWeapon(time);
	}

	public override void UseWeapon(GameObject target = null, System.Type targType = null, GameObject[] firePoints = null, Vector3 targetScanDir = default(Vector3), bool lockOn = false)
	{
		Vector3 firePoint = firePoints[specialFirePointIndex].transform.position;

		GameObject go = (GameObject)GameObject.Instantiate(glacialDiskPrefab, firePoint, Quaternion.identity);
		GlacialDisk disk = go.GetComponent<GlacialDisk>();
		disk.Shooter = Carrier;
		disk.Creator = this;
		disk.Faction = Faction;
		disk.shatterRadius = shatterRadius;
		disk.Damage = PrimaryDamage;
		disk.explosiveDamage = SpecialDamage;

		gDisks.Add(disk);

		Vector3 dir = targetScanDir - firePoint;
		dir.Normalize();
		dir += Vector3.up * .25f;
		dir.Normalize();

		disk.rigidbody.AddForce(dir * disk.ProjVel * disk.rigidbody.mass);

		disk.transform.LookAt(dir, Vector3.Cross(dir, Carrier.transform.right));

		Destroy(go, 5);
	}

	public override void UseWeaponSpecial(GameObject target = null, System.Type targType = null,  GameObject[] firePoints = null, Vector3 targetScanDir = default(Vector3), bool lockOn = false)
	{
		for (int i = gDisks.Count; i > 0; i--)
		{
			gDisks[i - 1].Shatter();
			gDisks.RemoveAt(i - 1);
		}
	}

	public void RemoveDisk(GlacialDisk removed)
	{
		gDisks.Remove(removed);
	}

	#region Static Functions
	public new static GlacialSling New()
	{
		GlacialSling gs = ScriptableObject.CreateInstance<GlacialSling>();
		gs.AbilityName = GlacialSling.GetWeaponName();
		gs.Durability = Random.Range(80, 95);
		gs.NormalCooldown = .3f;
		gs.SpecialCooldown = 4;
		gs.CdLeft = 0;
		gs.PrimaryDesc = "[Damage] [Combo]\nFling a slow moving glacial disk.";
		gs.SecondaryDesc = "[Damage], [Combo], [Explosive]\nCause all glacial disks to shatter.";
		return gs;
	}

	static string weaponName = "Glacial Sling";
	public new static string GetWeaponName()
	{
		return (weaponName);
	}
	#endregion
}
