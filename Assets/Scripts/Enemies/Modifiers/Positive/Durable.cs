using UnityEngine;
using UnityEngine.UI;
using System.Collections;

public class Durable : Modifier
{
	public new static string[] modNames = { "Durable" };

	public new static Durable New()
	{
		Durable newMod = ScriptableObject.CreateInstance<Durable>();
		return newMod;
	}
	public override void Init()
	{
		ModifierName = modNames[Random.Range(0, modNames.Length - 1)];
		Stacks = Random.Range(1, 5);
		UIColor = new Color(Random.Range(0, .999f), Random.Range(0, .999f), Random.Range(0, .999f), .4f);
		TextColor = Color.black;
	}

	public override void Gained(int stacksGained = 0, bool newStack = false)
	{
		if (Carrier.DamageMultiplier - stacksGained * .05f > .3f)
		{
			Carrier.DamageMultiplier -= stacksGained * .05f;
		}
		else
		{
			Debug.LogWarning(Carrier.name + "Durability Mitigation capped\n");
			Carrier.DamageMultiplier = .3f;
		}
		base.Gained(stacksGained, newStack);
	}

	public override void Update()
	{
		base.Update();
	}
}