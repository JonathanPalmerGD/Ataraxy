using UnityEngine;
using System.Collections;

public class StartPosition : MonoBehaviour 
{

	void Start()
	{
		GameManager.Instance.playerGO.transform.position = transform.position;
		GameManager.Instance.playerGO.transform.rotation = transform.rotation;
		//GameObject.FindGameObjectWithTag("Player").transform.position = transform.position;
		//GameObject.FindGameObjectWithTag("Player").transform.rotation = transform.rotation;
	}
	
	void Update() 
	{
	
	}
}
