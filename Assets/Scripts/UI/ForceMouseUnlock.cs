using UnityEngine;
using System.Collections;

public class ForceMouseUnlock : MonoBehaviour 
{
	void Update()
	{
		Screen.lockCursor = false;
		Cursor.visible = true;
	}
}
