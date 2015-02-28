using UnityEngine;
using System.Collections;

public class FlyingEnemy : Enemy 
{
	public enum FlyingMotion { Floating, Circling, Hover, Overhead };
	public FlyingMotion airState = FlyingMotion.Floating;

	public override void Start() 
	{
		//Depending on the state, enable or disable the flying motion behavior?
		this.Start();
	}
	
	public override void Update() 
	{
		base.Update();
	}

	public override void ThrowToken(GameObject newToken)
	{
		newToken.rigidbody.useGravity = false;
		float randBound = 500;
		newToken.rigidbody.AddForce(newToken.rigidbody.mass * (-Vector3.up * Random.Range(.8f, 1.5f) * randBound / 8) + new Vector3(Random.Range(-randBound, randBound), 0, Random.Range(-randBound, randBound)));
	}

	public override void HandleMovement()
	{
		
		base.HandleMovement();

	}
}
