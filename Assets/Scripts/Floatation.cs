using UnityEngine;
using System.Collections;

public class Floatation : MonoBehaviour 
{
	public bool verticalWander = true;
	public Vector3 homeRegion;
	public float distFromHome;
	public float directionChangeInterval = 1;

	public float maxVerticalDist;
	public float yChange = 1;
	public float vertPush = 10;
	public float yFromHome;
	public float yWanderBound = 4;

	/// <summary>
	/// Amplifies or dampens the maximum vertical movement in a frame.
	/// Values between .25 and 4 are preferable.
	/// </summary>
	public float yDampener = 4;

	void Awake()
	{
		// Set random initial rotation
		//heading = Random.Range(0, 360);
		//transform.eulerAngles = new Vector3(0, heading, 0);

		//Vertical
		//maxVerticalDist;
		yChange = 1;
		vertPush = 10;
		yWanderBound = 5;
		yDampener = 4;

		homeRegion = transform.position;

		StartCoroutine(NewHeading());
	}

	void Update()
	{
		if (!UIManager.Instance.paused)
		{
			distFromHome = Vector3.Distance(transform.position, homeRegion);
			yFromHome = transform.position.y - homeRegion.y;

			Vector3 homeDir = transform.position - homeRegion;

			//Debug.Log(vertPush + " " + yChange + " ");
			if (verticalWander)
			{
				rigidbody.AddForce(Vector3.up * vertPush * yChange * rigidbody.mass);
			}

			if (yFromHome < yWanderBound && yFromHome > -yWanderBound)
			{
				Debug.DrawLine(transform.position, homeRegion, Color.green);
			}
			else
			{
				Debug.DrawLine(transform.position, homeRegion, Color.red);
			}
		}
	}

	/// <summary>
	/// Calculates a new direction to move towards.
	/// </summary>
	void NewHeadingRoutine()
	{
		#region Vertical Wander
		if (verticalWander)
		{
			//float lowestY = homeRegion.y - yWanderBound;
			//float highestY = homeRegion.y + yWanderBound;

			float minHeight = (homeRegion.y - yWanderBound) - transform.position.y;
			float maxHeight = (homeRegion.y + yWanderBound) - transform.position.y;

			yChange = Random.Range(minHeight, maxHeight) / yDampener;
			if (yFromHome > yWanderBound)
			{
				yChange = homeRegion.y - transform.position.y;
				Mathf.Clamp(yChange, -maxVerticalDist, maxVerticalDist);
			}
			//Debug.Log(transform.position.y + "    " + minHeight + "  " + maxHeight + "\n" + lowestY + "  " + highestY);
		}
		#endregion
	}

	/// <summary>
	/// Repeatedly calculates a new direction to move towards.
	/// Use this instead of MonoBehaviour.InvokeRepeating so that the interval can be changed at runtime.
	/// </summary>
	IEnumerator NewHeading()
	{
		while (true)
		{
			NewHeadingRoutine();
			yield return new WaitForSeconds(directionChangeInterval);
		}
	}
}
