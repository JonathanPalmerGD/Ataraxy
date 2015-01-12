using UnityEngine.UI;
using UnityEngine;
using UnityEditor;
using System.Collections.Generic;
using System.Collections;

public class Player : Entity
{
	public List<Weapon> weapons;
	public List<Passive> passives;

	public Sprite thing;
	public Image SelectorUI;
	public GameObject WeaponUI;
	public GameObject PassiveUI;
	public GameObject iconPrefab;

	public Object[] Icons;

	public float flashSpeed = 5f;
	public Color flashColor = new Color(1f, 0f, 0f, 0.15f);

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

	void Start()
	{
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
		SetupAbility(Weapon.New());

		//Debug.Log(AssetDatabase.GetAssetPath(thing) +"\n");
		//Debug.Log(Icons.Length + "\n");
		
		SetupAbility(Passive.New());
		SetupResourceSystem();

		base.Start();
	}

	void Update()
	{
		GetInput();
		#region Handle Damage
		if (Damaged)
		{
			damageImage.color = flashColor;
		}
		else
		{
			damageImage.color = Color.Lerp(damageImage.color, Color.clear, flashSpeed * Time.deltaTime);
		}
		#endregion
		#region Resource System
		ManageResourceSystem();
		#endregion
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
						SelectorUI.fillAmount = 1 - (weapons[i].CdLeft / weapons[i].Cooldown);
					}
					else
					{
						SelectorUI.fillAmount = 1;
					}
				}
				weapons[i].UpdateWeapon(Time.deltaTime);
				if (weapons[i].CheckAbility())
				{
					weapons.RemoveAt(i);
					i--;
				}
			}
		}

		if (weaponIndex > weapons.Count - 1)
		{
			weaponIndex = weapons.Count - 1;
		}
		#endregion
		#region Handle Selector Location
		SelectorUI.rectTransform.position = new Vector3((1 + WeaponIndex)* 67 - 32, 35);
		int index = SelectorUI.transform.GetSiblingIndex();
		SelectorUI.transform.SetSiblingIndex(index + 1);
		#endregion

		Damaged = false;
	}

	void GetInput()
	{
		#region Mouse Buttons 1 & 2
		if (Input.GetButton("Fire1"))
		{
			//Debug.Log("Firing\n");
		}
		if (Input.GetButtonDown("Fire1"))
		{
			weapons[weaponIndex].UseWeapon(1);
		}
		if (Input.GetButtonUp("Fire1"))
		{
			//Debug.Log("Fire Ceased\n");
		}

		if (Input.GetButton("Fire2"))
		{
			//Debug.Log("Firing\n");
		}
		if (Input.GetButtonDown("Fire2"))
		{
			weapons[weaponIndex].UseWeapon(5);
		}
		if (Input.GetButtonUp("Fire2"))
		{
			//Debug.Log("Fire Ceased\n");
		}
		#endregion

		if (Input.GetKeyDown(KeyCode.T))
		{
			CharacterMotor charMotor = gameObject.GetComponent<CharacterMotor>();
			if (charMotor.jumping.jumping)
			{
				//Debug.Log(charMotor.movement.velocity.y + "\n");
				charMotor.movement.velocity = new Vector3(charMotor.movement.velocity.x, 20, charMotor.movement.velocity.z);
			}
		}

		#region Health & Resources
		if (Input.GetKeyDown(KeyCode.R))
		{
			TakeDamage(-1);
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
		if (Input.GetAxis("Mouse ScrollWheel") < 0)
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
		else if (Input.GetAxis("Mouse ScrollWheel") > 0)
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
	}

	void SetupAbility(Ability ToAdd)
	{
		if(ToAdd is Weapon)
		{
			Weapon w = (Weapon)ToAdd;
			//WeaponUI

			Image panel = ((GameObject)GameObject.Instantiate(iconPrefab)).GetComponent<Image>();

			panel.sprite = (Sprite)Icons[Random.Range(0, Icons.Length)];
			//w.Icon = (Sprite)Icons[Random.Range(0, 64)];
			panel.rectTransform.parent = WeaponUI.transform;
			
			weapons.Add(w);
			panel.rectTransform.position = new Vector3(((weapons.Count - 1) * 67) + 3, 35.5f);
		
		}
		else if( ToAdd is Passive)
		{
			passives.Add((Passive)ToAdd);

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
