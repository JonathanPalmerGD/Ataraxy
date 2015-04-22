using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using System.Linq;

public class Cluster : WorldObject
{
	public int encounterCounter = 0;
	#region Neighbor Information
	public Cluster[] neighborClusters = new Cluster[8];
	//This variable exists because we use an array of clusters for storing neighbors. We need to 
	/// <summary>
	/// How many non-null clusters are in neighborClusters currently.
	/// Since order of the array matters, we have to track this ourself.
	/// </summary>
	public int neighborsPopulated = 0;
	#endregion
	#region Island Platform Information
	public List<Island> platforms;
	public GameObject clusterContents;
	public bool generateIslands = true;
	#endregion

	#region Biome Properties
	public int poissonKVal = 20;
	public float sizeBonus = 0;

	public float tiltDeviation;

	public bool RandomScale = true;
	public bool RandomRotation = true;
	public bool RandomTexture = true;
	public bool RandomLandmarks = true;

	public List<Material> islandMaterials;
	#endregion

	#region Cluster Type
	public enum ClusterType { Biome = 0, Boss, Rare, Landmark };
	public ClusterType cType = ClusterType.Biome;
	#endregion

	#region Rising
	//private Vector3 start;
	public bool inPlace = false;

	public float riseCounter = 0;
	private float riseSpeed = 35.0f;
	#endregion

	public override void Start()
	{
		clusterContents = transform.FindChild("Contents").gameObject;
		//start = clusterContents.transform.position;

		transform.FindChild("Cylinder").gameObject.renderer.enabled = false;
		TerrainManager.Instance.RegisterCluster(this);
		gameObject.name = "Cluster: " + Nomenclature.GetName(Random.Range(0, 12), Random.Range(0, 12), Random.Range(0, 12), Random.Range(0, 12));

		poissonKVal = Random.Range(TerrainManager.poissonMinK, TerrainManager.poissonMaxK);
		sizeBonus = Random.Range(0, TerrainManager.minSizeHor);
		tiltDeviation = Random.Range(TerrainManager.minTiltDeviation, TerrainManager.maxTiltDeviation);

		//This is to help ensure that clusters with large islands will always have random scale applied to avoid sparse terrain
		RandomScale = Random.Range(0, 10) - sizeBonus / 10 < 7.5f;
		
		RandomRotation = Random.Range(0, 10) < 8;
		RandomTexture = true;

		if (islandMaterials == null || islandMaterials.Count < 1) 
		{
			ConfigureBiomeMaterials();
		}

		RandomLandmarks = Random.Range(0, 10) > 7;

		if (generateIslands)
		{
			CreateIslandsPoissonApproach();
			ConfigureClusterFeatures();
		}
	
		if (RandomLandmarks)
		{
			Invoke("CreateNeighborLandmark", 0.5f);
		}

		base.Start();
		gameObject.tag = "Cluster";
		//TweenInIslands();
		float riseSpeedVariation = Random.Range(-(riseSpeed - 5), 115);

		iTween.MoveBy(clusterContents, iTween.Hash("y", TerrainManager.underworldYOffset, "easeType", "easeOutCirc", "speed", riseSpeed + riseSpeedVariation, "loopType", "none", "delay", .2));

		float timeDelay = .6f + TerrainManager.underworldYOffset / (riseSpeed + riseSpeedVariation);
		Invoke("ConfigureClusterNodes", timeDelay);
		//iTween.MoveBy(clusterContents, iTween.Hash("y", TerrainManager.underworldYOffset, "easeType", "easeOutBounce", "speed", 50, "loopType", "none", "delay", .1));
	}

	private void CreateNeighborLandmark()
	{
		RandomLandmarks = false; 
		if (cType != ClusterType.Landmark)
		{
			TerrainManager.Instance.CreateNewCluster(this, ClusterType.Landmark);
		}
	}

	public override void Update() 
	{
		//Replced by the iTween call.
		/*if (!inPlace)
		{
			riseCounter += Time.deltaTime;

			//clusterContents.transform.position = Vector3.Lerp(start, start + Vector3.up * TerrainManager.underworldYOffset, riseCounter / riseDuration);

			if (riseCounter > riseDuration)
			{
				inPlace = true;
			}
		}*/
		base.Update();
	}
	
