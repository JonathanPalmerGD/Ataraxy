using UnityEngine;
using UnityEngine.UI;
using System.Collections;

public class Fragile : Modifier
{
	public new static string[] modNames = { "Fragile" };

	public new static Fragile New()
	{
		Fragile newMod = ScriptableObject.CreateInstance<Fragile>();
		return newMod;
	}
	public override void Init()
	{
		ModifierName = modNames[Random.Range(0, modNames.Length - 1)];
		Stacks = Random.Range(3, 5);
		UIColor = new Color(Random.Range(0, .999f), Random.Range(0, .999f), Random.Range(0, .999f), .4f);
	}

	public override void Gained(int stacksGained = 0, bool newStack = false)
	{
		Carrier.DamageMultiplier += stacksGained * .05f;
		
		base.Gained(stacksGained, newStack);
	}

	public override void Update()
	{
		base.Update();
	}
}