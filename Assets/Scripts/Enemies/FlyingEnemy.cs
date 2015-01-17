using UnityEngine;
using System.Collections;

public class FlyingEnemy : Enemy 
{
	public enum FlyingMotion { Floating, Circling, Hover, Overhead };
	public FlyingMotion airState = FlyingMotion.Floating;

	void Start() 
	{
		//Depending on the state, enable or disable the flying motion behavior?
	}
	
	public override void Update() 
	{
		base.Update();
	}

	public override void HandleMovement()
	{
		
		base.HandleMovement();

	}
}
