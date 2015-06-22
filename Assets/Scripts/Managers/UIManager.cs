using UnityEngine;
using UnityEngine.UI;
using System.Collections;
using System.Collections.Generic;

/// <summary>
/// This class stores references to key pieces of UI so that they do not need to be looked up multiple times.
/// </summary>
public class UIManager : Singleton<UIManager>
{
	public GameObject ScreenSpaceOverlayPrefab;
	public GameObject EnemyModifierPrefab;

	public Canvas UIROOT;

	CursorLockMode wantedMode;

	#region Target HUD elements
	public Canvas target_HUD;
	public Canvas target_Mod;
	public Canvas target_Info;
	public Text target_Name;
	public Slider target_HP;
	public Slider target_XP;
	public Text target_LevelText;
	public Slider target_Resource;
	public Image damage_Indicator;
	#endregion

	#region Player HUD elements
	public Canvas player_HUD;
	public Text player_Name;
	public Text player_HPText;
	public Slider player_HP;
	public Slider player_XP;
	public Text player_XPText;
	public Text player_LevelText;
	public Text player_ResourceText;
	public Text player_WeaponText;
	public Slider player_Resource;
	public Image player_Selector;
	public Image player_Crosshair;
	public GameObject player_WeaponFolder;
	public GameObject player_PassiveFolder;
	#endregion

	#region Target Modifiers
	public Canvas ModifierRoot;
	public GameObject ModifierTopRoot;
	public GameObject ModifierMiddleRoot;

	public List<ModifierUI> Modifiers;
	#endregion

	#region Pause Menu
	public Canvas pause_Menu;
	public GameObject modifier_Menu;
	public Text item_NameText;
	public Text item_PrimaryText;
	public Text item_SecondaryText;
	#endregion

	public Sprite[] Icons;

	public bool paused;

	public override void Awake()
	{
		base.Awake();
		/*
		//Load the UI element

		#region Root Setup
		if (ScreenSpaceOverlayPrefab == null)
		{
			ScreenSpaceOverlayPrefab = Resources.Load<GameObject>("UI/SS - Overlay");
		}
		GameObject go = GameObject.Find("SS - Overlay");
		if(go != null)
		{
			UIROOT = go.GetComponent<Canvas>();
		}
		if (UIROOT == null)
		{
			UIROOT = ((GameObject)GameObject.Instantiate(ScreenSpaceOverlayPrefab, Vector3.zero, Quaternion.identity)).GetComponent<Canvas>();
			UIROOT.gameObject.name = ScreenSpaceOverlayPrefab.name;
		}
		#endregion

		pause_Menu = GameObject.Find("Pause Menu").GetComponent<Canvas>();
		modifier_Menu = GameObject.Find("Modifier Help");

		#region Target UI
		target_HUD = GameObject.Find("Target_HUD").GetComponent<Canvas>();
		target_HUD.gameObject.SetActive(false);
		target_HUD.gameObject.SetActive(true);
		target_Name = GameObject.Find("Target_Name").GetComponent<Text>();
		target_HP = GameObject.Find("Target_HP").GetComponent<Slider>();
		target_XP = GameObject.Find("Target_XP").GetComponent<Slider>();
		target_LevelText = GameObject.Find("Target_LevelText").GetComponent<Text>();
		//target_Resource = GameObject.Find("Target_Resource").GetComponent<Slider>();
		#endregion

		#region Enemy Modifiers
		ModifierRoot = GameObject.Find("Target_Modifiers").GetComponent<Canvas>();
		ModifierTopRoot = ModifierRoot.transform.FindChild("Top").gameObject;
		ModifierMiddleRoot = ModifierRoot.transform.FindChild("Middle").gameObject;

		if (EnemyModifierPrefab == null)
		{
			EnemyModifierPrefab = Resources.Load<GameObject>("UI/Modifier_Root");
		}

		if (Modifiers == null)
		{
			Modifiers = new List<ModifierUI>();

			ModifierUI newModifier;
			for (int i = 0; i < 10; i++)
			{
				newModifier = ((GameObject)GameObject.Instantiate(EnemyModifierPrefab)).GetComponent<ModifierUI>();
				newModifier.gameObject.name = "Modifier [" + i + "]";
				newModifier.transform.SetParent(ModifierMiddleRoot.transform);
				newModifier.rootBackground.rectTransform.offsetMin = new Vector2(5, 0);
				newModifier.rootBackground.rectTransform.offsetMax = new Vector2(1, 1);
				newModifier.rootBackground.rectTransform.sizeDelta = new Vector2(0, 25);
				newModifier.rootBackground.rectTransform.anchoredPosition = new Vector2(0, -2.5f - i * 25);
				newModifier.gameObject.SetActive(false);
				Modifiers.Add(newModifier);
			}
			//Debug.Log("Finished creating modifiers: " + Modifiers.Count + "\n");
		}
		#endregion

		target_HUD.gameObject.SetActive(false);

		#region Player UI
		damage_Indicator = GameObject.Find("Damage_Indicator").GetComponent<Image>();

		//player_HPText = GameObject.Find("player_HPText").GetComponent<Text>();
		player_HP = GameObject.Find("Player_HP").GetComponent<Slider>();
		//player_XPText = GameObject.Find("player_XPText").GetComponent<Text>();
		player_LevelText = GameObject.Find("Player_LevelText").GetComponent<Text>();
		player_XP = GameObject.Find("Player_XP").GetComponent<Slider>();
		//player_ResourceText = GameObject.Find("player_ResourceText").GetComponent<Text>();
		//player_Resource = GameObject.Find("player_Resource").GetComponent<Slider>();
		player_Name = GameObject.Find("Player_Name").GetComponent<Text>();
		player_WeaponText = GameObject.Find("WeaponText").GetComponent<Text>();
		player_Selector = GameObject.Find("Selector").GetComponent<Image>();
		player_Crosshair = GameObject.Find("Crosshair").GetComponent<Image>();
		player_WeaponFolder = GameObject.Find("Weapons");
		player_PassiveFolder = GameObject.Find("Passives");
		#endregion

		#region Items
		item_NameText = GameObject.Find("Item Name Text").GetComponent<Text>();
		item_PrimaryText = GameObject.Find("Item Primary Text").GetComponent<Text>();
		item_SecondaryText = GameObject.Find("Item Secondary Text").GetComponent<Text>();

		Icons = new Sprite[1];
		Icons = Resources.LoadAll<Sprite>("Atlases/AtaraxyIconAtlas");
		#endregion
		*/
	}

