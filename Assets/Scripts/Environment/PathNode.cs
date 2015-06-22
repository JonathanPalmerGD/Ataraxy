using UnityEngine;
using System.Linq;
using System.Collections;
using System.Collections.Generic;

public class PathNode : MonoBehaviour 
{
	public Island island;
	//public List<NodeConnection> localConnection;

	void Start() 
	{
		GetComponent<Renderer>().enabled = false;
	}
	
	void Update() 
	{
		#if UNITY_EDITOR
		if (Input.GetKeyDown(KeyCode.Home))
		{
			GetComponent<Renderer>().enabled = true;
		}
		#endif
	}
}