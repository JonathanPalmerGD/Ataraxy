using UnityEngine;
using UnityEngine.UI;
using System.Collections;

public class Kamikaze : Modifier
{
	public new static string[] modNames = { "Kamikaze" };

	public new static Kamikaze New()
	{
		Kamikaze newMod = ScriptableObject.CreateInstance<Kamikaze>();
		return newMod;
	}
	public override void Init()
	{
		ModifierName = modNames[Random.Range(0, modNames.Length - 1)];
		Stacks = Random.Range(5, 8);
		//UIColor = new Color(.4f, 0.1f, 0.1f, .4f);
		UIColor = new Color(0, 0, 0, .4f);
		//TextColor = new Color(.8f, .2f, .2f);

		TextColor = new Color(1.0f, 0.7f, 0.7f);
	}

	public override void Gained(int stacksGained = 0, bool newStack = false)
	{
		Carrier.LifeStealPer -= .1f * stacksGained;
		Carrier.DamageAmplification += .05f * stacksGained;
		Carrier.DamageMultiplier += stacksGained * .125f;
		base.Gained(stacksGained, newStack);
	}

	public override void Update()
	{
		base.Update();
	}
}