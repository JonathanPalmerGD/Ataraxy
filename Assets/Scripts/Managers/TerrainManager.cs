using UnityEngine;
using System.Linq;
using System.Collections;
using System.Collections.Generic;

public class TerrainManager : Singleton<TerrainManager>
{
#if UNITY_EDITOR
	public static bool drawDebug = false;
#endif
	public static float minSizeHor = 40;
	public static Vector3 minDistance = new Vector3(10, -12, 10);
	public static Vector3 maxDistance = new Vector3(50, 5, 50);
	public static Vector3 minScale = new Vector3(3, 2, 3);
	public static Vector3 maxScale = new Vector3(45, 15, 45);
	public static Vector3 poissonMinScale = new Vector3(10, 1, 10);
	public static Vector3 poissonMaxScale = new Vector3(18, 12, 18);
	public static Vector3 clusterSize = new Vector3(100, 20, 100);
	public static int poissonMinK = 20;
	public static int poissonMaxK = 30;
	public static float minTiltDeviation = 2;
	public static float maxTiltDeviation = 40;
	public static float minTilt = -8;
	public static float maxTilt = 8;
	public static int minCountInCluster = 5;
	public static int maxCountInCluster = 9;
	public static float IslandNeighborDist = 35;
	public static int underworldYOffset = 80;

	public GameObject clusterPrefab;
	public GameObject pathNodePrefab;

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
		pathNodePrefab = Resources.Load<GameObject>("PathNode");
		textures = Resources.LoadAll<Texture2D>("Terrain").ToList();
		islandPrefabs = Resources.LoadAll<GameObject>("Islands").ToList();
		landmarkPrefabs = Resources.LoadAll<GameObject>("Landmarks").ToList();
		terrainFeatures = Resources.LoadAll<GameObject>("TerrainFeatures").ToList();
		enemies = Resources.LoadAll<GameObject>("Enemies").ToList();
	}

	public void RegisterCluster(Cluster reportingCluster)
	{
		clusters.Add(reportingCluster);

		SetupNeighborClusters(reportingCluster);

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

				int direction = Random.Range(0, center.neighborClusters.Length);

				if(center.neighborClusters[direction] == null)
				{
					//We want to create a new cluster near an existing one. We use center.GetFinalPosition to avoid an incorrect target height.
					c = ((GameObject)GameObject.Instantiate(clusterPrefab, center.transform.position, Quaternion.identity)).GetComponent<Cluster>();

					c.transform.position += FindOffsetOfDir(direction);
					c.clusterContents.transform.position -= Vector3.up * underworldYOffset;

					c.RandomLandmarks = true;
					

					tries = 1000;
				}
			}

			SetupNeighborClusters(c);

			#region Setup Neighbor Clusters (Was exported)
			/*if (c.neighborClusters != null)
			{
				//Loop through all the potential neighbors.
				for (int i = 0; i < c.neighborClusters.Length; i++)
				{
					//Try to find a cluster there.
					Cluster neighborC = FindNearestCluster(c.transform.position + FindOffsetOfDir(i), 10);

					//If we find one
					if (neighborC != null)
					{
						//Debug.DrawLine(c.transform.position, neighborC.transform.position + Vector3.up * i, Color.green, 36.0f);

						//Register ourselves with it. Use the opposite index of ourselves.
						neighborC.neighborClusters[FindOppositeDirIndex(i)] = c;

						//Set our neighbor as the newly found cluster.
						c.neighborClusters[i] = c;

						//Increase both's neighborCount.
						c.neighborsPopulated++;
						neighborC.neighborsPopulated++;
					}
				}
			}
			else
			{
				Debug.LogError("Error with cluster neighbor generation\n");
			}*/
			#endregion
		}
	}

	private void SetupNeighborClusters(Cluster center)
	{
		if (center.neighborClusters != null)
		{
			//Loop through all the potential neighbors.
			for (int i = 0; i < center.neighborClusters.Length; i++)
			{
#if UNITY_EDITOR
				if (drawDebug)
				{
					Debug.DrawLine(center.transform.position, center.transform.position + FindOffsetOfDir(i) + Vector3.up * 220, Color.red, 10f);
				}
#endif

				//Try to find a cluster there.
				Cluster neighborC = FindNearestCluster(center.transform.position + FindOffsetOfDir(i), 10);

				//If we find one
				if (neighborC != null && neighborC.gameObject != center.gameObject)
				{
					//Debug.DrawLine(c.transform.position, neighborC.transform.position + Vector3.up * i, Color.green, 36.0f);

					//Register ourselves with it. Use the opposite index of ourselves.
					neighborC.neighborClusters[FindOppositeDirIndex(i)] = center;

					//Set our neighbor as the newly found cluster.
					center.neighborClusters[i] = neighborC;

					//Increase both's neighborCount.
					center.neighborsPopulated++;
					neighborC.neighborsPopulated++;
				}
			}
		}
		else
		{
			Debug.LogError("Error with cluster neighbor generation\n");
		}
	}

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
		//Debug.DrawLine(location, location + Vector3.up * 100, Color.black, 8.0f);
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

public class DestinationConnection
{
	//The key for getting this object from the dictionary, in case it needs to be modified.
	public Island startIsl;

