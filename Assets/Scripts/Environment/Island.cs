using UnityEngine;
using System.Linq;
using System.Collections;
using System.Collections.Generic;

public class Island : WorldObject
{
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
	public List<Island> nearIslands;
	//This is the connections that this island has to other islands.
	public Dictionary<Island, DestinationConnection> islandConnections;
	public List<PathNode> nodes;
	public float connectionsInDictionary = 0;
	public bool editorCreateNodes = false;

	public new void Start()
	{
		/*nearIslands = new List<Island>();
		gameObject.name = "Island: " + Nomenclature.GetName(Random.Range(0, 12), Random.Range(0, 12), Random.Range(0, 12), Random.Range(0, 12));

		base.Start();
		gameObject.tag = "Island";
		PlaceRandomObject();

		CreatePathNodes();*/

		//For special case platforms.
		if (editorCreateNodes)
		{
			Init();
		}
	}

	public void ConfigureNodes()
	{
		string output = "";
		islandConnections = new Dictionary<Island, DestinationConnection>();

		//For every neighbor island
		foreach (Island neighbor in nearIslands)
		{
			DestinationConnection newDC = new DestinationConnection(this, neighbor);
			
			//Find all of the ways that we can REACH the island.
			foreach (PathNode myNode in nodes)
			{
				int i = 0;
				foreach (PathNode neighborNode in neighbor.nodes)
				{
					i++;
					//Find the distance between them
					float dist = Constants.CheckXZDistance(myNode.transform.position, neighborNode.transform.position);
					//Add the connection
					NodeConnection newNC = new NodeConnection(myNode, neighborNode, dist);

					if (newDC != null)
					{
						newDC.AddConnection(newNC);
						connectionsInDictionary++;
					}
				}
			}

			newDC.SortConnections();
			output += "Completed New Destination Connection between\t\t" + this.name + " and " + neighbor.name + "\nConnection Count: " + newDC.connections.Count + "\n\n";
		}

		//Debug.Log(output);
	}

	/*
	public void ConfigureNodes()
	{
		for (int i = 0; i < nodes.Count; i++)
		{
			ConfigurePathNode(nodes[i]);
		}
	}

	public void ConfigurePathNode(PathNode node)
	{
		node.nearNodes = new Dictionary<Island, PathNode>();
		node.nearIslandIndex = new List<Island>();
		node.nearNodeIndex = new List<PathNode>();

		//For each neighbor island
		foreach (Island curIsland in nearIslands)
		{
			PathNode nearestNode = null;
			float nearestDist = float.MaxValue;

			//Go through that islands nodes and find the closest one to the node we are configuring.
			foreach (PathNode curNode in curIsland.nodes)
			{
				//This checks the XZ since Y is a separate factor.
				//Can make a vertical check to make illegal  if it is too far a distance.
				float curDist = Constants.CheckXZDistance(node.transform.position, curNode.transform.position);
				if (curDist < nearestDist)
				{
					nearestNode = curNode;
				}
			}
			//If we found a node
			if (nearestNode != null)
			{
				if (!node.nearNodes.ContainsKey(curIsland))
				{
					//Add it to the current node's neighbors.
					node.nearNodes.Add(curIsland, nearestNode);
					node.nearIslandIndex.Add(curIsland);
					node.nearNodeIndex.Add(nearestNode);
				}
				if (!nearestNode.nearNodes.ContainsKey(this))
				{
					nearestNode.nearNodes.Add(this, node);
					nearestNode.nearIslandIndex.Add(this);
					nearestNode.nearNodeIndex.Add(node);
				}
				Debug.DrawLine(transform.position, node.transform.position, Color.green, 3.0f);
				Debug.DrawLine(node.transform.position, nearestNode.transform.position + Vector3.up * 4, Color.green, 3.0f);
			}
		}
		//Add relations to the path node for the shortest node to that island.
	}
	*/
	public PathNode GetRandomNode(PathNode nearest, bool thisIsland = false)
	{
		List<PathNode> remainingOptions = nodes.ToList();

		if (!thisIsland)
		{
			//Consider nearby islands. For each nearby island. Find the closest pathnode.
			for (int i = 0; i < nearIslands.Count; i++)
			{
				//remainingOptions.Add(islandConnections[nearIslands[i]].FindShortestConnectorFromStart(nearest).finishNode);
			//	remainingOptions.Add(
			//islandConnections[nearIslands[i]].FindShortestConnectorFromStartToIsland(nearest.finishNode, nearIslands[i]);


				//This needs to be improved. I want to set up basic pathfinding. I want to push nodes to send the character to the EDGE of this island, then to the next island


				remainingOptions.Add(nearIslands[i].NearestNode(nearest.transform.position));
				
				//remainingOptions.AddRange(nearIslands[i].nodes);
			}
		}
		
		//remainingOptions.Add(nearIslands.

		remainingOptions.Remove(nearest);

		return remainingOptions[Random.Range(0, remainingOptions.Count)];
	}

