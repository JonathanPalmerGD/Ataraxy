using UnityEngine;
using System.Linq;
using System.Collections;
using System.Collections.Generic;

public class TerrainManager : Singleton<TerrainManager>
{
	public static float minSizeHor = 40;
	public static Vector3 minDistance = new Vector3(10, -12, 10);
	public static Vector3 maxDistance = new Vector3(50, 5, 50);
	public static Vector3 minScale = new Vector3(3, 2, 3);
	public static Vector3 maxScale = new Vector3(45, 15, 45);
	public static Vector3 poissonMinScale = new Vector3(10, 1, 10);
	public static Vector3 poissonMaxScale = new Vector3(18, 12, 18);
	public static Vector3 clusterSize = new Vector3(90, 20, 90);
	public static int poissonMinK = 20;
	public static int poissonMaxK = 30;
	public static float minTiltDeviation = 2;
	public static float maxTiltDeviation = 40;
	public static float minTilt = -8;
	public static float maxTilt = 8;
	public static int minCountInCluster = 5;
	public static int maxCountInCluster = 9;

	public GameObject clusterPrefab;

	public List<Cluster> clusters;
	public List<Texture2D> textures;
	public List<GameObject> terrainFeatures;
	public List<GameObject> enemies;
	public List<GameObject> islandPrefabs;
	public List<GameObject> landmarkPrefabs;

	public override void Awake()
	{
		base.Awake();

		clusters = new List<Cluster>();
		clusterPrefab = Resources.Load<GameObject>("Cluster");
		textures = Resources.LoadAll<Texture2D>("Terrain").ToList();
		islandPrefabs = Resources.LoadAll<GameObject>("Islands").ToList();
		landmarkPrefabs = Resources.LoadAll<GameObject>("Landmarks").ToList();
		terrainFeatures = Resources.LoadAll<GameObject>("TerrainFeatures").ToList();
		enemies = Resources.LoadAll<GameObject>("Enemies").ToList();
	}

	public void RegisterCluster(Cluster reportingCluster)
	{
		clusters.Add(reportingCluster);

		reportingCluster.transform.SetParent(GameObject.Find("World").transform);
	}

	public void ResetClusters()
	{
		clusters.Clear();
	}

	public void OnLevelWasLoaded(int level)
	{
		ResetClusters();
	}

	public void CreateNewCluster(Cluster center)
	{
		//If the cluster's neighbors aren't full, make a new one.
		if (center.neighborsPopulated < 8)
		{
			Cluster c = null;
			int tries = 0;
			while (tries < 8)
			{
				tries++;

				int direction = Random.Range(0, center.neighbors.Length);

				if(center.neighbors[direction] == null)
				{
					c = ((GameObject)GameObject.Instantiate(clusterPrefab, center.transform.position, Quaternion.identity)).GetComponent<Cluster>();

					c.transform.position += FindOffsetOfDir(direction);

					c.RandomLandmarks = true;
					

					tries = 1000;
				}
			}

			if (c.neighbors != null)
			{
				//Loop through all the potential neighbors.
				for (int i = 0; i < c.neighbors.Length; i++)
				{
					//Try to find a cluster there.
					Cluster neighborC = FindNearestCluster(c.transform.position + FindOffsetOfDir(i), 10);

					//If we find one
					if (neighborC != null)
					{
						//Debug.DrawLine(c.transform.position, neighborC.transform.position + Vector3.up * i, Color.green, 36.0f);

						//Register ourselves with it. Use the opposite index of ourselves.
						neighborC.neighbors[FindOppositeDirIndex(i)] = c;

						//Set our neighbor as the newly found cluster.
						c.neighbors[i] = c;

						//Increase both's neighborCount.
						c.neighborsPopulated++;
						neighborC.neighborsPopulated++;
					}
				}
			}
			else
			{
				Debug.LogError("Error with cluster neighbor generation\n");
			}
		}
	}

	//
	/// <summary>
	/// Provide a directional index (0 being positive X axis) to get the opposite side index.
	/// </summary>
	/// <param name="directionIndex"></param>
	/// <returns></returns>
	private int FindOppositeDirIndex(int directionIndex)
	{
		return (directionIndex + 4) % 8;
	}

	public Vector3 FindOffsetOfDir(int directionIndex)
	{
		Vector3 offset = Vector3.zero;
		if (directionIndex == 7 || directionIndex == 0 || directionIndex == 1)
		{
			offset += Vector3.right * clusterSize.x;
		}
		if (directionIndex == 1 || directionIndex == 2 || directionIndex == 3)
		{
			offset += Vector3.forward * clusterSize.z;
		}
		if (directionIndex == 3 || directionIndex == 4 || directionIndex == 5)
		{
			offset -= Vector3.right * clusterSize.x;
		}
		if (directionIndex == 5 || directionIndex == 6 || directionIndex == 7)
		{
			offset -= Vector3.forward * clusterSize.z;
		}

		return offset;
	}

	private int FindNearestClusterIndex(Vector3 location, float maxDistance)
	{
		if (maxDistance == -1)
		{
			maxDistance = float.MaxValue;
		}
		int indexNearest = -1;
		float nearestDist = float.MaxValue;
		for (int i = 0; i < clusters.Count; i++)
		{
			if (indexNearest == -1)
			{
				if (Vector3.Distance(location, clusters[i].transform.position) < maxDistance)
				{
					indexNearest = i;
					nearestDist = Vector3.Distance(location, clusters[indexNearest].transform.position);
				}
			}
			
			float nextDist = Vector3.Distance(location, clusters[i].transform.position);
			//Debug.Log("Comparing " + nextDist + " & " + nearestDist + "\n");
			if (nextDist < nearestDist && nextDist < maxDistance)
			{
				nearestDist = nextDist;
				indexNearest = i;
			}
		}

		return indexNearest;
	}

	public Cluster FindNearestCluster(Vector3 location, float maxDistance = -1)
	{
		Debug.DrawLine(location, location + Vector3.up * 100, Color.black, 8.0f);
		int index = FindNearestClusterIndex(location, maxDistance);
		if(index == -1)
		{
			return null;
		}
		return clusters[index];
		
	}

	void Update()
	{
		if (Input.GetKeyDown(KeyCode.P))
		{
			int islandCount = 0;
			foreach (Cluster c in clusters)
			{
				islandCount += c.platforms.Count;
			}
			Debug.Log("There are currently " + islandCount + " islands.\n");
		}
	}
}

public class Nomenclature
{
	//12 values
	static string[] first = { "", "A", "B", "D", "G", "H", "N", "O", "Q", "T", "U", "W" };
	//12 values
	static string[] second = { "", "C", "E", "F", "G", "N", "M", "P", "U", "X", "Y", "Z" };
	//12 values
	static string[] third = { "", " ", "-", "_", "'", "^", "?", "%", "#", "~", "+", "*" };
	//12 values
	static string[] fourth = { "Foxtrot", "Merp", "Ostril", "Iuscone", "Dreuogawa", "Oltheon", "Neria", "Eastworth", "Creoruta", "Zalia", "Vokilk", "Tiamandias" };

	/// <summary>
	/// Provides a random name for an island or cluster. Provide 4 randomly generated numbers from 0-11.
	/// </summary>
	public static string GetName(int rndA, int rndB, int rndC, int rndD)
	{
		string newName = "";

		newName += first[rndA] + second[rndB] + third[rndC] + fourth[rndD];

		return newName;
	}
}
