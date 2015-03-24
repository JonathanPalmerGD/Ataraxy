using UnityEngine;
using UnityEngine.UI;
using System.Collections;

public class Bolstered : Modifier
{
	public new static string[] modNames = { "Bolstered" };

	public new static Bolstered New()
	{
		Bolstered newMod = ScriptableObject.CreateInstance<Bolstered>();
		return newMod;
	}
	public override void Init()
	{
		ModifierName = modNames[Random.Range(0, modNames.Length - 1)];
		Stacks = Random.Range(1, 5);
		UIColor = new Color(Random.Range(0, .999f), Random.Range(0, .999f), Random.Range(0, .999f), .4f);
	}

	public override void Gained(int stacksGained = 0, bool newStack = false)
	{
		Carrier.MaxHealth += 5 * stacksGained;
		Carrier.AdjustHealth(5 * stacksGained);
		base.Gained(stacksGained, newStack);
	}

	public override void Update()
	{
		base.Update();
	}
}