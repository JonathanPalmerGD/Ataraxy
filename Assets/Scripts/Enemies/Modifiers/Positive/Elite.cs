using UnityEngine;
using UnityEngine.UI;
using System.Collections;

public class Elite : Modifier
{
	public static string[] modNames = { "Elite" };

	public static Elite New()
	{
		Elite newMod = ScriptableObject.CreateInstance<Elite>();
		return newMod;
	}
	public override void Init()
	{
		ModifierName = modNames[Random.Range(0, modNames.Length - 1)];
		Stacks = 5;
		UIColor = new Color(1, 1, 1, .4f);
	}

	public override void Gained(int stacksGained = 0, bool newStack = false)
	{
		Carrier.DamageAmplification += .05f * stacksGained;

		Carrier.xpRateOverTime += 1f * stacksGained;
		Carrier.XpReward += 2f * stacksGained;

		Carrier.ProjSpeedAmp += .05f * stacksGained;
		
		Carrier.MaxHealth += 3 * stacksGained;
		Carrier.AdjustHealth(3 * stacksGained);
		
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