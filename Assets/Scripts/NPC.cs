using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class NPC : Entity 
{
	public List<Modifier> modifiers;

	[Header("Targeting Shader Info")]
	public Shader outline;
	public Shader outlineOnly;
	public Shader diffuse;
	public Shader normalShader;

	#region Experience Values
	/// <summary>
	/// The value this enemy provides when slain.
	/// </summary>
	public float XpReward = 5;
	public float xpRateOverTime = 1.5f;
	public bool gainXpWhenHit = false;
	public float xpGainWhenHit = .5f;
	public float expGainPerShot = 5;
	public float AlertRadius = 50;
	public float MentorModifier = 1.0f;
	public float FiringCooldown = 7;
	#endregion

	#region Specific Modifiers
	public string[] specificModifiers;
	#endregion

	public override Allegiance Faction
	{
		get { return Allegiance.Neutral; }
	}

	public override void Start()
	{
		outline = Shader.Find("Outlined/Silhouetted Diffuse");
		outlineOnly = Shader.Find("Outlined/Silhouette Only");
		diffuse = Shader.Find("Diffuse");
		normalShader = GetComponent<Renderer>().material.shader; 

		//Set up HUD with UIManager
		HealthSlider = UIManager.Instance.target_HP;
		XPSlider = UIManager.Instance.target_XP;
		NameText = UIManager.Instance.target_Name;
		LevelText = UIManager.Instance.target_LevelText;
		InfoHUD = UIManager.Instance.target_HUD;
		ModHUD = UIManager.Instance.ModifierRoot;

		Untarget();
		SetupXPUI();
		InitModifiers();
	}

	public override void Update() 
	{
		base.Update();
	}

	public override void UpdateHealthUI()
	{
		//Only update our UI if we're targeted.
		if (GameManager.Instance.player.targetedEntity == this)
		{
			base.UpdateHealthUI();
		}
	}

	public override void UpdateXPUI()
	{
		//Only update our UI if we're targeted.
		if (GameManager.Instance.player.targetedEntity == this)
		{
			base.UpdateXPUI();
		}
	}

	public override void UpdateLevelUI()
	{
		//Only update our UI if we're targeted.
		if (GameManager.Instance.player.targetedEntity == this)
		{
			//Debug.Log(name + " Updating Level UI\n");
			base.UpdateLevelUI();
		}
	}

	public virtual void Untarget()
	{
		if (InfoHUD != null)
		{
			if(GameManager.Instance.player.targetedEntity == this)
			{
				GameManager.Instance.player.targetedEntity = null;
				UIManager.Instance.Untarget();
			}
			GetComponent<Renderer>().material.shader = diffuse;
		}
	}

	public virtual void Target()
	{
		UIManager.Instance.Target();

		if (InfoHUD != null)
		{
			//InfoHUD.enabled = true;
			GetComponent<Renderer>().material.shader = outline;

			SetupHealthUI();
			SetupResourceUI();
			SetupNameUI();
			SetupXPUI();
			SetupModifiersUI();
			UpdateLevelUI();
		}
	}

	public virtual void InitModifiers()
	{
		//Debug.Log("Setting up Modifiers on: " + name + "\n");
		if (modifiers == null || modifiers.Count == 0)
		{
			modifiers = new List<Modifier>();
			Modifier m; 

			if (specificModifiers.Length > 0)
			{
				for (int i = 0; i < specificModifiers.Length; i++)
				{
					m = ModifierManager.Instance.GainNewModifier(Level, specificModifiers[i]);
					m.Init();
					GainModifier(m);
				}
			}
			else
			{
				m = ModifierManager.Instance.GainNewModifier(Level);
				m.Init();
				GainModifier(m);
				//Debug.Log(name + " has " + modifiers.Count + " modifiers\n");
			}
		}
		//Debug.Log("Finished Modifiers on: " + name + "\n" + modifiers.Count);
	}

	public override void SetupModifiersUI()
	{
		if (modifiers == null)
		{
			InitModifiers();
		}

		if (GameManager.Instance.player.targetedEntity == this)
		{
			if (ModHUD != null)
			{
				//Debug.Log("Configuring Modifiers with " + modifiers.Count + " entries\n");
				UIManager.Instance.ConfigureModifiers(modifiers);
			}

			base.SetupModifiersUI();
		}
	}

	public virtual void GainModifier(Modifier newModifier, bool fromSlainAlly = false)
	{
		newModifier.Carrier = this;

		//We don't get a perfect transfer. We can get more if we have high luck.
		if (fromSlainAlly)
		{
			int stacksWithLuck = (int)((newModifier.Stacks / 3) + (LuckFactor / 3));
			int lowerBound = (int)(LuckFactor / 3) > 1 ? (int)(LuckFactor / 3) : 1;
			int upperBound = Mathf.Max(1, stacksWithLuck);
			int stacksTransfered = Random.Range(lowerBound, upperBound);
			//Debug.Log("\t" + name + " gaining " + newModifier.ModifierName + " (" + stacksTransfered + ")\nPotential Upper BoundStacks with Luck: " + stacksWithLuck + " Luck: " + LuckFactor + "\t\tLower Bound: " + lowerBound);
			newModifier.Stacks = stacksTransfered;
		}

		//Do we have that modifier yet
		bool alreadyHave = false;
		for (int i = 0; i < modifiers.Count; i++)
		{
			if (!alreadyHave)
			{
				//Debug.Log("I: " + i + " modifiers.Count: " + modifiers.Count + "\n");
				if (modifiers[i].ModifierName == newModifier.ModifierName)
				{
					//If we do, break the search and increase the stacks
					alreadyHave = true;

					//So we never gain more than 20 of a stack.
					if (modifiers[i].Stacks + newModifier.Stacks >= 20)
					{
						modifiers[i].Stacks = 20;
					}
					else
					{
						modifiers[i].Stacks += newModifier.Stacks;
					}
					modifiers[i].Gained(newModifier.Stacks, true);
					//i = int.MaxValue;
				}
			}
		}

		//Otherwise gain it
		if (!alreadyHave)
		{
			newModifier.Carrier = this;
			newModifier.Gained(newModifier.Stacks, false);
			modifiers.Add(newModifier);
		}

		SetupModifiersUI();
	}

	public virtual void GainModifierStacks()
	{
		int index = Random.Range(0, modifiers.Count);

		int upperBound = Mathf.Max(1, (int)(LuckFactor / 3));

		int stacksGained = Random.Range(1, upperBound);

		//Debug.Log("Gaining stacks in " + modifiers[index].ModifierName + "  (" + stacksGained + ")\n\tLuck Factor: " + LuckFactor);
		modifiers[index].Stacks += stacksGained;

		//So we never gain more than 20 of a stack.
		if (modifiers[index].Stacks + stacksGained >= 20)
		{
			modifiers[index].Gained(20 - modifiers[index].Stacks, false);
			modifiers[index].Stacks = 20;
		}
		else
		{
			modifiers[index].Stacks += stacksGained;
			modifiers[index].Gained(stacksGained, false);
		}

		//Debug.Log("Gaining modifier " + modifiers[index].Stacks + " stacks of " + modifiers[index].ModifierName + "\n");
	}
}
