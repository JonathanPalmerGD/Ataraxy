using UnityEngine;
using System.Collections;

public class Beholder : FlyingEnemy 
{

	public override void Start() 
	{
		base.Start();

		name = "Beholder";
		FiringTimer = Random.Range(0, FiringCooldown);
		GetComponent<Floatation>().homeRegion = transform.position + Vector3.up * 20;
		projectilePrefab = Resources.Load<GameObject>("Projectile");
	}
	
	public override void Update() 
	{
		base.Update();

		if (CanSeePlayer)
		{
			transform.LookAt(GameManager.Instance.playerGO.transform);
		}
		else
		{
			transform.Rotate(Vector3.up, Random.Range(.3f, .7f));
		}
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
