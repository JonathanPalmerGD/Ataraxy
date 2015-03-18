using UnityEngine;
using System.Collections;

public class MainMenu : MonoBehaviour 
{
	public void Update()
	{
	}

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

	public void LoadScene(string levelName)
	{
		
		//if (Application.level)
		//{
		Application.LoadLevel(levelName);
		//}
		//else
		//{
		//	Debug.LogError("Invalid level load attempted.\nIndex provided: " + level);
		//}
	}

	public void PlayGame()
	{

		Application.LoadLevel("Gameplay Test");
}	

	public void LoadMenu()
	{
		Application.LoadLevel("MainMenu");
	}

	public void Quit()
	{
		AppHelper.Quit();
	}

	public void AdjustSensitivity(float change)
	{	
		//Use player prefs,

		//Save if navigating away?
	}

	public void LockCursor()
	{
		Screen.lockCursor = true;
	}

	public void TellUIMToUnpauseGame()
	{
		LockCursor();
		UIManager.Instance.UnpauseGame();
	}

	public void EngageFullScreen()
	{
#if !UNITY_EDITOR
		Screen.fullScreen = true;
#endif
	}
}