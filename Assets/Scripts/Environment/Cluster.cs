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

	public float dangerLevel = 35;
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
		string ct = cType.ToString();
		gameObject.name = ct.Substring(0, 3) + " Cluster: " + Nomenclature.GetName(Random.Range(0, 12), Random.Range(0, 12), Random.Range(0, 12), Random.Range(0, 12));

		ConfigureBiomeDangerLevel();

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
			int islandApproach = Random.Range(0, 10);
			if (islandApproach < 4)
			{
				CreateIslandsPoisson();
			}
			else if (islandApproach < 7)
			{
				CreateIslandsGrid();
			}
			else
			{
				CreateIslandsLayered();
			}
			ConfigureClusterFeatures();
		}
	
		if (RandomLandmarks)
		{
			Invoke("CreateNeighborLandmark", 0.5f);
		}

		base.Start();
		gameObject.tag = "Cluster";
		//TweenInIslands();
		float riseSpeedVariation = Random.Range(-(riseSpeed - 35), 115);

		iTween.MoveBy(clusterContents, iTween.Hash("y", TerrainManager.underworldYOffset, "easeType", "easeOutCirc", "speed", riseSpeed + riseSpeedVariation, "loopType", "none", "delay", .2));

		float timeDelay = .6f + TerrainManager.underworldYOffset / (riseSpeed + riseSpeedVariation);
		Invoke("ConfigureClusterNodes", timeDelay);
		
		transform.RotateAround(Vector3.up, Random.Range(0.0f, 360.0f)); 
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

	public void ConfigureBiomeDangerLevel()
	{
		float dangerOfNeighbors = dangerLevel;

		/*if (float.IsNaN(dangerLevel))
		{
			dangerLevel = 35;
		}*/

		//Look at all neighbors and add their danger levels up
		for (int i = 0; i < neighborClusters.Length; i++)
		{
			if (neighborClusters[i] != null)
			{
				//if (float.IsNaN(neighborClusters[i].dangerLevel))
				//{
				//	neighborClusters[i].dangerLevel = 35;
				//}

				//Debug.Log("Danger of neighbor: " + neighborClusters[i].dangerLevel + "\n");
				dangerOfNeighbors += neighborClusters[i].dangerLevel;
			}
		}

		//Debug.Log("Danger Level before: " + dangerLevel + "\n" + dangerOfNeighbors);
		//Average the danger level. Add/remove a slight element. Make sure it is between 0 and 100.
		dangerLevel = Mathf.Clamp(dangerOfNeighbors / (neighborsPopulated + 1) + Random.Range(-TerrainManager.ClusterDangerRangeVariation, TerrainManager.ClusterDangerRangeVariation), 0, 100);
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

					//If no neighbor
					if (nextNeighbor == null)
					{
						potentialNeighbors.Remove(nextTry);
					}
					//If that neighbor's danger level is similar to ours
					else if (Mathf.Abs(nextNeighbor.dangerLevel - dangerLevel) < TerrainManager.ClusterInheritanceTolerance)
					{
						islandMaterials.Add(nextNeighbor.islandMaterials[Random.Range(0, nextNeighbor.islandMaterials.Count)]);
						potentialNeighbors.Remove(nextTry);
					}
					//Disqualify our different neighbor.
					else
					{
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
			//Get an appropriate material. If we don't have any, get several
			for (int i = 0; i < 3 - alreadySetMats; i++)
			{
				//The terrain Manager will give us one appropriate to our danger level.
				//Currently randomly generates.
				islandMaterials.Add(TerrainManager.Instance.GetMaterial(dangerLevel));

				//Old random generation approach
				//newMat.mainTexture = TerrainManager.Instance.textures[Random.Range(0, TerrainManager.Instance.textures.Count)];
				//newMat.mainTextureScale = new Vector2(Random.Range(5, 25), Random.Range(5, 25));
				//newMat.color = new Color(Random.Range(.7f, 1f), Random.Range(.7f, 1f), Random.Range(.7f, 1f));
				//islandMaterials.Add(newMat);
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
	public void CreateIslandsPoisson()
	{
		PoissonDiscSampler pds = new PoissonDiscSampler(TerrainManager.clusterSize.x - 5, TerrainManager.clusterSize.z - 5, 16 + (sizeBonus * 1.2f), poissonKVal);
		
		Island newIsland = null;
		GameObject newIslandGO = null;
		GameObject choosenIslandPrefab = null;
		Vector3 newPosition;
		float yOffset;

		#region PD Sample Loop
		foreach (Vector2 sample in pds.Samples())
		{
			//Island newIsland
			if (TerrainManager.Instance.islandPrefabs.Count > 0)
			{
				//Select an island prefab.
				choosenIslandPrefab = TerrainManager.Instance.islandPrefabs[Random.Range(0, TerrainManager.Instance.islandPrefabs.Count)];
				
				yOffset = Random.Range(TerrainManager.minDistance.y, TerrainManager.maxDistance.y);

				//Set the new position
				newPosition = new Vector3(sample.x + transform.position.x, transform.position.y + yOffset, sample.y + transform.position.z);
				newPosition -= TerrainManager.clusterSize / 2;

				//Instantiate the new game object.
				newIslandGO = (GameObject)GameObject.Instantiate(choosenIslandPrefab, 
					newPosition, Quaternion.identity);

				newIsland = newIslandGO.GetComponent<Island>();

				ApplyIslandParent(newIsland);

				newIsland.Init();

				ApplyRandomScale(newIsland, true);
				ApplyRandomTexturing(newIsland);

				platforms.Add(newIslandGO.GetComponent<Island>());
			}
		}
		#endregion
	}

	public void CreateIslandsGrid()
	{
		Island newIsland = null;
		GameObject newIslandGO = null;
		GameObject choosenIslandPrefab = TerrainManager.Instance.islandPrefabs[Random.Range(0, TerrainManager.Instance.islandPrefabs.Count)];

		float xDiv = Random.Range(1, 5);
		float zDiv = Random.Range(1, 5);

		float xOffset, yOffset, zOffset;

		float spacing = Random.Range(0, 10);

		Vector3 newPosition;
		Vector3 scale = new Vector3(
					(TerrainManager.clusterSize.x / xDiv) - spacing,
					Random.Range(TerrainManager.minScale.y, TerrainManager.maxScale.y),
					(TerrainManager.clusterSize.z / zDiv) - spacing);

		#region Division Loops
		for (int i = 0; i < xDiv; i++)
		{
			for (int j = 0; j < zDiv; j++)
			{
				xOffset = Random.Range(-2, 2);
				yOffset = Random.Range(TerrainManager.minDistance.y, TerrainManager.maxDistance.y);
				zOffset = Random.Range(-2, 2);

				newPosition = new Vector3((TerrainManager.clusterSize.x / xDiv) * i + xOffset + transform.position.x, transform.position.y + yOffset, (TerrainManager.clusterSize.z / zDiv) * j + zOffset + transform.position.z);
				newPosition -= TerrainManager.clusterSize / 2;

				newIslandGO = (GameObject)GameObject.Instantiate(choosenIslandPrefab,
					newPosition, Quaternion.identity);

				newIsland = newIslandGO.GetComponent<Island>();

				ApplyIslandParent(newIsland);

				newIsland.Init();

				ApplySpecificScale(newIsland, scale, true);
				ApplyRandomTexturing(newIsland);

				platforms.Add(newIslandGO.GetComponent<Island>());
			}
		}
		#endregion
	}

	public void CreateIslandsLayered()
	{
		PoissonDiscSampler pds = new PoissonDiscSampler(TerrainManager.clusterSize.x - 5, TerrainManager.clusterSize.z - 5, 16 + (sizeBonus * 1.2f), poissonKVal);

		Island newIsland = null;
		GameObject newIslandGO = null;
		GameObject choosenIslandPrefab = null;
		Vector3 newPosition;
		float yOffset, xOffset, zOffset;

		#region PD Sample Loop
		foreach(Vector2 sample in pds.Samples())
		{
			//Chance to outright skip this sample.
			if (Random.Range(0, 10) < 5)
			{
				//Island newIsland
				if (TerrainManager.Instance.islandPrefabs.Count > 0)
				{
					for (int i = 0; i < 2; i++)
					{
						xOffset = Random.Range(-3, 3);
						yOffset = Random.Range(TerrainManager.minDistance.y, TerrainManager.maxDistance.y) + i * (20 + Random.Range(0, 25));
						zOffset = Random.Range(-3, 3);

						//Select an island prefab.
						choosenIslandPrefab = TerrainManager.Instance.islandPrefabs[Random.Range(0, TerrainManager.Instance.islandPrefabs.Count)];

						//Set the new position
						newPosition = new Vector3(sample.x + transform.position.x, transform.position.y + yOffset, sample.y + transform.position.z);
						newPosition -= TerrainManager.clusterSize / 2;

						//Instantiate the new game object.
						newIslandGO = (GameObject)GameObject.Instantiate(choosenIslandPrefab,
							newPosition, Quaternion.identity);

						newIsland = newIslandGO.GetComponent<Island>();

						ApplyIslandParent(newIsland);

						newIsland.Init();

						ApplyRandomScale(newIsland, true);
						ApplyRandomTexturing(newIsland);

						platforms.Add(newIslandGO.GetComponent<Island>());

					}
				}
			}
		}
		#endregion
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
	public void ApplySpecificScale(Island island, Vector3 scale, bool poisson)
	{
		if (RandomScale && !island.specialIsland)
		{
			island.transform.localScale = scale;
		}
	}
	
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

