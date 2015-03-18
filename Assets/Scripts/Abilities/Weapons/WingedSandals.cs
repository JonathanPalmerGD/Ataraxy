using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class WingedSandals : Weapon 
{
	public static int IconIndex = 60;
	Vector3 movementVector;

	public override void Init()
	{
		base.Init();
		//daggerStabPrefab = Resources.Load<GameObject>("DaggerStab");
		Icon = UIManager.Instance.Icons[IconIndex];

		crosshairIndex = 6;
		crosshairColor = Color.black;
		crosshairSize = new Vector2(128, 128);

		DurCost = 1;
		DurSpecialCost = 6;
		NormalCooldown = .5f;
		SpecialCooldown = 12f;
#if CHEAT
		//NormalCooldown = .30f;
		//SpecialCooldown = .5f;
		Durability = 100;
#else
		
#endif
		BeamColor = Color.white;
	}

	public override void UpdateWeapon(float time)
	{
		if (IconUI != null)
		{
			IconUI.color = new Color(.88f, .82f, .17f, IconUI.color.a);
		}
		base.UpdateWeapon(time);
	}

	public override void UseWeapon(GameObject target = null, System.Type targType = null, GameObject[] firePoints = null, Vector3 targetScanDir = default(Vector3), bool lockOn = false)
	{
		Vector3 firePoint = firePoints[0].transform.position;

		Vector3 dir = targetScanDir - firePoint;
		//Debug.DrawLine(firePoint, firePoint + dir, Color.black, 6.0f);
		Debug.DrawLine(GameManager.Instance.playerGO.transform.position, GameManager.Instance.playerGO.transform.position + dir * 10, Color.green, 2f);
		Debug.DrawLine(GameManager.Instance.playerGO.transform.position, GameManager.Instance.playerGO.transform.position + GameManager.Instance.playerGO.transform.forward * 10, Color.black, 2f);
		
		float lungeVel = 4.2f;
		Vector3 movementDir = dir;
		movementDir = new Vector3(movementDir.x, 0, movementDir.z);
		movementDir.Normalize();
		
		MoveCarrier(movementDir, lungeVel, Vector3.up, 3.5f, false);
	}

	public override void UseWeaponSpecial(GameObject target = null, System.Type targType = null, GameObject[] firePoints = null, Vector3 targetScanDir = default(Vector3), bool lockOn = false)
	{
		Vector3 firePoint = firePoints[0].transform.position;

		Vector3 dir = targetScanDir - firePoint;

		Vector3 movementDir = dir;
		movementDir = new Vector3(movementDir.x, 0, movementDir.z);

		MoveCarrier(Vector3.zero, 0, Vector3.up, 14f, false);
	}

	public override Vector3 AdjProjectileColliderPosition(MeleeProjectile proj)
	{
		return proj.projectileCollider.transform.forward * 1f;
	}

	#region Static Functions
	public new static WingedSandals New()
	{
		WingedSandals ws = ScriptableObject.CreateInstance<WingedSandals>();
		ws.AbilityName = WingedSandals.GetWeaponName();
		ws.Durability = Random.Range(35, 50);
		ws.NormalCooldown = .5f;
		ws.SpecialCooldown = 12f;
		ws.CdLeft = 0;
		ws.PrimaryDesc = "[Utility]\nA small flap of the sandal's tiny wings.\nCan be chained to fly short distances.";
		ws.SecondaryDesc = "[Utility]\nA powerful ascending gust.";
		return ws;
	}

	static string weaponName = "Winged Sandals";
	public new static string GetWeaponName()
	{
		return (weaponName);
	}
	#endregion
}
