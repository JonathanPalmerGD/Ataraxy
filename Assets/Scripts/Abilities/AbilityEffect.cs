using UnityEngine;
using System.Collections;

public class AbilityEffect : Ability 
{
	private float duration;
	public float Duration
	{
		get { return duration; }
		set { duration = value; }
	}

	public override void Start()
	{
		base.Update();
	}

	public override void Update() 
	{
		base.Update();
	}
}
