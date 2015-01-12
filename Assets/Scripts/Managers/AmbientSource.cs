using UnityEngine;
using System;
using System.Linq;
using System.Collections;
using System.Collections.Generic;

public class AmbientSource : MonoBehaviour 
{
	private GameObject player;
	private Mesh mesh;
	private float counter = 0f;
	private float threshold = 0f;
	public List<string> localSounds;

	void Start () 
	{
		player = GameObject.FindGameObjectWithTag("Player");
		mesh = this.GetComponent<MeshFilter>().mesh;
	}
	
	void Update () 
	{
		counter += Time.deltaTime;

		if (collider.bounds.Contains(player.transform.position))
		{
			Debug.Log("Inside Mesh\n");
			foreach (AmbientGroup aud in AmbientManager.Instance.ambGroups)
			{
				if (aud.sleeping)
				{
					if (aud.sleepCounter > 0)
					{
						aud.sleepCounter -= Time.deltaTime;
					}
					if (aud.sleepCounter < 0)
					{
						aud.sleepCounter = 0;
						aud.sleeping = false;
					}
				}
			}

			if (counter >= 3 + threshold)
			{
				counter -= 3.0f;
				threshold += .2f;

				RollForAmbient();
			}

		}
		//while the player is inside
		//Increase the counter of each ambient audio.
		//If we exceed the counter, play that audio.
	}

	private void RollForAmbient()
	{
		for (int i = 0; i < 3; i++)
		{
			int playIndex = UnityEngine.Random.Range(0, AmbientManager.Instance.ambGroups.Count);
			//Debug.Log("Rolling for ambient noise: " + playIndex + " out of " + AmbientManager.Instance.ambAudios.Count + "\n");

			if (AmbientManager.Instance.ambGroups[playIndex].minFreq > counter && !AmbientManager.Instance.ambGroups[playIndex].sleeping)
			{
				float randRange = UnityEngine.Random.Range(0.0f, 1.0f);
				//Debug.Log("Rolled: " + randRange + " out of " + AmbientManager.Instance.ambAudios[playIndex].playChance + "\n");
				if (randRange < AmbientManager.Instance.ambGroups[playIndex].playChance)
				{
					PlayAudio(AmbientManager.Instance.ambGroups[playIndex]);
				}
			}
		}
	}

	private void PlayAudio(AmbientGroup ambientToPlay)
	{
		AudioSource audio = player.AddComponent<AudioSource>();

		int randIndex = UnityEngine.Random.Range(0, ambientToPlay.clips.Count);
		Debug.Log("Playing Audio: " + ambientToPlay.clips[randIndex].name);
		if (ambientToPlay.clips[randIndex] == null)
		{
			Debug.LogError("Clip is null. Investigate further.\n");
		}
		audio.PlayOneShot(ambientToPlay.clips[randIndex]);
		ambientToPlay.GotPlayed();
	}
}
