//Written by Jonathan Palmer
//Email: Jon@JonathanPalmerGD.com
//Website: www.JonathanPalmerGD.com
//Documentation: http://www.jonathanpalmergd.com/2015/04/05/unity-simple-audiomanager/
//Feel free to use this script so long as you provide me credit and let me know you're using it!

using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class AudioManager : Singleton<AudioManager>
{
	//The dictionary that stores all of our audio clips.
	//Could support name variation with "ClipName" + random number at the end.
	public Dictionary<string, AudioClip> audioLib;

	//The currently live audio sources. This could be repurposed to use an object pooling pattern
	public List<AudioSource> liveSources;
	
	//The prefab that is created. It is an empty game object with an AudioSource on it.
	public GameObject audioSourcePrefab;

	//The music track audio sources as well as which tracks are enabled.
	public List<AudioSource> musicTracks;
	//Set these externally if you want to enable/disable a track or tracks. AudioManager's Update will handle the fading.
	public List<bool> tracksActive;

	//An inspector visible value. Editing this does not do anything.
	//Dictionary contents are not displayed in Unity's Inspector.
	[Header("Note: This variable is for display only")]
	public int audioClipsLoaded;

	public void Awake()
	{
		if (audioLib == null)
		{
			audioLib = new Dictionary<string, AudioClip>();
		}
		if (audioSourcePrefab == null)
		{
			audioSourcePrefab = Resources.Load<GameObject>("Audio Source");
		}
		//if (liveSources == null)
		//{
			liveSources = new List<AudioSource>();
		//}
		//if (musicTracks == null)
		//{
			musicTracks = new List<AudioSource>();
		//}
		//if (tracksActive == null)
		//{
			tracksActive = new List<bool>();
		//}
	}

	public void Update()
	{
		//Debug.Log("[AudioManager]\t " + tracksActive[0] + " " + tracksActive[1] + " " + tracksActive[2] + "\n");

		//We don't want to hold onto things forever. This could be udpated to object pooling
		for (int i = 0; i < liveSources.Count; i++)
		{
			if (liveSources[i] == null)
			{
				liveSources.RemoveAt(i);
			}
			else
			{
				if (liveSources[i].isPlaying == false)
				{
					Destroy(liveSources[i].gameObject, 3.5f);
					liveSources.RemoveAt(i);
				}
			}
		}

		//Clean up any null music tracks.
		for (int i = 0; i < musicTracks.Count; i++)
		{
			if (musicTracks[i] == null)
			{
				musicTracks.RemoveAt(i);
			}
			else
			{
				if (musicTracks[i].isPlaying == false)
				{
					Destroy(musicTracks[i].gameObject, 3.5f);
					tracksActive.RemoveAt(i);
					musicTracks.RemoveAt(i);
				}
			}
		}

		//So we can handle things fading in and out.
		for (int i = 0; i < musicTracks.Count; i++)
		{
			CrossFadeAudioSource(musicTracks[i], tracksActive[i]);
		}
	}

	#region Music Management

	public void AddMusicTrack(AudioSource audioSource, bool playing = false)
	{
		//Need to add it to both lists to be able to handle the fading in/out
		musicTracks.Add(audioSource);
		tracksActive.Add(playing);
	}

	public void CrossFadeAudioSource(AudioSource audioSource, bool fadingIn = false)
	{
		//Don't bother if it is null. Graceful failing
		if (audioSource)
		{
			if (fadingIn)
			{
				//We want to lerp our way towards louder until we're loud enough to snap the rest of the way.
				if (audioSource.volume < .90f)
				{
					audioSource.volume = Mathf.Lerp(audioSource.volume, 1.0f, 0.75f * Time.deltaTime);
				}
				else
				{
					audioSource.volume = 1;
				}
			}
			else
			{
				//Lerp our way out and snap to off (to avoid residual background noise)
				if (audioSource.volume > 0.05)
				{
					audioSource.volume = Mathf.Lerp(audioSource.volume, 0.0f, 1f * Time.deltaTime);
				}
				else
				{
					audioSource.volume = 0;
				}
			}
		}
	}
	#endregion

	#region Source Creation
	/// <summary>
	/// Makes and returns an audio source object either at the given position with the given parent or at the player's position.
	/// </summary>
	/// <param name="clipName">Will load if not currently loaded.</param>
	/// <param name="sourcePosition">Defaults to the player's position.</param>
	/// <param name="parent">Defaults to following the player around. If you want a nonmoving audio, use MakeSourceAtPos</param>
	/// <returns></returns>
	public AudioSource MakeSource(string clipName = "Error", Vector3 sourcePosition = default(Vector3), Transform parent = null)
	{
		GameObject newAudSourceGO = (GameObject)GameObject.Instantiate(audioSourcePrefab, Vector3.zero, Quaternion.identity);
		AudioSource newAudSource = null;
		
		newAudSourceGO.name = "Audio Source (" + clipName + ")";

		if (newAudSourceGO != null)
		{
			newAudSource = newAudSourceGO.GetComponent<AudioSource>();
			liveSources.Add(newAudSource);
		}
		#region Handle Defaults
		if (parent == null)
		{
			parent = GameManager.Instance.playerGO.transform;
		}
		if (sourcePosition == default(Vector3))
		{
			sourcePosition = GameManager.Instance.playerGO.transform.position;
		}
		#endregion

		newAudSourceGO.transform.SetParent(parent);
		newAudSourceGO.transform.position = sourcePosition;
		//newAudSource.loop = true;

		//Debug.DrawLine(newAudSourceGO.transform.position, newAudSourceGO.transform.position + Vector3.up * 10, Color.white, 15.0f); 

		newAudSource.clip = FindOrLoadAudioClip(clipName);

		return newAudSource;
	}

	/// <summary>
	/// Calls MakeSource with specific parameters.
	/// Sets AudioManager as the parent if the sound does not need to move.
	/// </summary>
	/// <param name="clipName">Will load if not currently loaded.</param>
	/// <param name="sourcePosition">Defaults to the player's position.</param>
	/// <returns></returns>
	public AudioSource MakeSourceAtPos(string clipName = "Error", Vector3 sourcePosition = default(Vector3))
	{
		return MakeSource(clipName, sourcePosition, transform);
	}

	public AudioClip FindOrLoadAudioClip(string clipName)
	{
		//If it already exists, serve it up
		if (audioLib.ContainsKey(clipName))
		{
			return audioLib[clipName];
		}
		else
		{
			//Otherwise we need to load it.
			//All Audio is inside of Audio/<contents>
			//Music is inside of Audio/Music/<contents>
			//Music is easily handled by passing in "Music/<Track Name>"

			AudioClip audioClipToAdd = Resources.Load<AudioClip>("Audio/" + clipName);
			//Debug.Log("Audio/" + clipName + "\n");
			
			//If we found something
			if(audioClipToAdd != null)
			{
				//Add it, increase our loaded counter
				audioClipsLoaded++;
				audioLib.Add(clipName, audioClipToAdd);

				//Fluff with a fork & serve to 4-6 hungry AudioSources.
				return audioLib[clipName];
			}
		}

		Debug.LogError("[AudioManager]\n\tCould not find: " + clipName + " in Audio Lib");
		return null;
	}
	#endregion

}
