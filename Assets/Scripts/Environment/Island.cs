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
	public List<PathNode> nodes;
	public bool createNodes = false;

	public new void Start()
	{
		/*nearIslands = new List<Island>();
		gameObject.name = "Island: " + Nomenclature.GetName(Random.Range(0, 12), Random.Range(0, 12), Random.Range(0, 12), Random.Range(0, 12));

		base.Start();
		gameObject.tag = "Island";
		PlaceRandomObject();

		CreatePathNodes();*/

		//For special case platforms.
		if (createNodes)
		{
			Init();
		}
	}

	public PathNode GetRandomNode(PathNode nearest, bool thisIsland = false)
	{
		List<PathNode> remainingOptions = nodes.ToList();

		if (!thisIsland)
		{
			//Consider nearby islands. For each nearby island. Find the closest pathnode.
			for (int i = 0; i < nearIslands.Count; i++)
			{
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
		if (!createNodes)
		{
			nearIslands = new List<Island>();
		}
		gameObject.name = "Island: " + Nomenclature.GetName(Random.Range(0, 12), Random.Range(0, 12), Random.Range(0, 12), Random.Range(0, 12));

		gameObject.tag = "Island";
		//PlaceRandomObject();

		//Used by special case debug platforms.
		if (createNodes)
		{
			CreatePathNodes();
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
