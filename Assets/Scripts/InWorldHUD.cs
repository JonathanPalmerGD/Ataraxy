using UnityEngine;
using System.Collections;

public class InWorldHUD : MonoBehaviour 
{

	void Start() 
	{
	}
	
	void Update() 
	{
		transform.LookAt(GameManager.Instance.player.transform.position);
		transform.Rotate(transform.up, Mathf.PI);
	}
}
