﻿using UnityEngine;
using System.Collections;

public class SliceAnimator : MonoBehaviour
{
	private float sOpenness = 1f;
	private float sClipFrequency = 1f;
	public float lerpDur = .55f;
	public bool opening = false;

	private float counter = 0;
	
	void Update() 
	{
		if (opening)
		{
			sOpenness = Mathf.Lerp(0, 1, counter / lerpDur);
		}
		else
		{
			sOpenness = Mathf.Lerp(1, 0, counter / lerpDur);
		}
		counter += Time.deltaTime;

		if (Input.GetKeyDown(KeyCode.Period))
		{
			opening = !opening;
			counter = 0;
			Debug.Log("Hit");
		}

		renderer.material.SetFloat("_Openness", sOpenness);
		renderer.material.SetFloat("_Offset", Time.realtimeSinceStartup%1);
		renderer.material.SetFloat("_Frequency", sClipFrequency);
	}
}
