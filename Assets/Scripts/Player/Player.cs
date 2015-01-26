using UnityEngine.UI;
using UnityEngine;
using System.Collections.Generic;
using System.Collections;

public class Player : Entity
{
	#region Lists - Weapons & Passives
	public List<Weapon> weapons;
	public List<Passive> passives;
	#endregion

	#region Player Unique Interface
	public Image SelectorUI;
	public Text WeaponText;
	public GameObject WeaponUI;
	public GameObject PassiveUI;
	public GameObject iconPrefab;

	public float flashSpeed = 5f;
	public Color flashColor = new Color(1f, 0f, 0f, 0.15f);
	#endregion

	#region Key GameObjects - Camera, Target
	public Camera mainCamera;
	public NPC targetedEntity = null;
	public GameObject hitscanTarget = null;
	public Vector3 hitPoint = Vector3.zero;
	public GameObject leftShFirePoint;
	public GameObject rightShFirePoint;
	public GameObject leftHipFirePoint;
	public GameObject rightHipFirePoint;
	public GameObject[] FirePoints;
	#endregion

	#region Weapon Variables
	private float counter = 0.0f;

	private int weaponIndex = 0;
	public int WeaponIndex
	{
		get { return weaponIndex; }
		set { weaponIndex = value; }
	}
	#endregion

	#region Player Stats
	//Damage Amplification
	//Invincibility Frames
	//Double Jump
	//Experience per Level
	//Damage Reduction
	//Bonus Knockback
	//Critical Hit Chance
	
	#endregion

	#region XP & Resource System
	public enum ResourceSystem { Mana, Rage, Energy };
	public ResourceSystem rSystem = ResourceSystem.Energy;

	public override void GainLevel()
	{
		base.GainLevel();

		SetupHealthUI();
		SetupXPUI();
		XPNeeded += 20;

	}

	void SetupResourceSystem()
	{

	}

	void ManageResourceSystem()
	{
		switch (rSystem)
		{
			case ResourceSystem.Mana:

				break;
			case ResourceSystem.Rage:

				break;
			case ResourceSystem.Energy:
				//Check distance from nearby enemies.

				break;
		}

	}
	#endregion

	#region Core Functions - Start, Update, GetInput
	#region Initialization
	public override void Start()
	{
		NameInGame = "Vant";

		List<GameObject> fPoints = new List<GameObject>();
		fPoints.Add(transform.FindChild("Main Camera").transform.FindChild("Front Firing Point").gameObject);
		fPoints.Add(transform.FindChild("Main Camera").transform.FindChild("RightShoulder Firing Point").gameObject);
		fPoints.Add(transform.FindChild("Main Camera").transform.FindChild("LeftShoulder Firing Point").gameObject);
		fPoints.Add(transform.FindChild("Main Camera").transform.FindChild("RightHip Firing Point").gameObject);
		fPoints.Add(transform.FindChild("Main Camera").transform.FindChild("LeftHip Firing Point").gameObject);

		FirePoints = fPoints.ToArray();

		SelectorUI.gameObject.SetActive(true);
		SelectorUI.fillMethod = Image.FillMethod.Radial360;
		SelectorUI.fillClockwise = true;

		weapons = new List<Weapon>();
		passives = new List<Passive>();

		//SetupAbility(MonkStaff.New());
		//SetupAbility(Longsword.New());
		//SetupAbility(Rapier.New());
		//SetupAbility(Dagger.New());
		//SetupAbility(RocketLauncher.New());
		//SetupAbility(ShockRifle.New());
		//SetupAbility(Weapon.New());
		//SetupAbility(MultiToken.NewWeapon());
		SetupAbility(GravityStaff.New());
		//SetupAbility(Weapon.New());
		
		//SetupAbility(Passive.New());
		//SetupAbility(Passive.New());
		//SetupAbility(Passive.New());
		//SetupAbility(Passive.New());

		SetupResourceSystem();
		Level = 1;
		XPNeeded = 30;

		HealthSlider = UIManager.Instance.player_HP;
		HealthText = UIManager.Instance.player_HPText;
		XPSlider = UIManager.Instance.player_XP;
		XPText = UIManager.Instance.player_XPText;
		LevelText = UIManager.Instance.player_LevelText;
		NameText = UIManager.Instance.player_Name;
		ResourceSlider = UIManager.Instance.player_Resource;
		ResourceText = UIManager.Instance.player_ResourceText;
		WeaponText = UIManager.Instance.player_WeaponText;

		SetupHealthUI();
		SetupResourceUI();
		SetupNameUI();
		SetupXPUI();

		DamageImage = UIManager.Instance.damage_Indicator;

		base.Start();

		gameObject.tag = "Player";
		Faction = Allegiance.Player;
	}

