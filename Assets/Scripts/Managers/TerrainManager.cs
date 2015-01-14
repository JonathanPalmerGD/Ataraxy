using UnityEngine;
using System.Linq;
using System.Collections;
using System.Collections.Generic;

public class TerrainManager : Singleton<TerrainManager>
{
	public static Vector3 minDistance = new Vector3(5, 0, 5);
	public static float maxDistance = 25;
	public static Vector3 minScale = new Vector3(5, 2, 5);
	public static Vector3 maxScale = new Vector3(45, 25, 45);
	public static float minTilt = -20;
	public static float maxTilt = 20;
	public static int minCountInCluster = 4;
	public static int maxCountInCluster = 12;

	public List<Cluster> clusters;

	void Awake()
	{
		clusters = new List<Cluster>();
	}

	void Start() 
	{
		/*
		GameObject[] clusterArray = GameObject.FindGameObjectsWithTag("Cluster");

		for (int i = 0; i < clusterArray.Length; i++)
		{
			clusters.Add(clusterArray[i].GetComponent<Cluster>());
		}*/
	}

	void Update() 
	{
	
	}

	public void RegisterCluster(Cluster reportingCluster)
	{
		clusters.Add(reportingCluster);
	}

	public void CreateNewCluster()
	{
		GameObject NewCluster = new GameObject();
		NewCluster.AddComponent<Cluster>();

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
