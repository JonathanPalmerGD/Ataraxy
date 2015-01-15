using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public enum Allegiance { Player, Neutral, Environment, Enemy, Dragon };

public class GameManager : Singleton<GameManager>
{
	public GameObject playerGO;
	public Player player;
	public GameObject tokenPrefab;
	public List<Enemy> enemies;
									//Pl	//Neu	//Env	//En	//Dr
	/*public bool[][] FactionHatred = {{false,	false, false,	true,	true},
								  {false,	false, false,	true,	true},
								  {false,	false, false,	true,	true},
								  {false,	false, false,	true,	true},
								  {false,	false, false,	true,	true},
								  };*/

	

	void Awake () 
	{
		playerGO = GameObject.FindGameObjectWithTag("Player");
		player = playerGO.GetComponent<Player>();
	}

	public void RegisterEnemy(Enemy reportingEnemy)
	{
		enemies.Add(reportingEnemy);
	}

	void Update () 
	{
		
	}
}