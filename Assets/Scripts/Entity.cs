using UnityEngine;
using System.Collections;

public class Entity : AtaraxyObject 
{
	public Shader outline;
	public Shader diffuse;

	public new void Start()
	{
		outline = Shader.Find("Outlined/Silhouetted Diffuse");
		diffuse = Shader.Find("Diffuse");

		gameObject.tag = "Entity";
		base.Start();

		HealthSlider = UIManager.Instance.target_HP;
		NameText = UIManager.Instance.target_Name;
		InfoHUD = UIManager.Instance.target_HUD;
		Untarget();
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

	public new void Update()
	{
		base.Update();
	}
}
