using UnityEngine;
using UnityEngine.UI;
using System.Collections;

public class Frail : Modifier
{
	public new static string[] modNames = { "Frail" };

	public new static Frail New()
	{
		Frail newMod = ScriptableObject.CreateInstance<Frail>();
		return newMod;
	}
	public override void Init()
	{
		ModifierName = modNames[Random.Range(0, modNames.Length - 1)];
		Stacks = Random.Range(1, 4);
		UIColor = new Color(Random.Range(0, .999f), Random.Range(0, .999f), Random.Range(0, .999f), .4f);
		TextColor = Color.black;
	}

	public override void Gained(int stacksGained = 0, bool newStack = false)
	{
		Carrier.MaxHealth -= 1.5f * stacksGained;
		Carrier.AdjustHealth(-1.5f * stacksGained);
		base.Gained(stacksGained, newStack);
	}

	public override void Update()
	{
		base.Update();
	}
}