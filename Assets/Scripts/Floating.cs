using UnityEngine;
using System.Collections;

public class Floating : MonoBehaviour 
{
	public bool verticalWander = true;
	//public bool planarWander = true;

	//public float horPush;

	public Vector3 homeRegion;
	//public float wanderBound;
	public float distFromHome;

	//public Vector3 targetRotation;
	//public float heading;
	//public float maxHeadingChange = 30;
	
	public float directionChangeInterval = 1;

	public float maxVerticalDist;
	public float yChange;
	public float vertPush;
	public float yFromHome;
	public float yWanderBound;

	/// <summary>
	/// Amplifies or dampens the maximum vertical movement in a frame.
	/// Values between .25 and 4 are preferable.
	/// </summary>
	public float yDampener;

	void Awake()
	{
		// Set random initial rotation
		//heading = Random.Range(0, 360);
		//transform.eulerAngles = new Vector3(0, heading, 0);

		//Vertical
		maxVerticalDist = 4;
		yChange = 0;
		vertPush = 10;
		yWanderBound = 5;

		//Horizontal
		//horPush = 25;
		//wanderBound = 20;
		homeRegion = transform.position;

		StartCoroutine(NewHeading());
	}

	void Update()
	{
		distFromHome = Vector3.Distance(transform.position, homeRegion);
		yFromHome = transform.position.y - homeRegion.y;
		//transform.eulerAngles = Vector3.Slerp(transform.eulerAngles, targetRotation, Time.deltaTime * directionChangeInterval);

		Vector3 homeDir = transform.position - homeRegion;

		//rigidbody.AddForce(homeDir * (distFromHome / (distFromHome - wanderBound)) * rigidbody.mass);
		
		if(verticalWander)
		{
			rigidbody.AddForce(Vector3.up * vertPush * yChange * rigidbody.mass);
		}
		//if (planarWander)
		//{
		//	rigidbody.AddForce(transform.forward * horPush * rigidbody.mass);
		//}

		//Debug.DrawLine(transform.position, transform.position + rigidbody.velocity * 3, Color.blue);
		if (yFromHome < yWanderBound && yFromHome > -yWanderBound)
		{
			Debug.DrawLine(transform.position, homeRegion, Color.green);
		}
		else
		{
			Debug.DrawLine(transform.position, homeRegion, Color.red);
		}

	}

	/// <summary>
	/// Calculates a new direction to move towards.
	/// </summary>
	void NewHeadingRoutine()
	{
		#region Horizontal Wander
		/*if (planarWander)
		{
			float floor = Mathf.Clamp(heading - maxHeadingChange, 0, 360);
			float ceil = Mathf.Clamp(heading + maxHeadingChange, 0, 360);
			heading = Random.Range(floor, ceil);
			targetRotation = new Vector3(0, heading, 0);

			if (distFromHome > wanderBound)
			{
				targetRotation = new Vector3(0, heading - 180, 0);
				yChange = homeRegion.y - transform.position.y;
				Mathf.Clamp(yChange, -maxVerticalDist, maxVerticalDist);
			}
		}*/
		#endregion
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
