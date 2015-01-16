using UnityEngine;
using System.Linq;
using System.Collections;
using System.Collections.Generic;

public class TerrainManager : Singleton<TerrainManager>
{
	public static Vector3 minDistance = new Vector3(10, -12, 10);
	public static Vector3 maxDistance = new Vector3(50, 5, 50);
	public static Vector3 minScale = new Vector3(3, 2, 3);
	public static Vector3 maxScale = new Vector3(45, 15, 45);
	public static Vector3 poissonMinScale = new Vector3(10, 1, 10);
	public static Vector3 poissonMaxScale = new Vector3(18, 12, 18);
	public static Vector3 clusterSize = new Vector3(75, 20, 75);
	public static int poissonMinK = 13;
	public static int poissonMaxK = 30;
	public static float minTilt = -8;
	public static float maxTilt = 8;
	public static int minCountInCluster = 5;
	public static int maxCountInCluster = 9;

	public GameObject clusterPrefab;

	public List<Cluster> clusters;
	public List<Texture2D> textures;

	void Awake()
	{
		clusters = new List<Cluster>();
		textures = Resources.LoadAll<Texture2D>("Terrain").ToList();
	}

	public void RegisterCluster(Cluster reportingCluster)
	{
		clusters.Add(reportingCluster);
	}

	public void CreateNewCluster()
	{
		

	}

	void Update()
	{
		if (Input.GetKeyDown(KeyCode.P))
		{
			int islandCount = 0;
			foreach (Cluster c in clusters)
			{
				islandCount += c.platforms.Count;
			}
			Debug.Log("There are currently " + islandCount + " islands.\n");
		}

	}
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
