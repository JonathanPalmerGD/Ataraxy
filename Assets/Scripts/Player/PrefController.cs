﻿using UnityEngine;
using UnityEngine.UI;
using System.Collections;

public class PrefController : MonoBehaviour 
{
	public Text nameDisplay;
	public InputField inputField;

	void Start()
	{
		Debug.Log(nameDisplay.text + "\n");
		if(inputField != null)	
		{
			inputField.text = PlayerPrefs.GetString("PlayerName", "Vant");

			inputField.textComponent.text = PlayerPrefs.GetString("PlayerName", "Vant");
		}
		//string namestr = PlayerPrefs.GetString("PlayerName", "Vant");
		nameDisplay.text = PlayerPrefs.GetString("PlayerName", "Vant");

		Debug.Log(nameDisplay.text + "\n");
	}
	
	void OnDestroy()
	{
		if (inputField != null)
		{
			if (nameDisplay.text != "")
			{
				PlayerPrefs.SetString("PlayerName", inputField.textComponent.text);
			}
		}
		else
		{
			if (nameDisplay.text != "")
			{
				PlayerPrefs.SetString("PlayerName", nameDisplay.text);
			}
		}
	}
}
