using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class MeleeProjectile : Projectile 
{
	public LineRenderer lr;
	public List<Vector3> lrPoints;
	public Color lrColor;
	public float movementDecay;
	public float visualDecay;
	public float counter;
	public Collider projectileCollider;

	public string ColliderName;

	public override void Init()
	{
		base.Init();

		if (projectileCollider == null)
		{
			projectileCollider = transform.FindChild(ColliderName).collider;

			if (projectileCollider == null)
			{
				Debug.LogError("Failure to detect melee projectile collider\n");
			}
		}

		if (lr == null)
		{
			lr = GetComponent<LineRenderer>();
			if (lr == null)
			{
				lr = gameObject.AddComponent<LineRenderer>();
			}
		}
	}
	
	public override void Update()
	{
		base.Update();

		if (movementDecay > 0)
		{
			movementDecay -= Time.deltaTime;
		}
		else
		{
			counter += Time.deltaTime;
			float percentageFade = (visualDecay - counter) / visualDecay;

			Color fadedColor = GetLineColor(percentageFade);
			lr.SetColors(fadedColor, fadedColor);

			if (percentageFade < .3f)
			{
				projectileCollider.enabled = false;
			}
			if (percentageFade < 0)
			{
				Destroy(gameObject);
			}
		}

		for (int i = 0; i < lrPoints.Count; i++)
		{
			lr.SetPosition(i, transform.position + lrPoints[i]);
		}
	}

	public virtual Color GetLineColor(float percentageFade)
	{
		return new Color(lrColor.r, lrColor.b, lrColor.g, lrColor.a * percentageFade);
	}

	public override void Collide()
	{
		//rigidbody.drag += 2;
	}
}
