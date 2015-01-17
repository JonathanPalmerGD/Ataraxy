using UnityEngine;
using System.Collections;

public class WorldObject : MonoBehaviour 
{

	public virtual void Start() 
	{
		gameObject.tag = "WorldObject";
	}

	public virtual void Update() 
	{
	
	}
}
