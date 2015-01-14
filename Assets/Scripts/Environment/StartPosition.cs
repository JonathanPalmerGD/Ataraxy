using UnityEngine;
using System.Collections;

public class StartPosition : MonoBehaviour 
{

	void Start() 
	{
		GameObject.FindGameObjectWithTag("Player").transform.position = transform.position;
		GameObject.FindGameObjectWithTag("Player").transform.rotation = transform.rotation;
	}
	
	void Update() 
	{
	
	}
}
