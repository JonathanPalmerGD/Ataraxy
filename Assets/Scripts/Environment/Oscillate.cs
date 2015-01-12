using UnityEngine;
using System.Collections;

public class Oscillate : MonoBehaviour
{
	//We oscillate back and forth across our starting position at a frequency and an amplitude.
	public Vector3 amp = new Vector3(4.0f, 4.0f, 4.0f);
	public Vector3 freq = new Vector3(10.0f, 10.0f, 10.0f);
	public bool xOscillate = true;
	public bool yOscillate = false;
	public bool zOscillate = false;
	//This could become more complex if we had an updating source position. Idea - a ghost that follows the player and oscillates up and down (A boo from Mario?)
	private Vector3 initial;

	// Use this for initialization
	void Start()
	{
		initial = transform.position;
	}

	// Update is called once per frame
	void Update()
	{
		Vector3 temp = initial;
		#region X Oscillation - If we want to oscillate in the X axis
		if (xOscillate)
		{
			//Take our initial x position and change it by an amount.
			temp.x = initial.x + (amp.x * Mathf.Sin(freq.x * (Time.time) / 10));
		}
		else
		{
			//Otherwise just go with what our original x position was.
			temp.x = transform.position.x;
		}
		#endregion
		#region Y Oscillation - If we want to oscillate in the Y axis
		if (yOscillate)
		{
			temp.y = initial.y + (amp.y * Mathf.Sin(freq.y * (Time.time) / 10));
		}
		else
		{
			//Otherwise just go with what our original y position was.
			temp.y = transform.position.y;
		}
		#endregion
		#region Z Oscillation - If we want to oscillate in the Z axis
		if (zOscillate)
		{
			temp.z = initial.z + (amp.z * Mathf.Sin(freq.z * (Time.time) / 10));
		}
		else
		{
			//Otherwise just go with what our original z position was.
			temp.z = transform.position.z;
		}
		#endregion

		//Set our position to the determined target position
		transform.position = temp;
	}
}