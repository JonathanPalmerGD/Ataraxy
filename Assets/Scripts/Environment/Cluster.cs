using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class Cluster : WorldObject 
{
	public List<Island> platforms;
	public int poissonKVal = 20;
	public int sizeBonus = 0;

	public float tiltDeviation;

	public bool RandomScale = true;
	public bool RandomRotation = true;
	public bool RandomTexture = true;
	public bool RandomLandmarks = true;

	public override void Start()
	{
		TerrainManager.Instance.RegisterCluster(this);
		gameObject.name = "Cluster: " + Nomenclature.GetName(Random.Range(0, 12), Random.Range(0, 12), Random.Range(0, 12), Random.Range(0, 12));

		poissonKVal = Random.Range(TerrainManager.poissonMinK, TerrainManager.poissonMaxK);
		sizeBonus = Random.Range(0, 8);
		tiltDeviation = Random.Range(TerrainManager.minTiltDeviation, TerrainManager.maxTiltDeviation);
		//Debug.Log(tiltDeviation);
		RandomScale = Random.Range(0, 10) < 8;
		RandomRotation = Random.Range(0, 10) < 8;
		RandomTexture = Random.Range(0, 10) < 8;
		RandomLandmarks = Random.Range(0, 10) > 8;

		CreateIslandsPoissonApproach();
		if (RandomLandmarks)
		{
			//if (Random.Range(0, 100) > 90)
			//{
			CreateLandmarksPoissonApproach();
			//}
		}
		
		base.Start();
		gameObject.tag = "Cluster";
	}
	
	public override void Update() 
	{
		base.Update();
	}

	#region Approaches
	public void CreateIslandsPoissonApproach()
	{
		PoissonDiscSampler pds = new PoissonDiscSampler(100, 100, 16+(int)(sizeBonus * 1.5), poissonKVal);
		foreach (Vector2 sample in pds.Samples())
		{
			GameObject newIsland = null;
			if (TerrainManager.Instance.islandPrefabs.Count > 0)
			{
				
				float yOffset = Random.Range(TerrainManager.minDistance.y, TerrainManager.maxDistance.y);

				Vector3 newPosition = new Vector3(sample.x + transform.position.x, transform.position.y + yOffset, sample.y + transform.position.z);
				newIsland = (GameObject)GameObject.Instantiate(
					TerrainManager.Instance.islandPrefabs[Random.Range(0, TerrainManager.Instance.islandPrefabs.Count)], 
					newPosition, Quaternion.identity);

				ApplyRandomScale(newIsland, true);
				ApplyRandomRotation(newIsland);
				ApplyRandomTexturing(newIsland);
				ApplyIslandParent(newIsland);

				platforms.Add(newIsland.GetComponent<Island>());
			}
		}
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
				newLandmark = (GameObject)GameObject.Instantiate(
					TerrainManager.Instance.islandPrefabs[Random.Range(0, TerrainManager.Instance.islandPrefabs.Count)], 
					newPosition, Quaternion.identity);

				//ApplyRandomScale(newLandmark, true);
				ApplyRandomRotation(newLandmark);
				//ApplyRandomTexturing(newLandmark);
				ApplyIslandParent(newLandmark);

			}
		}
	}
	#endregion

	public void ApplyRandomScale(GameObject island, bool poisson)
	{
		if (RandomScale)
		{
			Vector3 scale = Vector3.zero;

			if (poisson)
			{
				scale = new Vector3(
							Random.Range(TerrainManager.poissonMinScale.x + sizeBonus, TerrainManager.poissonMaxScale.x + sizeBonus),
							Random.Range(TerrainManager.poissonMinScale.y + sizeBonus, TerrainManager.poissonMaxScale.y + (sizeBonus / 2)),
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
		island.transform.SetParent(transform);
	}

	public void LocalizeNeighbors()
	{

	}
}

