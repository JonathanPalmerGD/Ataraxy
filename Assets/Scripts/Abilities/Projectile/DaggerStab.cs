using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class DaggerStab : Projectile 
{
	public LineRenderer lr;
	public List<Vector3> stabPoints;
	public float movementDecay;
	public float visualDecay;
	public float counter;
	public Collider stabCollider;
	public Color stabColor;

	void Start()
	{
		Damage = 1;
		ProjVel = 200;
		movementDecay = 0f;
		visualDecay = .35f;
		rigidbody.drag = 8;
#if CHEAT
		//visualDecay = 50f;
#else
		
#endif
		stabCollider = transform.FindChild("StabCollider").collider;

		if (lr == null)
		{
			lr = GetComponent<LineRenderer>();
			if (lr == null)
			{
				lr = gameObject.AddComponent<LineRenderer>();
			}
		}
	}

	void Update() 
	{
		if (movementDecay > 0)
		{
			movementDecay -= Time.deltaTime;
		}
		else
		{
			counter += Time.deltaTime;
			float per = (visualDecay - counter) / visualDecay;
			Dagger ls = (Dagger)creator;
			if (ls != null)
			{
				Color origColor = ls.BeamColor;
				//Color newColor = Color.cyan;
				stabColor = new Color(origColor.r, origColor.b, origColor.g, origColor.a * per);
				lr.SetColors(stabColor, stabColor);
			}
			if (per < .3f)
			{
				stabCollider.enabled = false;
			}
			if (per < 0)
			{
				Destroy(gameObject);
			}
		}

		for (int i = 0; i < stabPoints.Count; i++)
		{
			lr.SetPosition(i, transform.position + stabPoints[i]);
		}
	}

	public override void Collide()
	{
		rigidbody.drag += 2;
	}
}
