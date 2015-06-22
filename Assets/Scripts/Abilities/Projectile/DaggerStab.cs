using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class DaggerStab : MeleeProjectile 
{
	public override void Init()
	{
		ColliderName = "StabCollider";
		base.Init();

		Damage = 1;
		ProjVel = 1200;
		movementDecay = 0f;
		visualDecay = .35f;
		GetComponent<Rigidbody>().drag = 8;
#if CHEAT
		//visualDecay = 50f;
#else
		
#endif
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
