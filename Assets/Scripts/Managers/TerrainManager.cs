
using UnityEngine;
using System.Linq;
using System.Collections;
using System.Collections.Generic;

public class TerrainManager : Singleton<TerrainManager>
{
	#region Terrain Manager Static Variables
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
	public static float IslandNeighborDist = 10;
	public static bool RaycastToNodeChecking = true;
	public static int underworldYOffset = 80;
	public static bool CreateIslandFeatures = true;
	public static bool CreateCosmeticFeatures = true;
	#endregion

	#region Prefabs & Lists of Prefabs
	public GameObject clusterPrefab;
	public GameObject pathNodePrefab;
	public List<Cluster> clusters;
	public List<Texture2D> textures;
	public List<Material> terrainMats;
	public List<GameObject> terrainFeatures;
	public List<GameObject> cosmeticFeatures;
	public List<GameObject> enemyPrefabs;
	public List<GameObject> encounterPrefabs;
	public List<GameObject> islandPrefabs;
	public List<GameObject> landmarkPrefabs;

	#region Clusters
	public List<GameObject> bossClusterPrefabs;
	public List<GameObject> landmarkClusterPrefabs;
	public List<GameObject> biomeClusterPrefabs;
	public List<GameObject> rareClusterPrefabs;
	#endregion
	#endregion

	public override void Awake()
	{
		base.Awake();

		clusters = new List<Cluster>();
		clusterPrefab = Resources.Load<GameObject>("Cluster");
		pathNodePrefab = Resources.Load<GameObject>("PathNode");
		textures = Resources.LoadAll<Texture2D>("Terrain").ToList();
		terrainMats = Resources.LoadAll<Material>("TerrainMaterials").ToList();
		islandPrefabs = Resources.LoadAll<GameObject>("Islands").ToList();
		landmarkPrefabs = Resources.LoadAll<GameObject>("Landmarks").ToList();
		terrainFeatures = Resources.LoadAll<GameObject>("TerrainFeatures").ToList();
		cosmeticFeatures = Resources.LoadAll<GameObject>("TerrainCosmetics").ToList();
		enemyPrefabs = Resources.LoadAll<GameObject>("Enemies").ToList();
		encounterPrefabs = Resources.LoadAll<GameObject>("Encounters").ToList();

		bossClusterPrefabs = new List<GameObject>();
		landmarkClusterPrefabs = new List<GameObject>();
		biomeClusterPrefabs = new List<GameObject>();
		rareClusterPrefabs = new List<GameObject>();
		bossClusterPrefabs = Resources.LoadAll<GameObject>("Clusters/Boss").ToList();
		landmarkClusterPrefabs = Resources.LoadAll<GameObject>("Clusters/Landmark").ToList();
		biomeClusterPrefabs = Resources.LoadAll<GameObject>("Clusters/Biomes").ToList();
		rareClusterPrefabs = Resources.LoadAll<GameObject>("Clusters/Rare").ToList();
	}
	#region Cluster Setup
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

	public void CreateNewCluster(Cluster center, Cluster.ClusterType newClusterType = default(Cluster.ClusterType))
	{
		if (center != null)
		{
			//If the cluster's neighbors aren't full, make a new one.
			if (center.neighborsPopulated < 8)
			{
				Cluster newCluster = null;
				GameObject newClusterPrefab = GetClusterPrefab(newClusterType);

				int tries = 0;
				while (tries < 8)
				{
					tries++;

					int direction = Random.Range(0, center.neighborClusters.Length);

					if (center.neighborClusters[direction] == null)
					{
						if (newClusterPrefab != null)
						{
							//We want to create a new cluster near an existing one. We use center.GetFinalPosition to avoid an incorrect target height.
							newCluster = ((GameObject)GameObject.Instantiate(newClusterPrefab, center.transform.position, Quaternion.identity)).GetComponent<Cluster>();

							newCluster.transform.position += FindOffsetOfDir(direction);
							newCluster.clusterContents.transform.position -= Vector3.up * underworldYOffset;

							newCluster.RandomLandmarks = true;

							tries = 1000;
						}
						else
						{
							Debug.LogError("[TerrainManager]\tCould not create cluster of type " + newClusterType.ToString() + "\nCluster Prefab was null.");
						}
					}
				}

				if (newCluster != null)
				{
					SetupNeighborClusters(newCluster);
				}
			}
			else
			{
				//This a debug error for troubleshooting cluster generation.
				//Debug.LogError("All neighbor slots are filled. Cannot create cluster\n");
			}
				
		}
	}

