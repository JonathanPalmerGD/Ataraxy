using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class RapierStab : MeleeProjectile
{
	public override void Init()
	{
		ColliderName = "StabCollider";
		base.Init();
		
		ProjVel = 1900;
		movementDecay = 0f;
		visualDecay = .45f;
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
