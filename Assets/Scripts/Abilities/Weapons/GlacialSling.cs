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

		AbilityName = GlacialSling.GetWeaponName();
		SetupDurability(140, 190);
		PrimaryDesc = "[Damage] [Combo]\nFling an arcing glacial disk.\nDisks do minimal impact damage.";
		SecondaryDesc = "[Damage], [Combo], [Explosive]\nShatter all active glacial disks.\nArcing disks by, over or under an enemy allows a combo to deal more damage.";

		primaryFirePointIndex = 1;
		specialFirePointIndex = 1;
		crosshairColor = new Color(.6f, .6f, .9f);

		crosshairIndex = 4;
		PrimaryDamage = .15f;
		SpecialDamage = 1.5f;
		DurSpecialCost = 5;
		NormalCooldown = .20f;
		shatterRadius = 5;
		HasAudio = true;
		SpecialCooldown = Random.Range(1.0f, 1.5f);
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

	public override bool HandleDurability(bool specialAttack = false, GameObject target = null, bool lockOn = false)
	{
		//Don't let them waste ammo detonating nothing.
		if (specialAttack && gDisks.Count == 0)
		{
			return false;
		}
		
		//Otherwise we'll check the normal conditions.
		return base.HandleDurability(specialAttack, target, lockOn);
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

		disk.GetComponent<Rigidbody>().AddForce(dir * disk.ProjVel * Carrier.ProjSpeedAmp * disk.GetComponent<Rigidbody>().mass);

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
		if(gDisks.Contains(removed))
		{
			gDisks.Remove(removed);
		}
	}

	#region Static Functions
	public new static GlacialSling New()
	{
		GlacialSling gs = ScriptableObject.CreateInstance<GlacialSling>();
		gs.AbilityName = GlacialSling.GetWeaponName();
		gs.SetupDurability(140, 190);
		gs.PrimaryDesc = "[Damage] [Combo]\nFling an arcing glacial disk.\nDisks do minimal impact damage.";
		gs.SecondaryDesc = "[Damage], [Combo], [Explosive]\nShatter all active glacial disks.\nArcing disks by, over or under an enemy allows a combo to deal more damage.";
		return gs;
	}

	static string weaponName = "Glacial Sling";
	public new static string GetWeaponName()
	{
		return (weaponName);
	}
	#endregion
}
