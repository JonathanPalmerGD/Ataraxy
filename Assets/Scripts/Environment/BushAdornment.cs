using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class BushAdornment : MonoBehaviour 
{
	//public int numCrystals;
	public List<GameObject> bushPrefabs;

	void Start() 
	{
		if (bushPrefabs == null)
		{
			bushPrefabs = new List<GameObject>();
		}
		if (bushPrefabs.Count > 0)
		{
			float xSize = transform.localScale.x - 3;
			float zSize = transform.localScale.z - 3;

			PoissonDiscSampler pds = new PoissonDiscSampler(xSize, zSize, 12f, 20);

			#region PD Sample Loop
			GameObject newCrystal;

			foreach (Vector2 sample in pds.Samples())
			{
				newCrystal = (GameObject)GameObject.Instantiate(bushPrefabs[Random.Range(0, bushPrefabs.Count)]);

				newCrystal.transform.position = transform.position + new Vector3(sample.x - xSize / 2, transform.localScale.y / 2, sample.y - zSize / 2);
				newCrystal.transform.eulerAngles = new Vector3(0, Random.Range(0, 360), 00);

				newCrystal.transform.parent = transform;
			}
			#endregion
		}
	}
	
	void Update() 
	{
	
	}
}