	private GameObject GetClusterPrefab(Cluster.ClusterType newClusterType)
	{
		GameObject prefabToCreate = null;	

		if (newClusterType == Cluster.ClusterType.Biome)
		{
			if (biomeClusterPrefabs.Count > 0)
			{
				prefabToCreate = biomeClusterPrefabs[Random.Range(0, biomeClusterPrefabs.Count)];
			}
		}
		else if (newClusterType == Cluster.ClusterType.Rare)
		{
			if (rareClusterPrefabs.Count > 0)
			{
				prefabToCreate = rareClusterPrefabs[Random.Range(0, rareClusterPrefabs.Count)];
			}
		}
		else if (newClusterType == Cluster.ClusterType.Boss)
		{
			if (bossClusterPrefabs.Count > 0)
			{
				prefabToCreate = bossClusterPrefabs[Random.Range(0, bossClusterPrefabs.Count)];
			}
		}
		else if (newClusterType == Cluster.ClusterType.Landmark)
		{
			if (landmarkClusterPrefabs.Count > 0)
			{
				prefabToCreate = landmarkClusterPrefabs[Random.Range(0, landmarkClusterPrefabs.Count)];
			}
		}

		return prefabToCreate;
	}

	private void SetupNeighborClusters(Cluster center)
	{
		if(center != null && center.neighborClusters != null)
		{
			//Loop through all the potential neighbors.
			for (int i = 0; i < center.neighborClusters.Length; i++)
			{
#if UNITY_EDITOR
				if (drawDebug)
				{
					Debug.DrawLine(center.transform.position, center.transform.position + FindOffsetOfDir(i) + Vector3.up * 220, Color.cyan, 10f);
				}
#endif

				//Try to find a cluster there.
				Cluster neighborC = FindNearestCluster(center.transform.position + FindOffsetOfDir(i), 10);

				//If we find one
				if (neighborC != null && neighborC.gameObject != center.gameObject)
				{
					//Debug.DrawLine(c.transform.position, neighborC.transform.position + Vector3.up * i, Color.green, 36.0f);

					//If we didn't already exist, increase both neighbor populated.
					if (neighborC.neighborClusters[FindOppositeDirIndex(i)] == null)
					{
						//Register ourselves with it. Use the opposite index of ourselves.
						neighborC.neighborClusters[FindOppositeDirIndex(i)] = center;

						//Set our neighbor as the newly found cluster.
						center.neighborClusters[i] = neighborC;


						//Increase both's neighborCount.
						center.neighborsPopulated++;
						neighborC.neighborsPopulated++;
					}
					//else
					//{
					//	Debug.Log("Reverse recording. Skipping addition?\n");
					//}
				}
			}
		}
		else
		{
			if (center == null)
			{
				Debug.LogError("Error with cluster neighbor generation\n");
			}
			else
			{
				Debug.LogError("Error with center.neighborClusters being null\n");
			}
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
	#endregion

	#region Pathfinding
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

	public Stack<PathNode> FindPathToRandomNeighborIsland(Island start, PathNode nearest)
	{
		Stack<PathNode> newPath = new Stack<PathNode>();
		//If we have neighbors
		if (start.nearIslands.Count > 0)
		{
			//Select a random neighbor of the current island.
			Island randomNeighbor = start.nearIslands[Random.Range(0, start.nearIslands.Count)];

			//If we have log data
			if (start.islandConnections.ContainsKey(randomNeighbor))
			{
				//If we have a connection that reaches them
				if(start.islandConnections[randomNeighbor].connections.Count > 0)
				{
					//This currently doesn't find OUR shortest connector. It finds our island's shortest connector. This is good for if we want to path to the easiest jump. Suggestible option would be to find the most adequate jump but that will be more expensive.
					NodeConnection shortestConnection = start.islandConnections[randomNeighbor].connections[0];
					//Add the destination island's center node.
					newPath.Push(randomNeighbor.nodes[0]);

					//Add the connector's end node (located on destination island)
					newPath.Push(shortestConnection.finishNode);

					//Add the connector's start node (located on my island)
					newPath.Push(shortestConnection.startNode);

					//If the nearest node isn't the start node)
					if(nearest != shortestConnection.startNode)
					{
						//The best approach would be to check if the nearest node is on the way to the jump begin node... but we don't have the pathing entity's position, so this is fine for now.
						newPath.Push(nearest);

						//float nearestNodeDistanceToPath = Constants.CheckXZDistance(nearest.transform.position, shortestConnection.startNode.transform.position);
						//float nearestNodeDistanceToPath = Constants.CheckXZDistance(nearest.transform.position, );
						
						//Add the nearest node if it is closer than we are.
					}
				}
				else
				{
					Debug.LogError("We have no connection that reaches our neighbor.\nThis is likely a problem of an obstacle or being located inside another piece of terrain.");
				}
			}
			else
			{
				Debug.LogError("We didn't know about a neighbor island.\nError in World Generation or Island neighbor layout.");
			}
		}
		return newPath;
	}

	public Stack<PathNode> ConnectedToDestination(Island start, Island destination)
	{
		Stack<PathNode> path = new Stack<PathNode>();

		//Are we connected to that island?
		//if (destination.Family.inPlace && start.islandConnections.ContainsKey(destination))
		/*if (start == null)
		{
			Debug.Log("Hit\n");
		}
		if (start.islandConnections != null)
		{
			Debug.Log("Hit\n");
		}
		if (destination != null)
		{
			Debug.Log("Hit\n");
		}*/

		if (start.islandConnections != null && start.islandConnections.ContainsKey(destination))
		{
			//If we are, get the connection.
			DestinationConnection dc = start.islandConnections[destination];

			if (dc != null && dc.connections.Count > 0)
			{
				//Compare the two shortest connectors to that island
				//NodeConnection shortest = dc.connections[0];
				//NodeConnection second = dc.connections[1];

				//Take the shortest connector to that island.
				NodeConnection nc = dc.connections[0];

				//Debug.DrawLine(nc.finishNode.transform.position + Vector3.back * .5f, nc.startNode.transform.position + Vector3.back * .5f, Color.black, 35f);

				//After we know we're going to the right edge of the shortest jump, push the destination.
				path.Push(nc.finishNode);

				path.Push(nc.startNode);
				//Debug.DrawLine(nc.finishNode.transform.position + Vector3.forward * .5f, nc.finishNode.transform.position + Vector3.forward * .5f + Vector3.up * 3, Color.magenta, 35f);
			}
			else
			{
				Debug.LogWarning("We say we're connected but we don't have a non-null destination connection, wtf?\n");
				return new Stack<PathNode>();
			}
		}

		return path;
	}
	
	public Stack<PathNode> PathToIsland(Island start, Island destination, float distanceThreshold, int depth = 0)
	{
		depth++;
		if (depth > 10)
		{
			return new Stack<PathNode>();
		}
		//Debug.Log(depth + "\tCalling FindPathToIsland\nCurrent " + destination.name + " backtracking to node on " + start.island.name + "\n\n");

		Stack<PathNode> path = new Stack<PathNode>();

		#region Have Reached Destination
		//If we can reach destination from the current island
		path = ConnectedToDestination(start, destination);
		//Have we reached it?
		if (path != null && path.Count > 0)
		{
			//If we have, return
			return path;
		}
		#endregion
		#region Checking Neighbor for Path to Start
		else
		{
			List<Island> neighborRanking = RankNeighborsByDistanceFromGoal(start, destination.transform.position, distanceThreshold);

			//Look through all the ranked neighbors
			for (int i = 0; i < neighborRanking.Count; i++)
			{
				//Debug.Log("Inside Neighbor Ranking.\n");
				//Call this function on those neighbors
				path = PathToIsland(neighborRanking[i], destination, distanceThreshold, depth);

				//If the return has content in it.
				if (path != null)
				{
					//Add it to our current path.
					/*List<PathNode> foundPathList = foundPath.ToList();
					for (int k = 0; k < foundPathList.Count; k++)
					{
						Debug.Log(depth + "\tAdding " + foundPathList[k].island.name + " to the path\n");
						path.Push(foundPath.Pop());
						//path.Push(foundPathList[k]);
					}*/

					//Debug.Log(depth + "\t[Adding Neighbor]" + curIsland.name + " to " + neighborRanking[i] + "\n");

					if (start.islandConnections[neighborRanking[i]].connections.Count > 0)
					{
						NodeConnection nc = start.islandConnections[neighborRanking[i]].connections[0];

						//Push the destination island edge node
						path.Push(nc.finishNode);
						//Debug.DrawLine(nc.finishNode.transform.position, nc.finishNode.transform.position + Vector3.up * 3, Color.yellow, 35f);

						//Push the destination island center node.
						//path.Push(neighborRanking[i].nodes[0]);

						//Push current island edge node
						path.Push(nc.startNode);
						//Debug.DrawLine(nc.startNode.transform.position + Vector3.right * .5f, nc.startNode.transform.position + Vector3.right * .5f + Vector3.up * 3, Color.blue, 35f);

						return path;
					}
					else
					{
						//Debug.LogWarning("Connection fizzled.\n");
						return new Stack<PathNode>();
					}
				}
				//Otherwise, continue searching.
			}
		}
		#endregion

		//This is if we didn't find a route there.
		return new Stack<PathNode>();
	}

	public Stack<PathNode> CanReachDestination(PathNode targetNode, PathNode currentNode)
	{
		Stack<PathNode> wayToNode = new Stack<PathNode>();

		//Are we on the same island.
		if (targetNode.island == currentNode.island)
		{
			//Push the target node onto the stack.
			wayToNode.Push(targetNode);
		}
		else
		{
			//Are we connected to that island?
			if(currentNode.island.islandConnections.ContainsKey(targetNode.island))
			{
				//If we are, get the connection.
				DestinationConnection dc = currentNode.island.islandConnections[targetNode.island];

				if (dc != null)
				{
					//Take the shortest connector to that island.
					NodeConnection nc = dc.connections[0];


					Debug.DrawLine(nc.finishNode.transform.position + Vector3.back * .5f, nc.startNode.transform.position + Vector3.back * .5f, Color.black, 35f);

					//Is current node the start of that jump? If not, add it.
					if (currentNode != nc.startNode)
					{
						wayToNode.Push(nc.startNode);
					}
					//After we know we're going to the right edge of the shortest jump, push the destination.
					wayToNode.Push(nc.finishNode);
					Debug.DrawLine(nc.finishNode.transform.position + Vector3.forward * .5f, nc.finishNode.transform.position + Vector3.forward * .5f + Vector3.up * 3, Color.magenta, 35f);
				}
				else
				{
					Debug.LogError("We say we're connected but we don't have a non-null destination connection, wtf?\n");
				}
			}
		}

		return wayToNode;
	}
		
	public List<Island> RankNeighborsByDistanceFromGoal(Island start, Vector3 target, float distanceThreshold, bool addAll = false)
	{
		string output = "";
		List<Island> rankings = new List<Island>();

		//We use distances to compare for adding the future neighbors to the rankings.
		List<float> distances = new List<float>();
		//Loop through the neighbors of the current island.
		for (int i = 0; i < start.nearIslands.Count; i++)
		{
			//Get the distance they are to the goal.
			float curDistance = Constants.CheckXZDistance(target, start.nearIslands[i].transform.position);
			float myDistance = Constants.CheckXZDistance(target, start.transform.position);

			//Debug.DrawLine(target, start.transform.position, Color.black, .2f);
			//Debug.DrawLine(target, start.nearIslands[i].transform.position, Color.grey, .2f);

			//Debug.Log("My distance to next node: " + myDistance + "\nNeighbor's distance to next node: " + curDistance);

			//If distance is less than the threshold && decently farther away than we are
			if (curDistance < myDistance * 1.3f && curDistance < distanceThreshold)
			{
				//If we don't have any rankings yet, add one.
				if (rankings.Count == 0)
				{
					output += "Added 0th ranking: " + start.nearIslands[i].name + "\n";
					rankings.Add(start.nearIslands[i]);
					distances.Add(curDistance);
				}
				else
				{
					//Look through the rankings. Add new entries in between the right points.
					for (int k = 0; k < rankings.Count; k++)
					{
						//If our distance is less than the current one
						if (curDistance < distances[k])
						{
							//Add the distance and ranking at that spot.
							distances.Insert(k, curDistance);
							rankings.Insert(k, start.nearIslands[i]);

							//Set this so we only add once.
							k = rankings.Count;
						}
					}
				}
			}
			else
			{
				//output += "Disregarding " + start.nearIslands[i].name + " because distance\n";
			}
		}

		output += "\n\nRankings Final Order:\n";

		//Look through the rankings. Add new entries in between the right points.
		for (int k = 0; k < rankings.Count; k++)
		{
			output += "[" + k + "]" + rankings[k].name;
		}

		output += "\n\n\n";
		//Debug.Log(output);

		return rankings;
	}

	#region Unused Pathfinding Methods
	public List<Island> RankNeighborsByDistanceFromSelf(Island start, float distanceThreshold)
	{
		List<Island> rankings = new List<Island>();

		//We use distances to compare for adding the future neighbors to the rankings.
		List<float> distances = new List<float>();
		//Loop through the neighbors of the current island
		for (int i = 0; i < start.nearIslands.Count; i++)
		{
			float curDistance = Constants.CheckXZDistance(start.transform.position, start.nearIslands[i].transform.position);

			//If distance to current island is less than the threshold
			if (curDistance < distanceThreshold)
			{
				//If we don't have any rankings yet, add one.
				if (rankings.Count == 0)
				{
					rankings.Add(start.nearIslands[i]);
					distances.Add(curDistance);
				}
				else
				{
					//Look through the rankings. Add new entries in between the right points.
					for (int k = 0; k < rankings.Count; k++)
					{
						//If our distance is less than the current one
						if (curDistance < distances[k])
						{
							//Add the distance and ranking at that spot.
							distances.Insert(k, curDistance);
							rankings.Insert(k, start.nearIslands[i]);

							//Set this so we only add once.
							k = rankings.Count;
						}
					}
				}
			}
		}

		return rankings;
	}

	public bool AreTwoIslandsConnected(Island start, Island destination)
	{
		if (start.islandConnections.ContainsKey(destination))
		{
			return true;
		}
		return false;
	}

	public bool IsIslandConnectedToNode(Island start, PathNode destination)
	{
		//Are the two islands connected
		if (AreTwoIslandsConnected(start, destination.island))
		{
			//Are any of the connections

		}

		return false;
	}

	public Island FindIslandNearTarget(Entity target)
	{
		Island nearestIsland = null;
		//Use the terrain manager to find the nearest cluster.
		//Cluster nearCluster = FindNearestCluster(target.transform.position, TerrainManager.clusterSize.x / 2);

		RaycastHit hit;

		if (target != null)
		{
			if( target.tag == "Player")
			{
				return target.GetComponent<Controller>().lastLocation;
			}
			else
			{
				bool result = Physics.Raycast(target.transform.position, Vector3.up, out hit, 15);

				if (result)
				{

				}
			}
		}


		return nearestIsland;
	}
	#endregion
	#endregion

	#region Update
	void Update()
	{
		#if UNITY_EDITOR
		if (Input.GetKeyDown(KeyCode.P))
		{
			string TerrainInfo = "";
			
			int islandCount = 0;
			int encCounter = 0;
			foreach (Cluster c in clusters)
			{
				string islandName = c.name;
				string platformInfo = c.platforms.Count + " islands";
				string encounterInfo = c.encounterCounter + " encounters";

				TerrainInfo += string.Format("{0,30} {1,30} {2,30}", islandName, platformInfo, encounterInfo) + "\n";
					
					//+ c.name + "\t\t" + c.platforms.Count + " islands\t\t" + c.encounterCounter + " encounters\n";
				islandCount += c.platforms.Count;
				encCounter += c.encounterCounter;
			}

			TerrainInfo = "TerrainManager Information Report:\n" +
				"   # of Clusters: " + clusters.Count + "\t" +
				"   # of Islands: " + islandCount + "\t" +
				"   # of Encounters: " + encCounter + "\n\n" + TerrainInfo;
			Debug.Log(TerrainInfo + "\n\n\n\n");
		}
		#endif
	}
	#endregion
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
			DrawNthOrderConnections(1);
			//DrawNthOrderConnections(2);
			//DrawNthOrderConnections(3);
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

		if (nc != null)
		{
			Vector3 sPos = nc.startNode.transform.position;
			Vector3 tPos = nc.finishNode.transform.position;

			if (nc.distance > 20)
			{
				float darken = nc.distance / 90;
				drawColor = new Color(drawColor.r - darken, drawColor.g - darken, drawColor.b - darken);
			}

			Debug.DrawLine(sPos + Vector3.up * 0, tPos + Vector3.up * 0, drawColor, 35.0f);
		}
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
		//string output = startIsl + "  to  " + targetIsl + "\n";

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
	public NodeConnection(PathNode start, PathNode finish, float dist, float height = -767)
	{
		distance = dist;
		startNode = start;
		finishNode = finish;
	}
	public float distance;
	public float heightDifference;
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