	#region Island Configuration
	#region Old Island Configuration
	/// <summary>
	/// Determines island neighbors for all existing islands.
	/// </summary>
	public void ConfigureIslands()
	{
		/*for (int i = 0; i < platforms.Count; i++)
		{
			ConfigureIsland(platforms[i]);
		}*/

		if (TerrainManager.CreateIslandFeatures)
		{
			//To check if we're making progression impossible clusters.
			if(encounterCounter == 0)
			{
				//Select the largest island, make an encounter there
				//Debug.Log("Forcing Random Encounter");
				platforms[Random.Range(0, platforms.Count)].PlaceRandomEncounter(true);
				//Debug.Log("Cluster generated with 0 valid encounters\n");
			}
		}
	}

	/// <summary>
	/// Perform Physics Overlap Spheres to determine an island's neighbors.
	/// Then register the relationship with both islands.
	/// </summary>
	/// <param name="island"></param>
	public void ConfigureIsland(Island island)
	{
		island.Family = this;
		island.CreatePathNodes();

		float dist = Mathf.Max(island.transform.localScale.x, island.transform.localScale.z) + TerrainManager.IslandNeighborDist;
		//Debug.Log("Distance from island: " + island.name + "\n" + (int)dist + " units.\n");
		Collider[] c = Physics.OverlapSphere(island.transform.position, dist);

		#region Neighbor Overlap Spheres
		//For each collider
		for (int i = 0; i < c.Length; i++)
		{
			if (c[i].gameObject != null && c[i].gameObject.tag == "Island")
			{
				if (island.gameObject != c[i].gameObject)
				{
					Island foundNeighbor = c[i].GetComponent<Island>();

					if (foundNeighbor != null)
					{
						//If we don't have that island registered
						if (!island.nearIslands.Contains(foundNeighbor))
						{
							//I don't think we need to check if they have us registered, since neighbors always register both involved islands.

							//Add us to both
							island.nearIslands.Add(foundNeighbor);
							foundNeighbor.nearIslands.Add(island);

							//Debug.DrawLine(island.transform.position, foundNeighbor.transform.position, Color.blue, 36.0f);
						}
					}
				}
				else
				{
					//To make sure we are dodging illegal neighbors.
					//Debug.Log(c[i].gameObject.name + "\nMy Name: " + island.name);
				}
			}
		}
		#endregion

		//Debug.Log("Finished Island (" + island.name + ") Configuration.\nI have " + island.nearIslands.Count + " neighbors\n");

		island.ConfigureNodes();
		
		if (TerrainManager.CreateIslandFeatures)
		{
			island.PlaceRandomEncounter();
			//island.PlaceRandomEnemy();
			island.PlaceRandomObject();
			island.PlaceCosmeticObjects();
		}
	}
	#endregion

	public void ConfigureClusterFeatures()
	{
		for (int i = 0; i < platforms.Count; i++)
		{
			platforms[i].Family = this;
			ProcessFeatures(platforms[i]);
		}
	}

	public void ConfigureBiomeMaterials(List<Material> biomeMats = null)
	{
		if (biomeMats == null)
		{
			islandMaterials = new List<Material>();

			Material newMat = new Material(Shader.Find("Diffuse"));

			//Concoct the list of all untried neighbors.
			List<int> potentialNeighbors = new List<int>();
			for (int j = 0; j < neighborClusters.Length; j++)
			{
				potentialNeighbors.Add(j);
			}

			//We want 2 of neighbor materials, 1 random.
			for (int i = 0; i < 2; i++)
			{
				while (potentialNeighbors.Count > 0)
				{
					//Pick a random potential neighbor
					int nextTry = potentialNeighbors[Random.Range(0, potentialNeighbors.Count)];
					Cluster nextNeighbor = neighborClusters[nextTry];

					if (nextNeighbor == null)
					{
						potentialNeighbors.Remove(nextTry);
					}
					else
					{
						islandMaterials.Add(nextNeighbor.islandMaterials[Random.Range(0, nextNeighbor.islandMaterials.Count)]);
						potentialNeighbors.Remove(nextTry);
					}

					if (islandMaterials.Count >= 2)
					{
						potentialNeighbors.Clear();
					}
				}
			}

			int alreadySetMats = islandMaterials.Count;
			//string report = alreadySetMats + " mats already set\n";
			//Make a random mat. Make up to 3 if we had no neighbors
			for (int i = 0; i < 3 - alreadySetMats; i++)
			{
				newMat.mainTexture = TerrainManager.Instance.textures[Random.Range(0, TerrainManager.Instance.textures.Count)];
				newMat.mainTextureScale = new Vector2(Random.Range(5, 25), Random.Range(5, 25));
				newMat.color = new Color(Random.Range(.7f, 1f), Random.Range(.7f, 1f), Random.Range(.7f, 1f));
				islandMaterials.Add(newMat);
			}

			//Debug.Log(report + islandMaterials.Count + " after second loop\n");
		}
		else
		{
			for (int i = 0; i < biomeMats.Count; i++)
			{
				islandMaterials.Add(biomeMats[i]);
			}
		}
	}

