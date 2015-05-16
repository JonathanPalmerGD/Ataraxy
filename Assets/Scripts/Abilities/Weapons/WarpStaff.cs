using UnityEngine;
using System.Collections;

public class WarpStaff : Weapon
{

	#region Class Variables
	public static int IconIndex = 37;

	public GameObject TeleDestPrefab;
	public GameObject TeleDestObj;
	public GameObject ExplosionPrefab;

	// Teleport Magnetude
	public float EnemyTeleMag = 30;
	public float SelfTeleMag = 30;

	// Explosive Prefab
	public float InnerBlastSphere = 2.0f;
	public float blastRadius = 8.0f;
	public float MaxExplosiveDmg = 2.5f;
	public float MinExplosiveDmg = 0.3f;

	// Aiming direction
	Vector3 AimDir;
	#endregion

	#region Initialization

	public override void Init()
	{
		// Loads the Teleport Destination Prefab.
		TeleDestPrefab = Resources.Load<GameObject>("Projectiles/TeleDestPrefab");
		// If it does not exists, create it.
		//if (GameManager.Instance.playerGO.transform.FindChild("TeleDestPrefab") == null)
		//{
			TeleDestObj = (GameObject)GameObject.Instantiate(TeleDestPrefab, Vector3.zero, Quaternion.identity);
			//TeleDestObj.name = "TeleDestPrefab";
			// Sets the player as the Parent of the GO.
			//TeleDestObj.transform.SetParent(GameManager.Instance.playerGO.transform);
		//}
		//else
		//{
		//	TeleDestObj = GameManager.Instance.playerGO.transform.FindChild("TeleDestPrefab").gameObject;
		//}

		// As of right now this creates multiple instances of the Detonator as well. Fix this later.
		ExplosionPrefab = Resources.Load<GameObject>("Detonator-Telefrag01");

		//UI Elements:
		// Sets up Weapon's Icon.
		Icon = UIManager.Instance.Icons[IconIndex];

		AbilityName = WarpStaff.GetWeaponName();
		// Weapon Description on Pause Menu
		PrimaryDesc = "[Damage], [Utility]\nShort Teleport\nCreates an explosion at the destination.";
		SecondaryDesc = "[Damage], [Utility]\nTeleport enemies in random direction.\nEnemies that can't be teleported will be dimensionally torn.";

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
		SpecialCooldown = 2.50f;
		CdLeft = 0.0f;

		// Audio
		primaryAudio = "Teleport_Fwomp";
		specialAudio = "Teleport_Bring";
		HasAudio = true;
		
		// Durability Stats
		DurCost = 3;
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

		if (GameManager.Instance.player.WeaponIndex >= 0)
		{
			//If player has the WarpStaff Equipped:
			if (GameManager.Instance.player.weapons[GameManager.Instance.player.WeaponIndex] == this)
			{
				// Enable the teleFragDestObj and keep it in front of the player
				TeleDestObj.renderer.enabled = true;
				TeleDestObj.particleSystem.enableEmission = true;
				// Calculate where the player is poiting at:
				AimDir = GameManager.Instance.player.targetScanDir - GameManager.Instance.player.FirePoints[0].transform.position;
				AimDir.Normalize();
				// Updates TeledestObj position:
				TeleDestObj.transform.position = Carrier.transform.position + AimDir * SelfTeleMag;
			}
			else
			{
				//Disable the TeleFrag Obj renderer and particle emiter.
				TeleDestObj.renderer.enabled = false;
				TeleDestObj.particleSystem.enableEmission = false;
				TeleDestObj.particleSystem.Clear();
			}
		}
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
		
		AudioSource warpAud = AudioManager.Instance.MakeSource(primaryAudio);
		warpAud.Play();
		
		// Creates Explosion.
		if (ExplosionPrefab != null)
		{
			GameObject.Instantiate(ExplosionPrefab, Carrier.transform.position, Quaternion.identity);
		}
		applyExplosionDamege(Carrier.transform.position);
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
						Vector3 TeleDir = new Vector3(Random.Range(-1.0f, 1.0f), Random.Range(0.0f, 1.0f), Random.Range(-1.0f, 1.0f));
						TeleDir.Normalize();
						Vector3 NewTargPosition = CurrTargPos + EnemyTeleMag * TeleDir;
						e.transform.position = NewTargPosition;
					
						// Creates Sparks.
						if (ExplosionPrefab != null)
						{
							GameObject.Instantiate(ExplosionPrefab, CurrTargPos, Quaternion.identity);
							GameObject.Instantiate(ExplosionPrefab, NewTargPosition, Quaternion.identity);
						}
						
						// Damages Target. This is necessary because overlap sphere is not hitting it.
						e.AdjustHealth(-MaxExplosiveDmg);
						
						AudioSource warpAud = AudioManager.Instance.MakeSource(primaryAudio);
						warpAud.Play();
					}
					else
					{	// Target has NO rigidBody
						// Creates Explosion.
						if (ExplosionPrefab != null)
						{
							GameObject.Instantiate(ExplosionPrefab, e.transform.position, Quaternion.identity);
						}
						
						AudioSource warpAud = AudioManager.Instance.MakeSource(specialAudio);
						warpAud.Play();
					}
					// Applies damage
					applyExplosionDamege(e.transform.position);
				}//if (e.Faction != Faction)
			}// if (targType.IsSubclassOf(typeof(Enemy)) || targType == typeof(Enemy))
		}// if (targType != null)
	}

	void applyExplosionDamege(Vector3 ExplosionPosition)
	{
		// Creates overlap sphere and aplies damage to Enemies.
		Collider[] hitColliders = Physics.OverlapSphere(ExplosionPosition, blastRadius);
		int i = 0;

		float distFromBlast;
		float parameterForMessage;

		while (i < hitColliders.Length)
		{
			distFromBlast = Vector3.Distance(hitColliders[i].transform.position, ExplosionPosition);

			if (hitColliders[i].gameObject != this.Carrier.gameObject)
			{ // Carrier fo this wepon is not damaged by it.
				// Calculate Explosion Damage.
				// Linear Equation ax +b = y where x = distFromBlast and y = parameterForMessage
				//		y			=					a									*	x			+									b
				parameterForMessage = (-MaxExplosiveDmg / (blastRadius - InnerBlastSphere)) * distFromBlast + ((blastRadius / (blastRadius - InnerBlastSphere)) * MaxExplosiveDmg);
				// Caps Damage:
				if (parameterForMessage > MaxExplosiveDmg) parameterForMessage = MaxExplosiveDmg;
				else if (parameterForMessage < MinExplosiveDmg) parameterForMessage = MinExplosiveDmg;
				hitColliders[i].gameObject.SendMessage("AdjustHealth", -parameterForMessage * Carrier.DamageAmplification, SendMessageOptions.DontRequireReceiver);
			}//if (hitColliders[i].gameObject != this.Carrier.gameObject)
			i++;
		}//while (i < hitColliders.Length)
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
		ws.PrimaryDesc = "[Damage], [Utility]\nShort Teleport\nCreates an explosion at the destination.";
		ws.SecondaryDesc = "[Damage], [Utility]\nTeleport enemies in random direction.\nEnemies that can't be teleported will be dimensionally torn.";
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