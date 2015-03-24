using UnityEngine;
using UnityEngine.UI;
using System.Collections;

public class Kamikaze : Modifier
{
	public static string[] modNames = { "Kamikaze" };

	public static Kamikaze New()
	{
		Kamikaze newMod = ScriptableObject.CreateInstance<Kamikaze>();
		return newMod;
	}
	public override void Init()
	{
		ModifierName = modNames[Random.Range(0, modNames.Length - 1)];
		Stacks = Random.Range(5, 8);
		UIColor = new Color(Random.Range(0, .999f), Random.Range(0, .999f), Random.Range(0, .999f), .4f);
	}

	public override void Gained(int stacksGained = 0, bool newStack = false)
	{
		Carrier.LifeStealPer -= .3f * stacksGained;
		Carrier.DamageAmplification += .3f * stacksGained;
		base.Gained(stacksGained, newStack);
	}

	public override void Update()
	{
		base.Update();
	}
}