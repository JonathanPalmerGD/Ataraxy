using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class CrystalAdornment : MonoBehaviour 
{
	public int numCrystals;
	public List<GameObject> crystalPrefabs;

	void Start() 
	{
		if (crystalPrefabs == null)
		{
			crystalPrefabs = new List<GameObject>();
		}

		float xSize = transform.localScale.x - 3;
		float zSize = transform.localScale.z - 3;

		PoissonDiscSampler pds = new PoissonDiscSampler(transform.localScale.x - 3, zSize, 3.5f, 20);

		#region PD Sample Loop
		GameObject newCrystal;
			
		foreach (Vector2 sample in pds.Samples())
		{
			newCrystal = (GameObject)GameObject.Instantiate(crystalPrefabs[Random.Range(0, crystalPrefabs.Count)]);

			newCrystal.transform.position = transform.position + new Vector3(sample.x - xSize / 2, -transform.localScale.y / 2, sample.y - zSize / 2);
			newCrystal.transform.eulerAngles = new Vector3(0, Random.Range(0, 360), 180);

			newCrystal.transform.parent = transform;
		}
		#endregion

		/*if (crystalPrefabs.Count > 0)
		{
			GameObject newCrystal;
			for (int i = 0; i < numCrystals; i++)
			{
				newCrystal = (GameObject)GameObject.Instantiate(crystalPrefabs[Random.Range(0, crystalPrefabs.Count)]);

				float xRange = Random.Range(-transform.localScale.x / 2 + newCrystal.transform.localScale.x, transform.localScale.x / 2 - newCrystal.transform.localScale.x);
				float zRange = Random.Range(-transform.localScale.z / 2 + newCrystal.transform.localScale.z, transform.localScale.z / 2 - newCrystal.transform.localScale.z);
				newCrystal.transform.position = transform.position + new Vector3(xRange, -transform.localScale.y / 2, zRange);
				newCrystal.transform.eulerAngles = new Vector3(0, Random.Range(0, 360), 180);

				newCrystal.transform.parent = transform;
			}
		}*/
	}
	
	void Update() 
	{
	
	}
}
