using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class BladeSlash : Projectile 
{
	public LineRenderer lr;
	public List<Vector3> slashPoints;
	public float movementDecay;
	public float visualDecay;
	public float counter;
	public Collider slashCollider;
	public Color slashColor;

	void Start()
	{
		Damage = 3;
		ProjVel = 300;
		movementDecay = 0f;
		visualDecay = .75f;
		rigidbody.drag = 8;
#if CHEAT
		//visualDecay = 50f;
#else
		
#endif

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
			Longsword ls = (Longsword)creator;
			if (ls != null)
			{
				Color origColor = ls.BeamColor;
				//Color newColor = Color.cyan;
				slashColor = new Color(origColor.r, origColor.b, origColor.g, origColor.a * per);
				lr.SetColors(slashColor, slashColor);
			}
			if (per < .3f)
			{
				slashColor = Color.cyan;
				slashCollider.enabled = false;
			}
			if (per < 0)
			{
				Destroy(gameObject);
			}
		}

		for (int i = 0; i < slashPoints.Count; i++)
		{
			lr.SetPosition(i, transform.position + slashPoints[i]);
		}
	}

	public override void Collide()
	{
		rigidbody.drag += 2;
	}
}
