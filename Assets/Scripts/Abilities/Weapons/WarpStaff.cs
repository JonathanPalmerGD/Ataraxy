using UnityEngine;
using System.Collections;

public class WarpStaff : Weapon
{

	#region Class Variables
	public static int IconIndex = 37;

	public GameObject TeleDestPrefab;
	public GameObject ExplosionPrefab;

	// Teleport Magnetude
	public float EnemyTeleMag = 30;
	public float SelfTeleMag = 30;

	// Explosive Prefab
	public float InnerBlastSphere = 2.0f;
	public float blastRadius = 8.0f;
	public float MaxExplosiveDmg = 3.5f;
	public float MinExplosiveDmg = 0.3f;


	#endregion

	#region Initialization
	public override void Init()
	{
		// Loads the Teleport Destination Prefab.
		TeleDestPrefab = Resources.Load<GameObject>("Projectiles/TeleDestPrefab");

		ExplosionPrefab = Resources.Load<GameObject>("Detonator-Telefrag01");

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

		// Colors the beam
		BeamColor = new Color(Random.Range(0.0f, 1.0f), Random.Range(0.0f, 1.0f), Random.Range(0.0f, 1.0f));
	}
	#endregion

	#region Update Weapon
	public override void UpdateWeapon(float time)
	{
		if (IconUI != null)
		{
			IconUI.color = new Color(.19f, .7f, .95f, IconUI.color.a);
		}
		base.UpdateWeapon(time);
	}
	#endregion

	#region Fire Weapon
	// Primary Fire
	public override void UseWeapon(GameObject target = null, System.Type targType = null, GameObject[] firePoints = null, Vector3 targetScanDir = default(Vector3), bool lockOn = false)
	{
		// Get direction Player is poiting at and Normalize it.
		Vector3 firePoint = firePoints[0].transform.position;
		Vector3 dir = targetScanDir - firePoint;
		Vector3 movementDir = dir;
		movementDir.Normalize();


		// Teleports player to targeted position.
		Vector3 TelePosition = Carrier.transform.position;
		TelePosition += movementDir * SelfTeleMag;
		Carrier.transform.position = TelePosition;

		// Stops Players momentum
		Carrier.transform.rigidbody.velocity = new Vector3(0, 0, 0);

		// Creates Explosion.
		if (ExplosionPrefab != null)
		{
			GameObject.Instantiate(ExplosionPrefab, Carrier.transform.position, Quaternion.identity);
		}

		// Creates overlap sphere and aplies damage to Enemies.
		Collider[] hitColliders = Physics.OverlapSphere(Carrier.transform.position, blastRadius);
		int i = 0;

		float distFromBlast;
		float parameterForMessage;

		while (i < hitColliders.Length)
		{
			distFromBlast = Vector3.Distance(hitColliders[i].transform.position, Carrier.transform.position);
			// Aplies Damage
			if (hitColliders[i].gameObject != this.Carrier.gameObject)
			{
				// Calculate Explosion Damage.
				// Linear Equation ax +b = y where x = distFromBlast and y = parameterForMessage
				//		y			=					a									*	x			+									b
				parameterForMessage = (-MaxExplosiveDmg / (blastRadius - InnerBlastSphere)) * distFromBlast + ((blastRadius / (blastRadius - InnerBlastSphere)) * MaxExplosiveDmg);

				// Caps Damage:
				if (parameterForMessage > MaxExplosiveDmg) parameterForMessage = MaxExplosiveDmg;
				else if (parameterForMessage < MinExplosiveDmg) parameterForMessage = MinExplosiveDmg;
				hitColliders[i].gameObject.SendMessage("AdjustHealth", -parameterForMessage, SendMessageOptions.DontRequireReceiver);
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
						Vector3 CurrTargPos = e.transform.position;

						// Gets Ramdom direction and normalize.
						Vector3 TeleDir =
						new Vector3(Random.Range(-1.0f, 1.0f), Random.Range(0.0f, 1.0f), Random.Range(-1.0f, 1.0f));
						TeleDir.Normalize();

						Vector3 NewTargPosition = CurrTargPos + EnemyTeleMag * TeleDir;
						//Debug.Log(NewTargPosition);
						e.transform.position = NewTargPosition;


						// Creates Sparks.
						if (ExplosionPrefab != null)
						{
							GameObject.Instantiate(ExplosionPrefab, CurrTargPos, Quaternion.identity);
							GameObject.Instantiate(ExplosionPrefab, e.transform.position, Quaternion.identity);
						}

						//Aplies Damage

						//Normal Damge
						e.AdjustHealth(-MinExplosiveDmg);

						//Explosion Damage:
						/*
						// Creates overlap sphere and aplies damage to Enemies.
						Collider[] hitColliders = Physics.OverlapSphere(e.transform.position, blastRadius);
						int i = 0;

						float distFromBlast;
						float parameterForMessage;

						while (i < hitColliders.Length)
						{
							distFromBlast = Vector3.Distance(hitColliders[i].transform.position, e.transform.position);
							// Aplies Damage
							if (hitColliders[i].gameObject != this.Carrier.gameObject)
							{
								// Calculate Explosion Damage.
								// Linear Equation ax +b = y where x = distFromBlast and y = parameterForMessage
								//		y			=					a									*	x			+									b
								parameterForMessage = (-MaxExplosiveDmg / (blastRadius - InnerBlastSphere)) * distFromBlast + ((blastRadius / (blastRadius - InnerBlastSphere)) * MaxExplosiveDmg);

								// Caps Damage:
								// Max Damage
								if (parameterForMessage > MaxExplosiveDmg) parameterForMessage = MaxExplosiveDmg;
								//Minimum Damage
								else if (parameterForMessage < MinExplosiveDmg) parameterForMessage = MinExplosiveDmg;

								hitColliders[i].gameObject.SendMessage("AdjustHealth", -parameterForMessage, SendMessageOptions.DontRequireReceiver);
							}
							i++;
						}
						 */
					}
					else
					{	// Target has NO rigidBody
						// Creates Explosion.
						if (ExplosionPrefab != null)
						{
							GameObject.Instantiate(ExplosionPrefab, e.transform.position, Quaternion.identity);

						}

						// Creates overlap sphere and aplies damage to Enemies.
						Collider[] hitColliders = Physics.OverlapSphere(e.transform.position, blastRadius);
						int i = 0;

						float distFromBlast;
						float parameterForMessage;

						while (i < hitColliders.Length)
						{
							distFromBlast = Vector3.Distance(hitColliders[i].transform.position, e.transform.position);
							// Aplies Damage
							if (hitColliders[i].gameObject != this.Carrier.gameObject)
							{
								// Calculate Explosion Damage.
								// Linear Equation ax +b = y where x = distFromBlast and y = parameterForMessage
								//		y			=					a									*	x			+									b
								parameterForMessage = (-MaxExplosiveDmg / (blastRadius - InnerBlastSphere)) * distFromBlast + ((blastRadius / (blastRadius - InnerBlastSphere)) * MaxExplosiveDmg);

								// Caps Damage:
								if (parameterForMessage > MaxExplosiveDmg) parameterForMessage = MaxExplosiveDmg;
								else if (parameterForMessage < MinExplosiveDmg) parameterForMessage = MinExplosiveDmg;
								hitColliders[i].gameObject.SendMessage("AdjustHealth", -parameterForMessage, SendMessageOptions.DontRequireReceiver);
							}
							i++;
						}
					}
				}
			}
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