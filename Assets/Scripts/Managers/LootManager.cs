using UnityEngine;
using System.Collections;

public class LootManager : Singleton<LootManager>
{
	public static string[] nameTable = {
			"Rocket Launcher", "Shock Rifle", "Longsword", "Dagger",
			"Rapier", "Gravity Staff", "Bounding Staff", "Winged Sandals",
			"Grappling Hook", "Hemotick", "Glacial Sling", "Warp Staff" };
	public static string[] lootTable = {
			"RocketLauncher", "ShockRifle", "Longsword", "Dagger",
			"Rapier", "GravityStaff", "BoundingStaff", "WingedSandals",
			"GrapplingHook", "Hemotick", "GlacialSling", "WarpStaff" };

	public static Ability NewWeapon(string weaponName = "")
	{
		if (weaponName == "")
		{
			weaponName = lootTable[Random.Range(0, lootTable.Length)];
		}
		
		for (int i = 0; i < nameTable.Length; i++)
		{
			if (weaponName == nameTable[i])
			{
				weaponName = lootTable[i];
				i = nameTable.Length - 1;
			}
		}
		
		try
		{
			Ability ab = (Ability)ScriptableObject.CreateInstance(weaponName);
			//Debug.Log(ab.ToString());
			return ab;
		}
		catch
		{
			Debug.LogError("Failed to create weapon: " + weaponName + "\n");
			return Weapon.New();
		}
	}
}
