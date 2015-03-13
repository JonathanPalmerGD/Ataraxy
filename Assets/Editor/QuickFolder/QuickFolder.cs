//----------------------------
//		Quickfolder
//	 Written by Jon Palmer
//	   Darkwind Media
//----------------------------

using UnityEngine;
using UnityEditor;
using System.Collections;

public class QuickFolder : EditorWindow 
{
	const int versionNum = 3;

	//Adjust the hotkey by changing #%y to other keys. # = Shift, % is Control or Command (depending on platform). Key must be lower case
	[MenuItem ("GameObject/QuickFolderHotkey #%y")]
	static void HotKeyQuickFolder()
	{
		string qFolderName = "";
		//Our naming convention is kept simple
		if(GameObject.Find("New QuickFolder"))
		{
			qFolderName = "New QuickFolder (" + Random.Range(0, 100) + ")";
		}
		else
		{
			qFolderName = "New QuickFolder";
		}
		
		string message = "";
		bool succeeded = true;

		//Make folders if things are selected.
		if(Selection.gameObjects.Length > 0)
		{
			GameObject sharedParent = null;
			for(int i = 0; i < Selection.gameObjects.Length; i++)
			{
				//If we fail, we can stop
				if(succeeded)
				{
					//If we have a parent
					if(Selection.gameObjects[i].transform.parent != null)
					{
						//If we haven't seen a parent yet
						if(sharedParent == null)
						{
							sharedParent = Selection.gameObjects[i].transform.parent.gameObject;
						}

						//If the current object's parent is everyone elses parent
						if(Selection.gameObjects[i].transform.parent.gameObject != sharedParent)
						{
							//We fail
							succeeded = false;
							message = "[Quickfolder] - Failure\nSelected Objects do not share a parent";
						}
					}
					//If we don't have a parent
					else
					{
						//And someone else does
						if(sharedParent != null)
						{
							//We fail
							succeeded = false;
							message = "[Quickfolder] - Failure\nSelected Objects do not share a parent";
						}
					}
				}
			}

			//If we have a good shared parent
			if(succeeded)
			{
				//Make a new game object, set the name and set the parent
				GameObject newFolder = new GameObject();
				newFolder.name = qFolderName;
				if(sharedParent != null)
				{
					newFolder.transform.parent = sharedParent.transform;
				}
				
				for(int i = 0; i < Selection.gameObjects.Length; i++)
				{
					Selection.gameObjects[i].transform.parent = newFolder.transform;
				}

				//have the user select the new folder
				Selection.activeGameObject = newFolder;

				//Print out the name just in case they click away.
				message = "[Quickfolder] - Success\n" + "Folder Name: " + qFolderName;
			}
		}
		else
		{
			//Fail with no objects selected
			succeeded = false;
			message = "[Quickfolder] - Failure\nNo Objects Selected";
		}
		//Print out results of quickfolder attempt
		if(succeeded)
		{
			Debug.Log(message);
		}
		else
		{
			Debug.LogError(message);
		}
	}
}