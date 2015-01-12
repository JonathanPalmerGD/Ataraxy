using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class Island : MonoBehaviour
{
	public List<Island> neighbors;
	private Cluster family;
	public Cluster Family
	{
		get { return family; }
		set
		{
			if (family == null)
			{
				family = value;
			}
			else
			{
				Debug.LogError("Tried to set Family of " + gameObject.name + "\n");
			}
		}
	}

	void Start()
	{
		gameObject.name = "Island: " + Nomenclature.GetName(Random.Range(0, 12), Random.Range(0, 12), Random.Range(0, 12), Random.Range(0, 12));
	}

	void Update()
	{

	}
}
