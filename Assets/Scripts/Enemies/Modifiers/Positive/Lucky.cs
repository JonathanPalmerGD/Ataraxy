using UnityEngine;
using UnityEngine.UI;
using System.Collections;

public class Lucky : Modifier
{
	public new static string[] modNames = { "Lucky" };

	public new static Lucky New()
	{
		Lucky newMod = ScriptableObject.CreateInstance<Lucky>();
		return newMod;
	}
	public override void Init()
	{
		ModifierName = modNames[Random.Range(0, modNames.Length - 1)];
		Stacks = Random.Range(1, 5);
		UIColor = new Color(Random.Range(0, .999f), Random.Range(0, .999f), Random.Range(0, .999f), .4f);
		TextColor = Color.yellow;
	}

	public override void Gained(int stacksGained = 0, bool newStack = false)
	{
		Carrier.LuckFactor += .5f * stacksGained;
		base.Gained(stacksGained, newStack);
	}

	public override void Update()
	{
		base.Update();
	}
}