	//The location the entity is trying to go to
	public Island targetIsl;

	//A list in increasing order of ways to get from the start island to the target island.
	public List<NodeConnection> connections;

	//Constructor
	public DestinationConnection(Island start, Island finish)
	{
		startIsl = start;
		targetIsl = finish;
		connections = new List<NodeConnection>();
	}

	public void AddConnection(NodeConnection newNC)
	{
		connections.Add(newNC);
	}

	public NodeConnection FindShortestConnectorFromStart(PathNode start)
	{
		NodeConnection nc = null;
		float shortestDist = float.MaxValue;

		//Look through all connections
		for (int i = 0; i < connections.Count; i++)
		{
			//If it begins at start
			if (connections[i].startNode == start)
			{
				//If it is shorter than the fastest.
				if (connections[i].distance < shortestDist)
				{
					//Replace
					shortestDist = connections[i].distance;
					nc = connections[i];
				}
			}
		}
		//Return the shortest route.
		return nc;
	}

	public NodeConnection FindShortestConnectorToFinish(PathNode finish)
	{
		NodeConnection nc = null;
		float shortestDist = float.MaxValue;

		//Look through all connections
		for (int i = 0; i < connections.Count; i++)
		{
			//If it ends at where we want to go
			if (connections[i].finishNode == finish)
			{
				//If it is shorter than the fastest.
				if (connections[i].distance < shortestDist)
				{
					//Replace
					shortestDist = connections[i].distance;
					nc = connections[i];
				}
			}
		}
		//Return the shortest route.
		return nc;
	}

	public NodeConnection FindShortestConnectorFromStartToIsland(PathNode start, Island destination)
	{
		NodeConnection nc = null;
		float shortestDist = float.MaxValue;

		//Look through all connections
		for (int i = 0; i < connections.Count; i++)
		{
			//If it begins at start AND is on the island we want
			if (connections[i].startNode == start && connections[i].finishNode.island == destination)
			{
				//If it is shorter than the fastest.
				if (connections[i].distance < shortestDist)
				{
					//Replace
					shortestDist = connections[i].distance;
					nc = connections[i];
				}
			}
		}
		//Return the shortest route.
		return nc;
	}

	/// <summary>
	/// This is used to draw lines representing the relationships of this Destination Connection.
	/// </summary>
	public void DisplayConnections()
	{
		if (connections != null && connections.Count > 0)
		{
			//DrawIslandConnections();
			DrawNthOrderConnections(0);
			//DrawLastOrderConnection();
		}
	}

	/// <summary>
	/// [Cyan Lines] This shows the island positions related to each other.
	/// </summary>
	public void DrawIslandConnections()
	{
		Vector3 sPos = startIsl.transform.position;
		Vector3 tPos = targetIsl.transform.position;

		Debug.DrawLine(sPos + Vector3.up * 1, tPos + Vector3.up * 1, Color.cyan, 35.0f);
	}

	/// <summary>
	/// [Defaults to Gold Lines] This draws the Nth best node connection to the destination island
	/// </summary>
	/// <param name="n"></param>
	public void DrawNthOrderConnections(int n = 0, Color drawColor = default(Color))
	{
		if(drawColor == default(Color))
		{
			drawColor = Color.yellow;

		}
		NodeConnection nc = GetConnectionOrderOfN(n);

		Vector3 sPos = nc.startNode.transform.position;
		Vector3 tPos = nc.finishNode.transform.position;

		if (nc.distance > 20)
		{
			drawColor = Color.black;
		}

		Debug.DrawLine(sPos + Vector3.up * 0, tPos + Vector3.up * 0, drawColor, 35.0f);
	}

	/// <summary>
	/// [Red Lines] This draws the worst node connection to the destination island.
	/// </summary>
	public void DrawLastOrderConnection()
	{
		DrawNthOrderConnections(connections.Count - 1, Color.red);
	}

	/// <summary>
	/// This gets the Nth best connection from the start island to the target
	/// </summary>
	/// <param name="n">Make sure this is less than the number of connections.</param>
	/// <returns>Returns a connection if any. Check for null values.</returns>
	public NodeConnection GetConnectionOrderOfN(int n = 0)
	{
		if (connections != null && connections.Count > n)
		{
			//Return the shortest one (the smallest connection)
			return connections[n];
		}
		return null;
	}

	/// <summary>
	/// Reorders the connections to put the shortest connections at the front of the list. Uses LINQ sorting by element.distance.
	/// </summary>
	public void SortConnections()
	{
		//string output = "";

		/*for (int i = 0; i < connections.Count; i++)
		{
			output += connections[i].distance + "\t";
		}*/

		var ordered = from element in connections
					  orderby element.distance
					  select element;

		//output += "\n";
		connections = ordered.ToList();

		/*for (int i = 0; i < connections.Count; i++)
		{
			output += connections[i].distance + "\t";
		}*/

		//connections = connections.Sort((n1, n2) => x.distance).ToList();
		//Debug.Log(output);
		DisplayConnections();
	}
}

public class NodeConnection
{
	public NodeConnection(PathNode start, PathNode finish, float dist)
	{
		distance = dist;
		startNode = start;
		finishNode = finish;
	}
	public float distance;
	public PathNode startNode;
	public PathNode finishNode;
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
