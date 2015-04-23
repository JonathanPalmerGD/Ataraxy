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
	public List<NodeConnection> localConnections;
	public List<PathNode> nodes;
	public List<GameObject> assistingPlatforms;
	public float connectionsInDictionary = 0;
	public bool editorCreateNodes = false;
	public bool specialIsland = false;
	public bool miniIsland = false;

	//This variable shouldnt get changed in the inspector.
	[HideInInspector]
	public bool nodesInitialized = false;

	public new void Start()
	{
		if (assistingPlatforms == null)
		{
			assistingPlatforms = new List<GameObject>();
		}
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
		if(islandConnections == null)
		{
			islandConnections = new Dictionary<Island,DestinationConnection>();
		}

		if (localConnections == null)
		{
			localConnections = new List<NodeConnection>();
		}

		#region Local Node Connections
		//Debug.Log("Configuring local nodes\n");
		//Set up our node connections within our own island.
		for (int i = 0; i < nodes.Count; i++)
		{
			for (int j = 0; j < nodes.Count; j++)
			{
				if (i != j)
				{
					//Debug.Log("Configuring local nodes\n" + i + "    " + j);
					bool validRay = true;
					#region Raycast Handling
					if (TerrainManager.RaycastToNodeChecking && Vector3.Distance(nodes[i].transform.position, nodes[j].transform.position) < 40)
					{
						//Debug.Log("Inside Ray Node checking\n");

						RaycastHit hit;
						Ray ray = new Ray(nodes[i].transform.position, nodes[j].transform.position - nodes[i].transform.position);
						Ray revRay = new Ray(nodes[j].transform.position, nodes[i].transform.position - nodes[j].transform.position);

						//If we hit something with the raycast, it is not a valid ray.
						bool forRayCheck = !Physics.Raycast(ray, out hit, Vector3.Distance(nodes[i].transform.position, nodes[j].transform.position));

						#if UNITY_EDITOR
						if (TerrainManager.drawDebug && !forRayCheck)
						{
							Debug.DrawLine(nodes[i].transform.position, hit.point, Color.red, 5.0f);
						}
						#endif

						//If we hit something with the raycast, it is not a valid ray.
						bool revRayCheck = !Physics.Raycast(revRay, out hit, Vector3.Distance(nodes[i].transform.position, nodes[j].transform.position));

						#if UNITY_EDITOR
						if (TerrainManager.drawDebug && !revRayCheck)
						{
							Debug.DrawLine(nodes[j].transform.position, hit.point, Color.red, 5.0f);
						}

						if (revRayCheck && forRayCheck)
						{
							//Debug.DrawLine(nodes[i].transform.position, nodes[j].transform.position, Color.cyan, 5.0f);
						}
						#endif

						validRay = forRayCheck && revRayCheck;
					}
					#endregion

					//Start at the path node. Fire to the other path node.
					if (validRay)
					{
						#region NodeConnection Creation
						//Find the distance between them
						float dist = Constants.CheckXZDistance(nodes[i].transform.position, nodes[j].transform.position);
						float height = nodes[i].transform.position.y - nodes[j].transform.position.y;

						//Create a connection & a mirror connection
						NodeConnection newNC = new NodeConnection(nodes[i], nodes[j], dist, height);
						NodeConnection mirrowNewNC = new NodeConnection(nodes[j], nodes[i], dist, height);

						localConnections.Add(newNC);
						localConnections.Add(mirrowNewNC);

						//We need to add the new node connection to both or destination grid
						//selfToNeighborDC.AddConnection(newNC);
						//And to our target.
						//neighborToSelfDC.AddConnection(mirrowNewNC);
						#endregion
					}
					else
					{
						//Debug.DrawRay(myNode.transform.position, neighborNode.transform.position - myNode.transform.position, Color.red, 5.0f);
					}
				}
			}
		}

		#endregion

		#region Connect to neighbors
		//For every neighbor island
		foreach (Island neighbor in nearIslands)
		{
			//If our island connections contains our neighbor, they have already set up this connection.
			if (islandConnections.ContainsKey(neighbor))
			{
				//We can just move onto the next neighbor.
			}
			else
			{
				//Set up the new connection from ourself to our neighbor.
				DestinationConnection selfToNeighborDC = new DestinationConnection(this, neighbor);
				//And from our neighbor to us.
				DestinationConnection neighborToSelfDC = new DestinationConnection(neighbor, this);

				#region Neighbor Safety Initialization Checking
				//We call this because inside it checks if the nodes are initialized already.
				//If they are already set up, it does nothing.
				neighbor.CreatePathNodes();
				
				//If the neighbor isn't set up at all, give them a hand.
				if (neighbor.islandConnections == null)
				{
					neighbor.islandConnections = new Dictionary<Island, DestinationConnection>();
				}
				#endregion

				#region Handle Node Relations
				//Find all of the nodes we can start at.
				foreach (PathNode myNode in nodes)
				{
					//Find all of the nodes we can go to.
					foreach (PathNode neighborNode in neighbor.nodes)
					{
						bool validRay = true;
						#region Raycast Handling
						if(TerrainManager.RaycastToNodeChecking && Vector3.Distance(myNode.transform.position, neighborNode.transform.position) < 40)
						{
							RaycastHit hit;
							Ray ray = new Ray(myNode.transform.position, neighborNode.transform.position - myNode.transform.position);
							Ray revRay = new Ray(neighborNode.transform.position, myNode.transform.position - neighborNode.transform.position);

							//If we hit something with the raycast, it is not a valid ray.
							bool forRayCheck = !Physics.Raycast(ray, out hit, Vector3.Distance(myNode.transform.position, neighborNode.transform.position));

							#if UNITY_EDITOR
							if (TerrainManager.drawDebug && !forRayCheck)
							{
								Debug.DrawLine(myNode.transform.position, hit.point, Color.red, 5.0f);
							}
							#endif

							//If we hit something with the raycast, it is not a valid ray.
							bool revRayCheck = !Physics.Raycast(revRay, out hit, Vector3.Distance(myNode.transform.position, neighborNode.transform.position));

							#if UNITY_EDITOR
							if (TerrainManager.drawDebug && !revRayCheck)
							{
								Debug.DrawLine(neighborNode.transform.position, hit.point, Color.red, 5.0f);
							}
							#endif

							validRay = forRayCheck && revRayCheck;
						}
						#endregion
						
						//Start at the path node. Fire to the other path node.
						if (validRay)
						{
							#region NodeConnection Creation
							//Find the distance between them
							float dist = Constants.CheckXZDistance(myNode.transform.position, neighborNode.transform.position);
							float height = myNode.transform.position.y - neighborNode.transform.position.y;

							//Create a connection & a mirror connection
							NodeConnection newNC = new NodeConnection(myNode, neighborNode, dist, height);
							NodeConnection mirrowNewNC = new NodeConnection(neighborNode, myNode, dist, height);

							//We need to add the new node connection to both or destination grid
							selfToNeighborDC.AddConnection(newNC);
							//And to our target.
							neighborToSelfDC.AddConnection(mirrowNewNC);
							#endregion
						}
						else
						{
							//Debug.DrawRay(myNode.transform.position, neighborNode.transform.position - myNode.transform.position, Color.red, 5.0f);
						}
					}
				}
				#endregion

				#region Connection Adding
				//Debug.Log("NC: " + nodes.Count + "\tNN: " + neighbor.nodes.Count + "\tNI: " + nearIslands.Count + "\n" + selfToNeighborDC.connections.Count + "\t\t" + neighborToSelfDC.connections.Count + "\n");
				islandConnections.Add(neighbor, selfToNeighborDC);
				neighbor.islandConnections.Add(this, neighborToSelfDC);

				selfToNeighborDC.SortConnections();
				neighborToSelfDC.SortConnections();
				//output += "Completed New Destination Connection between\t\t" + this.name + " and " + neighbor.name + "\nConnection Count: " + selfToNeighborDC.connections.Count + "\n\n";
				#endregion

			}
		}
		#endregion

		//Debug.Log(output);
		//Debug.Log(islandConnections.Count + "\n");
	}

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

		if (assistingPlatforms == null)
		{
			assistingPlatforms = new List<GameObject>();
		}

		/* The idea here was to have individual islands tween in. This is scrapped for now.
		GameObject go = new GameObject();
		go.transform.SetParent(transform.parent);
		transform.SetParent(go.transform);*/

		gameObject.name = "Island: " + Nomenclature.GetName(Random.Range(0, 12), Random.Range(0, 12), Random.Range(0, 12), Random.Range(0, 12));

		gameObject.tag = "Island";
		//PlaceRandomObject();

		//Set any children to the parent object.
		for (int i = 0; i < transform.childCount; i++)
		{
			Transform childTransform = transform.GetChild(i);
			if (childTransform.name != "Encounter")
			{
				childTransform.SetParent(transform.parent);
				assistingPlatforms.Add(childTransform.gameObject);
			}
		}

		//Used by special case debug platforms.
		if (editorCreateNodes)
		{
			CreatePathNodes();
			ConfigureNodes();
		}
	}

	/// <summary>
	/// Creates the appropriate path nodes on an island.
	/// Will not create nodes if the bool nodes initialized is true.
	/// </summary>
	public void CreatePathNodes()
	{
		if (!nodesInitialized)
		{
			//Create 5 islands, 1 at each corner, 1 in the center.

			float distAbovePlatform = transform.localScale.y / 2 + 2;
			Vector3 adjustment = Vector3.zero;

			nodes.Add(CreateNewNode(distAbovePlatform, adjustment));

			if (!miniIsland)
			{
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

			//We have set up our nodes
			nodesInitialized = true;
		}
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

	public void SetupLocalConnections()
	{
		//For each node on this island, determine if it validly connects to other nondes.

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
		if (Random.Range(0, 100) > 60)
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
	
	public void PlaceCosmeticObjects()
	{
		if (Random.Range(0, 100) > 60)
		{
			int numCosmetics = Random.Range(1, 3);
			for(int i = 0; i < numCosmetics; i++)
			{
				Vector3 featurePosition = transform.position + Vector3.up * transform.localScale.y / 2;
				float xRnd = Random.Range(-transform.localScale.x / 2, transform.localScale.x / 2);
				float zRnd = Random.Range(-transform.localScale.z / 2, transform.localScale.z / 2);

				featurePosition = new Vector3(transform.position.x + xRnd * .8f, transform.position.y + transform.localScale.y / 2 + .2f, transform.position.z + zRnd * .8f);

				GameObject newFeature = (GameObject)GameObject.Instantiate(TerrainManager.Instance.cosmeticFeatures[Random.Range(0, TerrainManager.Instance.cosmeticFeatures.Count)], Vector3.zero, transform.rotation);

				newFeature.transform.position = featurePosition;
				newFeature.transform.Rotate(Vector3.up, Random.Range(0,360));

				newFeature.transform.SetParent(transform.parent);
			}
		}
	}

	public void PlaceRandomEncounter(bool force = false)
	{
		if (force || Random.Range(0, 100) + family.dangerLevel / 2 > 75)
		{
			Family.encounterCounter++;
			Vector3 featurePosition = transform.position + (Vector3.up * (transform.localScale.y / 2 + .2f));

			GameObject newEncounter = (GameObject)GameObject.Instantiate(TerrainManager.Instance.encounterPrefabs[Random.Range(0, TerrainManager.Instance.encounterPrefabs.Count)], Vector3.zero, transform.rotation);

			EncounterCreator encCreator = newEncounter.GetComponent<EncounterCreator>();

			newEncounter.transform.SetParent(transform);
			newEncounter.transform.position = featurePosition;

			encCreator.location = this;
			encCreator.Init();
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

			GameObject newEnemy = (GameObject)GameObject.Instantiate(TerrainManager.Instance.enemyPrefabs[Random.Range(0, TerrainManager.Instance.enemyPrefabs.Count)], Vector3.zero, transform.rotation);

			newEnemy.transform.SetParent(transform.parent);
			newEnemy.transform.position = featurePosition;
		}
	}
}
