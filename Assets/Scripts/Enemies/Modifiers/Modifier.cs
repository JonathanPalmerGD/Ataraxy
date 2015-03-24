using UnityEngine;
using UnityEngine.UI;
using System.Collections;

public class Modifier : ScriptableObject
{
	public string ModifierName;
	public int Stacks;
	public NPC Carrier;
	public Color UIColor;
	public Color TextColor;

	public static string[] modNames = { "Barking", "Explosive", "Auto-Destruct", "Replicating","Illusionist", "Summoning", "Flickering", "Mentor", "Venomous", "Fiery", "Chilling", "Frigid", "Electric", "Dazing", "Thorned", "Stoneskin", "Dexetrous", "Swift", "Steadfast", "Brutal", "Rusting", "Defiler", "Nimble", "Avenger", "Numbing", "Deadly", "Furious"};

	public static Modifier New()
	{
		Modifier newMod = ScriptableObject.CreateInstance<Modifier>();
		return newMod;
	}

	public virtual void Init()
	{
		ModifierName = modNames[Random.Range(0, modNames.Length - 1)];
		Stacks = Random.Range(1, 5);
		UIColor = new Color(Random.Range(0, .999f),Random.Range(0, .999f),Random.Range(0, .999f), .4f);
		TextColor = Color.black;
	}

	/// <summary>
	/// Called when a modifier is gained.
	/// </summary>
	/// <param name="newStack">If it is an entirely new modifier gained, hand in true.</param>
	public virtual void Gained(int stacksGained = 0, bool newStack = false)
	{
		Carrier.DamageAmplification += .01f * stacksGained;
	}

	public virtual void Update()
	{

	}

	public virtual void HandleVisuals()
	{

	}

	public virtual bool CheckAbility()
	{
		return false;
	}

	public virtual string GetInfo()
	{
		return ModifierName + " (" + Stacks + ")";
	}

}