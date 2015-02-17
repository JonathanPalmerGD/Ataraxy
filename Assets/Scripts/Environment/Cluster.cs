using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using System.Linq;

public class Cluster : WorldObject
{
	public int encounterCounter = 0;
	public Cluster[] neighborClusters = new Cluster[8];
	public int neighborsPopulated = 0;
	public List<Island> platforms;
	public int poissonKVal = 20;
	public float sizeBonus = 0;
	public GameObject clusterContents;

	private Vector3 start;
	public bool inPlace = false;

	public float riseCounter = 0;
	private float riseDuration = 6.0f;

	public float tiltDeviation;

	public bool LargeIsland = true;
	public bool RandomScale = true;
	public bool RandomRotation = true;
	public bool RandomTexture = true;
	public bool RandomLandmarks = true;

	public override void Start()
	{
		clusterContents = transform.FindChild("Contents").gameObject;
		start = clusterContents.transform.position;

		transform.FindChild("Cylinder").gameObject.renderer.enabled = false;
		TerrainManager.Instance.RegisterCluster(this);
		gameObject.name = "Cluster: " + Nomenclature.GetName(Random.Range(0, 12), Random.Range(0, 12), Random.Range(0, 12), Random.Range(0, 12));

		poissonKVal = Random.Range(TerrainManager.poissonMinK, TerrainManager.poissonMaxK);
		sizeBonus = Random.Range(0, TerrainManager.minSizeHor);
		tiltDeviation = Random.Range(TerrainManager.minTiltDeviation, TerrainManager.maxTiltDeviation);
		//Debug.Log(tiltDeviation);

		//This is to help ensure that clusters with large islands will always have random scale applied to avoid sparse terrain
		RandomScale = Random.Range(0, 10) - sizeBonus / 10 < 7.5f;
		//LargeIsland = Random.Range(0, 10) < 7;
		LargeIsland = false;
		RandomRotation = Random.Range(0, 10) < 8;
		RandomTexture = Random.Range(0, 10) < 8;
		RandomLandmarks = Random.Range(0, 10) > 7;

		CreateIslandsPoissonApproach();
		if (RandomLandmarks)
		{
			//if (Random.Range(0, 100) > 90)
			//{
			//CreateLandmarksPoissonApproach();
			//}
		}

		ConfigureIslands();
		
		base.Start();
		gameObject.tag = "Cluster";
		iTween.MoveBy(clusterContents, iTween.Hash("y", TerrainManager.underworldYOffset, "easeType", "easeOutCirc", "speed", 35, "loopType", "none", "delay", .1));
		//iTween.MoveBy(clusterContents, iTween.Hash("y", TerrainManager.underworldYOffset, "easeType", "easeOutBounce", "speed", 50, "loopType", "none", "delay", .1));
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
	
	#region Island Path Nodes
	/// <summary>
	/// Determines island neighbors for all existing islands.
	/// </summary>
	public void ConfigureIslands()
	{
		for (int i = 0; i < platforms.Count; i++)
		{
			ConfigureIsland(platforms[i]);
		}

		//To check if we're making progression impossible clusters.
		if(encounterCounter == 0)
		{
			//Debug.Log("Cluster generated with 0 valid encounters\n");
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

		//Debug.Log("Finished Island (" + island.name + ") Configuration.\nI have " + island.nearIslands.Count + " neighbors\n");

		island.ConfigureNodes();

		island.PlaceRandomEncounter();
		//island.PlaceRandomEnemy();
		island.PlaceRandomObject();
	}
	#endregion

	#region Approaches
	public void CreateIslandsPoissonApproach()
	{
		PoissonDiscSampler pds = new PoissonDiscSampler(TerrainManager.clusterSize.x, TerrainManager.clusterSize.z, 16 + (sizeBonus * 1.2f), poissonKVal);
		foreach (Vector2 sample in pds.Samples())
		{
			GameObject newIsland = null;
			if (TerrainManager.Instance.islandPrefabs.Count > 0)
			{
				
				float yOffset = Random.Range(TerrainManager.minDistance.y, TerrainManager.maxDistance.y);

				Vector3 newPosition = new Vector3(sample.x + transform.position.x, transform.position.y + yOffset, sample.y + transform.position.z);
				newPosition -= TerrainManager.clusterSize / 2;

				newIsland = (GameObject)GameObject.Instantiate(
					TerrainManager.Instance.islandPrefabs[Random.Range(0, TerrainManager.Instance.islandPrefabs.Count)], 
					newPosition, Quaternion.identity);

				newIsland.GetComponent<Island>().Init();

				ApplyRandomScale(newIsland, true);
				ApplyRandomTexturing(newIsland);
				ApplyIslandParent(newIsland);

				//newIsland.GetComponent<Island>().CreatePathNodes();
				//ApplyRandomRotation(newIsland);

				platforms.Add(newIsland.GetComponent<Island>());
			}
		}

		CreateLargeIsland();
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
				ApplyRandomRotation(newLandmark);
				//ApplyRandomTexturing(newLandmark);
				ApplyIslandParent(newLandmark);

			}
		}
	}
	#endregion

