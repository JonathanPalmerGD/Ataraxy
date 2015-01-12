using UnityEngine;
using UnityEngine.UI;
using System.Collections;

public class Entity : MonoBehaviour 
{
	private string name;
	public string Name
	{
		get { return name; }
		set { name = value; }
	}
	private Sprite icon;
	public Sprite Icon
	{
		get { return icon; }
		set { icon = value; }
	}
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

	public Slider healthSlider;
	public Slider resourceSlider;
	public Text healthText;
	public Text resourceText;
	public Image damageImage;
	public AudioClip deathClip;

	public void TakeDamage(int amount)
	{
		damaged = true;

		Health += amount;

		healthSlider.value = Health;
		healthText.text = Health.ToString();

		if (Health <= 0 && !isDead)
		{
			KillEntity();
		}
	}

	public void RestoreHealth(int amount)
	{
		if (Health + amount >= MaxHealth)
		{
			Health = MaxHealth;
		}
		else
		{
			Health += amount;
		}

		healthSlider.value = Health;
		healthText.text = Health.ToString();
	}

	public bool AdjustResource(int amount)
	{
		if (Resource + amount <= MaxResource && Resource + amount >= 0)
		{
			Resource += amount;

			resourceSlider.value = Resource;
			resourceText.text = Resource.ToString();

			return true;
		}
		return false;
	}

	public void KillEntity()
	{
		IsDead = true;
	}

	public void Start()
	{
		healthSlider.maxValue = MaxHealth;
		healthSlider.value = Health;
		healthText.text = Health.ToString();
		resourceSlider.maxValue = MaxResource;
		resourceSlider.value = Resource;
		resourceText.text = Resource.ToString();
	}
	
	void Update()
	{
	
	}
}
