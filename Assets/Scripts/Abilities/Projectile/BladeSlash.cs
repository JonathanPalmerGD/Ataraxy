using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class BladeSlash : MeleeProjectile
{
	public override void Init()
	{
		//This needs to happen before our parent's execution.

		ColliderName = "SlashCollider";
		base.Init();

		Damage = 3;
		ProjVel = 2400;
		movementDecay = 0f;
		visualDecay = .75f;
#if CHEAT
		visualDecay = 50f;
#else
		
#endif
		//Reaching this point.
	}

	public override void Update() 
	{
		base.Update();
	}

	public override void Collide()
	{
		GetComponent<Rigidbody>().drag += 2;
	}
}