	public void SetupAbility(Ability ToAdd)
	{
		if (ToAdd is Weapon)
		{
			Weapon w = (Weapon)ToAdd;

			w.Init();

			//WeaponUI
			Image panel = ((GameObject)GameObject.Instantiate(iconPrefab)).GetComponent<Image>();

			panel.name = "I: " + w.AbilityName;

			w.Faction = Allegiance.Player;

			panel.rectTransform.SetParent(WeaponUI.transform);
			w.Remainder = panel.transform.FindChild("Remainder").GetComponent<Text>();
			
			w.Remainder.text = w.Durability.ToString();
			w.WeaponBearer = gameObject;
			
			panel.sprite = w.Icon;
			w.IconUI = panel;
			weapons.Add(w);
			panel.rectTransform.anchoredPosition = new Vector2((weapons.Count - 1) * 67, 0);
		}
		else if (ToAdd is Passive)
		{

			Passive p = (Passive)ToAdd;

			Image panel = ((GameObject)GameObject.Instantiate(iconPrefab)).GetComponent<Image>();

			panel.name = "I: " + p.AbilityName;

			panel.rectTransform.anchorMin = new Vector2(1, 1);
			panel.rectTransform.anchorMax = new Vector2(1, 1);

			p.Icon = UIManager.Instance.Icons[Random.Range(1, UIManager.Instance.Icons.Length)];
			panel.color = new Color(0, .8f, 0);
			panel.rectTransform.SetParent(PassiveUI.transform);
			p.Remainder = panel.transform.FindChild("Remainder").GetComponent<Text>();
			p.Remainder.text = ((int)(p.DurationRemaining * 10)).ToString();

			panel.sprite = p.Icon;
			p.IconUI = panel;
			passives.Add(p);

			panel.rectTransform.anchoredPosition = new Vector2((passives.Count) * -67, 0);
		}
	}
	#endregion

	#region Update, MaintainAbilities
	public override void Update()
	{
		GetInput();
		if (!UIManager.Instance.paused)
		{
			//Debug.DrawLine(transform.position,TerrainManager.Instance.clusters[ TerrainManager.Instance.FindNearestCluster(transform.position)].transform.position, Color.red);

			/*if ((null != TerrainManager.Instance.FindNearestCluster(transform.position, 15)))
			{
				Debug.DrawLine(transform.position, TerrainManager.Instance.FindNearestCluster(transform.position, 15).transform.position, Color.white);
			}*/
			//Debug.DrawLine(transform.position, transform.position + TerrainManager.Instance.FindOffsetOfDir(tempIndex % 8), Color.cyan, 3f);

			#region Handle Damage
			if (Damaged)
			{
				DamageImage.color = flashColor;
			}
			else
			{
				if (DamageImage != null)
				{
					DamageImage.color = Color.Lerp(DamageImage.color, Color.clear, flashSpeed * Time.deltaTime);
				}
			}
			#endregion
			#region Resource System
			ManageResourceSystem();
			#endregion
			bool dirtyAbilityBar = false;
			#region Update Passive Durations & Check Removal
			for (int i = 0; i < passives.Count; i++)
			{
				if (passives[i] != null)
				{
					passives[i].UpdatePassive(Time.deltaTime);
					if (passives[i].CheckAbility())
					{
						passives.RemoveAt(i);
						i--;
						dirtyAbilityBar = true;
					}
				}
			}
			#endregion
			#region Check Weapons for Removal

			for (int i = 0; i < weapons.Count; i++)
			{
				if (weapons[i] != null)
				{
					if (weaponIndex == i)
					{
						WeaponText.text = weapons[i].AbilityName;
						if (weapons[i].CdLeft > 0)
						{
							SelectorUI.type = Image.Type.Filled;
							SelectorUI.fillCenter = true;
							float startCooldownAmt = 0;
							if (weapons[i].UseSpecialCooldown)
							{
								startCooldownAmt = weapons[i].SpecialCooldown;
							}
							else
							{
								startCooldownAmt = weapons[i].NormalCooldown;
							}
							SelectorUI.fillAmount = 1 - (weapons[i].CdLeft / startCooldownAmt);
						}
						else
						{
							//SelectorUI.fillAmount = 1;
							SelectorUI.type = Image.Type.Sliced;
							SelectorUI.fillCenter = false;
						}
					}
					weapons[i].UpdateWeapon(Time.deltaTime);
					if (weapons[i].CheckAbility())
					{
						weapons[i].CleanUp();
						weapons.RemoveAt(i);
						i--;
						dirtyAbilityBar = true;
					}
				}
			}

			if (weaponIndex > weapons.Count - 1)
			{
				weaponIndex = weapons.Count - 1;
			}

			if (dirtyAbilityBar)
			{
				MaintainAbilities();
			}
			#endregion
			#region Handle Selector Location
			SelectorUI.rectTransform.position = new Vector3((1 + WeaponIndex) * 67 - 32, 35);
			int index = SelectorUI.transform.GetSiblingIndex();
			SelectorUI.transform.SetSiblingIndex(index + 1);
			#endregion


			UIManager.Instance.item_NameText.text = weapons[weaponIndex].AbilityName;
			UIManager.Instance.item_PrimaryText.text = weapons[weaponIndex].PrimaryDesc;
			UIManager.Instance.item_SecondaryText.text = weapons[weaponIndex].SecondaryDesc;

			hitscanTarget = TargetScan();
			//Debug.Log(hitscanTarget != null? hitscanTarget.name + "\n" : "\n");
			HandleTarget();

			Damaged = false;

			base.Update();
		}
	}

