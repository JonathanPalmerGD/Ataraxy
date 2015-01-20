using UnityEngine;
using System.Collections;

public class MainMenu : MonoBehaviour 
{
	public void NextScene()
	{
		Application.LoadLevel(Application.loadedLevel + 1);
	}

	public void LoadScene(int level)
	{
		if (level < Application.levelCount)
		{
			Application.LoadLevel(level);
		}
		else
		{
			Debug.LogError("Invalid level load attempted.\nIndex provided: " + level);
		}
	}

	public void PlayGame()
	{

		Application.LoadLevel("Gameplay Test");
	}

	public void LoadMenu()
	{
		Application.LoadLevel(0);
	}

	public void AdjustSensitivity(float change)
	{	
		//Use player prefs,

		//Save if navigating away?
	}
}
