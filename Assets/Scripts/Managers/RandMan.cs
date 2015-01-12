using UnityEngine;
using System.Collections;

public class RandMan : Singleton<GameManager>
{
	System.Random rand;

	void Start() 
	{
		rand = new System.Random();
	}
	
	public int Next(int max)
	{
		return rand.Next(max);
	}

	public int Next(int min, int max)
	{
		return rand.Next(min, max);
	}

	void Update() 
	{
	
	}
}
