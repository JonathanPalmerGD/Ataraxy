using UnityEngine;
using UnityEngine.UI;
using System.Collections;
using System.Collections.Generic;

/// <summary>
/// This class stores references to key pieces of UI so that they do not need to be looked up multiple times.
/// </summary>
public class UIManager : Singleton<UIManager> 
{
	public Canvas target_HUD;
	public Text target_Name;
	public Slider target_HP;
	public Slider target_XP;
	public Text target_LevelText;
	public Slider target_Resource;
	public Image damage_Indicator;

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

	public Sprite[] Icons;

	void Awake()
	{
		target_HUD = GameObject.Find("Target_HUD").GetComponent<Canvas>();
		target_Name = GameObject.Find("Target_Name").GetComponent<Text>();
		target_HP = GameObject.Find("Target_HP").GetComponent<Slider>();
		target_XP = GameObject.Find("Target_XP").GetComponent<Slider>();
		target_LevelText = GameObject.Find("Target_LevelText").GetComponent<Text>();
		//target_Resource = GameObject.Find("Target_Resource").GetComponent<Slider>();
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
		Icons = new Sprite[1];
		Icons = Resources.LoadAll<Sprite>("Atlases/AtaraxyIconAtlas");	
	}

	void Start()
	{
	}
	
	void Update()
	{
	
	}
}
