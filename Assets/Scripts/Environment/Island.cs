using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class Island : WorldObject
{
	public List<Island> neighbors;
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

	public new void Start()
	{
		/*nearIslands = new List<Island>();
		gameObject.name = "Island: " + Nomenclature.GetName(Random.Range(0, 12), Random.Range(0, 12), Random.Range(0, 12), Random.Range(0, 12));

		base.Start();
		gameObject.tag = "Island";
		PlaceRandomObject();

		CreatePathNodes();*/
	}

	public void Init()
	{
		nearIslands = new List<Island>();
		gameObject.name = "Island: " + Nomenclature.GetName(Random.Range(0, 12), Random.Range(0, 12), Random.Range(0, 12), Random.Range(0, 12));

		gameObject.tag = "Island";
		//PlaceRandomObject();
	}

	public void CreatePathNodes()
	{
		//Create 5 islands, 1 at each corner, 1 in the center.

		float distAbovePlatform = transform.localScale.y / 2 + 2;
		Vector3 adjustment = Vector3.zero;

		nodes.Add(CreateNewNode(distAbovePlatform, adjustment));

		//Forward Right
		adjustment = Vector3.right * (transform.localScale.x / 2 * .95f) + Vector3.forward * (transform.localScale.z / 2 * .95f);
		nodes.Add(CreateNewNode(distAbovePlatform, adjustment));

		//Backward Right
		adjustment = Vector3.right * (transform.localScale.x / 2 * .95f) - Vector3.forward * (transform.localScale.z / 2 * .95f);
		nodes.Add(CreateNewNode(distAbovePlatform, adjustment));

		//Backward Left
		adjustment = -Vector3.right * (transform.localScale.x / 2 * .95f) - Vector3.forward * (transform.localScale.z / 2 * .95f);
		nodes.Add(CreateNewNode(distAbovePlatform, adjustment));

		//Forward Left
		adjustment = -Vector3.right * (transform.localScale.x / 2 * .95f) + Vector3.forward * (transform.localScale.z / 2 * .95f);
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
