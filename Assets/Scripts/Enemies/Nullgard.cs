using UnityEngine;
using System.Linq;
using System.Collections;
using System.Collections.Generic;

public class Nullgard : GroundEnemy 
{

	//Reference to the shield
	public List<NullShield> nullShields;
	float distFromPlayer;
	public float storingShieldDur = 1.25f;
	public float drawShieldDur = 0.35f;
	float shieldCounter = 0;
	bool stored = false;

	public enum ShieldState { Down, Storing, Drawing, Up }
	public ShieldState shieldState;

	public override void Start()
	{
		closeQuartersDist = 8;
		
		nullShields = new List<NullShield>();
		nullShields = GetComponentsInChildren<NullShield>().ToList();
		base.Start();
		name = "Nullgard";
	}

	public override void Update()
	{
		//Debug.Log(shieldCounter + "\n" + shieldState.ToString());
		shieldCounter -= Time.deltaTime;

		distStateCounter += Time.deltaTime;

		#region Shield Counter
		if (shieldCounter < 0)
		{
			if(shieldState == ShieldState.Storing)
			{
				ChangeShieldState(ShieldState.Up);
			}
			if(shieldState == ShieldState.Drawing)
			{
				ChangeShieldState(ShieldState.Down);
			}
		}
		#endregion

		#region [Editor] Forced State Adjustment
		#if UNITY_EDITOR
		if (Input.GetKeyDown(KeyCode.Z))
		{
			if (state == EnemyState.Idle)
			{
				ChangeState(EnemyState.Preparing);
			}
			else if (state == EnemyState.Preparing)
			{
				ChangeState(EnemyState.Attacking);
			}
			else
			{
				ChangeState(EnemyState.Idle);
			}
		}
		#endif
		#endregion

		#region Update State
		switch (state)
		{
			case EnemyState.Idle:
				if (CanSeePlayer)
				{
					if (distState == PlayerDistState.Close)
					{
						ChangeState(EnemyState.Preparing);
					}
				}
				break;
			case EnemyState.Preparing:
				if (FacingPlayer && distState == PlayerDistState.Close)
				{
					ChangeState(EnemyState.Attacking);
				}
				if (distState == PlayerDistState.Far)
				{
					ChangeState(EnemyState.Idle);
				}
				break;
			case EnemyState.Attacking:
				if (distState == PlayerDistState.Far)
				{
					ChangeState(EnemyState.Idle);
				}
				break;
		}
		#endregion

		base.Update();
	}

	public override void HandleFiringTimer()
	{
		//if(CloseQuarters && FacingPlayer)
		//{
			FiringTimer -= Time.deltaTime;
		//}
	}

	public override void HandleKnowledge()
	{
		distFromPlayer = Vector3.Distance(transform.position, GameManager.Instance.player.transform.position);

		if (distFromPlayer < AlertRadius)
		{
			CanSeePlayer = true;

			TargVisualKnowledgeOfPlayer(true);
			TargVisualUpdateTracking(true);
		}
		else
		{
			CanSeePlayer = false;

			TargVisualKnowledgeOfPlayer(false);
			TargVisualUpdateTracking(true);
		}

		firing = CanSeePlayer;

		base.HandleKnowledge();
	}

	public override void HandleAggression()
	{
		if(stateTimer > 0)
		{
			stateTimer = Time.deltaTime;
		}

		switch (state)
		{
			case EnemyState.Idle:
				
				break;
			case EnemyState.Preparing:
				
				break;
			case EnemyState.Attacking:
				if (firing)
				{
					//Raycast to the player, if it is a clear shot, fire in that direction.
					if (FiringTimer <= 0)
					{
						if (FacingPlayer && distState == PlayerDistState.Close)
						{
							//Fire at the player
							AttackPlayer();

							#if UNITY_EDITOR
							FiringTimer = 0.5f;
							#else
							//Set ourselves on cooldown
							FiringTimer = FiringCooldown;
							#endif
							ChangeState(EnemyState.Preparing);

							//Learn from the experience. Leveling up is checked earlier in the update loop.
							GainExperience(expGainPerShot);
						}
					}
					else
					{
						//Debug.Log("Cooldown Remaining: " + Counter + "\n");
					}
				}

				break;
		}

		//If in Idle mode
			//Do we know of player?
				//Advance towards player
		
		//If in Searching mode

		//If in Preparing mode
			//Are we close to player
				//Switch to attack mode
				//Switch to Idle Mode
		
		//If in Attack Mode
			//Are we close to player
				//Attack
				//Switch to Idle Mode
	}

	public override void AttackPlayer()
	{
		weapon.UseWeapon(null, null, FirePoints, transform.position + dirToTarget * 500, false);
		//weapon.UseWeapon(null, null, FirePoints, transform.position - transform.up * .5f + transform.forward * 500, false);
	}

	public void ChangeShieldState(ShieldState targetState)
	{
		switch (targetState)
		{
			case ShieldState.Down:
				//If the shield is up
				if (shieldState == ShieldState.Up)// || shieldState == ShieldState.Storing)
				{
					foreach (NullShield ns in nullShields)
					{
						if (ns.gameObject.GetComponent<iTween>() == null)
						{
							shieldState = ShieldState.Drawing;
							ns.gameObject.transform.position = new Vector3(ns.gameObject.transform.position.x, ns.gameObject.transform.position.y - 5, ns.gameObject.transform.position.z);
							//iTween.RotateTo(ns.gameObject.transform.parent.gameObject, iTween.Hash("x", 0, "time", drawShieldDur, "easeType", "easeOutCirc"));
							shieldCounter = drawShieldDur;
						}
					}
				}
				//Otherwise put it down
				else if(shieldState == ShieldState.Drawing)
				{
					shieldState = ShieldState.Down;
				}
				break;
			case ShieldState.Up:
				//If the shield is down or drawing
				if (shieldState == ShieldState.Down)// || shieldState == ShieldState.Drawing)
				{
					foreach (NullShield ns in nullShields)
					{
						if(ns.gameObject.GetComponent<iTween>() == null)
						{ 
							shieldState = ShieldState.Storing;
							ns.gameObject.transform.position = new Vector3(ns.gameObject.transform.position.x, ns.gameObject.transform.position.y + 5, ns.gameObject.transform.position.z);
							//iTween.RotateTo(ns.gameObject.transform.parent.gameObject, iTween.Hash("x", -90, "time", storingShieldDur, "easeType", "easeOutCirc"));
							shieldCounter = storingShieldDur;
						}
					}
				}
				//Otherwise put it down
				else if(shieldState == ShieldState.Storing)
				{
					shieldState = ShieldState.Up;
				}
				break;
		}
	}

	void ChangeState(EnemyState targetState)
	{
		//Debug.Log("\t" + name + " target state: " + targetState.ToString() + "\n");
		switch (targetState)
		{
			//Put shield in front of self
			case EnemyState.Idle:
				ChangeShieldState(ShieldState.Down);
				state = EnemyState.Idle;
				stateTimer = 0;
				
				rigidbody.velocity = Vector3.zero;
				break;

			case EnemyState.Searching:
				state = EnemyState.Searching;
				break;

			//Move shield behind self
			case EnemyState.Preparing:
				ChangeShieldState(ShieldState.Up);
				stateTimer = 2.0f;
				//State that we should navigate to the player.
				state = EnemyState.Preparing;
				break;

			//Attack
			case EnemyState.Attacking:

				state = EnemyState.Attacking;
				break;
		}
	}
}
