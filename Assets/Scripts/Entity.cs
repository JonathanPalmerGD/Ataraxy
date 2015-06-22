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

	private float healthToAdjust;
	public float HealthToAdjust
	{
		get { return healthToAdjust; }
		set { healthToAdjust = value; }
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

	private float xPToGain;
	public float XPToGain
	{
		get { return xPToGain; }
		set { xPToGain = value; }
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
	private Canvas modHUD;
	public Canvas ModHUD
	{
		get { return modHUD; }
		set { modHUD = value; }
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

	#region Entity Combat variables
	public float DamageAmplification = 1.0f;
	public float ProjSpeedAmp = 1.0f;
	public float LifeStealPer = 0.0f;
	public float LuckFactor = 10.0f;
	public float DamageMultiplier = 1.0f;
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

	public GameObject[] FirePoints;

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
		UpdateHealth();
		UpdateHealthUI();
		UpdateXP();
		UpdateXPUI();
	}

	/// <summary>
	/// Animated Healthbars Handling.
	/// Converts HealthToAdjust into actual health/damage.
	/// </summary>
	public virtual void UpdateHealth()
	{
		//If our displayed health value isn't correct
		if (HealthToAdjust != 0)
		{
			float rate = Time.deltaTime * 8;
			float gainThisFrame = 0;
			if (HealthToAdjust < 0)
			{
				if (Health + HealthToAdjust < 0)
				{
					rate *= 10;
				}

				//If the health left to adjust is LESS than the rate, just lose the remaining debt.
				gainThisFrame = HealthToAdjust > -rate ? HealthToAdjust : -rate;
			}
			else
			{
				if (Health + HealthToAdjust >= MaxHealth)
				{
					rate *= 4;
				}

				//If the health left to adjust is greater than the rate, use the rate.
				gainThisFrame = HealthToAdjust > rate ? rate : HealthToAdjust;
			}

			//Note this damage as taken, pay off our debt
			HealthToAdjust -= gainThisFrame;

			//Add it to displayed HP
			Health += gainThisFrame;

			if (Health <= 0 && !isDead)
			{
				KillEntity();
			}
		}
	}

	/// <summary>
	/// Animated XPBar Handling.
	/// Converts XPToGain into actual XP/Leveling.
	/// </summary>
	public virtual void UpdateXP()
	{
		if (XPToGain > 0)
		{
			//If it is enough to level, increase our rate
			float rate = Time.deltaTime * 8;
			if (XP + XPToGain > XPNeeded)
			{
				rate *= 4 * (1 + (XP + XPToGain) / XPNeeded);
			}
			//Debug.Log("\t" + name + "\tRate " + rate + "\t\tTotalXP: " + (XP + XPToGain) + "\n\t\t\t" + (XP + XPToGain) / XPNeeded);

			//Drain our XP from the pool. Don't overdraw.
			float gainThisFrame = XPToGain > rate ? rate : XPToGain;

			//Subtract it from the pool
			XPToGain -= gainThisFrame;

			//Add it to the real XP
			XP += gainThisFrame;

			if (XP > XPNeeded)
			{
				GainLevel();
			}
		}
	}

	public virtual void AdjustHealth(float amount)
	{
		//Debug.Log(name + " is hurt\t" + amount + "\n");
		if (amount < 0)
		{
			damaged = true;
		}
		if (Health + amount >= MaxHealth)
		{
			HealthToAdjust += MaxHealth - Health;
		}
		else
		{
			if (amount < 0)
			{
				HealthToAdjust += amount * DamageMultiplier;
			}
			else
			{
				HealthToAdjust += amount;
			}
		}
	}

	public void GainExperience(float xpValue, int xpMultiplier = 1)
	{
		if (xpValue < 0)
		{
			Debug.LogError("Negative Experience value (" + xpValue + ")  provided to : " + gameObject.name + "\n");
		}
		if (Level < GameManager.Instance.player.Level + 4)
		{
			XPToGain += xpValue * (1 + xpMultiplier / 10);
		}
	}

	public virtual Vector3 GetForward()
	{
		return transform.forward;
	}

	public virtual void ExternalMove(Vector3 direction, float force, ForceMode fMode = ForceMode.Force)
	{
		if (GetComponent<Rigidbody>() != null)
		{
			//Debug.DrawLine(transform.position, transform.position + direction * force * 10, Color.gray, 2f);
			GetComponent<Rigidbody>().AddForce(direction * force, fMode);
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

	public virtual void NearbyEntityDied(Entity deceased)
	{
		if (deceased.gameObject.tag == "Enemy")
		{
			//Debug.Log(name + " heard of " + deceased.name + "'s death\n");
			
			Enemy e = (Enemy)deceased;

			//Gain experience
			Debug.Log("Reward: " + e.XpReward + "\nMentor Modifier: " + e.MentorModifier + " Needed: " + XPNeeded);
			GainExperience(e.XpReward * e.MentorModifier);

			//Get alerted of the perceived player's location
			//Debug.Log("Dying entity had: " + e.modifiers.Count + " modifiers\n");

			if (e.modifiers.Count > 0)
			{
				Enemy self = (Enemy)this;

				Modifier transferMod = e.modifiers[Random.Range(0, e.modifiers.Count)];
				self.GainModifier(transferMod, true);
				//self.modifiers.Add(e.modifiers[Random.Range(0, e.modifiers.Count)]);

				SetupModifiersUI();
			}

			//Gain relevant death effects
			//For each e.DeathEffect
				//Do something
		}
	}

	public virtual void GainLevel()
	{
		XP -= XPNeeded;
		Level++;

		UpdateLevelUI();
	}

	public virtual void UpdateHealthUI()
	{
		if (HealthSlider != null)
		{
			HealthSlider.maxValue = MaxHealth;
			HealthSlider.value = Health;
		}
		if (HealthText != null)
		{
			HealthText.text = ((int)Health).ToString();
		}
	}

	public virtual void UpdateXPUI()
	{
		if (XPSlider != null)
		{
			XPSlider.value = XP;
			XPSlider.maxValue = XPNeeded;
		}
		if (XPText != null)
		{
			XPText.text = XP.ToString();
		}
	}

	public virtual void UpdateLevelUI()
	{
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

	public virtual void SetupNameUI()
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

	public virtual void SetupModifiersUI()
	{

	}
	#endregion
}
