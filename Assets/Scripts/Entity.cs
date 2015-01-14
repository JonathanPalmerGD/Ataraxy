using UnityEngine;
using System.Collections;

public class Entity : AtaraxyObject 
{
	public Shader outline;
	public Shader diffuse;
	private bool targeted = false;
	public bool Targeted
	{
		get { return targeted; }
		set
		{
			if (value)
			{
				counter = targetFade;
			}
			targeted = value;
		}
	}
	private float counter = 0.0f;
	private float targetFade = 3.0f;

	public new void Start()
	{
		outline = Shader.Find("Outlined/Silhouetted Diffuse");
		diffuse = Shader.Find("Diffuse");

		gameObject.tag = "Entity";
		base.Start();
	}

	public new void Update()
	{
		if (Targeted)
		{
			infoHUD.enabled = true;
			counter -= Time.deltaTime;
			renderer.material.shader = outline;

			if (counter < 0)
			{
				Targeted = false;
			}
		}
		else
		{
			infoHUD.enabled = false;
			renderer.material.shader = diffuse;
		}

		/*if (Input.GetKey(KeyCode.M))
		{
			Debug.Log("M Held\n");
			Targeted = true;
		}
		else
		{
			Targeted = false;
		}*/
		base.Update();
	}
}
