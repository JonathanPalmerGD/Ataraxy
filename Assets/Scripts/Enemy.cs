using UnityEngine;
using System.Collections;

public class Enemy : NPC
{
	public GameObject projectilePrefab;
	private GameObject gunMuzzle;
	private Vector3 dirToTarget;

	private float counter;
	public float Counter
	{
		get { return counter; }
		set { counter = value; }
	}

	private float xpReward = 5;

	private float xpRateOverTime = .5f;

	private bool gainXpWhenHit = false;
	private float xpGainWhenHit = .5f;

	private float firingCooldown;
	public float FiringCooldown
	{
		get { return firingCooldown; }
		set { firingCooldown = value; }
	}

	public float expGainPerShot = 5;

	public override Allegiance Faction
	{
		get { return Allegiance.Enemy; }
	}

	public new void Start()
	{
		base.Start();
		gameObject.tag = "Enemy";
		gunMuzzle = transform.FindChild("Gun Muzzle").gameObject;
		FiringCooldown = 2;
	}

	public new void Update()
	{
		counter -= Time.deltaTime;
		//If we know where the player is, update them as the target.

		HandleExperience();

		HandleKnowledge();

		HandleAggression();

		base.Update();
	}

	public void HandleExperience()
	{
		GainExperience(Time.deltaTime * xpRateOverTime);

		if (XP > 100)
		{
			XP -= 100;
			Level++;
		}
	}

	/// <summary>
	/// A method for handling movement
	/// </summary>
	public void HandleMovement()
	{

	}

	/// <summary>
	/// A method for handling if the enemy can see the player.
	/// </summary>
	public void HandleKnowledge()
	{
		//Set the direction to the player, can be updated to lead the player's movement.
		dirToTarget = (GameManager.Instance.player.transform.position - gunMuzzle.transform.position);// +charMotor.movement.velocity * percentagePlayerVelLeading;

		dirToTarget.Normalize();
	}

	public void HandleAggression()
	{
		//Raycast to the player, if it is a clear shot, fire in that direction.
		if (counter <= 0)
		{
			//Fire at the player
			AttackPlayer();

			//Set ourselves on cooldown
			counter = FiringCooldown;

			//Learn from the experience. Leveling up is checked earlier in the update loop.
			GainExperience(expGainPerShot);
		}
		else
		{
			//Debug.Log("Cooldown Remaining: " + Counter + "\n");
		}
	}

	public override void AdjustHealth(int amount)
	{
		base.AdjustHealth(amount);
		if (gainXpWhenHit)
		{
			GainExperience(xpGainWhenHit);
		}
	}

	/// <summary>
	/// 
	/// </summary>
	public void AttackPlayer()
	{
		GameObject projectile = (GameObject)GameObject.Instantiate(projectilePrefab, gunMuzzle.transform.position, Quaternion.identity);

		Projectile proj = projectile.GetComponent<Projectile>();

		if (proj != null)
		{
			proj.Faction = Faction;
			proj.Shooter = this;

			projectile.rigidbody.AddForce(dirToTarget * proj.ProjVel * projectile.rigidbody.mass);
			Destroy(projectile, proj.ProjLife);
		}
	}

	public override void KillEntity()
	{
		//AtaraxyObject sets IsDead to true but nothing else.
		base.KillEntity();

		//Give the player experience.
		GameManager.Instance.player.GainExperience(xpReward, Level);

		//Drop a token.
		GameObject newToken = (GameObject)GameObject.Instantiate(GameManager.Instance.tokenPrefab, transform.position + Vector3.up * 2, Quaternion.identity);
		float randBound = 8000;
		newToken.rigidbody.AddForce(newToken.rigidbody.mass * (Vector3.up * Random.Range(.8f, 1.5f) * randBound / 8) + new Vector3(Random.Range(-randBound, randBound), 0, Random.Range(-randBound, randBound))); 

		//Give nearby enemies experience.

		//And after all that, destroy ourself.
		Destroy(gameObject);
	}
}