	public PathNode NearestNode(Vector3 pos)
	{
		PathNode pn = null;
		float shortestDist = float.MaxValue;

		//For each path node
		for (int i = 0; i < nodes.Count; i++)
		{
			//Flatten our Vector3s. Height won't ever matter dealing with a flat plane. Easier to debug for now.
			Vector2 posFlat = new Vector2(pos.x, pos.z);
			Vector2 nodePosFlat = new Vector2(nodes[i].gameObject.transform.position.x, nodes[i].gameObject.transform.position.z);
			
			//Find distance to the position.
			float thisDist = Vector2.Distance(posFlat, nodePosFlat);

			if (thisDist < shortestDist)
			{
				shortestDist = thisDist;
				pn = nodes[i];
			}
		}
		return pn;
	}

	public void Init()
	{
		if (!editorCreateNodes)
		{
			nearIslands = new List<Island>();
		}
		gameObject.name = "Island: " + Nomenclature.GetName(Random.Range(0, 12), Random.Range(0, 12), Random.Range(0, 12), Random.Range(0, 12));

		gameObject.tag = "Island";
		//PlaceRandomObject();

		//Used by special case debug platforms.
		if (editorCreateNodes)
		{
			CreatePathNodes();
			ConfigureNodes();
		}
	}

	public void CreatePathNodes()
	{
		//Create 5 islands, 1 at each corner, 1 in the center.

		float distAbovePlatform = transform.localScale.y / 2 + 2;
		Vector3 adjustment = Vector3.zero;

		nodes.Add(CreateNewNode(distAbovePlatform, adjustment));

		//Forward Right
		adjustment = Vector3.right * (transform.localScale.x / 2 * .90f) + Vector3.forward * (transform.localScale.z / 2 * .90f);
		nodes.Add(CreateNewNode(distAbovePlatform, adjustment));

		//Backward Right
		adjustment = Vector3.right * (transform.localScale.x / 2 * .90f) - Vector3.forward * (transform.localScale.z / 2 * .90f);
		nodes.Add(CreateNewNode(distAbovePlatform, adjustment));

		//Backward Left
		adjustment = -Vector3.right * (transform.localScale.x / 2 * .90f) - Vector3.forward * (transform.localScale.z / 2 * .90f);
		nodes.Add(CreateNewNode(distAbovePlatform, adjustment));

		//Forward Left
		adjustment = -Vector3.right * (transform.localScale.x / 2 * .90f) + Vector3.forward * (transform.localScale.z / 2 * .90f);
		nodes.Add(CreateNewNode(distAbovePlatform, adjustment));

	}

	private PathNode CreateNewNode(float distAbovePlatform, Vector3 adjustment)
	{
		GameObject newNode = (GameObject)GameObject.Instantiate(TerrainManager.Instance.pathNodePrefab, transform.position + Vector3.up * distAbovePlatform, Quaternion.identity);
		newNode.transform.SetParent(transform);

		newNode.transform.position -= adjustment;

		newNode.transform.rotation = Quaternion.identity;

		newNode.GetComponent<PathNode>().island = this;

		newNode.renderer.enabled = true;
#if !UNITY_EDITOR
		newNode.renderer.enabled = false;
#endif

		return newNode.GetComponent<PathNode>();
	}

	public void DisplayConnections()
	{
		foreach (KeyValuePair<Island, DestinationConnection> kvp in islandConnections)
		{
			Debug.Log("Hit");
			kvp.Value.DisplayConnections();
		}
	}

	public new void Update()
	{
		
		base.Update();
	}

	public void PlaceRandomObject()
	{
		if (Random.Range(0, 100) > 90)
		{
			Vector3 featurePosition = transform.position + Vector3.up * transform.localScale.y / 2;
			float xRnd = Random.Range(-transform.localScale.x / 2, transform.localScale.x / 2);
			float zRnd = Random.Range(-transform.localScale.z / 2, transform.localScale.z / 2);


			featurePosition = new Vector3(transform.position.x + xRnd * .8f, transform.position.y + transform.localScale.y / 2 + .2f, transform.position.z + zRnd * .8f);

			GameObject newFeature = (GameObject)GameObject.Instantiate(TerrainManager.Instance.terrainFeatures[Random.Range(0, TerrainManager.Instance.terrainFeatures.Count)], Vector3.zero, transform.rotation);

			newFeature.transform.position = featurePosition;

			newFeature.transform.SetParent(transform.parent);
		}
	}

	public void PlaceRandomEnemy()
	{
		if (Random.Range(0, 100) > 80)
		{
			Vector3 featurePosition = transform.position + Vector3.up * transform.localScale.y / 2;
			float xRnd = Random.Range(-transform.localScale.x / 2, transform.localScale.x / 2);
			float zRnd = Random.Range(-transform.localScale.z / 2, transform.localScale.z / 2);

			featurePosition = new Vector3(transform.position.x + xRnd * .8f, transform.position.y + transform.localScale.y / 2 + .2f, transform.position.z + zRnd * .8f);

			GameObject newEnemy = (GameObject)GameObject.Instantiate(TerrainManager.Instance.enemies[Random.Range(0, TerrainManager.Instance.enemies.Count)], Vector3.zero, transform.rotation);

			newEnemy.transform.SetParent(transform.parent);
			newEnemy.transform.position = featurePosition;
		}
	}
}