	public void MaintainAbilities()
	{
		for (int i = 0; i < weapons.Count; i++)
		{
			weapons[i].IconUI.rectTransform.anchoredPosition = new Vector2((i) * 67, 0);
		}
		for (int i = 0; i < passives.Count; i++)
		{
			passives[i].IconUI.rectTransform.anchorMin = new Vector2(1, 1);
			passives[i].IconUI.rectTransform.anchorMax = new Vector2(1, 1);

			passives[i].IconUI.rectTransform.anchoredPosition = new Vector2((i + 1) * -67, 0);
		}
	}
	#endregion

	void GetInput()
	{
		if (Input.GetKeyDown(KeyCode.Period))
		{
			Debug.LogError(Shader.Find("Particles/Alpha Blended").ToString() + "\n");
			Debug.LogError(new Material(Shader.Find("Particles/Alpha Blended")).ToString() + "\n");
			//Debug.LogError(new Material(Shader.Find("Particles/Alpha Blended")).ToString() + "\n");
		}
		if (!UIManager.Instance.paused)
		{
			#region Mouse Buttons 1
			if (Input.GetButtonDown("Fire1"))
			{
				//Debug.Log("Firing\n");
			}
			if (Input.GetButton("Fire1"))
			{
				//If we have a weapon
				if (weapons.Count > 0)
				{
					//If the weapon has ammo and is off cooldown, DO IT.
					if (weapons[weaponIndex].HandleDurability(false))
					{
						//If we have the same target this frame as our HUD
						if (targetedEntity != null && hitscanTarget != null && hitscanTarget == targetedEntity.gameObject)
						{
							weapons[weaponIndex].UseWeapon(targetedEntity.gameObject, targetedEntity.GetType(), FirePoints, hitPoint, true);
						}
						else
						{
							weapons[weaponIndex].UseWeapon(hitscanTarget, null, FirePoints, hitPoint, true);
						}
					}

				}
			}
			if (Input.GetButtonUp("Fire1"))
			{
				//Debug.Log("Fire Ceased\n");
			}
			#endregion

			#region Mouse Buttons 2
			if (Input.GetButtonDown("Fire2"))
			{
				//Debug.Log("Firing\n");
			}
			if (Input.GetButton("Fire2"))
			{
				//If we have a weapon
				if (weapons.Count > 0)
				{
					//If the weapon has ammo and is off cooldown, DO IT.
					if (weapons[weaponIndex].HandleDurability(true))
					{
						//If we have the same target this frame as our HUD
						if (targetedEntity != null && hitscanTarget != null && hitscanTarget == targetedEntity.gameObject)
						{
							weapons[weaponIndex].UseWeaponSpecial(targetedEntity.gameObject, targetedEntity.GetType(), FirePoints, hitPoint, true);
						}
						else
						{
							weapons[weaponIndex].UseWeaponSpecial(hitscanTarget, null, FirePoints, hitPoint, true);
						}
					}

				}
			}
			if (Input.GetButtonUp("Fire2"))
			{
				//Debug.Log("Fire Ceased\n");
			}
			#endregion

			#region Testing Zone
#if UNITY_EDITOR
			if (Input.GetKeyDown(KeyCode.T))
			{
				CharacterMotor charMotor = gameObject.GetComponent<CharacterMotor>();
				Vector3 newVel = new Vector3(0.0f, 1, 0.0f);
				newVel.Normalize();
				newVel *= 30;
				charMotor.SetVelocity(newVel);
			}
			if (Input.GetKeyDown(KeyCode.G))
			{
				CharacterMotor charMotor = gameObject.GetComponent<CharacterMotor>();
				Vector3 newVel = new Vector3(0.0f, -1, 0.0f);
				newVel.Normalize();
				newVel *= 50;
				charMotor.SetVelocity(newVel);
			}
			if (Input.GetKeyDown(KeyCode.LeftShift))
			{
				CharacterMotor charMotor = gameObject.GetComponent<CharacterMotor>();
				Vector3 newVel = new Vector3(transform.forward.x * 20.0f, 1, transform.forward.z * 20.0f);
				newVel.Normalize();
				newVel *= 120;
				charMotor.SetVelocity(newVel);
			}
			if (Input.GetKeyDown(KeyCode.LeftControl))
			{
				CharacterMotor charMotor = gameObject.GetComponent<CharacterMotor>();
				Vector3 newVel = new Vector3(0, 100, 0);
				newVel.Normalize();
				newVel *= 100;
				charMotor.SetVelocity(newVel);
			}
#endif
			#endregion

			#region Health & Resources
#if UNITY_EDITOR
			if (Input.GetKeyDown(KeyCode.R))
			{
				AdjustHealth(-1);
			}

			if (Input.GetKeyDown(KeyCode.Y))
			{
				AdjustResource(-3);
			}
			if (Input.GetKeyDown(KeyCode.H))
			{
				AdjustResource(3);
			}
#endif
			#endregion

			//These are controllable while paused.
			#region Scroll Wheel
			if (Input.GetAxis("Mouse ScrollWheel") > 0 || Input.GetButtonDown("Previous Weapon"))
			{
				if (weaponIndex == 0)
				{
					weaponIndex = weapons.Count - 1;
				}
				else
				{
					weaponIndex -= 1;
				}
			}
			else if (Input.GetAxis("Mouse ScrollWheel") < 0 || Input.GetButtonDown("Next Weapon"))
			{
				if (weaponIndex == weapons.Count - 1)
				{
					weaponIndex = 0;
				}
				else
				{
					weaponIndex += 1;
				}
			}
			#endregion

			#region Number Checking
			if (Input.GetButton("Quickslot 1"))
			{
				weaponIndex = 0;
			}
			if (Input.GetButton("Quickslot 2"))
			{
				weaponIndex = 1;
			}
			if (Input.GetButton("Quickslot 3"))
			{
				weaponIndex = 2;
			}
			if (Input.GetButton("Quickslot 4"))
			{
				weaponIndex = 3;
			}
			if (Input.GetButton("Quickslot 5"))
			{
				weaponIndex = 4;
			}
			if (Input.GetButton("Quickslot 6"))
			{
				weaponIndex = 5;
			}
			if (Input.GetButton("Quickslot 7"))
			{
				weaponIndex = 6;
			}
			if (Input.GetButton("Quickslot 8"))
			{
				weaponIndex = 7;
			}
			if (Input.GetButton("Quickslot 9"))
			{
				weaponIndex = 8;
			}
			if (Input.GetButton("Quickslot 10"))
			{
				weaponIndex = 9;
			}
			#endregion

			#region Cheat Weapons
			if (Input.GetKeyDown(KeyCode.M))
			{
				//SetupAbility(MonkStaff.New());
				SetupAbility(Longsword.New());
				SetupAbility(Rapier.New());
				SetupAbility(Dagger.New());
				SetupAbility(RocketLauncher.New());
				SetupAbility(ShockRifle.New());
				SetupAbility(GravityStaff.New());
				SetupAbility(GravityStaff.New());
			}
			#endregion

			if (Input.GetKeyDown(KeyCode.LeftBracket))
			{
				MouseLook look = gameObject.GetComponent<MouseLook>();

				look.sensitivityX--;
				look.sensitivityY--;

				MouseLook yLook = GameObject.Find("Main Camera").GetComponent<MouseLook>();

				yLook.sensitivityX--;
				yLook.sensitivityY--;
			}
			if (Input.GetKeyDown(KeyCode.RightBracket))
			{
				MouseLook look = gameObject.GetComponent<MouseLook>();

				look.sensitivityX++;
				look.sensitivityY++;

				MouseLook yLook = GameObject.Find("Main Camera").GetComponent<MouseLook>();

				yLook.sensitivityX++;
				yLook.sensitivityY++;
			}
		}
	}
	#endregion

