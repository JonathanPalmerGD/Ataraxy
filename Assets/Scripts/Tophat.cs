using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class Tophat : NPC 
{
	public override void Start()
	{
		base.Start();
		name = "Top Hat";
		Level = 3;
	}

	public override void InitModifiers()
	{
		modifiers = new List<Modifier>();

		Modifier m = Modifier.New();
		m.Init();

		m.ModifierName = "Friendly";
		m.Stacks = 1;
		m.UIColor = new Color(.1f, .8f, .1f, .4f);
		modifiers.Add(m);
	}

}
