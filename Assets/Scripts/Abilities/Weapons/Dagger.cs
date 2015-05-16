using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class Dagger : Weapon 
{
	public static int IconIndex = 44;
	public GameObject daggerStabPrefab;
	Vector3 movementVector;

	public override void Init()
	{
		base.Init();
		daggerStabPrefab = Resources.Load<GameObject>("Projectiles/DaggerStab");
		Icon = UIManager.Instance.Icons[IconIndex];
		
		AbilityName = Dagger.GetWeaponName();
		SetupDurability(40, 70);
		PrimaryDesc = "[Damage]\nA quick stab.\nBackstabs coming soon!";
		SecondaryDesc = "[Utility]\nA quick dash forward.\nUseful for getaways or crossing length gaps.";

		crosshairSize = new Vector2(128, 128);

		crosshairIndex = 1;
		DurSpecialCost = 5; 
		NormalCooldown = .20f;
		SpecialCooldown = 2f;
		HasAudio = true;
		primaryAudio = "Dagger_Stab";
		specialAudio = "Dagger_Stab";
#if CHEAT
		//NormalCooldown = .30f;
		//SpecialCooldown = .5f;
		Durability = 100;
#else
		
#endif
		BeamColor = Color.white;
	}

	public override void UseWeapon(GameObject target = null, System.Type targType = null, GameObject[] firePoints = null, Vector3 targetScanDir = default(Vector3), bool lockOn = false)
	{
		Vector3 firePoint = firePoints[0].transform.position;

		Vector3 dir = targetScanDir - firePoint;
		dir.Normalize();

		GameObject go = (GameObject)GameObject.Instantiate(daggerStabPrefab, firePoint, Quaternion.identity);
		DaggerStab stab = go.GetComponent<DaggerStab>();

		stab.Init();
		stab.Shooter = Carrier;

		AudioSource stabAud = AudioManager.Instance.MakeSourceAtPos(primaryAudio, stab.transform.position);
		stabAud.Play();
		
		List<Vector3> stabPoints = new List<Vector3>();

		stabPoints.Add(stab.transform.position - firePoints[0].transform.position - dir * 2);
		stabPoints.Add(stab.transform.position - firePoints[0].transform.position - dir);
		stabPoints.Add(stab.transform.position - firePoints[0].transform.position);

		SetupMeleeProjectile(stab, dir, stabPoints, new Vector2(.5f, .0f));
	}

	public override void UseWeaponSpecial(GameObject target = null, System.Type targType = null, GameObject[] firePoints = null, Vector3 targetScanDir = default(Vector3), bool lockOn = false)
	{
		Vector3 firePoint = firePoints[0].transform.position;

		Vector3 dir = targetScanDir - firePoint;

		float lungeVel = 15;
		Vector3 movementDir = dir;
		movementDir = new Vector3(movementDir.x, 0, movementDir.z);
		movementDir.Normalize();
		//Debug.Log(dir + "\n" + movementDir + "\n");
		MoveCarrier(movementDir, lungeVel, Vector3.up, 3, true);
	}

	public override Vector3 AdjProjectileColliderPosition(MeleeProjectile proj)
	{
		return proj.projectileCollider.transform.forward * 1f;
	}

	#region Static Functions
	public new static Dagger New()
	{
		Dagger w = ScriptableObject.CreateInstance<Dagger>();
		w.AbilityName = Dagger.GetWeaponName();
		w.SetupDurability(40, 70);
		w.PrimaryDesc = "[Damage]\nA quick stab.\nBackstabs coming soon!";
		w.SecondaryDesc = "[Utility]\nA quick dash forward.\nUseful for getaways or crossing length gaps.";
		return w;
	}

	static string weaponName = "Dagger";
	public new static string GetWeaponName()
	{
		return (weaponName);
	}
	#endregion
}