	/// <summary>
	/// For bringing in individual islands
	/// </summary>
	public void TweenInIslands()
	{
		//This has been scrapped for now given that the islands themselves would need parent objects to ensure island features & enemies rose with the island.
		for (int i = 0; i < platforms.Count; i++)
		{
			float riseSpeedVariation = Random.Range(-15, 85);
			iTween.MoveBy(platforms[i].gameObject.transform.parent.gameObject, iTween.Hash("y", TerrainManager.underworldYOffset, "easeType", "easeOutCirc", "speed", riseSpeed + riseSpeedVariation, "loopType", "none", "delay", .1));
		}
	}

	public void ConfigureClusterNodes()
	{
		//inPlace is only set once we have finished rising.
		//It is checked when handling neighbor connection setup to avoid setting incorrect neighbors.
		inPlace = true;

		for (int i = 0; i < platforms.Count; i++)
		{
			ProcessNodes(platforms[i]);
		}
	}

	public void ProcessFeatures(Island island)
	{
		if (TerrainManager.CreateIslandFeatures)
		{
			island.PlaceRandomEncounter();
			//island.PlaceRandomEnemy();
			island.PlaceRandomObject();
			island.PlaceCosmeticObjects();
		}
	}

	public void ProcessNodes(Island island)
	{
		island.CreatePathNodes();

		float dist = Mathf.Max(island.transform.localScale.x, island.transform.localScale.z) + TerrainManager.IslandNeighborDist;
		//Debug.Log("Distance from island: " + island.name + "\n" + (int)dist + " units.\n");
		Collider[] c = Physics.OverlapSphere(island.transform.position, dist);

		#region Neighbor Overlap Spheres
		//For each collider
		for (int i = 0; i < c.Length; i++)
		{
			if (c[i].gameObject != null && c[i].gameObject.tag == "Island")
			{
				if (island.gameObject != c[i].gameObject)
				{
					Island foundNeighbor = c[i].GetComponent<Island>();

					//If the neighbor is valid & that cluster is in place
					if (foundNeighbor != null && foundNeighbor.Family && foundNeighbor.Family.inPlace)
					{
						//If we don't have that island registered
						if (!island.nearIslands.Contains(foundNeighbor))
						{
							//I don't think we need to check if they have us registered, since neighbors always register both involved islands.

							//Add us to both
							island.nearIslands.Add(foundNeighbor);
							foundNeighbor.nearIslands.Add(island);

							//Debug.DrawLine(island.transform.position, foundNeighbor.transform.position, Color.blue, 36.0f);
						}
					}
				}
				else
				{
					//To make sure we are dodging illegal neighbors.
					//Debug.Log(c[i].gameObject.name + "\nMy Name: " + island.name);
				}
			}
		}
		#endregion

		//Debug.Log("Finished Island (" + island.name + ") Configuration.\nI have " + island.nearIslands.Count + " neighbors\n");

		island.ConfigureNodes();
	}
	#endregion

	#region Approaches
	public void CreateIslandsPoissonApproach()
	{
		PoissonDiscSampler pds = new PoissonDiscSampler(TerrainManager.clusterSize.x, TerrainManager.clusterSize.z, 16 + (sizeBonus * 1.2f), poissonKVal);
		foreach (Vector2 sample in pds.Samples())
		{
			Island newIsland = null;
			GameObject newIslandGO = null;
			GameObject choosenIslandPrefab = null;
			//Island newIsland
			if (TerrainManager.Instance.islandPrefabs.Count > 0)
			{
				//Select an island prefab.
				choosenIslandPrefab = TerrainManager.Instance.islandPrefabs[Random.Range(0, TerrainManager.Instance.islandPrefabs.Count)];
				
				float yOffset = Random.Range(TerrainManager.minDistance.y, TerrainManager.maxDistance.y);

				Vector3 newPosition = new Vector3(sample.x + transform.position.x, transform.position.y + yOffset, sample.y + transform.position.z);
				newPosition -= TerrainManager.clusterSize / 2;

				newIslandGO = (GameObject)GameObject.Instantiate(choosenIslandPrefab, 
					newPosition, Quaternion.identity);

				newIsland = newIslandGO.GetComponent<Island>();

				ApplyIslandParent(newIsland);

				newIsland.Init();

				ApplyRandomScale(newIsland, true);
				ApplyRandomTexturing(newIsland);

				//newIsland.GetComponent<Island>().CreatePathNodes();
				//ApplyRandomRotation(newIsland);

				platforms.Add(newIslandGO.GetComponent<Island>());
			}
		}

		//CreateLargeIsland();
	}

