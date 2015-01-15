using UnityEngine;
using System.Collections;

public class NPC : Entity 
{
	public Shader outline;
	public Shader diffuse;

	public virtual void Start() 
	{
		outline = Shader.Find("Outlined/Silhouetted Diffuse");
		diffuse = Shader.Find("Diffuse");

		//Set up HUD with UIManager
		HealthSlider = UIManager.Instance.target_HP;
		NameText = UIManager.Instance.target_Name;
		InfoHUD = UIManager.Instance.target_HUD;

		Untarget();
	}

	public virtual void Update() 
	{
	
	}

	public virtual void Untarget()
	{
		if (InfoHUD != null)
		{
			InfoHUD.enabled = false;
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
			SetNameUI();
		}
	}
}
