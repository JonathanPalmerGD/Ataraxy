using UnityEngine;
using UnityEngine.UI;
using System.Collections;

public class Regenerating : Modifier
{
	public static string[] modNames = { "Regenerating" };

	public static Regenerating New()
	{
		Regenerating newMod = ScriptableObject.CreateInstance<Regenerating>();
		return newMod;
	}
	public override void Init()
	{
		ModifierName = modNames[Random.Range(0, modNames.Length - 1)];
		Stacks = Random.Range(1, 2);
		UIColor = new Color(Random.Range(0, .999f), Random.Range(0, .999f), Random.Range(0, .999f), .4f);
	}

	public override void Gained(int stacksGained = 0, bool newStack = false)
	{
		base.Gained(stacksGained, newStack);
	}

	public override void Update()
	{
		if (Carrier.Health < (Carrier.MaxHealth * Stacks / 10))
		{
			float regenRate = .05f * Stacks;
			regenRate = Mathf.Clamp(regenRate, 0.0f, 1.0f);
			Carrier.AdjustHealth(regenRate * Time.deltaTime);
		}
		base.Update();
	}
}