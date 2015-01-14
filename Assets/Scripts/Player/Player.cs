using UnityEngine.UI;
using UnityEngine;
using System.Collections.Generic;
using System.Collections;

public class Player : AtaraxyObject
{
	public List<Weapon> weapons;
	public List<Passive> passives;

	public Sprite thing;
	public Image SelectorUI;
	public GameObject WeaponUI;
	public GameObject PassiveUI;
	public GameObject iconPrefab;
	public Camera mainCamera;

	public Object[] Icons;

	public float flashSpeed = 5f;
	public Color flashColor = new Color(1f, 0f, 0f, 0.15f);

	public Entity targetedEntity = null;

	public override Allegiance Faction
	{
		get { return Allegiance.Player; }
	}

	private float counter = 0.0f;
	private static float targetFade = 3.0f;

	private int weaponIndex = 0;
	public int WeaponIndex
	{
		get { return weaponIndex; }
		set { weaponIndex = value; }
	}

	#region Player Stats
	
	private int experience;
	public int Experience
	{
		get { return experience; }
		set { experience = value; }
	}

	//Damage Amplification
	//Invincibility Frames
	//Double Jump
	//Experience per Level
	//Damage Reduction
	//Bonus Knockback
	//Critical Hit Chance
	
	#endregion

	public enum ResourceSystem { Mana, Rage, Energy };
	public ResourceSystem rSystem = ResourceSystem.Energy;

	public new void Start()
	{
		SelectorUI.gameObject.SetActive(true);
		SelectorUI.fillMethod = Image.FillMethod.Radial360;
		SelectorUI.fillClockwise = true;

		Icons = new Object[1];
		Icons = Resources.LoadAll("Atlases/VortexIconAtlas");

		weapons = new List<Weapon>();
		passives = new List<Passive>();

		SetupAbility(Weapon.New());
		SetupAbility(Weapon.New());
		SetupAbility(Weapon.New());
		SetupAbility(Weapon.New());
		//SetupAbility(Weapon.New());

		//Debug.Log(AssetDatabase.GetAssetPath(thing) +"\n");
		//Debug.Log(Icons.Length + "\n");

		SetupAbility(Passive.New());
		SetupAbility(Passive.New());
		//SetupAbility(Passive.New());
		//SetupAbility(Passive.New());
		SetupResourceSystem();

		HealthSlider = UIManager.Instance.player_HP;
		HealthText = UIManager.Instance.player_HPText;
		ResourceSlider = UIManager.Instance.player_Resource;
		ResourceText = UIManager.Instance.player_ResourceText;

		SetupHealthUI();
		SetupResourceUI();
		SetNameUI();

		DamageImage = UIManager.Instance.damage_Indicator;

		base.Start();

		gameObject.tag = "Player";
	}

