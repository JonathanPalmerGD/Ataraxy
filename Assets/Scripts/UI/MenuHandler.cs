using UnityEngine;
using UnityEngine.UI;
using UnityEngine.EventSystems;

/// <summary>
/// This menu handler was written by Jonathan Palmer.
/// Free to use to anyone.
/// Version 1.02
/// www.JonathanPalmerGD.com
/// </summary>
public class MenuHandler : MonoBehaviour
{
	public Animator mainMenu;
	public Animator credits;
	public Animator options;

	// Add more menu states as necessary.
	public enum MenuState { Main, Options, Credits };
	public MenuState curState = MenuState.Main;

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
		Application.LoadLevel(levelName);
	}

	//Set the default string here to whatever your first level is if you'd like.
	public void PlayGame(string levelName = "")
	{
		if (levelName == "")
		{
			Debug.LogError("You should set your default game name in the MenuHandler script\nAlternative: Hand in a string that matches a level.");
		}

		Application.LoadLevel(levelName);
	}	

	public void LoadMenu()
	{
		Application.LoadLevel("MainMenu");
	}

	public void SetState(int newState)
	{
		//Check previous state, phase out that menu
		if (curState == MenuState.Main)
		{
			mainMenu.enabled = true;
			mainMenu.Play("MainMenuFadeOut");
		}
		else if (curState == MenuState.Options)
		{
			options.Play("SideMenuSlideOut");
		}
		else if (curState == MenuState.Credits)
		{
			credits.Play("SideMenuSlideOut");
		}

		//Phase in new state's menu.
		if (newState == 0)
		{
			mainMenu.gameObject.SetActive(true);
			mainMenu.Play("MainMenuFadeIn");

			if (curState == MenuState.Options)
			{
				EventSystem.current.SetSelectedGameObject(GameObject.Find("Options Button"));
			}
			else if (curState == MenuState.Credits)
			{
				EventSystem.current.SetSelectedGameObject(GameObject.Find("Credits Button"));
			}
				curState = MenuState.Main;
		}
		else if (newState == 1)
		{
			options.Play("SideMenuSlideIn");
			EventSystem.current.SetSelectedGameObject(GameObject.Find("Options Back Button"));
			curState = MenuState.Options;
		}
		else if (newState == 2)
		{
			credits.Play("SideMenuSlideIn");
			EventSystem.current.SetSelectedGameObject(GameObject.Find("Credits Back Button"));
			curState = MenuState.Credits;
		}

	}

	public void Update()
	{
		if (Input.GetButtonDown("Cancel"))
		{
			if (curState == MenuState.Options || curState == MenuState.Credits)
			{
				mainMenu.Play("MainMenuFadeIn");
			}
		}
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
}