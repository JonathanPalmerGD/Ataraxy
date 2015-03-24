using UnityEngine;
using UnityEngine.UI;
using System.Collections;

public class Rare : Modifier
{
	public new static string[] modNames = { "Rare" };

	public new static Rare New()
	{
		Rare newMod = ScriptableObject.CreateInstance<Rare>();
		return newMod;
	}
	public override void Init()
	{
		ModifierName = modNames[Random.Range(0, modNames.Length - 1)];
		Stacks = Random.Range(1, 3);
		UIColor = new Color(Random.Range(0, .999f), Random.Range(0, .999f), Random.Range(0, .999f), .4f);
		TextColor = Color.black;
	}

	public override void Gained(int stacksGained = 0, bool newStack = false)
	{
		Carrier.XpReward += stacksGained * 3;
		base.Gained(stacksGained, newStack);
	}

	public override void Update()
	{
		base.Update();
	}
}