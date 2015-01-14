using UnityEngine;
using System.Collections;

public class InWorldHUD : MonoBehaviour 
{
	Player player;

	void Start() 
	{
		player = GameObject.FindGameObjectWithTag("Player").GetComponent<Player>();
	}
	
	void Update() 
	{
		transform.LookAt(player.transform.position);
		transform.Rotate(transform.up, Mathf.PI);
	}
}