	public override void KillEntity()
	{
		base.KillEntity();

		Application.LoadLevel("GameOver");
	}

	#region Targetting and Hitscan
	GameObject TargetScan()
	{
		Ray ray = mainCamera.ScreenPointToRay(new Vector3(Screen.width / 2, Screen.height / 2));
		RaycastHit hit;
		//Debug.DrawLine(transform.position, (transform.position + ray) * 100, Color.green);

		//If we fire, set hitPoint to someplace arbitrarily far away in the shooting. Even if we hit something, we want to target wherever the cursor pointed.
		hitPoint = transform.position + (ray.direction * 500);

		if (Physics.Raycast(ray, out hit))
		{
			//Debug.Log(hit.collider.gameObject.tag + "\n");
			if (hit.collider.gameObject.tag == "NPC")
			{
				NPC n = hit.collider.gameObject.GetComponent<NPC>();
				CheckNewTarget((NPC)n);

				return n.gameObject;
			}
			else if (hit.collider.gameObject.tag == "Enemy")
			{
				Enemy e = hit.collider.gameObject.GetComponent<Enemy>();
				CheckNewTarget((Enemy)e);

				return e.gameObject;
			}
			//This isn't necessary for right now.
			else if (hit.collider.gameObject.tag == "WorldObject")
			{
				//Island e = hit.collider.gameObject.GetComponent<Island>();
				//CheckNewTarget((Entity)e);
			}
			else if (hit.collider.gameObject.tag == "Projectile")
			{
				Projectile p = hit.collider.gameObject.GetComponent<Projectile>();
				if (p != null)
				{
					return p.gameObject;
				}
			}
			else
			{
				return hit.collider.gameObject;
			}
			//Debug.Log(hit.collider.gameObject.name + "\n");
		}
		else
		{
			//Debug.Log("Targetting nothing\n");
		}
		return null;
	}

