using UnityEngine;
using System.Collections;

public class Longsword : Weapon
{
	public static int IconIndex = 35;


	//Angle of the sweep
	//Check the distance then check the angle.
	Vector3 movementVector;

	public override void Init()
	{
		base.Init();
		//rocketPrefab = Resources.Load<GameObject>("Rocket");
		Icon = UIManager.Instance.Icons[IconIndex];

#if CHEAT
		NormalCooldown = 1.2f;
		SpecialCooldown = 1.2f;
		Durability = 100;
#else
		SpecialCooldown = Random.Range(4, 5);
#endif
		BeamColor = new Color(Random.Range(0.0f, 1.0f), Random.Range(0.0f, 1.0f), Random.Range(0.0f, 1.0f));
	}
	public override void UseWeapon(GameObject target = null, System.Type targType = null, Vector3 firePoint = default(Vector3), Vector3 hitPoint = default(Vector3), bool lockOn = false)
	{
		//Check if the target is within the distance of our weapon.

		//Check if the target is within the angle of our attack

		//Check for collateral enemies?
	}

	void Start() 
	{
		
	}
	
	void Update() 
	{
		
	}

	#region Static Functions
	public static Longsword New()
	{
		Longsword w = ScriptableObject.CreateInstance<Longsword>();
		w.AbilityName = Longsword.GetWeaponName();
		w.Durability = Random.Range(10, 60);
		w.NormalCooldown = 1;
		w.SpecialCooldown = 6;
		w.CdLeft = 0;
		return w;
	}

	static string[] adj = { "Basic", "Bulky", "Hasty", "Deadly", "Steel", "Vampiric", "Anachronic", "Violent", "Nimble", "Strange" };
	static string weaponName = "Longsword";
	public static string GetWeaponName()
	{
		int rndA = Random.Range(0, adj.Length);

		return (adj[rndA] + " " + weaponName);
	}
	#endregion
}
