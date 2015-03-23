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

	public override Allegiance Faction
	{
		get { return Allegiance.Neutral; }
	}

	public override void Start()
	{
		outline = Shader.Find("Outlined/Silhouetted Diffuse");
		outlineOnly = Shader.Find("Outlined/Silhouette Only");
		diffuse = Shader.Find("Diffuse");
		normalShader = renderer.material.shader; 

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
				InfoHUD.enabled = false;
			}
			renderer.material.shader = diffuse;
		}
	}

	public virtual void Target()
	{
		if (InfoHUD != null)
		{
			InfoHUD.enabled = true;
			renderer.material.shader = outline;

			SetupHealthUI();
			SetupResourceUI();
			SetupNameUI();
			SetupXPUI();
			SetupModifiersUI();
		}
	}

	public virtual void InitModifiers()
	{
		modifiers = new List<Modifier>();

		for (int i = 0; i < Random.Range(1,3); i++)
		{
			Modifier m = Modifier.New();
			m.Init();
			modifiers.Add(m);
		}
		//Debug.Log(name + " has " + modifiers.Count + " modifiers\n");
	}

	public override void SetupModifiersUI()
	{
		if (ModHUD != null)
		{
			//Debug.Log("Configuring Modifiers with " + modifiers.Count + " entries\n");
			UIManager.Instance.ConfigureModifiers(modifiers);
		}

		base.SetupModifiersUI();
	}
}
