using UnityEngine;
using System.Collections;

public class Enemy : Entity
{

	public new void Start()
	{
		base.Start();
		gameObject.tag = "Enemy";
	}

	public new void Update()
	{
		
		base.Update();
	}
}
