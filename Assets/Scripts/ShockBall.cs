using UnityEngine;
using System.Collections;

public class ShockBall : Projectile 
{
	void Start()
	{
		Damage = 5;
		//body = transform.FindChild("Rocket Body").gameObject;
		//explosive = transform.FindChild("Detonator-Base").GetComponent<Detonator>();
	}
}
