using UnityEngine;
using System.Collections;

public class WarpStaff : Weapon {

#region Class Variables
	public static int IconIndex = 37;
	Vector3 movementVector;
	public GameObject TeleDestPrefab;
	public GameObject TeleDestObj;

	// Design Knobs:
	public float NormalCooldownKnob = 4.0f;
	public float SpecialCooldownKnob = 1.0f;
#endregion

#region Initialization
	public override void Init()
	{
		// Calls the Weapon Class Init
		base.Init();
		// Loads the Teleport Destination Prefab.
		TeleDestPrefab = Resources.Load<GameObject>("Projectiles/TeleDestPrefab");
		
		//UI Elements:
		// Sets up Weapon's Icon.
		Icon = UIManager.Instance.Icons[IconIndex];
		//Sets up Weapon's Crosshair
		crosshairIndex = 1;
		crosshairColor = Color.blue;
		specialCrosshairColor = Color.red;

		// Define where the shot is fired from:
		//0: Middle of body 1: Upper Right//2: Lower right//3: Upper Left//4: Lower Right
		primaryFirePointIndex = 1;
		specialFirePointIndex = 1;

		// Durability and Cooldown Stats
#if CHEAT
		NormalCooldown = .7f;
		SpecialCooldown = 0.5f;
		Durability = 100;

		Durability = 999;
		DurCost = 0;
		DurCost = 1;
		DurSpecialCost = 2;
#else
		NormalCooldown = NormalCooldownKnob;
		SpecialCooldown = SpecialCooldownKnob;
		CdLeft = 0.0f;

		DurCost = 1;
		DurSpecialCost = 2;
		Durability = 50;
#endif
		
		// Colors the beam
		BeamColor = new Color(Random.Range(0.0f, 1.0f), Random.Range(0.0f, 1.0f), Random.Range(0.0f, 1.0f));
	}
#endregion

#region Fire Weapon
	// Primary Fire
	public override void UseWeapon(GameObject target = null, System.Type targType = null, GameObject[] firePoints = null, Vector3 targetScanDir = default(Vector3), bool lockOn = false)
{
 	base.UseWeapon(target, targType, firePoints, targetScanDir, lockOn);

}

	public override void UseWeaponSpecial(GameObject target = null, System.Type targType = null, GameObject[] firePoints = null, Vector3 targetScanDir = default(Vector3), bool lockOn = false)
	{
		Vector3 firePoint = firePoints[0].transform.position;

		Vector3 dir = targetScanDir - firePoint;

		//float float = 45;
		Vector3 movementDir = dir;
		movementDir = new Vector3(movementDir.x, 0, movementDir.z);

		//Debug.Log(dir + "\n" + movementDir + "\n");
		//MoveCarrier(movementDir, 0, Vector3.up, 1.5f, true);
		MoveCarrier(movementDir, .2f, Vector3.up, 0.45f, true);
	}

#endregion

	#region Static Functions
	// Sets Up weapon initialization on New() call. Also Sets up descriptions
	public new static WarpStaff New()
	{
		WarpStaff ws = ScriptableObject.CreateInstance<WarpStaff>();
		// Weapon Name
		ws.AbilityName = WarpStaff.GetWeaponName();
		// Weapon Description on Pause Menu
		ws.PrimaryDesc = "[Damage], [Utility]\nShort Teleport\n Causes an explosion on arrivals.";
		ws.SecondaryDesc = "[Utility]\nTeleport enemies in ramdom direction.";
		return ws;
	}

	// Creates Wepon Name string for UI elements
	static string[] adj = { "Basic", "Bulky", "Hasty", "Deadly", "Steel", "Vampiric", "Anachronic", "Violent", "Nimble", "Strange" };
	static string weaponName = "Warp Staff";
	public new static string GetWeaponName()
	{
		//int rndA = Random.Range(0, adj.Length);

		return (/*adj[rndA] + " " + */weaponName);
	}
	#endregion
}

/*

	public override void UseWeaponSpecial(GameObject target = null, System.Type targType = null, GameObject[] firePoints = null, Vector3 targetScanDir = default(Vector3), bool lockOn = false)
	{
		Vector3 firePoint = firePoints[0].transform.position;

		Vector3 dir = targetScanDir - firePoint;

		//float float = 45;
		Vector3 movementDir = dir;
		movementDir = new Vector3(movementDir.x, 0, movementDir.z);

		//Debug.Log(dir + "\n" + movementDir + "\n");
		//MoveCarrier(movementDir, 0, Vector3.up, 1.5f, true);
		MoveCarrier(movementDir, .2f, Vector3.up, 0.45f, true);
	}
*/