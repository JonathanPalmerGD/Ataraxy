using UnityEngine;
using System.Linq;
using System.Collections;
using System.Collections.Generic;

public class TerrainManager : Singleton<TerrainManager>
{
	public static Vector3 minDistance = new Vector3(10, 0, 10);
	public static Vector3 maxDistance = new Vector3(50, 15, 50);
	public static Vector3 minScale = new Vector3(3, 2, 3);
	public static Vector3 maxScale = new Vector3(45, 25, 45);
	public static float minTilt = -15;
	public static float maxTilt = 15;
	public static int minCountInCluster = 6;
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
