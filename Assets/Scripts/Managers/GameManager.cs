using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public enum Allegiance { Player, Neutral, Environment, Enemy, Dragon };

public class GameManager : Singleton<GameManager>
{
	public GameObject playerGO;
	public Player player;
	public GameObject tokenPrefab;
	public GameObject xpHomePrefab;
	public List<Enemy> enemies;
									//Pl	//Neu	//Env	//En	//Dr
	/*public bool[][] FactionHatred = {{false,	false, false,	true,	true},
								  {false,	false, false,	true,	true},
								  {false,	false, false,	true,	true},
								  {false,	false, false,	true,	true},
								  {false,	false, false,	true,	true},
								  };*/

	public override void Awake () 
	{
		base.Awake();
		tokenPrefab = Resources.Load<GameObject>("Token");
		xpHomePrefab = Resources.Load<GameObject>("Projectiles/HomingDeathAlert");
		playerGO = GameObject.FindGameObjectWithTag("Player");
		if (playerGO != null)
		{
			player = playerGO.GetComponent<Player>();
		}

		//Screen.showCursor = false;
		//Screen.lockCursor = true;
	}

	public HomingDeathAlert CreateTransferParticle(Enemy dyingEnemy, GameObject target, string message = "")
	{
		HomingDeathAlert homingSphere = ((GameObject)GameObject.Instantiate(xpHomePrefab, dyingEnemy.transform.position, Quaternion.identity)).GetComponent<HomingDeathAlert>();

		homingSphere.target = target;
		homingSphere.message = message;
		homingSphere.messageParameter = dyingEnemy;

		return homingSphere;
	}

	public void RegisterEnemy(Enemy reportingEnemy)
	{
		enemies.Add(reportingEnemy);
	}

	void Update () 
	{
		
	}
}