	public void CreateLandmarksPoissonApproach()
	{
		PoissonDiscSampler pds = new PoissonDiscSampler(100, 100, 75, poissonKVal);
		foreach (Vector2 sample in pds.Samples())
		{
			GameObject newLandmark = null;
			if (TerrainManager.Instance.islandPrefabs.Count > 0)
			{
				float yOffset = Random.Range(TerrainManager.minDistance.y, TerrainManager.maxDistance.y);

				Vector3 newPosition = new Vector3(sample.x + transform.position.x, transform.position.y + yOffset, sample.y + transform.position.z);
				newPosition -= TerrainManager.clusterSize / 2;
				
				newLandmark = (GameObject)GameObject.Instantiate(
					TerrainManager.Instance.landmarkPrefabs[Random.Range(0, TerrainManager.Instance.landmarkPrefabs.Count)], 
					newPosition, Quaternion.identity);

				//ApplyRandomScale(newLandmark, true);
				//ApplyRandomRotation(newLandmark);
				//ApplyRandomTexturing(newLandmark);
				//ApplyIslandParent(newLandmark);

			}
		}
	}
	#endregion

	#region Island Modification
	public void ApplyRandomScale(Island island, bool poisson)
	{
		if (RandomScale && !island.specialIsland)
		{
			Vector3 scale = Vector3.zero;

			if (poisson)
			{
				scale = new Vector3(
							Random.Range(TerrainManager.poissonMinScale.x + sizeBonus, TerrainManager.poissonMaxScale.x + sizeBonus),
							Random.Range(TerrainManager.poissonMinScale.y + sizeBonus, TerrainManager.poissonMaxScale.y),
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
			if (island.miniIsland)
			{
				scale = new Vector3(scale.x / 2.5f, scale.y / 2.5f, scale.z / 2.5f);
			}

			island.transform.localScale = scale;
		}
	}

	public void ApplyRandomRotation(Island island)
	{
		if (RandomRotation && !island.specialIsland)
		{
			Vector3 rndRotation = transform.eulerAngles;
			rndRotation = new Vector3(
				Random.Range(-tiltDeviation / 2, tiltDeviation / 2),
				Random.Range(0, 360),
				Random.Range(-tiltDeviation / 2, tiltDeviation / 2));
			island.transform.eulerAngles = rndRotation;
		}
	}

	public void ApplyRandomTexturing(Island island)
	{
		if (islandMaterials != null)
		{
			island.renderer.material = islandMaterials[Random.Range(0, islandMaterials.Count)];
		}
		else
		{
			/*if (RandomTexture)
			{
				island.renderer.material = TerrainManager.Instance.terrainMats[Random.Range(0, TerrainManager.Instance.terrainMats.Count)];
				/*island.renderer.material.mainTextureScale = new Vector2(Random.Range(5, 25), Random.Range(5, 25));
				island.renderer.material.color = new Color(Random.Range(.7f, 1f), Random.Range(.7f, 1f), Random.Range(.7f, 1f));*/
			//}

			if (RandomTexture/* && !island.specialIsland*/)
			{
				island.renderer.material.mainTexture = TerrainManager.Instance.textures[Random.Range(0, TerrainManager.Instance.textures.Count)];
				island.renderer.material.mainTextureScale = new Vector2(Random.Range(5, 25), Random.Range(5, 25));
				island.renderer.material.color = new Color(Random.Range(.7f, 1f), Random.Range(.7f, 1f), Random.Range(.7f, 1f));
			}
		}
	}

	public void ApplyIslandParent(Island island)
	{
		island.transform.SetParent(clusterContents.transform);
	}
	#endregion

	public void LocalizeNeighbors()
	{

	}
}

