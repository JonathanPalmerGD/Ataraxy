using UnityEngine;
using UnityEngine.UI;
using System.Collections;

public class Alert : Modifier
{
	public new static string[] modNames = { "Alert" };

	public new static Alert New()
	{
		Alert newMod = ScriptableObject.CreateInstance<Alert>();
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
		Carrier.AlertRadius += 10 * stacksGained;
		base.Gained(stacksGained, newStack);
	}

	public override void Update()
	{
		base.Update();
	}
}