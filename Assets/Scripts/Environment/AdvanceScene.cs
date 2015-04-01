using UnityEngine;
using System.Collections;

public class AdvanceScene : MonoBehaviour 
{

	void OnTriggerEnter(Collider collider)
	{
		if (enabled)
		{
			if (collider.tag == "Player")
			{
				Application.LoadLevel(Application.loadedLevel + 1);
			}
		}
	}
}
