using UnityEngine;
using UnityEngine.UI;
using System.Collections;

public class Berserk : Modifier
{
	public new static string[] modNames = { "Berserk" };

	public new static Berserk New()
	{
		Berserk newMod = ScriptableObject.CreateInstance<Berserk>();
		return newMod;
	}
	public override void Init()
	{
		ModifierName = modNames[Random.Range(0, modNames.Length - 1)];
		Stacks = Random.Range(1, 3);
		UIColor = new Color(Random.Range(0, .999f), Random.Range(0, .999f), Random.Range(0, .999f), .4f);
		TextColor = new Color(.6f, .2f, .2f);
	}

	public override void Gained(int stacksGained = 0, bool newStack = false)
	{
		Carrier.DamageAmplification += .08f * stacksGained;
		Carrier.DamageMultiplier += stacksGained * .05f;

		Carrier.ProjSpeedAmp += .05f * stacksGained;

		if (Carrier.FiringCooldown - stacksGained * .55f > 2)
		{
			Carrier.FiringCooldown -= stacksGained * .55f;
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