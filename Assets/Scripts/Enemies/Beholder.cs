using UnityEngine;
using System.Collections;

public class Beholder : FlyingEnemy 
{

	public override void Start() 
	{
		base.Start();

		projectilePrefab = Resources.Load<GameObject>("Projectile");
	}
	
	public override void Update() 
	{
		base.Update();
		transform.LookAt(GameManager.Instance.playerGO.transform);
	}

	public override void Untarget()
	{
		if (InfoHUD != null)
		{
			InfoHUD.enabled = false;
			renderer.enabled = false;
			renderer.material.shader = normalShader;
		}
	}

	public override void Target()
	{
		if (InfoHUD != null)
		{
			InfoHUD.enabled = true;
			renderer.enabled = true;
			renderer.material.shader = outlineOnly;

			SetupHealthUI();
			SetupResourceUI();
			SetNameUI();
		}
	}
}
