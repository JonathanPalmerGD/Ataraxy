using UnityEngine;
using UnityEngine.UI;
using System.Collections;

public class Modifier : ScriptableObject
{
	private string modifierName;
	public string ModifierName
	{
		get { return modifierName; }
		set { modifierName = value; }
	}
	private int stacks;
	public int Stacks
	{
		get { return stacks; }
		set { stacks = value; }
	}
	private Entity carrier;
	public Entity Carrier
	{
		get { return carrier; }
		set { carrier = value; }
	}

	private Color uiColor;
	public Color UIColor
	{
		get { return uiColor; }
		set { uiColor = value; }
	}

	public static string[] modNames = { "Barking", "Explosive", "Auto-Destruct", "Replicating","Illusionist", "Summoning", "Flickering", "Mentor", "Venomous", "Fiery", "Chilling", "Frigid", "Electric", "Dazing", "Thorned", "Stoneskin", "Dexetrous", "Swift", "Steadfast", "Brutal", "Rusting", "Defiler", "Nimble", "Avenger", "Numbing", "Deadly", "Furious"};
	public virtual void Init()
	{
		modifierName = modNames[Random.Range(0, modNames.Length - 1)];
		Stacks = Random.Range(1, 5);
		UIColor = new Color(Random.Range(0, .999f),Random.Range(0, .999f),Random.Range(0, .999f), .4f);
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

	public static Modifier New()
	{
		Modifier w = ScriptableObject.CreateInstance<Modifier>();
		return w;
	}	
}
