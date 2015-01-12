using UnityEngine;
using System;
using System.Linq;
using System.Collections;
using System.Collections.Generic;

public class AmbientManager : Singleton<AmbientManager>
{
	protected AmbientManager() { } 

	[SerializeField]
	public List<AmbientGroup> ambGroups;
}

[Serializable]
public class AmbientGroup
{
	public string name;
	public float volume;
	[SerializeField]
	public List<AudioClip> clips;
	
	public bool expanded;

	public float playChance;

	/// <summary>
	/// The minimum frequency with which this sound can play.
	/// </summary>
	public float minFreq;

	/// <summary>
	/// How long the entire Ambient Audio group goes to sleep when 
	/// </summary>
	public float sleepDuration;
	public float sleepCounter;
	public bool sleeping;
	public bool expandedSliders;

	public AmbientGroup()
	{
		name = "New Ambient Sound";
		volume = 1.0f;
		minFreq = 10f;
		sleepDuration = 10f;
		sleepCounter = 3;
		sleeping = true;
		clips = new List<AudioClip>();
		expandedSliders = false;
	}

	public void GotPlayed()
	{
		sleeping = true;
		sleepCounter = sleepDuration;
	}
}
