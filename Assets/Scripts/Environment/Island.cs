using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class Island : WorldObject
{
	public List<Island> neighbors;
	private Cluster family;
	public Cluster Family
	{
		get { return family; }
		set
		{
			if (family == null)
			{
				family = value;
			}
			else
			{
				Debug.LogError("Tried to set Family of " + gameObject.name + "\n");
			}
		}
	}

	public new void Start()
	{
		gameObject.name = "Island: " + Nomenclature.GetName(Random.Range(0, 12), Random.Range(0, 12), Random.Range(0, 12), Random.Range(0, 12));

		base.Start();
		gameObject.tag = "Island";
	}

	public new void Update()
	{
		base.Update();
	}

	//This functionality has been removed from islands.
	//Hover targetting them was mostly annoying. May return later?
	/*
	public override void Target()
	{
		if (InfoHUD != null)
		{
			InfoHUD.enabled = true;
			
			SetupHealthUI();
			SetupResourceUI();
			SetNameUI();
		}
	}

	public override void Untarget()
	{
		if (InfoHUD != null)
		{
			InfoHUD.enabled = false;
			renderer.material.shader = diffuse;
		}
	}*/
}
