using UnityEngine;
using System.Collections;

public class MultiToken : MonoBehaviour 
{
	//Bunch of references to stats and such. We could trim this down honestly.
	private GameObject player;

	public bool healPlayer = true;
	public bool grantWeapon = true;
	public bool grantPassive = true;
	public bool grantExperience = false;

	public int heal = 10;
	public float xpReward = 10;

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
		if (collider.gameObject.tag == "Player")
		{
			//Debug.Log("Collected");
			//Give the player a new passive/weapon.
			//Spawn new terrain
			//Spawn new enemies

			if (grantExperience)
			{
				GameManager.Instance.player.GainExperience(xpReward);
			}
			if (Random.Range(0, 2) == 0)
			{
				GameManager.Instance.player.SetupAbility(NewWeapon());
			}
			else
			{
				GameManager.Instance.player.SetupAbility(Passive.New());
			}
			

			TerrainManager.Instance.CreateNewCluster();

			if (playOnPickup && acquireClip != null)
			{
				player.audio.clip = acquireClip;
				player.audio.Play();
			}
			if (light != null)
			{
				light.enabled = false;
			}
			gameObject.SetActive(false);
			renderer.enabled = false;
			if (particleSystem != null)
			{
				particleSystem.enableEmission = false;
			}
		}
	}

	Ability NewWeapon()
	{
		switch(Random.Range(0, 3))
		{
			case 0:
				return RocketLauncher.New();
			case 1:
				return ShockRifle.New();
			case 2:
				return Longsword.New();
			case 3:
				return Dagger.New();
			//case 4:
			//	return Dagger.New();
			default:
				return Weapon.New();
		}
	}
}