	public new void Update()
	{
		GetInput();
		#region Handle Damage
		if (Damaged)
		{
			DamageImage.color = flashColor;
		}
		else
		{
			DamageImage.color = Color.Lerp(DamageImage.color, Color.clear, flashSpeed * Time.deltaTime);
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
							startCooldownAmt = weapons[i].Cooldown;
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
		SelectorUI.rectTransform.position = new Vector3((1 + WeaponIndex)* 67 - 32, 35);
		int index = SelectorUI.transform.GetSiblingIndex();
		SelectorUI.transform.SetSiblingIndex(index + 1);
		#endregion

		TargetScan();
		HandleTarget();

		Damaged = false;

		base.Update();
	}

	void GetInput()
	{
		#region Mouse Buttons 1 & 2
		if (Input.GetButton("Fire1"))
		{
			//Debug.Log("Firing\n");
		}
		if (Input.GetButton("Fire1"))
		{
			if (weapons.Count > 0)
			{
				weapons[weaponIndex].UseWeapon(1, true);
			}
		}
		if (Input.GetButtonUp("Fire1"))
		{
			//Debug.Log("Fire Ceased\n");
		}

		if (Input.GetButton("Fire2"))
		{
			//Debug.Log("Firing\n");
		}
		if (Input.GetButton("Fire2"))
		{
			if (weapons.Count > 0)
			{
				weapons[weaponIndex].UseWeapon(5, false);
			}
		}
		if (Input.GetButtonUp("Fire2"))
		{
			//Debug.Log("Fire Ceased\n");
		}
		#endregion

		#region Testing Zone
		if (Input.GetKeyDown(KeyCode.T))
		{
			CharacterMotor charMotor = gameObject.GetComponent<CharacterMotor>();
			if (charMotor.jumping.jumping)
			{
				//Debug.Log(charMotor.movement.velocity.y + "\n");
				charMotor.movement.velocity = new Vector3(charMotor.movement.velocity.x, 20, charMotor.movement.velocity.z);
			}
		}

		if (Input.GetKeyDown(KeyCode.LeftShift))
		{
			CharacterMotor charMotor = gameObject.GetComponent<CharacterMotor>();
			if (charMotor.jumping.jumping)
			{
				//Debug.Log(charMotor.movement.velocity.y + "\n");
				charMotor.movement.velocity = new Vector3(charMotor.movement.velocity.x * 20.0f, 75, charMotor.movement.velocity.z * 20.0f);

				charMotor.movement.velocity.Normalize();
				charMotor.movement.velocity *= 120;
			}
		}
		#endregion

		#region Health & Resources
		if (Input.GetKeyDown(KeyCode.R))
		{
			TakeDamage(1);
		}

		if (Input.GetKeyDown(KeyCode.Y))
		{
			AdjustResource(3);
		}
		if (Input.GetKeyDown(KeyCode.H))
		{
			AdjustResource(-3);
		}
		#endregion

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
		#endregion

		#region Quit Section
		if (Input.GetButton("Quit"))
		{
			AppHelper.Quit();
		}
		#endregion
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

	public void SetupAbility(Ability ToAdd)
	{
		if(ToAdd is Weapon)
		{
			Weapon w = (Weapon)ToAdd;
			//WeaponUI

			Image panel = ((GameObject)GameObject.Instantiate(iconPrefab)).GetComponent<Image>();

			panel.name = "I: " + w.AbilityName;

			panel.sprite = (Sprite)Icons[Random.Range(1, Icons.Length)];
			panel.rectTransform.SetParent(WeaponUI.transform);
			w.Remainder = panel.transform.FindChild("Remainder").GetComponent<Text>();
			w.Cooldown = Random.Range(.01f, .7f);
			w.SpecialCooldown = Random.Range(4, 16);
			w.Remainder.text = w.Durability.ToString();

			w.IconUI = panel;
			weapons.Add(w);
			panel.rectTransform.anchoredPosition = new Vector2((weapons.Count - 1) * 67, 0);
		}
		else if( ToAdd is Passive)
		{
			
			Passive p = (Passive)ToAdd;
			
			Image panel = ((GameObject)GameObject.Instantiate(iconPrefab)).GetComponent<Image>();
			
			panel.name = "I: " + p.AbilityName;

			panel.rectTransform.anchorMin = new Vector2(1, 1);
			panel.rectTransform.anchorMax = new Vector2(1, 1);

			panel.sprite = (Sprite)Icons[Random.Range(1, Icons.Length)];
			panel.color = new Color(0, .8f, 0); 
			panel.rectTransform.SetParent(PassiveUI.transform);
			p.Remainder = panel.transform.FindChild("Remainder").GetComponent<Text>();
			p.Remainder.text = ((int)(p.DurationRemaining * 10)).ToString();

			p.IconUI = panel;
			passives.Add(p);
			
			panel.rectTransform.anchoredPosition = new Vector2((passives.Count) * -67, 0);
		}
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

	void TargetScan()
	{
		Ray ray = mainCamera.ScreenPointToRay(new Vector3(Screen.width / 2, Screen.height / 2));
		RaycastHit hit;
		//Debug.DrawLine(transform.position, (transform.position + ray) * 100, Color.green);

		if (Physics.Raycast(ray, out hit))
		{
			if(hit.collider.gameObject.tag == "Entity")
			{
				Entity e = hit.collider.gameObject.GetComponent<Entity>();
				CheckNewTarget((Entity)e);
			}
			if (hit.collider.gameObject.tag == "Island")
			{
				Island e = hit.collider.gameObject.GetComponent<Island>();
				CheckNewTarget((Entity)e);
			}
			if (hit.collider.gameObject.tag == "Enemy")
			{
				Enemy e = hit.collider.gameObject.GetComponent<Enemy>();
				CheckNewTarget((Entity)e);
				
				if (Input.GetMouseButtonDown(0))
				{
					e.TakeDamage(1);
				}
				if (Input.GetMouseButtonDown(1))
				{
					e.TakeDamage(5);
				}
			}
			//Debug.Log(hit.collider.gameObject.name + "\n");
		}
	}

	void CheckNewTarget(Entity newTarget)
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
		counter = Player.targetFade;
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
}
