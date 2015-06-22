using UnityEngine;
using System.Collections;

public class Beholder : FlyingEnemy 
{
	public override void Start() 
	{
		base.Start();

		name = "Beholder";
		FiringTimer = Random.Range(0, FiringCooldown);
		if (belowStage)
		{
			GetComponent<Floatation>().homeRegion = transform.position + Vector3.up * (15 + TerrainManager.underworldYOffset);
		}
		else
		{
			GetComponent<Floatation>().homeRegion = transform.position + Vector3.up * 15;
		}
		projectilePrefab = Resources.Load<GameObject>("Projectiles/Projectile");
	}
	
	public override void Update() 
	{
		if (!UIManager.Instance.paused)
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
	}

	public override void ThrowToken(GameObject newToken)
	{
		float randBound = 500;
		//newToken.rigidbody.AddForce(newToken.rigidbody.mass * (-Vector3.up * Random.Range(.8f, 1.5f) * randBound / 8) + new Vector3(Random.Range(-randBound, randBound), 0, Random.Range(-randBound, randBound)));
	}

	public override void Untarget()
	{
		base.Untarget();

		if (InfoHUD != null)
		{
			InfoHUD.enabled = false;
			GetComponent<Renderer>().enabled = false;
			GetComponent<Renderer>().material.shader = normalShader;
		}
	}

	public override void Target()
	{
		base.Target();

		if (InfoHUD != null)
		{
			InfoHUD.enabled = true;
			GetComponent<Renderer>().enabled = true;
			GetComponent<Renderer>().material.shader = outlineOnly;

			SetupHealthUI();
			SetupResourceUI();
			SetupNameUI();
			SetupModifiersUI();
		}
	}
}
