using UnityEngine;
using System.Collections;

public class Passive : Ability
{
	private float durationRemaining;
	public float DurationRemaining
	{
		get { return durationRemaining; }
		set { durationRemaining = value; }
	}
	private bool timed;

	public void UpdatePassive(float time)
	{
		durationRemaining -= time;
	}

	public override bool CheckAbility()
	{
		if (durationRemaining <= 0)
		{
			return true;
		}
		return false;
	}

	public override string GetInfo()
	{
		return Name + " : " + (int)durationRemaining + " seconds left";
	}

	#region Static Functions
	public static Passive New()
	{
		Passive p = ScriptableObject.CreateInstance<Passive>();
		p.Name = Passive.GetPassiveName();
		p.durationRemaining = Random.Range(60.0f, 120.0f);
		return p;
	}

	static string[] noun = { "Damage Reduction", "Double Jump", "Safety Frames", "Bonus Knockback", "Hyper Jump", "Wisdom" };
	public static string GetPassiveName()
	{
		int rndA = Random.Range(0, noun.Length);

		return (noun[rndA]);
	}
	#endregion
}
