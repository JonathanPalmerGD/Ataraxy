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

	//An inspector visible value. Does not do anything.
	[Header("Note: This variable is for display only")]
	public int audioClipsLoaded;

	public void Init()
	{
		if (audioLib == null)
		{
			audioLib = new Dictionary<string, AudioClip>();
		}
		if (audioSourcePrefab == null)
		{
			audioSourcePrefab = Resources.Load<GameObject>("Audio Source");
		}
		if (liveSources == null)
		{
			liveSources = new List<AudioSource>();
		}
	}

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

	public AudioClip FindOrLoadAudioClip(string clipName)
	{
		if (audioLib.ContainsKey(clipName))
		{
			return audioLib[clipName];
		}
		else
		{
			AudioClip audioClipToAdd = Resources.Load<AudioClip>("Audio/" + clipName);
			//AudioClip audioClipToAdd = Resources.Load<AudioClip>(clipName);
			if(audioClipToAdd != null)
			{
				audioClipsLoaded++;
				audioLib.Add(clipName, audioClipToAdd);
				return audioLib[clipName];
			}
		}

		Debug.LogError("[AudioManager]\n\tCould not find: " + clipName + " in Audio Lib");
		return null;
	}

	public void Update()
	{
		for (int i = 0; i < liveSources.Count; i++)
		{
			if (liveSources[i].isPlaying == false)
			{
				Destroy(liveSources[i].gameObject, 3.5f);
				liveSources.RemoveAt(i);
			}
		}
	}
}
