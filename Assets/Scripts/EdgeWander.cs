﻿using UnityEngine;
using System.Collections;

/// <summary>
/// Creates wandering behaviour for a CharacterController.
/// </summary>
[RequireComponent(typeof(Rigidbody))]
public class EdgeWander : MonoBehaviour
{
	public float speed = 5;
	public float maxSpeed = 5;
	public float directionChangeInterval = 1;
	public float maxHeadingChange = 30;

	public Vector3 checkDistance;
	public float checkHeight;
	public float checkDist;

	public Vector3 Direction;
	public Vector3 CurrentVelocity;
	
	public bool haveNewHeading = false;

	public float counter = 0;
	public float turnTime = 1;

	float heading;
	Vector3 targetRotation;
	Vector3 oldRotation;

	void Awake()
	{
		// Set random initial rotation
		heading = Random.Range(0, 360);
		transform.eulerAngles = new Vector3(0, heading, 0);
		oldRotation = transform.eulerAngles;

		//StartCoroutine(NewHeading());
	}

	void FixedUpdate()
	{
		Direction = Vector3.zero;

		if (haveNewHeading)
		{
			counter += Time.deltaTime;
			transform.eulerAngles = Vector3.Lerp(oldRotation, targetRotation, (counter / turnTime));
			transform.eulerAngles = Vector3.Slerp(oldRotation, targetRotation, (counter / turnTime));
			if (counter > turnTime)
			{
				haveNewHeading = false;
			}
		}

		//Debug.DrawLine(transform.position, transform.position + transform.forward * 8, Color.red, 15);
		//Vector3 velocityHeading = new Vector3(0, rigidbody.velocity.y, 0);
		//transform.eulerAngles = velocityHeading;

		CurrentVelocity = rigidbody.velocity;
		Vector3 desiredDirection = transform.forward;
		//Vector3 desiredDirection = CurrentVelocity.normalized - transform.forward;

		//Debug.DrawLine(transform.position, transform.position + CurrentVelocity, Color.cyan, 15.0f);
		float forceAmt = maxSpeed - rigidbody.velocity.magnitude;

		//output += CurrentVelocity + "\t\t" + forceAmt + "\n";
		//If we are at an edge

		//get new heading?
		if (CheckEdge())
		{
			if (!haveNewHeading)
			{
				rigidbody.velocity = Vector3.zero;
				rigidbody.AddForce(-desiredDirection * forceAmt * forceAmt * rigidbody.mass);
				RandomNewHeading();
			}
		}
		else
		{
			if (!haveNewHeading)
			{
				rigidbody.AddForce(desiredDirection * forceAmt * rigidbody.mass);
			}
		}

		Direction.Normalize();

		#region Comments
		//Check if edge detection
		//If edge, change heading

		//If not edge, move forward
		/*
		heading *= maxSpeed;
		rigidbody.AddForce(heading * rigidbody.mass);
		transform.eulerAngles = Vector3.Slerp(transform.eulerAngles, targetRotation, Time.deltaTime * directionChangeInterval);
		var forward = transform.TransformDirection(Vector3.forward);
		controller.SimpleMove(forward * speed);*/

		/*transform.eulerAngles = Vector3.Slerp(transform.eulerAngles, targetRotation, Time.deltaTime * directionChangeInterval);
		var forward = transform.TransformDirection(Vector3.forward);
		controller.SimpleMove(forward * speed);*/
		#endregion
	}

	public bool CheckEdge()
	{
		RaycastHit[] hits;

		Vector3 start = transform.position + transform.forward * checkDist;
		Vector3 dir = -transform.up * (transform.localScale.y / 2 + checkHeight);
		Ray r = new Ray(start, dir);
		Debug.DrawRay(start, dir, Color.green);
		hits = Physics.RaycastAll(start, dir, (transform.localScale.y / 2 + 1));
		for(int i = 0; i < hits.Length; i++)
		{
			//Debug.Log(hits[i].collider.gameObject.tag);
			if (hits[i].collider.gameObject.tag == "Island")
			{
				//Debug.Log("Floor ahead");
				return false;
			}
		}
		//Perform a check to see if we're on a platform?

		return true;
	}

	void RandomNewHeading()
	{
		float newFloor = heading;
		if (newFloor + 180 > 360)
		{
			newFloor -= 180;
		}
		else
		{
			newFloor += 180;
		}

		float floor = Mathf.Clamp(newFloor - maxHeadingChange, 0, 360);
		float ceil = Mathf.Clamp(newFloor + maxHeadingChange, 0, 360);
		oldRotation = transform.eulerAngles;

		//Debug.Log("Heading: " + heading + "\tOpposite Direction: " + newFloor + "\n");
		heading = Random.Range(floor, ceil);
		//Debug.Log("Heading: " + heading + "\tOpposite Direction: " + newFloor + "\n");
		targetRotation = new Vector3(0, heading, 0);
		counter = 0;
		haveNewHeading = true;
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

	/// <summary>
	/// Calculates a new direction to move towards.
	/// </summary>
	void NewHeadingRoutine()
	{
		var floor = Mathf.Clamp(heading - maxHeadingChange, 0, 360);
		var ceil = Mathf.Clamp(heading + maxHeadingChange, 0, 360);
		heading = Random.Range(floor, ceil);
		targetRotation = new Vector3(0, heading, 0);
	}

}