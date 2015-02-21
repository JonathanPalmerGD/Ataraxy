using UnityEngine;
using System.Collections;

public class GrimKeep : Enemy 
{
	public GameObject[] Blocks;
	public override void Start()
	{
		base.Start();

		name = "GrimKeep";
		FiringCooldown = 4;
		FiringTimer = Random.Range(0, FiringCooldown);
		projectilePrefab = Resources.Load<GameObject>("Projectiles/Projectile");

		foreach (GameObject go in Blocks)
		{
			go.renderer.material = renderer.material;
		}
	}

	public override void Target()
	{
		base.Target();

		foreach (GameObject go in Blocks)
		{
			go.renderer.material.shader = outline;
		}
	}



	public override void Untarget()
	{
		base.Untarget();

		foreach (GameObject go in Blocks)
		{
			go.renderer.material.shader = diffuse;
		}

	}
	//When it dies, it destroys the eye at the top and leaves the body behind.

	public override void KillEntity()
	{
		foreach (GameObject go in Blocks)
		{
			go.renderer.material.shader = diffuse;
		}
		base.KillEntity();
	}

	public override void ThrowToken(GameObject newToken)
	{
		newToken.rigidbody.useGravity = true;
	}
}
