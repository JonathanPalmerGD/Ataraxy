using UnityEngine;
using System.Collections;

public class TowerMaster : Enemy 
{
	public GameObject[] Blocks;
	public GrapplingHook EvilHand;

	#region Start & Update
	public override void Start () 
	{

		EvilHand = GrapplingHook.New();
		EvilHand.Init();
		EvilHand.DurCost = 0;
		EvilHand.Faction = Faction;
		EvilHand.hookPrefab = Resources.Load<GameObject>("Projectiles/Evil Grappling Hook"); 
		EvilHand.NormalCooldown = 0;
		EvilHand.primaryFirePointIndex = 0;
		EvilHand.Carrier = this;
		MaxHealth = 18;
		Health = MaxHealth;

		base.Start();


		name = "Towermaster";
		FiringCooldown = 5;
		FiringTimer = Random.Range(0, FiringCooldown);
		//Get our grappling hook?

		foreach (GameObject go in Blocks)
		{
			go.renderer.material = renderer.material;
		}
	}
	
	public override void Update () 
	{
		EvilHand.UpdateWeapon(Time.deltaTime);

		//Update my grappling hook.
		if(Input.GetKeyDown(KeyCode.Z))
		{
			//Get the player current position
			Vector3 targetScanDir = GameManager.Instance.playerGO.transform.position - FirePoints[0].transform.position;
			//Debug.DrawLine(FirePoints[0].transform.position, FirePoints[0].transform.position + targetScanDir, Color.green, 5.0f);

			if (EvilHand.weaponState == GrapplingHook.GrapplingHookWeaponState.Ready)
			{
				//Fire Grappling Hook at the player
				EvilHand.UseWeapon(null, null, FirePoints, GameManager.Instance.playerGO.transform.position, false);
			}
		}

		//{ Idle, Searching, Preparing, Attacking };
		/*
		if(state == EnemyState.Idle)
		{
		}
		else if(state == EnemyState.Searching)
		{
			if(CanSeePlayer)
			{
				ChangeState(EnemyState.Preparing);
			}
		}
		else if(state == EnemyState.Preparing)
		{
			//Count up for a second or so
			if (stateTimer > 0)
			{
				stateTimer -= Time.deltaTime;
			}
			else
			{
				stateTimer = 1;

				ChangeState(EnemyState.Attacking);



				//Gain experience on throwing a grappling
				//GainExperience(3);
				
				ChangeState(EnemyState.Attacking);
				//Vector3 dirToPlayer = (chargeTargetLocation - transform.position + Vector3.up * .1f);
				//dirToPlayer.Normalize();

				//particleSystem.startColor = attackColor;
			}
		}
		else if(state == EnemyState.Attacking)
		{

			//Wait for projectile to time out
			//If it times out, switch to searching.
		}*/
	}
	#endregion

	void ChangeState(EnemyState targetState)
	{
		switch (targetState)
		{
		case EnemyState.Idle:
			state = EnemyState.Idle;
			stateTimer = 0;
			//particleSystem.startColor = passiveColor;
			break;
		case EnemyState.Searching:

			state = EnemyState.Searching;
			break;
		case EnemyState.Preparing:
			
			//particleSystem.startColor = prepareColor;
			state = EnemyState.Preparing;
			break;
		case EnemyState.Attacking:
			
			//particleSystem.startColor = attackColor;
			state = EnemyState.Attacking;
			break;
		}
	}

	#region Target & Untarget
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
	#endregion

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
		newToken.transform.position -= Vector3.up * 2;
		newToken.rigidbody.useGravity = false;
	}

}
