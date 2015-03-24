using UnityEngine;
using UnityEngine.UI;
using System.Collections;

public class Vampiric : Modifier
{
	public static string[] modNames = { "Vampiric" };

	public static Vampiric New()
	{
		Vampiric newMod = ScriptableObject.CreateInstance<Vampiric>();
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
		Carrier.LifeStealPer += .2f * stacksGained;
		base.Gained(stacksGained, newStack);
	}

	public override void Update()
	{
		base.Update();
	}
}