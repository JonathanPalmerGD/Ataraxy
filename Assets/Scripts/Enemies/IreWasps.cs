using UnityEngine;
using System.Collections;

public class IreWasps : FlyingEnemy 
{
	public bool attackingPlayer = false;
	public float attackDuration = 0;
	public float attackDamage;
	private Vector3 chargeTargetLocation;
	private Color prepareColor = new Color(.90f, .58f, 0.0f, .7f);
	private Color passiveColor = new Color(.97f, 1.0f, 0.0f, .7f);
	private Color attackColor = new Color(.87f, .1f, 0.1f, .7f);
	private Vector3 home;

	float distFromPlayer;

	public override void Start() 
	{
		base.Start();
		home = transform.position + Vector3.up * (1 * TerrainManager.underworldYOffset);
		attackDamage = .01f;
		name = "Ire Wasps";
		FiringTimer = Random.Range(0, FiringCooldown);
		//GetComponent<Floatation>().homeRegion = transform.position + Vector3.up * 20;
		projectilePrefab = Resources.Load<GameObject>("Projectile");
	}

	public override void FindFiringPositions()
	{
		//Do nothing
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

	public override void Untarget()
	{
		if (InfoHUD != null)
		{
			InfoHUD.enabled = false;
			renderer.enabled = false;
			particleSystem.startSpeed = .7f;
			//renderer.material.shader = normalShader;
		}
	}

	public override void HandleKnowledge()
	{
		distFromPlayer = Vector3.Distance(transform.position, GameManager.Instance.player.transform.position);
	}

	public override void HandleAggression()
	{		
		//If we're inside the player
		if (attackingPlayer)
		{
			//Record that to increase intensity of attack
			attackDuration += Time.deltaTime;

			//Hurt the player
			//Debug.Log("Dealing : " + attackDuration * attackDamage + " damage\n");
			GameManager.Instance.player.AdjustHealth(-1 * attackDuration * attackDamage);
			GainExperience((attackDuration) / 4);

			//Then check if the player got away
			if (!CanSeePlayer || distFromPlayer > transform.localScale.x)
			{
				attackingPlayer = false;
				attackDuration = 0;
			}
			else
			{
				ChangeState(EnemyState.Attacking);
				if (rigidbody.velocity.magnitude < 2)
				{
					rigidbody.velocity = Vector3.zero;
				}
				else
				{
					rigidbody.velocity = rigidbody.velocity * .9f;
				}
			}
		}
		else
		{
			#region Update Knowledge of Player
			if (distFromPlayer < 50)
			{
				CanSeePlayer = true;
			}
			else
			{
				if (CanSeePlayer)
				{
					rigidbody.velocity = Vector3.zero;
				}
				CanSeePlayer = false;
			}
			#endregion

			#region If we can see the player
			if (CanSeePlayer)
			{
				#region Idle State
				//Change particles to much more aggitated.
				if (state == EnemyState.Idle)
				{
					if (stateTimer > 0)
					{
						stateTimer -= Time.deltaTime;
					}
					else
					{
						//Go into aggro mode, noting the player's location
						chargeTargetLocation = GameManager.Instance.player.transform.position;

						//Set counter for preparing

						stateTimer = 1.2f;

						ChangeState(EnemyState.Preparing);
					}

				}
				#endregion
				#region Preparing State
				else if (state == EnemyState.Preparing)
				{
					//Count up for a second or so
					if (stateTimer > 0)
					{
						stateTimer -= Time.deltaTime;
					}
					else
					{
						stateTimer = 1;
						//Gain experience on charge
						GainExperience(10);
				
						ChangeState(EnemyState.Attacking);
						//Apply a force and fly in the direction of the player
						//Debug.DrawLine(transform.position, chargeTargetLocation, Color.green, 5.0f);
						Vector3 dirToPlayer = (chargeTargetLocation - transform.position + Vector3.up * .1f);
						dirToPlayer.Normalize();
						rigidbody.AddForce(dirToPlayer * rigidbody.mass * 2000);
						particleSystem.startColor = attackColor;
					}
				}
				#endregion
				#region Attacking State
				else if (state == EnemyState.Attacking)
				{
					//Let us charge for a bit
					if (stateTimer > 0)
					{
						stateTimer -= Time.deltaTime;
					}
					else
					{
						//Then switch to idle and stop
						ChangeState(EnemyState.Idle);
						stateTimer = 2;
						
					}
				}
				#endregion
			}
			#endregion
			#region Can't See Player
			else
			{
				if(Vector3.Distance(transform.position, home) > 3)
				{
					Vector3 dirToHome = (home - transform.position);
					dirToHome.Normalize();
					Debug.DrawLine(transform.position, transform.position + dirToHome, Color.red);
					rigidbody.AddForce(dirToHome * rigidbody.mass * 15);
				}
			}
			#endregion
		}
	}

	void ChangeState(EnemyState targetState)
	{
		switch (targetState)
		{
			case EnemyState.Idle:
				state = EnemyState.Idle;
				stateTimer = 0;
				particleSystem.startColor = passiveColor;
				rigidbody.velocity = Vector3.zero;
				break;
			case EnemyState.Searching:

				state = EnemyState.Searching;
				break;
			case EnemyState.Preparing:

				particleSystem.startColor = prepareColor;
				state = EnemyState.Preparing;
				break;
			case EnemyState.Attacking:

				particleSystem.startColor = attackColor;
				state = EnemyState.Attacking;
				break;
		}
	}

	void OnTriggerEnter(Collider collider)
	{
		if (enabled && !attackingPlayer)
		{
			string cTag = collider.gameObject.tag;
			if (cTag == "Player")
			{
				//Inside the player
				attackingPlayer = true;
			}
		}
	}

	public override void Target()
	{
		if (InfoHUD != null)
		{
			InfoHUD.enabled = true;
			particleSystem.startSpeed = 1.3f;
			//renderer.enabled = true;
			//renderer.material.shader = outlineOnly;

			SetupHealthUI();
			SetupResourceUI();
			SetupNameUI();
		}
	}
}
