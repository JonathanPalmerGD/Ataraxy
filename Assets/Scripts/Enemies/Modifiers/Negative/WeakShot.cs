using UnityEngine;
using UnityEngine.UI;
using System.Collections;

public class WeakShot : Modifier
{
	public new static string[] modNames = { "Weak Shot" };

	public new static WeakShot New()
	{
		WeakShot newMod = ScriptableObject.CreateInstance<WeakShot>();
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
		//Debug.Log(Carrier.ProjSpeedAmp + " " + stacksGained + "\n" + (Carrier.ProjSpeedAmp - (.05f * stacksGained)));
		if (Carrier.ProjSpeedAmp - (.05f * stacksGained) > .25f)
		{
			Carrier.ProjSpeedAmp -= .05f * stacksGained;
		}
		else
		{
			Carrier.ProjSpeedAmp = .25f;
		}
		base.Gained(stacksGained, newStack);
	}

	public override void Update()
	{
		base.Update();
	}
}