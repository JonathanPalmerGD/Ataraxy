using UnityEngine;
using System.Collections;

public class Jumppad : MonoBehaviour 
{
	//The velocity we want to send the player at
	public Vector3 jumpVel = new Vector3(0, 15, 0);
	public float forwardAmplify = 1.2f;

	// Use this for initialization
	void Start () 
	{
		//NOTE: If you do not create a tag named Jumppad, this and DetectPad WILL NOT WORK.
		this.tag = "Jumppad";
	}
	
	// Update is called once per frame
	void Update () 
	{
		
	}
}
