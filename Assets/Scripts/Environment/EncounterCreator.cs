using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using System.Linq;

public class EncounterCreator : MonoBehaviour 
{
	public int numEnemies = 5;
	public List<GameObject> enemies;
	public Island location;
	public bool initialized = false;
	public bool nonRandomEncounter = false;

	public void Init()
	{
		if (!initialized)
		{
			numEnemies = Random.Range(1, 4);
			initialized = true;
			//Setup encounter.

			//Debug.Log("Island Size: " + location.transform.localScale + "\nPds: " + location.transform.localScale.x + "\t" + location.transform.localScale.z);
			PoissonDiscSampler pds = new PoissonDiscSampler(location.transform.localScale.x * .8f, location.transform.localScale.z * .8f, 8f, 20);

			List<Vector2> samples = pds.Samples().ToList();
			//Debug.Log(samples.Count + " \n");
			GameObject newEnemy = null;
			if (numEnemies <= samples.Count)
			{
				for (int i = 0; i < numEnemies; i++)
				{
					GameObject prefab = TerrainManager.Instance.enemyPrefabs[Random.Range(0, TerrainManager.Instance.enemyPrefabs.Count)];

					int randSample = Random.Range(0, samples.Count);

					Vector3 newPos = new Vector3(location.transform.position.x + samples[randSample].x - location.transform.localScale.x / 2, location.transform.position.y + location.transform.localScale.y / 2, location.transform.position.z + samples[randSample].y - location.transform.localScale.z / 2);

					//Debug.Log(newPos + "\n");

					newEnemy = (GameObject)GameObject.Instantiate(prefab, newPos, Quaternion.identity);

					newEnemy.transform.SetParent(location.transform.parent);
					enemies.Add(newEnemy);
					samples.RemoveAt(randSample);
				}
			}
			else
			{
				//Debug.Log(samples.Count + "\tFailed to create encounter.\nNot enough Poisson samples.");
				location.Family.encounterCounter--;
				Destroy(this.gameObject);
			}
		}

		//Debug.Log("Completed Init()\n" + enemies.Count);
	}

	void Start() 
	{
		if (nonRandomEncounter)
		{
			location = transform.parent.GetComponent<Island>();
			Init();
		}
	}
	
	void Update() 
	{
	
	}
}