	public void Init()
	{
		//Load the UI element

		#region Root Setup
		if (ScreenSpaceOverlayPrefab == null)
		{
			ScreenSpaceOverlayPrefab = Resources.Load<GameObject>("UI/SS - Overlay");
		}
		GameObject go = GameObject.Find("SS - Overlay");
		if (go != null)
		{
			UIROOT = go.GetComponent<Canvas>();
		}
		if (UIROOT == null)
		{
			UIROOT = ((GameObject)GameObject.Instantiate(ScreenSpaceOverlayPrefab, Vector3.zero, Quaternion.identity)).GetComponent<Canvas>();
			UIROOT.gameObject.name = ScreenSpaceOverlayPrefab.name;
		}
		#endregion

		pause_Menu = GameObject.Find("Pause Menu").GetComponent<Canvas>();
		modifier_Menu = GameObject.Find("Modifier Help");

		#region Target UI
		target_HUD = GameObject.Find("Target_HUD").GetComponent<Canvas>();
		target_Mod = GameObject.Find("Target_Modifiers").GetComponent<Canvas>();
		target_Info = GameObject.Find("Target_Info").GetComponent<Canvas>();
		target_Name = GameObject.Find("Target_Name").GetComponent<Text>();
		target_HP = GameObject.Find("Target_HP").GetComponent<Slider>();
		target_XP = GameObject.Find("Target_XP").GetComponent<Slider>();
		target_LevelText = GameObject.Find("Target_LevelText").GetComponent<Text>();
		//target_Resource = GameObject.Find("Target_Resource").GetComponent<Slider>();
		#endregion

		#region Enemy Modifiers
		ModifierRoot = GameObject.Find("Target_Modifiers").GetComponent<Canvas>();
		ModifierTopRoot = ModifierRoot.transform.FindChild("Top").gameObject;
		ModifierMiddleRoot = ModifierRoot.transform.FindChild("Middle").gameObject;

		if (EnemyModifierPrefab == null)
		{
			EnemyModifierPrefab = Resources.Load<GameObject>("UI/Modifier_Root");
		}

		//if (Modifiers == null || (Modifiers.Count == 0 && Modifiers[0] == null))
		//{
		Modifiers = new List<ModifierUI>();

		ModifierUI newModifier;
		for (int i = 0; i < 10; i++)
		{
			newModifier = ((GameObject)GameObject.Instantiate(EnemyModifierPrefab)).GetComponent<ModifierUI>();
			newModifier.gameObject.name = "Modifier [" + i + "]";
			newModifier.transform.SetParent(ModifierMiddleRoot.transform);
			newModifier.rootBackground.rectTransform.offsetMin = new Vector2(5, 0);
			newModifier.rootBackground.rectTransform.offsetMax = new Vector2(1, 1);
			newModifier.rootBackground.rectTransform.sizeDelta = new Vector2(0, 25);
			newModifier.rootBackground.rectTransform.localScale = Vector3.one;
			newModifier.rootBackground.rectTransform.anchoredPosition = new Vector2(0, -2.5f - i * 25);
			newModifier.gameObject.SetActive(false);
			Modifiers.Add(newModifier);
		}
			//Debug.Log("Finished creating modifiers: " + Modifiers.Count + "\n");
		//}
		#endregion

		Untarget();

		#region Player UI
		damage_Indicator = GameObject.Find("Damage_Indicator").GetComponent<Image>();

		//player_HPText = GameObject.Find("player_HPText").GetComponent<Text>();
		player_HP = GameObject.Find("Player_HP").GetComponent<Slider>();
		//player_XPText = GameObject.Find("player_XPText").GetComponent<Text>();
		player_LevelText = GameObject.Find("Player_LevelText").GetComponent<Text>();
		player_XP = GameObject.Find("Player_XP").GetComponent<Slider>();
		//player_ResourceText = GameObject.Find("player_ResourceText").GetComponent<Text>();
		//player_Resource = GameObject.Find("player_Resource").GetComponent<Slider>();
		player_Name = GameObject.Find("Player_Name").GetComponent<Text>();
		player_WeaponText = GameObject.Find("WeaponText").GetComponent<Text>();
		player_Selector = GameObject.Find("Selector").GetComponent<Image>();
		player_Crosshair = GameObject.Find("Crosshair").GetComponent<Image>();
		player_WeaponFolder = GameObject.Find("Weapons");
		player_PassiveFolder = GameObject.Find("Passives");
		#endregion

		#region Items
		item_NameText = GameObject.Find("Item Name Text").GetComponent<Text>();
		item_PrimaryText = GameObject.Find("Item Primary Text").GetComponent<Text>();
		item_SecondaryText = GameObject.Find("Item Secondary Text").GetComponent<Text>();

		Icons = new Sprite[1];
		Icons = Resources.LoadAll<Sprite>("Atlases/AtaraxyIconAtlas");
		#endregion

		UnpauseGame();
	}

