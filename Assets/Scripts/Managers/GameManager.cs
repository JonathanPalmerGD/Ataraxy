using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public enum Allegiance { Player, Neutral, Environment, Enemy, Dragon };

public class GameManager : Singleton<GameManager>
{
	public GameObject playerGO;
	public Player player;
	public Controller playerCont;
	public GameObject tokenPrefab;
	public GameObject repairTokenPrefab;
	public GameObject xpHomePrefab;
	public List<Enemy> enemies;

	// These are demo songs I am using. I do not have the license to distribute these songs.
	public string exploreClip = "Music/Seas of Valoran";
	public string combatClip = "Music/Freljord Expedition";
	public string bossClip = "Music/A Strange New World";
	public bool exploreActive = true;
	public bool combatActive = false;
	public bool bossActive = false;

	public bool enableMusic = true;

	public List<Enemy> enemiesEngaged;

	public override void Awake () 
	{
		base.Awake();
		tokenPrefab = Resources.Load<GameObject>("Token");
		repairTokenPrefab = Resources.Load<GameObject>("RepairToken");
		xpHomePrefab = Resources.Load<GameObject>("Projectiles/HomingDeathAlert");
		playerGO = GameObject.FindGameObjectWithTag("Player");
		if (playerGO != null)
		{
			player = playerGO.GetComponent<Player>();
			playerCont = playerGO.GetComponent<Controller>();
		}

		enemiesEngaged = new List<Enemy>();
#if !UNITY_EDITOR
		//Cursor.visible = false;
		//Cursor.lockState = CursorLockMode.Locked;

		//Unity <5
		//Screen.showCursor = false;
		//Screen.lockCursor = true;
#endif

	}

	public void BeginGameMusic()
	{		
		AudioSource exploreSource = AudioManager.Instance.MakeSource(exploreClip);
		AudioSource combatSource = AudioManager.Instance.MakeSource(combatClip);
		AudioSource bossSource = AudioManager.Instance.MakeSource(bossClip);

		exploreSource.loop = true;
		combatSource.loop = true;
		bossSource.loop = true;

		exploreSource.volume = 0;
		combatSource.volume = 0;
		bossSource.volume = 0;

		exploreSource.Play();
		combatSource.Play();
		bossSource.Play();

		AudioManager.Instance.AddMusicTrack(exploreSource, exploreActive);
		AudioManager.Instance.AddMusicTrack(combatSource, combatActive);
		AudioManager.Instance.AddMusicTrack(bossSource, bossActive);
	}

	/// <summary>
	/// This is called from Enemy.CanSeePlayer's mutator.
	/// Only called if the value is changing (meaning we need to remove or add the enemy to the engaged list.
	/// </summary>
	/// <param name="engagedEnemy">We update if they lose sight of the player, die or are removed.</param>
	/// <param name="canSeePlayer">Self Explanator</param>
	public void CanSeePlayer(Enemy engagedEnemy, bool canSeePlayer = false)
	{
		if (canSeePlayer)
		{
			if(!enemiesEngaged.Contains(engagedEnemy))
			{
				enemiesEngaged.Add(engagedEnemy);
			}
		}
		else
		{
			if (enemiesEngaged.Contains(engagedEnemy))
			{
				enemiesEngaged.Remove(engagedEnemy);
			}
		}
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
		#region Game Music State Handling
		if (enableMusic)
		{
			//Check through the enemies engaged
			//Debug.Log("[GameManager]\tEnemies Engaged: " + enemiesEngaged.Count + "\n");
			for (int i = 0; i < enemiesEngaged.Count; i++)
			{
				//Remove any that are dead or no longer exist
				if (enemiesEngaged[i] == null || enemiesEngaged[i].IsDead)
				{
					enemiesEngaged.RemoveAt(i);
					i--;
				}
			}

			//This will be improved later with a state machine, but it is a simple approach.
			if (enemiesEngaged.Count == 0)
			{
				AudioManager.Instance.SetTrackActivity(0, true);
				AudioManager.Instance.SetTrackActivity(1, false);
				AudioManager.Instance.SetTrackActivity(2, false);
				//AudioManager.Instance.tracksActive[0] = true;
				//AudioManager.Instance.tracksActive[1] = false;
				//AudioManager.Instance.tracksActive[2] = false;
			}
			else if (enemiesEngaged.Count > 0 && enemiesEngaged.Count < 6)
			{
				AudioManager.Instance.SetTrackActivity(0, false);
				AudioManager.Instance.SetTrackActivity(1, true);
				AudioManager.Instance.SetTrackActivity(2, false);
				//AudioManager.Instance.tracksActive[0] = false;
				//AudioManager.Instance.tracksActive[1] = true;
				//AudioManager.Instance.tracksActive[2] = false;
			}
			else
			{
				AudioManager.Instance.SetTrackActivity(0, false);
				AudioManager.Instance.SetTrackActivity(1, false);
				AudioManager.Instance.SetTrackActivity(2, true);
				//AudioManager.Instance.tracksActive[0] = false;
				//AudioManager.Instance.tracksActive[1] = false;
				//AudioManager.Instance.tracksActive[2] = true;
			}
		}
		else
		{
			if(AudioManager.Instance.tracksActive.Count > 0)
			{
				//If we don't want music on
				AudioManager.Instance.tracksActive[0] = false;
				AudioManager.Instance.tracksActive[1] = false;
				AudioManager.Instance.tracksActive[2] = false;
			}
		}

		if (Input.GetKeyDown(KeyCode.M))
		{
			enableMusic = !enableMusic;
		}

		/*
		if (Input.GetKey(KeyCode.RightShift) && Input.GetButtonDown("Quickslot 1"))
		{
			AudioManager.Instance.tracksActive[0] = !AudioManager.Instance.tracksActive[0];
		}
		if (Input.GetKey(KeyCode.RightShift) && Input.GetButtonDown("Quickslot 2"))
		{
			AudioManager.Instance.tracksActive[1] = !AudioManager.Instance.tracksActive[1];
		}
		if (Input.GetKey(KeyCode.RightShift) && Input.GetButtonDown("Quickslot 3"))
		{
			AudioManager.Instance.tracksActive[2] = !AudioManager.Instance.tracksActive[2];
		}*/
		#endregion
	}
}