using UnityEngine;
using UnityEngine.UI;
using System.Collections;

public class Ability : ScriptableObject 
{
	private string name;
	public string Name
	{
		get { return name; }
		set { name = value; }
	}
	private Sprite icon;
	public Sprite Icon
	{
		get { return icon; }
		set { icon = value; }
	}
	private Text remainder;
	public Text Remainder
	{
		get { return remainder; }
		set { remainder = value; }
	}
	private int timesGained;
	public int TimesGained
	{
		get { return timesGained; }
		set { timesGained = value; }
	}

	void Start () 
	{
	
	}
	
	void Update () 
	{
	
	}

	public virtual bool CheckAbility()
	{
		return false;
	}

	public virtual string GetInfo()
	{
		return name + " (" + timesGained + ")";
	}
}