	void Start()
	{
		if (pause_Menu != null)
		{
			pause_Menu.gameObject.SetActive(false);
			UnpauseGame();
		}
	}

	bool wasFullScreen;
	void Update()
	{
		SetCursorState();
		if (!paused)
		{
#if !UNITY_EDITOR
			
#endif
		}
		#region Quit Section
		if (Input.GetButtonDown("Quit"))
		{
			if (paused)
			{
				UnpauseGame();
			}
			else
			{
				PauseGame();
			}
		}
		#endregion

		if (Screen.fullScreen)
		{
			wasFullScreen = true;
		}
		else
		{
			if (wasFullScreen && !UIManager.Instance.paused)
			{
				PauseGame();
			}
			wasFullScreen = false;
		}

		if (pause_Menu != null)
		{
			//Set the item name text
			//Set the primary fire description
			
			
			/*if (Screen.lockCursor && !paused)
			{
				PauseGame();
			}
			if (paused)
			{

			}*/
		}
	}

	public void PauseGame()
	{
		//AudioListener.pause = true;
		paused = true;
		Time.timeScale = 0f;
		if (pause_Menu != null)
		{
			pause_Menu.gameObject.SetActive(paused);
			//Bring in the elements for the pause menu
			//Unlock the mouse

			wantedMode = CursorLockMode.Confined;
		}
	}

	public void UnpauseGame()
	{
		//AudioListener.pause = false;
		paused = false;
		Time.timeScale = 1.0f;
		if (pause_Menu != null)
		{
			pause_Menu.gameObject.SetActive(paused);
		}

		wantedMode = CursorLockMode.Locked;
#if !UNITY_EDITOR
#endif
	}

	public void Untarget()
	{
		target_HUD.enabled = false;
		target_Mod.enabled = false;
		target_Info.enabled = false;
	}
	public void Target()
	{
		target_HUD.enabled = true;
		target_Mod.enabled = true;
		target_Info.enabled = true;
	}

	void SetCursorState()
	{
		Cursor.lockState = wantedMode;
		Cursor.visible = (CursorLockMode.Locked != wantedMode);
	}

	public void SetupModifier(Modifier m)
	{
		
	}

	public void ConfigureModifiers(List<Modifier> npcMods)
	{
		for (int i = 0; i < Modifiers.Count; i++)
		{
			if (i < npcMods.Count)
			{
				Modifiers[i].multiplierText.text = npcMods[i].Stacks + "x";
				Modifiers[i].nameText.text = npcMods[i].ModifierName;
				Modifiers[i].gameObject.SetActive(true);
				Modifiers[i].SetPlateColor(npcMods[i].UIColor, npcMods[i].TextColor);
			}
			else
			{
				Modifiers[i].gameObject.SetActive(false);
			}
		}
	}
}
