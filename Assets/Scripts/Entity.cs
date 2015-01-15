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
		get { return Allegiance.Neutral; }
	}
	#endregion

	#region Health, Resource, Experience, and Level
	private int health = 8;
	public int Health
	{
		get { return health; }
		set { health = value; }
	}

	private int maxHealth = 8;
	public int MaxHealth
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

	private int level = 1;
	public int Level
	{
		get { return level; }
		set { level = value; }
	}

	private int resource = 30;
	public int Resource
	{
		get { return resource; }
		set { resource = value; }
	}

	private int maxResource = 30;
	public int MaxResource
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

	public virtual void AdjustHealth(int amount)
	{
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

		if (healthSlider != null)
		{
			healthSlider.value = Health;
		}
		if (healthText != null)
		{
			healthText.text = Health.ToString();
		}

		if (Health <= 0 && !isDead)
		{
			KillEntity();
		}
	}

	public bool AdjustResource(int amount)
	{
		if (Resource + amount <= MaxResource && Resource + amount >= 0)
		{
			Resource += amount;

			if (resourceSlider != null)
			{
				resourceSlider.value = Resource;
			}
			if (resourceText != null)
			{
				resourceText.text = Resource.ToString();
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

	public void SetNameUI()
	{
		if (nameText != null)
		{
			nameText.text = gameObject.name;
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
