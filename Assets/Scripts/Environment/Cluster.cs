using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class Cluster : MonoBehaviour 
{
	public List<Island> platforms;
	public GameObject[] islandPrefabs;
	public GameObject[] landmarkPrefabs;
	public enum GenMethod { Early, Grid, Poisson };
	public GenMethod GenApproach = GenMethod.Early;
	public int poissonKVal = 20;
	public int sizeBonus = 0;

	void Start()
	{
		TerrainManager.Instance.RegisterCluster(this);
		gameObject.name = "Cluster: " + Nomenclature.GetName(Random.Range(0, 12), Random.Range(0, 12), Random.Range(0, 12), Random.Range(0, 12));

		poissonKVal = Random.Range(TerrainManager.poissonMinK, TerrainManager.poissonMaxK);
		sizeBonus = Random.Range(0, 10);

		if (GenApproach == GenMethod.Poisson)
		{
			CreateIslandsPoissonApproach();
			if (Random.Range(0, 100) > 90)
			{
				CreateLandmarksPoissonApproach();
			}
		}
		else if (GenApproach == GenMethod.Grid)
		{
			CreateIslandsGridApproach();
		}
		else
		{
			CreateIslands();
		}
	}
	
	void Update() 
	{
		
	}

	public void CreateIslandsPoissonApproach()
	{
		PoissonDiscSampler pds = new PoissonDiscSampler(100, 100, 16+(int)(sizeBonus * 1.5), poissonKVal);
		foreach (Vector2 sample in pds.Samples())
		{
			GameObject newIsland = null;
			if (islandPrefabs.Length > 0)
			{
				float yOffset = Random.Range(TerrainManager.minDistance.y, TerrainManager.maxDistance.y);

				Vector3 newPosition = new Vector3(sample.x + transform.position.x, transform.position.y + yOffset, sample.y + transform.position.z);
				newIsland = (GameObject)GameObject.Instantiate(islandPrefabs[Random.Range(0, islandPrefabs.Length)], newPosition, Quaternion.identity);

				ApplyRandomScale(newIsland, true);
				ApplyRandomRotation(newIsland);
				ApplyRandomTexturing(newIsland);
				ApplyIslandParent(newIsland);

				platforms.Add(newIsland.GetComponent<Island>());
			}
		}
	}

	public void CreateLandmarksPoissonApproach()
	{
		PoissonDiscSampler pds = new PoissonDiscSampler(100, 100, 75, poissonKVal);
		foreach (Vector2 sample in pds.Samples())
		{
			GameObject newLandmark = null;
			if (islandPrefabs.Length > 0)
			{
				float yOffset = Random.Range(TerrainManager.minDistance.y, TerrainManager.maxDistance.y);

				Vector3 newPosition = new Vector3(sample.x + transform.position.x, transform.position.y + yOffset, sample.y + transform.position.z);
				newLandmark = (GameObject)GameObject.Instantiate(landmarkPrefabs[Random.Range(0, landmarkPrefabs.Length)], newPosition, Quaternion.identity);

				//ApplyRandomScale(newLandmark, true);
				ApplyRandomRotation(newLandmark);
				//ApplyRandomTexturing(newLandmark);
				ApplyIslandParent(newLandmark);

			}
		}
	}

	public void CreateIslandsGridApproach()
	{
		int islandsToCreate = Random.Range(TerrainManager.minCountInCluster, TerrainManager.maxCountInCluster);

		int emptySpaces = 9 - islandsToCreate;

		List<GameObject> newIslands = new List<GameObject>();

		//Select N random X,Y points from within this cluster area.
		List<Vector3> islandLocs = new List<Vector3>();
		
		Vector3 cSize = TerrainManager.clusterSize;
		Vector3 spaceSize = cSize / ((Mathf.Sqrt(9)));
		//Debug.Log(spaceSize + "\n");

		#region Populate New Islands List
		for (int i = 0; i < islandsToCreate; i++)
		{
			newIslands.Add((GameObject)GameObject.Instantiate(islandPrefabs[Random.Range(0, islandPrefabs.Length)], transform.position, Quaternion.identity));
		}

		for (int i = 0; i < emptySpaces; i++)
		{
			newIslands.Add(new GameObject());
		}
		#endregion

		#region Shuffle Island List
		int n = newIslands.Count;
		while (n > 1)
		{
			n--;
			int k = Random.Range(0, n + 1);
			GameObject value = newIslands[k];
			newIslands[k] = newIslands[n];
			newIslands[n] = value;
		}
		#endregion

		for (int xIndex = 0; xIndex < 3; xIndex++)
		{
			for (int zIndex = 0; zIndex < 3; zIndex++)
			{
				int curIndex = (xIndex * 3) + zIndex;
				if (newIslands[curIndex].name != "New Game Object")
				{
					newIslands[curIndex].transform.position = new Vector3(
						newIslands[curIndex].transform.position.x + ((xIndex - 1) * (spaceSize.x + 5)),
						newIslands[curIndex].transform.position.y,
						newIslands[curIndex].transform.position.z + ((zIndex - 1) * (spaceSize.z + 5)));
					
					float yOffset = Random.Range(TerrainManager.minDistance.y, TerrainManager.maxDistance.y);

					Vector3 islandLoc = new Vector3(
						Random.Range(spaceSize.x, -spaceSize.x),
						yOffset,
						Random.Range(spaceSize.z, -spaceSize.z));

					newIslands[curIndex].transform.position = new Vector3(
						newIslands[curIndex].transform.position.x + islandLoc.x,
						newIslands[curIndex].transform.position.y,
						newIslands[curIndex].transform.position.z + islandLoc.z);

					ApplyRandomScale(newIslands[curIndex], false);
					ApplyRandomRotation(newIslands[curIndex]);
					ApplyRandomTexturing(newIslands[curIndex]);
				}
			}
		}

		for (int i = 0; i < newIslands.Count; i++)
		{
			if (newIslands[i].gameObject.name == "New Game Object")
			{
				GameObject reference = newIslands[i];
				newIslands.RemoveAt(i);
				i--;
				GameObject.Destroy(reference);
			}
			else
			{
				ApplyIslandParent(newIslands[i]);
			}
		}
	}
	
	public void CreateIslands()
	{
		int islandsToCreate = Random.Range(TerrainManager.minCountInCluster, TerrainManager.maxCountInCluster);

		for (int i = 0; i < islandsToCreate; i++)
		{
			Vector3 displacementAmt = new Vector3(
				Random.Range(TerrainManager.minDistance.x, TerrainManager.maxDistance.x),
				Random.Range(TerrainManager.minDistance.y, TerrainManager.maxDistance.y),
				Random.Range(TerrainManager.minDistance.z, TerrainManager.maxDistance.z));

			int disDirX = Random.Range(0, 100) > 50 ? 1 : -1;
			int disDirY = Random.Range(0, 100) > 30 ? 1 : -1;
			int disDirZ = Random.Range(0, 100) > 50 ? 1 : -1;
			Vector3 displacement = new Vector3(displacementAmt.x * disDirX, displacementAmt.y * disDirY, displacementAmt.z * disDirZ);

			//Create an island.
			GameObject newIsland = (GameObject)GameObject.Instantiate(islandPrefabs[Random.Range(0, islandPrefabs.Length)], transform.position, Quaternion.identity);

			newIsland.transform.position += displacement;

			ApplyRandomScale(newIsland, false);
			ApplyRandomRotation(newIsland);
			ApplyRandomTexturing(newIsland);
			ApplyIslandParent(newIsland);
		}
	}

	public void ApplyRandomScale(GameObject island, bool poisson)
	{
		Vector3 scale = Vector3.zero;

		if (poisson)
		{
			scale = new Vector3(
						Random.Range(TerrainManager.poissonMinScale.x + sizeBonus, TerrainManager.poissonMaxScale.x + sizeBonus),
						Random.Range(TerrainManager.poissonMinScale.y + sizeBonus, TerrainManager.poissonMaxScale.y + (sizeBonus / 2)),
						Random.Range(TerrainManager.poissonMinScale.z + sizeBonus, TerrainManager.poissonMaxScale.z + sizeBonus));
		}
		else
		{
			scale = new Vector3(
						Random.Range(TerrainManager.minScale.x, TerrainManager.maxScale.x),
						Random.Range(TerrainManager.minScale.y, TerrainManager.maxScale.y),
						Random.Range(TerrainManager.minScale.z, TerrainManager.maxScale.z));
		}
		float smallest = Mathf.Min(scale.x, scale.z);
		smallest = Mathf.Min(scale.y, smallest);

		if (scale.x == smallest)
		{
			scale.x = scale.y;
		}
		else if (scale.z == smallest)
		{
			scale.z = scale.y;
		}
		scale.y = smallest;

		island.transform.localScale = scale;
	}

	public void ApplyRandomRotation(GameObject island)
	{
		Vector3 rndRotation = transform.eulerAngles;
		rndRotation = new Vector3(
			Random.Range(TerrainManager.minTilt, TerrainManager.maxTilt),
			Random.Range(0, 360),
			Random.Range(TerrainManager.minTilt, TerrainManager.maxTilt));
		island.transform.eulerAngles = rndRotation;
	}

	public void ApplyRandomTexturing(GameObject island)
	{
		island.renderer.material.mainTexture = TerrainManager.Instance.textures[Random.Range(0, TerrainManager.Instance.textures.Count)];
		island.renderer.material.mainTextureScale = new Vector2(Random.Range(5, 25), Random.Range(5, 25));
		island.renderer.material.color = new Color(Random.Range(.7f, 1f), Random.Range(.7f, 1f), Random.Range(.7f, 1f));
	}

	public void ApplyIslandParent(GameObject island)
	{
		island.transform.SetParent(transform);
	}

	public void LocalizeNeighbors()
	{

	}
}