	#region Island Modification
	public void ApplyRandomScale(GameObject island, bool poisson)
	{
		if (RandomScale)
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

			island.transform.localScale = scale;
		}
	}

	public void ApplyRandomRotation(GameObject island)
	{
		if (RandomRotation)
		{
			Vector3 rndRotation = transform.eulerAngles;
			rndRotation = new Vector3(
				Random.Range(-tiltDeviation / 2, tiltDeviation / 2),
				Random.Range(0, 360),
				Random.Range(-tiltDeviation / 2, tiltDeviation / 2));
			island.transform.eulerAngles = rndRotation;
		}
	}

	public void ApplyRandomTexturing(GameObject island)
	{
		if (RandomTexture)
		{
			island.renderer.material.mainTexture = TerrainManager.Instance.textures[Random.Range(0, TerrainManager.Instance.textures.Count)];
			island.renderer.material.mainTextureScale = new Vector2(Random.Range(5, 25), Random.Range(5, 25));
			island.renderer.material.color = new Color(Random.Range(.7f, 1f), Random.Range(.7f, 1f), Random.Range(.7f, 1f));
		}
	}

	public void ApplyIslandParent(GameObject island)
	{
		island.transform.SetParent(clusterContents.transform);
	}

	public void CreateLargeIsland()
	{
		if (LargeIsland)
		{
			Debug.Log("Creating Large Island\n");
			float xPos = transform.position.x - Random.Range(-TerrainManager.clusterSize.x/2, TerrainManager.clusterSize.x/2);
			float zPos = transform.position.z - Random.Range(-TerrainManager.clusterSize.z/2, TerrainManager.clusterSize.z/2);

			Vector3 megaIslandPosition = new Vector3(xPos, transform.position.y, zPos);

			Vector3 randomIslandPoint = megaIslandPosition;
			
			Vector3 scale = Vector3.zero;

			scale = new Vector3(Random.Range(60, 75), Random.Range(5, 10), Random.Range(60, 75));

			float dist = Mathf.Min(scale.x, scale.z);

			Collider[] c = Physics.OverlapSphere(randomIslandPoint, dist);
			for (int i = 0; i < c.Length; i++)
			{
				if (c[i].gameObject.tag == "Island")
				{
					platforms.Remove(c[i].GetComponent<Island>());
					Destroy(c[i].gameObject);
				}
			}

			
			GameObject newIsland = (GameObject)GameObject.Instantiate(
				TerrainManager.Instance.islandPrefabs[Random.Range(0, TerrainManager.Instance.islandPrefabs.Count)],
				megaIslandPosition, Quaternion.identity);

			newIsland.GetComponent<Island>().Init();


			
			newIsland.transform.localScale = scale;

			ApplyRandomTexturing(newIsland);

			ApplyIslandParent(newIsland);

			newIsland.GetComponent<Island>().CreatePathNodes();
			ApplyRandomRotation(newIsland);

			platforms.Add(newIsland.GetComponent<Island>());

			//Pick a random point within the area.
			//Do a spherecast of that point
			//Make an island slightly larger than that point (very flat)
			//Delete all islands that touched that point.

			//float dist = Mathf.Max(island.transform.localScale.x, island.transform.localScale.z) + TerrainManager.IslandNeighborDist;
			//Debug.Log("Distance from island: " + island.name + "\n" + (int)dist + " units.\n");
			//Collider[] c = Physics.OverlapSphere(island.transform.position, dist);
		}
	}
	#endregion

	public void LocalizeNeighbors()
	{

	}
}

