using UnityEngine;
using System.Collections;

//Credit to Bunny83 from StackOverflow - http://answers.unity3d.com/questions/161858/startstop-playmode-from-editor-script.html
public static class AppHelper
{
#if UNITY_WEBPLAYER
     //public static string webplayerQuitURL = "http://google.com";
#endif
	public static void Quit()
	{
#if UNITY_EDITOR
		UnityEditor.EditorApplication.isPlaying = false;
#elif UNITY_WEBPLAYER
		//Application.OpenURL(webplayerQuitURL);
#else
		Application.Quit();
#endif
	}
}
