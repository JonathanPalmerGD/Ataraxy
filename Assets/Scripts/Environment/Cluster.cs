using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class Cluster : MonoBehaviour 
{
	public List<Island> platforms;
	public GameObject[] islandPrefabs;

	void Start() 
	{
		TerrainManager.Instance.RegisterCluster(this);
		gameObject.name = "Cluster: " + Nomenclature.GetName(Random.Range(0, 12), Random.Range(0, 12), Random.Range(0, 12), Random.Range(0, 12));
	}
	
	void Update() 
	{
	
	}

	public void CreateIslands()
	{
		int islandsToCreate = Random.Range(TerrainManager.minCountInCluster, TerrainManager.maxCountInCluster);

		for (int i = 0; i < islandsToCreate; i++)
		{
			//Create an island.
		}
	}
}

