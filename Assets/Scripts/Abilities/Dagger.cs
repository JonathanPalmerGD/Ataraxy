using UnityEngine;
using System.Collections;

public class Dagger : Weapon 
{
	public static int IconIndex = 29;

	public override void UseWeapon(GameObject target = null, System.Type targType = null, Vector3 firePoint = default(Vector3), Vector3 hitPoint = default(Vector3), bool lockOn = false)
	{

	}

	void Start() 
	{
	
	}
	
	void Update() 
	{
		//If we're behind our target, change the color of the Icon to red?
	}

	#region Static Functions
	public static Dagger New()
	{
		Dagger w = ScriptableObject.CreateInstance<Dagger>();
		w.AbilityName = Dagger.GetWeaponName();
		w.Durability = Random.Range(10, 60);
		w.NormalCooldown = 1;
		w.SpecialCooldown = 6;
		w.CdLeft = 0;
		return w;
	}

	static string[] adj = { "Basic", "Bulky", "Hasty", "Deadly", "Steel", "Vampiric", "Anachronic", "Violent", "Nimble", "Strange" };
	static string weaponName = "Dagger";
	public static string GetWeaponName()
	{
		int rndA = Random.Range(0, adj.Length);

		return (adj[rndA] + " " + weaponName);
	}
	#endregion
}
