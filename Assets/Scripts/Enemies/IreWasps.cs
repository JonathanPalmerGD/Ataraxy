using UnityEngine;
using System.Collections;

public class IreWasps : FlyingEnemy 
{
	public bool attackingPlayer = false;
	public float attackDuration = 0;
	public float attackDamage;
	private Vector3 chargeTargetLocation;
	private Color prepareColor = new Color(.70f, .45f, 0.1f, .9f);
	private Color passiveColor = new Color(.7f, 0.7f, 0.3f, .9f);
	private Color passiveColorLighter = new Color(.9f, 0.9f, 0.4f, .9f);
	private Color attackColor = new Color(.87f, .1f, 0.1f, .9f);
	private Vector3 home;
	private ParticleSystem waspPartSys;
	ParticleSystem.Particle[] m_Particles;
	private AudioSource buzzingSource;
	float distFromPlayer;

	public override void Start() 
	{
		base.Start();
		if (belowStage)
		{
			home = transform.position + Vector3.up * (1 * TerrainManager.underworldYOffset);
		}
		else
		{
			home = transform.position + Vector3.up;
		}
		attackDamage = .01f;
		name = "Ire Wasps";
		FiringTimer = Random.Range(0, FiringCooldown);
		projectilePrefab = Resources.Load<GameObject>("Projectiles/Projectile");

		buzzingSource = AudioManager.Instance.MakeSource ("Wasp_Buzz", transform.position, transform);
		buzzingSource.loop = true;
		//buzzingSource.rolloffMode = AudioRolloffMode.Linear;
		buzzingSource.maxDistance = 50;
		//buzzingSource.minDistance = 15;
		buzzingSource.volume = .2f;
		buzzingSource.Play ();
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
			GetComponent<Renderer>().enabled = false;
			GetComponent<ParticleSystem>().startSpeed = .7f;
			//renderer.material.shader = normalShader;
		}
	}

	public override void HandleKnowledge()
	{
		distFromPlayer = Vector3.Distance(transform.position, GameManager.Instance.player.transform.position);
	}

	public override void HandleAggression()
	{
		#region Attacking Player
		//If we're inside the player
		if (attackingPlayer)
		{
			TargVisualKnowledgeOfPlayer(true);

			//Record that to increase intensity of attack
			attackDuration += Time.deltaTime;

			//Hurt the player
			//Debug.Log("Dealing : " + attackDuration * attackDamage + " damage\n");

			float damageAmount = 1 * attackDuration * attackDamage * DamageAmplification;
			
			//In case we life steal
			AdjustHealth(damageAmount * LifeStealPer);

			//Deal damage to the player
			GameManager.Instance.player.AdjustHealth(-damageAmount);
			GainExperience((attackDuration) / 6);

			//Then check if the player got away
			if (!CanSeePlayer || distFromPlayer > transform.localScale.x)
			{
				attackingPlayer = false;
				attackDuration = 0;
			}
			else
			{
				ChangeState(EnemyState.Attacking);
				if (GetComponent<Rigidbody>().velocity.magnitude < 2)
				{
					GetComponent<Rigidbody>().velocity = Vector3.zero;
				}
				else
				{
					GetComponent<Rigidbody>().velocity = GetComponent<Rigidbody>().velocity * .9f;
				}
			}
		}
		#endregion
		#region Else Not Attacking Player
		else
		{
			#region Update Knowledge of Player
			if (distFromPlayer < AlertRadius)
			{
				CanSeePlayer = true;
				if (targVisual != null)
				{
					TargVisualKnowledgeOfPlayer(true);
					TargVisualUpdateTracking(true);
				}
			}
			else
			{
				if (CanSeePlayer)
				{
					GetComponent<Rigidbody>().velocity = Vector3.zero;
					ChangeState(EnemyState.Idle);
				}
				CanSeePlayer = false;
				TargVisualKnowledgeOfPlayer(false);
				TargVisualUpdateTracking(true);
				
			}
			#endregion

			#region If we can see the player
			if (CanSeePlayer)
			{
				#region Idle State
				//Change particles to much more aggitated.
				if (state == EnemyState.Idle)
				{
					TargVisualKnowledgeOfPlayer(true);
					TargVisualUpdateTracking(true);

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
					TargVisualUpdateTracking(false);
					//Count up for a second or so
					if (stateTimer > 0)
					{
						stateTimer -= Time.deltaTime;
					}
					else
					{
						stateTimer = 1;
						//Gain experience on charge
						GainExperience(3);
				
						ChangeState(EnemyState.Attacking);
						//Apply a force and fly in the direction of the player
						//Debug.DrawLine(transform.position, chargeTargetLocation, Color.green, 5.0f);
						Vector3 dirToPlayer = (chargeTargetLocation - transform.position + Vector3.up * .1f);
						dirToPlayer.Normalize();
						GetComponent<Rigidbody>().AddForce(dirToPlayer * ProjSpeedAmp * GetComponent<Rigidbody>().mass * 2000);
						GetComponent<ParticleSystem>().startColor = attackColor;
					}
				}
				#endregion
				#region Attacking State
				else if (state == EnemyState.Attacking)
				{
					TargVisualUpdateTracking(false);
					
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
					//Debug.DrawLine(transform.position, transform.position + dirToHome, Color.red);
					GetComponent<Rigidbody>().AddForce(dirToHome * GetComponent<Rigidbody>().mass * 15);
				}
			}
			#endregion
		}
		#endregion
	}

	void ChangeState(EnemyState targetState)
	{
		switch (targetState)
		{
			case EnemyState.Idle:
				state = EnemyState.Idle;
				stateTimer = 0;
				buzzingSource.volume = .2f;
				//particleSystem.startColor = passiveColor;
				UpdateParticleColor(passiveColor);
				TargVisualUpdateLineColor(passiveColorLighter, passiveColorLighter);
				GetComponent<Rigidbody>().velocity = Vector3.zero;
				break;
			case EnemyState.Searching:
				state = EnemyState.Searching;
				break;
			case EnemyState.Preparing:
			
				buzzingSource.volume = .6f;
				//particleSystem.startColor = prepareColor;
				UpdateParticleColor(prepareColor);
				TargVisualUpdateLineColor(prepareColor, prepareColor);
				state = EnemyState.Preparing;
				break;
			case EnemyState.Attacking:
			
				buzzingSource.volume = .8f;
				//particleSystem.startColor = attackColor;
				UpdateParticleColor(attackColor);
				TargVisualUpdateLineColor(attackColor, attackColor);
				state = EnemyState.Attacking;
				break;
		}
	}

	void UpdateParticleColor(Color newColor)
	{
		InitializeIfNeeded();

		GetComponent<ParticleSystem>().startColor = newColor;

		int numParticlesAlive = waspPartSys.GetParticles(m_Particles);

		// Change only the particles that are alive
		for (int i = 0; i < numParticlesAlive; i++)
		{
			m_Particles[i].color = newColor;
		}

		// Apply the particle changes to the particle system
		waspPartSys.SetParticles(m_Particles, numParticlesAlive);
	}
	void InitializeIfNeeded()
	{
		if (waspPartSys == null)
		{
			waspPartSys = GetComponent<ParticleSystem>();
		}
		if (m_Particles == null || m_Particles.Length < waspPartSys.maxParticles)
		{
			m_Particles = new ParticleSystem.Particle[waspPartSys.maxParticles];
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
		base.Target();

		if (InfoHUD != null)
		{
			InfoHUD.enabled = true;
			GetComponent<ParticleSystem>().startSpeed = 1.3f;

			SetupHealthUI();
			SetupResourceUI();
			SetupNameUI();
			SetupModifiersUI();
		}
	}
}
