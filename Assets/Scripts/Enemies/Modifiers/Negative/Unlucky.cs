using UnityEngine;
using UnityEngine.UI;
using System.Collections;

public class Unlucky : Modifier
{
	public new static string[] modNames = { "Unlucky" };

	public new static Unlucky New()
	{
		Unlucky newMod = ScriptableObject.CreateInstance<Unlucky>();
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
		if (Carrier.LuckFactor - .35f * stacksGained >= 0)
		{
			Carrier.LuckFactor -= .35f * stacksGained;
		}
		else
		{
			Carrier.LuckFactor = 0;
		}
		base.Gained(stacksGained, newStack);
	}

	public override void Update()
	{
		base.Update();
	}
}