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

	public new void Start()
	{
		gameObject.name = "Island: " + Nomenclature.GetName(Random.Range(0, 12), Random.Range(0, 12), Random.Range(0, 12), Random.Range(0, 12));

		base.Start();
		gameObject.tag = "Island";
		PlaceRandomObject();
	}

	public new void Update()
	{
		base.Update();
	}

	public void PlaceRandomObject()
	{
		if (Random.Range(0, 100) > 96)
		{
			Vector3 featurePosition = transform.position + Vector3.up * transform.localScale.y / 2;
			float xRnd = Random.Range(-transform.localScale.x / 2, transform.localScale.x / 2);
			float zRnd = Random.Range(-transform.localScale.z / 2, transform.localScale.z / 2);

			/*xRnd = 0;
			zRnd = 0;*/

			featurePosition = new Vector3(transform.position.x + xRnd * .8f, transform.position.y + transform.localScale.y / 2 + .2f, transform.position.z + zRnd * .8f);

			GameObject newFeature = (GameObject)GameObject.Instantiate(TerrainManager.Instance.terrainFeatures[Random.Range(0, TerrainManager.Instance.terrainFeatures.Count)], Vector3.zero, transform.rotation);

			newFeature.transform.SetParent(transform.parent);
			newFeature.transform.position = featurePosition;
		}
	}
}
