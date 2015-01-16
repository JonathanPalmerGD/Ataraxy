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

	public override void Init()
	{
		base.Init();
	}
}
