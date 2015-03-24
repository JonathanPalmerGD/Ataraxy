using UnityEngine;
using System.Collections;

public class TowerMaster : Enemy 
{
	public GameObject[] Blocks;
	public GrapplingHook EvilHand;
	float distFromPlayer;
	public float searchingTime = 3;
	public float preparingTime = 1;

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
		EvilHand.firePointOffset = Vector3.zero;
		MaxHealth = 24;
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

		ChangeState(EnemyState.Searching);
	}
	
	public override void Update () 
	{
		EvilHand.UpdateWeapon(Time.deltaTime);


		base.Update();
	}
	#endregion

	public override void HandleKnowledge()
	{
		distFromPlayer = Vector3.Distance(transform.position, GameManager.Instance.player.transform.position);

		#region Update Knowledge of Player
		if (distFromPlayer < AlertRadius)
		{
			CanSeePlayer = true;
		}
		else
		{
			//If we could see them and can't anymore
			if (CanSeePlayer)
			{
				//Do something specific.
			}
			CanSeePlayer = false;
		}
		#endregion
	}

	public override void HandleAggression()
	{
		#region Idle State
		if (state == EnemyState.Idle)
		{
			FacePlayer();
		}
		#endregion
		#region Searching State
		else if (state == EnemyState.Searching)
		{
			FacePlayer();

			if (CanSeePlayer)
			{
				if (stateTimer > 0)
				{
					stateTimer -= Time.deltaTime;
				}
				else
				{
					ChangeState(EnemyState.Preparing);
				}
			}
		}
		#endregion
		#region Preparing State
		else if (state == EnemyState.Preparing)
		{
			FacePlayer();

			//Count up for a second or so
			if (stateTimer > 0)
			{
				stateTimer -= Time.deltaTime;
				//Use the line renderer to draw in direction of the player?
			}
			else
			{
				stateTimer = 3;

				ChangeState(EnemyState.Attacking);
			}
		}
		#endregion
		#region Attacking State
		else if (state == EnemyState.Attacking)
		{
			//Wait for projectile to time out
			if (EvilHand.weaponState != GrapplingHook.GrapplingHookWeaponState.Busy)
			{
				//If it times out, switch to searching.
				ChangeState(EnemyState.Searching);
			}
		}
		#endregion
	}

	void FireGrapple()
	{
		//Get the player current position
		Vector3 targetScanDir = GameManager.Instance.playerGO.transform.position - FirePoints[0].transform.position;

		Debug.DrawLine(FirePoints[0].transform.position, FirePoints[0].transform.position + targetScanDir, Color.green, 5.0f);

		if (EvilHand.weaponState == GrapplingHook.GrapplingHookWeaponState.Ready)
		{
			//Fire Grappling Hook at the player
			EvilHand.UseWeapon(null, null, FirePoints, GameManager.Instance.playerGO.transform.position, false);
		}
	}

	void FacePlayer()
	{
		if (CanSeePlayer)
		{
			transform.LookAt(GameManager.Instance.playerGO.transform.position, Vector3.forward);
		}
	}

	void ChangeState(EnemyState targetState)
	{
		//Debug.Log("Switching to " + targetState.ToString() + " state\n");
		switch (targetState)
		{
		case EnemyState.Idle:
			state = EnemyState.Idle;
			stateTimer = 0;
			gunMuzzle.GetComponent<Rotate>().rotationSpeed = 2;

			break;

		case EnemyState.Searching:

			gunMuzzle.GetComponent<Rotate>().rotationSpeed = 2;
			stateTimer = searchingTime;
			state = EnemyState.Searching;
			break;

		case EnemyState.Preparing:
			stateTimer = preparingTime;

			gunMuzzle.GetComponent<Rotate>().rotationSpeed = 20;

			state = EnemyState.Preparing;
			break;

		case EnemyState.Attacking:
			FireGrapple();

			gunMuzzle.GetComponent<Rotate>().rotationSpeed = 6;
			state = EnemyState.Attacking;
			break;
		}
	}

	public override void GainLevel()
	{
		XpReward += 5;
		int randomBonuses = Random.Range(0, 4);
		if (randomBonuses == 1)
		{
			//Debug.Log("Decreasing Searching Time\n");
			if (searchingTime > .5f)
			{
				searchingTime -= .75f;
			}
		}
		else if (randomBonuses == 2)
		{
			//Debug.Log("Bonus Damage\n");
			EvilHand.PrimaryDamage += 2;
		}
		else if (randomBonuses == 3)
		{
			//Debug.Log("Faster Hook Speed\n");
			EvilHand.assignedHookSpeed *= 1.5f;
		}
		else
		{
			//Debug.Log("Health\n");
			MaxHealth += 5;
			base.AdjustHealth(5);
		}


		XP -= XPNeeded;
		Level++;

		if (LevelText != null)
		{
			LevelText.text = Level.ToString();
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
		//newToken.transform.position -= Vector3.up * 4;
		newToken.rigidbody.useGravity = true;
	}

}
