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

		CreateIslands();
	}
	
	void Update() 
	{
		
	}

	public void CreateIslands()
	{
		int islandsToCreate = Random.Range(TerrainManager.minCountInCluster, TerrainManager.maxCountInCluster);

		for (int i = 0; i < islandsToCreate; i++)
		{
			Vector3 scale = new Vector3(
				Random.Range(TerrainManager.minScale.x, TerrainManager.maxScale.x), 
				Random.Range(TerrainManager.minScale.y, TerrainManager.maxScale.y),
				Random.Range(TerrainManager.minScale.z, TerrainManager.maxScale.z));

			float smallest = Mathf.Min(scale.x, scale.z);
			smallest = Mathf.Min(scale.y, smallest);

			if (scale.x == smallest)
			{
				scale.x = scale.y;
			}
			else if(scale.z == smallest)
			{
				scale.z = scale.y;
			}
			scale.y = smallest;

			Vector3 displacementAmt = new Vector3(
				Random.Range(TerrainManager.minDistance.x, TerrainManager.maxDistance.x),
				Random.Range(TerrainManager.minDistance.y, TerrainManager.maxDistance.y),
				Random.Range(TerrainManager.minDistance.z, TerrainManager.maxDistance.z));
			
			int disDirX = Random.Range(0, 100) > 50 ? 1 : -1;
			int disDirY = Random.Range(0, 100) > 30 ? 1 : -1;
			int disDirZ = Random.Range(0, 100) > 50 ? 1 : -1;
			Vector3 displacement = new Vector3(displacementAmt.x * disDirX, displacementAmt.y * disDirY, displacementAmt.z * disDirZ );

			//Create an island.
			GameObject newIsland = (GameObject)GameObject.Instantiate(islandPrefabs[Random.Range(0, islandPrefabs.Length)], transform.position, Quaternion.identity);

			newIsland.transform.localScale = scale;
			newIsland.transform.position += displacement;

			Vector3 rndRotation = transform.eulerAngles;
			rndRotation = new Vector3(
				Random.Range(TerrainManager.minTilt, TerrainManager.maxTilt),
				Random.Range(TerrainManager.minTilt, TerrainManager.maxTilt),
				Random.Range(TerrainManager.minTilt, TerrainManager.maxTilt));
			newIsland.transform.eulerAngles = rndRotation;

			newIsland.renderer.material.mainTexture = TerrainManager.Instance.textures[Random.Range(0, TerrainManager.Instance.textures.Count)];
			newIsland.renderer.material.mainTextureScale = new Vector2(Random.Range(5, 25), Random.Range(5, 25));
			newIsland.renderer.material.color = new Color(Random.Range(.7f, 1f), Random.Range(.7f, 1f), Random.Range(.7f, 1f));

			newIsland.transform.SetParent(transform);
		}
	}

	public void LocalizeNeighbors()
	{

	}
}