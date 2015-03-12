using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class MeleeProjectile : Projectile 
{
	public LineRenderer lineRenderer;
	public List<Vector3> lineRendPoints;
	/// <summary>
	/// Used to handle the fading of the line's alpha over time. Assigned during SetupMeleeProjectile() in Weapon.cs
	/// </summary>
	public Color lineColor;
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

		if (lineRenderer == null)
		{
			lineRenderer = GetComponent<LineRenderer>();
			if (lineRenderer == null)
			{
				lineRenderer = gameObject.AddComponent<LineRenderer>();
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
			lineRenderer.SetColors(fadedColor, fadedColor);

			if (percentageFade < .3f)
			{
				projectileCollider.enabled = false;
			}
			if (percentageFade < 0)
			{
				Destroy(gameObject);
			}
		}

		for (int i = 0; i < lineRendPoints.Count; i++)
		{
			lineRenderer.SetPosition(i, transform.position + lineRendPoints[i]);
		}
	}

	public virtual Color GetLineColor(float percentageFade)
	{
		return new Color(lineColor.r, lineColor.b, lineColor.g, lineColor.a * percentageFade);
	}

	public override void Fizzle()
	{
		rigidbody.drag = 50;
		projectileCollider.enabled = false;
		fizzled = true;
	}

	public override void Collide()
	{
		//rigidbody.drag += 2;
	}
}