	void CheckNewTarget(NPC newTarget)
	{
		//If we had a target
		if (targetedEntity != null)
		{
			//If our new target is different
			if (newTarget != targetedEntity)
			{
				//Untarget the old.
				targetedEntity.Untarget();

				//Set new target.
				targetedEntity = newTarget;

				//Tell em they're fabulous
				targetedEntity.Target();
			}
		}
		//If we had no target
		else
		{
			//Set new target.
			targetedEntity = newTarget;

			//Tell em they're fabulous
			targetedEntity.Target();
		}
		counter = Constants.targetFade;
	}

	void HandleTarget()
	{
		if (targetedEntity != null)
		{
			counter -= Time.deltaTime;
			if (counter <= 0)
			{
				targetedEntity.Untarget();
				targetedEntity = null;
			}
		}
	}
	#endregion

	#region Unneeded Code
	void OnGUI()
	{
		//float rectWidth = 250;
		//float rectHeight = 125;

		//GUI.Box(new Rect(Screen.width / 2 - rectWidth / 2, Screen.height - rectHeight, rectWidth, rectHeight), TextPrint());
	}

	string TextPrint()
	{
		string output = "Weapon Selected: " + weaponIndex + "  " + (float)((int)(weapons[WeaponIndex].CdLeft * 10))/10 + "\n";
		foreach (Weapon w in weapons)
		{
			output += "" + w.GetInfo() + "\n";
		}

		output += "\n";

		foreach (Passive p in passives)
		{
			output += p.GetInfo() + "\n";
		}
		return output;
	}

	public void MoveInHierarchy(int delta)
	{
		int index = transform.GetSiblingIndex();
		transform.SetSiblingIndex(index + delta);
	}
	#endregion
}
