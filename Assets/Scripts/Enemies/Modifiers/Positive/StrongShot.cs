using UnityEngine;
using UnityEngine.UI;
using System.Collections;

public class StrongShot : Modifier
{
	public new static string[] modNames = { "Strong Shot" };

	public new static StrongShot New()
	{
		StrongShot newMod = ScriptableObject.CreateInstance<StrongShot>();
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
		Carrier.ProjSpeedAmp += .05f * stacksGained;
		base.Gained(stacksGained, newStack);
	}

	public override void Update()
	{
		base.Update();
	}
}