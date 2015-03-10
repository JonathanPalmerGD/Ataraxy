using UnityEngine;
using System.Collections;

public class WarpStaff : Weapon {

#region Class Variables
	public static int IconIndex = 37;
	
	public GameObject TeleDestPrefab;
	public GameObject ExplosionPrefab;

	// Teleport Magnetude
	public float EnemyTeleMag = 30;
	public float SelfTeleMag = 30;

	// Explosive Prefab
	public float blastRadius;
	public float explosiveDamage;

#endregion

#region Initialization
	public override void Init()
	{
		// Loads the Teleport Destination Prefab.
		TeleDestPrefab = Resources.Load<GameObject>("Projectiles/TeleDestPrefab");
		
		ExplosionPrefab = Resources.Load<GameObject>("Detonator-Telefrag01");
		
		Debug.Log(ExplosionPrefab == null);
		
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

		// Cooldown Stats
		NormalCooldown = 3.0f;
		SpecialCooldown = 0.50f;
		CdLeft = 0.0f;

		// Durability Stats
		DurCost = 1;
		DurSpecialCost = 1;
		Durability = 50;

		// Explosion Stats
		blastRadius = 15;
		explosiveDamage = 2;
		
		/*
#if CHEAT
		NormalCooldown = .7f;
		SpecialCooldown = 0.5f;
		Durability = 100;

		Durability = 999;
		DurCost = 0;
		DurCost = 1;
		DurSpecialCost = 2;
#else
		NormalCooldown = 10.0f;
		SpecialCooldown = 3.0f;
		CdLeft = 0.0f;

		DurCost = 1;
		DurSpecialCost = 2;
		Durability = 50;
#endif*/

/*
#if UNITY_EDITOR
		NormalCooldown = 0.5f;
		SpecialCooldown = 0.5f;
		CdLeft = 0.0f;

		DurCost = 1;
		DurSpecialCost = 1;
		Durability = 999;
#endif
 * */

		
		// Colors the beam
		BeamColor = new Color(Random.Range(0.0f, 1.0f), Random.Range(0.0f, 1.0f), Random.Range(0.0f, 1.0f));
	}
#endregion

#region Fire Weapon
	// Primary Fire
	public override void UseWeapon(GameObject target = null, System.Type targType = null, GameObject[] firePoints = null, Vector3 targetScanDir = default(Vector3), bool lockOn = false)
{
		// Get direction Player is poiting at.
		Vector3 firePoint = firePoints[0].transform.position;
		Vector3 dir = targetScanDir - firePoint;
		Debug.DrawLine(Carrier.transform.position, Carrier.transform.position + dir, Color.red,15);
		
		Vector3 movementDir = dir;
		movementDir.Normalize();

		Vector3 TelePosition = Carrier.transform.localPosition;
		TelePosition += movementDir * SelfTeleMag;

		Carrier.transform.localPosition = TelePosition;
		
		// Stops Players momentum
		Carrier.transform.rigidbody.velocity = new Vector3 (0, 0, 0);
		 
		// Creates Explosion.
		if (ExplosionPrefab != null)
		{
			GameObject.Instantiate(ExplosionPrefab, Carrier.transform.position, Quaternion.identity);
		}

		// Creates overlap sphere and aplies damage.
		Collider[] hitColliders = Physics.OverlapSphere(Carrier.transform.position, blastRadius);
		int i = 0;
		while (i < hitColliders.Length)
		{
			float distFromBlast = Vector3.Distance(hitColliders[i].transform.position, Carrier.transform.position);
			float parameterForMessage = -(explosiveDamage * blastRadius / distFromBlast);

			if (hitColliders[i].gameObject != this.Carrier.gameObject)
			{
				hitColliders[i].gameObject.SendMessage("AdjustHealth", parameterForMessage, SendMessageOptions.DontRequireReceiver);
			}
			i++;
		}
}

	public override void UseWeaponSpecial(GameObject target = null, System.Type targType = null, GameObject[] firePoints = null, Vector3 targetScanDir = default(Vector3), bool lockOn = false)
	{

		if (targType != null)
		{	// There is a target

			if (targType.IsSubclassOf(typeof(Enemy)) || targType == typeof(Enemy))
			{	// Target is a enemy
				Enemy e = target.GetComponent<Enemy>();
				//Check If my faction is the same as the target
				if (e.Faction != Faction)
				{	// If different

					if (e.gameObject.rigidbody != null)
					{	//If target has a rigidBody
						Vector3 CurrTargPos = e.transform.localPosition;

						// Gets Ramdom direction and normalize.
						Vector3 TeleDir =
						new Vector3(Random.Range(-1.0f, 1.0f), Random.Range(0.0f, 1.0f), Random.Range(-1.0f, 1.0f));
						TeleDir.Normalize();
						Debug.Log(TeleDir);

						Vector3 NewTargPosition = CurrTargPos + EnemyTeleMag * TeleDir;
						Debug.Log(NewTargPosition);
						e.transform.localPosition = NewTargPosition;


						// Creates Sparks.
						if (ExplosionPrefab != null)
						{
							GameObject.Instantiate(ExplosionPrefab, e.transform.localPosition, Quaternion.identity);
						}

					}
					else
					{	// Target has NO rigidBody
						// Creates Explosion.
						if (ExplosionPrefab != null)
						{
							GameObject.Instantiate(ExplosionPrefab, e.transform.localPosition, Quaternion.identity);
						}

						// Creates overlap sphere and aplies damage.
						Collider[] hitColliders = Physics.OverlapSphere(e.transform.localPosition, blastRadius);
						int i = 0;
						while (i < hitColliders.Length)
						{
							float distFromBlast = Vector3.Distance(hitColliders[i].transform.position, e.transform.localPosition);
							float parameterForMessage = -(explosiveDamage * blastRadius / distFromBlast);

							if (hitColliders[i].gameObject != this.Carrier.gameObject)
							{
								hitColliders[i].gameObject.SendMessage("AdjustHealth", parameterForMessage, SendMessageOptions.DontRequireReceiver);
							}
							i++;
						}
					}
				}
			}
			/*
			else if (targType == typeof(NPC))
			{
				Debug.Log("You hit a NPC\n");

			}
			else
			{
				Debug.Log("You nothing son!\n");
			}
			*/
		}
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
	static string weaponName = "Warp Staff";
	public new static string GetWeaponName()
	{
		return (weaponName);
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