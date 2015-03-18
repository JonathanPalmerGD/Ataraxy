using UnityEngine;
using UnityEditor;
using System.Collections;

public class PlayerPrefManagement : Editor 
{
	[MenuItem ("Assets/Clear Player Prefs")]
	public static void ResetPlayerPrefs()
	{
		PlayerPrefs.DeleteAll();
	}
}
