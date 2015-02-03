using UnityEngine;
using UnityEngine.UI;
using System.Collections;

public class Ability : ScriptableObject 
{
	private string abilityName;
	public string AbilityName
	{
		get { return abilityName; }
		set { abilityName = value; }
	}
	private Sprite icon;
	public Sprite Icon
	{
		get { return icon; }
		set { icon = value; }
	}
	private Image iconUI;
	public Image IconUI
	{
		get { return iconUI; }
		set { iconUI = value; }
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
	private Entity carrier;
	public Entity Carrier
	{
		get { return carrier; }
		set { carrier = value; }
	}

	public virtual void Init()
	{
	}

	public virtual void HandleVisuals()
	{

	}

	public virtual void CleanUp()
	{
		Destroy(IconUI.gameObject);
	}

	public virtual bool CheckAbility()
	{
		return false;
	}

	public virtual string GetInfo()
	{
		return abilityName + " (" + timesGained + ")";
	}
}
