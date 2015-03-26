using UnityEngine;
using System.Collections;

public class MultiToken : MonoBehaviour 
{
	//Bunch of references to stats and such. We could trim this down honestly.
	private Player player;

	public bool grantSpecificWeapon = false;
	public string specificWeaponName = "";

	/// <summary>
	/// Note: This is only set during Start().
	/// </summary>
	public string InGameName = "Essence of Treasure";

	public bool createTerrain = true;
	public bool healPlayer = true;
	public bool grantWeapon = true;
	public bool grantPassive = false;
	public bool repairWeapon = false;
	public bool grantExperience = false;

	public int heal = 2;
	public int minRepair = 20;
	public int maxRepair = 45;
	public float xpReward = 10;

	//Some audio info
	public bool playOnPickup = true;
	public AudioClip acquireClip;

	// Use this for initialization
	void Start () 
	{
		if (grantSpecificWeapon)
		{
			name = "Treasure: " + specificWeaponName;
		}
		else
		{
			name = InGameName;
		}

		player = GameManager.Instance.player;
		if (player == null)
		{
			Debug.LogError("Token cannot find player Game Object.\n");
		}
		tag = "Token";

		//Give each child the token tag
		foreach(Transform child in transform)
		{
			child.tag = "Token";
		}
 
	}

	void OnTriggerEnter(Collider collider)
	{
		//Only when we hit the player
		if (collider.gameObject.tag == "Player")
		{
			if (grantExperience)
			{
				GameManager.Instance.player.GainExperience(xpReward);
			}
			if (healPlayer)
			{
				player.AdjustHealth(heal);
			}
			//This checks if we can actually give those things
			GrantWeaponOrPassive();
			//Checks if the token creates terrain.
			CreateTerrain();
			//Checks if the token repairs.
			RepairWeapon();
			//Plays audio if we have it.
			PlayAudio();

			//Disables the token
			DisableToken();
		}
	}
	
	#region Grant Weapons & Passives
	public void GrantWeaponOrPassive()
	{
		if (!grantPassive && !grantWeapon)
		{
			return;
		}
		if (!grantPassive && grantWeapon)
		{
			GrantWeapon();
		}
		else if (!grantWeapon && grantPassive)
		{
			GrantPassive();
		}
		else
		{
			if (player.weapons.Count == 0 || Random.Range(0, 4) != 0)
			{
				GrantWeapon();
			}
			else
			{
				GrantPassive();
			}
		}
	}

	public void GrantPassive()
	{
		player.SetupAbility(Passive.New());
	}
	public void GrantWeapon()
	{
		if (grantWeapon)
		{
			if (grantSpecificWeapon)
			{
				player.SetupAbility(LootManager.NewWeapon(specificWeaponName));
			}
			else
			{
				player.SetupAbility(LootManager.NewWeapon());
			}
		}
	}
	#endregion
	
	public void CreateTerrain()
	{
		if (createTerrain)
		{
			Cluster nearest = TerrainManager.Instance.FindNearestCluster(transform.position);

			if (nearest != null)
			{
				Debug.DrawLine(transform.position, nearest.transform.position, Color.cyan, 8.0f);
				TerrainManager.Instance.CreateNewCluster(nearest);
			}
			else
			{
				//Debug.LogError("Nearest cluster not found\n");
			}

		}
	}

	public void RepairWeapon()
	{
		if (repairWeapon)
		{
			player.AdjustActiveDurability(Random.Range(minRepair, maxRepair));
		}
	}

	public void PlayAudio()
	{
		if (playOnPickup && acquireClip != null)
		{
			player.gameObject.audio.clip = acquireClip;
			player.gameObject.audio.Play();
		}
	}

	public void DisableToken()
	{
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
