﻿using UnityEngine;
using System.Collections;

public class HomingDeathAlert : MonoBehaviour
{
	public GameObject target;
	public Vector3 start;

	public bool homing = true;

	public float counter = 0.0f;
	public float homeDuration = 1.5f;
	public bool burst = false;
	public string message = "";
	public Enemy messageParameter = null;

	void Start()
	{
		start = transform.position;
	}

	void Update()
	{
		if (homing)
		{
			counter += Time.deltaTime;
			if (target == null)
			{
				gameObject.particleSystem.enableEmission = false;

				Destroy(gameObject);
			}
			else
			{
				transform.position = Vector3.Lerp(start, target.transform.position, counter / homeDuration);
			}

			if (counter >= homeDuration)
			{
				if (!burst)
				{
					burst = true;

					if(message != "" && messageParameter != null)
					{
						target.SendMessage(message, messageParameter, SendMessageOptions.DontRequireReceiver);
					}

					//Increase the particle size greatly to create an arrival flash.
					gameObject.particleSystem.startSpeed *= target.transform.localScale.x * 2;
					gameObject.particleSystem.startSize *= target.transform.localScale.x;

					//Destroy in a moment.
					Destroy(gameObject, 2.0f);
				}
			}
		}
	}
}