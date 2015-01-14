using UnityEngine;
using System.Collections;

public class Enemy : Entity
{
	public GameObject projectile;
	public override Allegiance Faction
	{
		get { return Allegiance.Enemy; }
	}
	public new void Start()
	{
		base.Start();
		gameObject.tag = "Enemy";
	}

	public new void Update()
	{
		
		base.Update();
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

	}

	/// <summary>
	/// 
	/// </summary>
	public void AttackPlayer()
	{

	}
}
