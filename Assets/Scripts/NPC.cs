using UnityEngine;
using System.Collections;

public class NPC : Entity 
{
	public Shader outline;
	public Shader outlineOnly;
	public Shader diffuse;
	public Shader normalShader;

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
		InfoHUD = UIManager.Instance.target_HUD;

		Untarget();
	}

	public override void Update() 
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
