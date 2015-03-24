using UnityEngine;
using UnityEngine.UI;
using System.Collections;

public class RapidFire : Modifier
{
	public static string[] modNames = { "Rapid-Fire" };

	public static RapidFire New()
	{
		RapidFire newMod = ScriptableObject.CreateInstance<RapidFire>();
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
		if (Carrier.FiringCooldown - stacksGained * .75f > 2)
		{
			Carrier.FiringCooldown -= stacksGained * .75f;
		}
		else
		{
			Debug.LogWarning(Carrier.name + "Firing Cooldown capped\n");
			Carrier.FiringCooldown = 2;
		}
		base.Gained(stacksGained, newStack);
	}

	public override void Update()
	{
		base.Update();
	}
}