using UnityEngine;
using System.Collections;

public class MultiToken : MonoBehaviour 
{
	//Bunch of references to stats and such. We could trim this down honestly.
	private GameObject player;
	private GameObject boss;
	//private BossStats bStats;
	//private PlayerStats pStats;
	//private Cryomancer runner;

	//What the token does. This class is more complex, but it's easier than having it inherit into anywhere from 3-10 different tokens. May get broken up later
	public bool damageBoss = true;
	public bool healPlayer = true;
	public bool restoreIce = true;
	public bool increaseMaxIce = true;
	public bool drainLava = false;
	
	public int damage = 5;
	public int heal = 10;
	public int iceGain = 10;
	public int maxIceGain = 5;
	public int lavaDrainDuration = 5;

	//Some audio info
	public bool playOnPickup = true;
	public AudioClip acquireClip;

	// Use this for initialization
	void Start () 
	{
		player = GameObject.FindGameObjectWithTag("Player");
		//boss = GameObject.FindGameObjectWithTag("Boss");
		//if (boss != null)
		//{
		//	bStats = boss.GetComponent<BossStats>();
		//}
		if (player != null)
		{
		//	pStats = player.GetComponent<PlayerStats>();
		//	runner = player.GetComponent<Cryomancer>();
		}
	}

	void OnTriggerEnter(Collider collider)
	{
		//Only when we hit the player
		if (collider.gameObject.name == "Player")
		{
			//Give the player a new passive/weapon.
			//Spawn new terrain
			//Spawn new enemies

			if (playOnPickup && acquireClip != null)
			{
				player.audio.clip = acquireClip;
				player.audio.Play();
			}
			if (light != null)
			{
				light.enabled = false;
			}
			enabled = false;
			renderer.enabled = false;
			//particleSystem.enableEmission = false;
		}
	}
}
