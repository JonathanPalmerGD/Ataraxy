using UnityEngine;
using UnityEngine.UI;
using System.Collections;
using System.Collections.Generic;

public class Entity : MonoBehaviour
{
	#region Key Elements - Name, Icon, Faction
	private string nameInGame;
	public string NameInGame
	{
		get { return nameInGame; }
		set { nameInGame = value; }
	}

	public Sprite icon;
	/*
	private Sprite icon;
	public Sprite Icon
	{
		get { return icon; }
		set { icon = value; }
	}*/

	private Allegiance faction;
	public virtual Allegiance Faction
	{
		get { return faction; }
		set { faction = value; }
	}
	#endregion

	#region Health, Resource, Experience, and Level
	private float health = 8;
	public float Health
	{
		get { return health; }
		set { health = value; }
	}

	private float maxHealth = 8;
	public float MaxHealth
	{
		get { return maxHealth; }
		set { maxHealth = value; }
	}

	private float xP;
	public float XP
	{
		get { return xP; }
		set { xP = value; }
	}
	private float xPNeeded = 100;
	public float XPNeeded
	{
		get { return xPNeeded; }
		set { xPNeeded = value; }
	}

	private int level = 1;
	public int Level
	{
		get { return level; }
		set { level = value; }
	}

	private float resource = 30;
	public float Resource
	{
		get { return resource; }
		set { resource = value; }
	}

	private float maxResource = 30;
	public float MaxResource
	{
		get { return maxResource; }
		set { maxResource = value; }
	}
	#endregion

	#region Interface Elements
	private Canvas infoHUD;
	public Canvas InfoHUD
	{
		get { return infoHUD; }
		set { infoHUD = value; }
	}
	private Slider healthSlider;
	public Slider HealthSlider
	{
		get { return healthSlider; }
		set { healthSlider = value; }
	}
	private Slider resourceSlider;
	public Slider ResourceSlider
	{
		get { return resourceSlider; }
		set { resourceSlider = value; }
	}
	private Slider xPSlider;
	public Slider XPSlider
	{
		get { return xPSlider; }
		set { xPSlider = value; }
	}
	private Text healthText;
	public Text HealthText
	{
		get { return healthText; }
		set { healthText = value; }
	}
	private Text resourceText;
	public Text ResourceText
	{
		get { return resourceText; }
		set { resourceText = value; }
	}
	private Text xPText;
	public Text XPText
	{
		get { return xPText; }
		set { xPText = value; }
	}
	private Text levelText;
	public Text LevelText
	{
		get { return levelText; }
		set { levelText = value; }
	}
	private Text nameText;
	public Text NameText
	{
		get { return nameText; }
		set { nameText = value; }
	}
	private Image damageImage;
	public Image DamageImage
	{
		get { return damageImage; }
		set { damageImage = value; }
	}
	#endregion

	#region State Tracking, definition bools, abilityEffects
	public List<AbilityEffect> abilityEffects;

	private bool isDead = false;
	public bool IsDead
	{
		get { return isDead; }
		set { isDead = value; }
	}

	private bool damaged = false;
	public bool Damaged
	{
		get { return damaged; }
		set { damaged = value; }
	}
	#endregion

	#region Assets - Audio, Animation, and Textures
	private AudioClip deathClip;
	public AudioClip DeathClip
	{
		get { return deathClip; }
		set { deathClip = value; }
	}
	#endregion

	public bool ApplyAbilityEffect(AbilityEffect newEffect)
	{
		if (abilityEffects == null)
		{
			abilityEffects = new List<AbilityEffect>();
		}

		abilityEffects.Add(newEffect);

		return true;
	}

	public virtual void KillEntity()
	{
		IsDead = true;
	}

	public virtual void Start()
	{
		gameObject.tag = "Entity";
	}
	
	public virtual void Update()
	{
	
	}

	public virtual void AdjustHealth(float amount)
	{
		//Debug.Log("I AM HURT " + gameObject.name + " " + amount);
		if (amount < 0)
		{
			damaged = true;
		}

		if (Health + amount >= MaxHealth)
		{
			Health = MaxHealth;
		}
		else
		{
			Health += amount;
		}

		if (HealthSlider != null)
		{
			HealthSlider.value = Health;
		}
		if (HealthText != null)
		{
			HealthText.text = ((int)Health).ToString();
		}

		if (Health <= 0 && !isDead)
		{
			KillEntity();
		}
	}

	public bool AdjustResource(float amount)
	{
		if (Resource + amount <= MaxResource && Resource + amount >= 0)
		{
			Resource += amount;

			if (ResourceSlider != null)
			{
				ResourceSlider.value = Resource;
			}
			if (ResourceText != null)
			{
				ResourceText.text = Resource.ToString();
			}

			return true;
		}
		return false;
	}

	public void GainExperience(float xpValue, int level = 1)
	{
		if (xpValue < 0)
		{
			Debug.LogError("Negative Experience value (" + xpValue + ")  provided to : " + gameObject.name + "\n");
		}
		XP += xpValue * level;

		if (XPSlider != null)
		{
			XPSlider.value = XP;
		}
		if (XPText != null)
		{
			XPText.text = XP.ToString();
		}

		if (XP > XPNeeded)
		{
			GainLevel();
		}
	}

	public virtual void GainLevel()
	{
		XP -= XPNeeded;
		Level++;

		MaxHealth += 5;
		AdjustHealth(MaxHealth);

		if (LevelText != null)
		{
			LevelText.text = Level.ToString();
		}
	}

	#region UI Setup - Health, Name, Resource
	public void SetupHealthUI()
	{
		if (healthSlider != null)
		{
			healthSlider.maxValue = MaxHealth;
			healthSlider.value = Health;
		}
		if (healthText != null)
		{
			healthText.text = Health.ToString();
		}
	}

	public void SetupNameUI()
	{
		if (nameText != null)
		{
			nameText.text = gameObject.name;
		}
	}

	public void SetupXPUI()
	{
		if (XPSlider != null)
		{
			XPSlider.value = XP;
		}
		if (XPText != null)
		{
			XPText.text = XP.ToString();
		}
		if (LevelText != null)
		{
			LevelText.text = Level.ToString();
		}
	}

	public void SetupResourceUI()
	{
		if (resourceSlider != null)
		{
			resourceSlider.maxValue = MaxResource;
			resourceSlider.value = Resource;
		}
		if (resourceText != null)
		{
			resourceText.text = Resource.ToString();
		}
	}
	#endregion
}
