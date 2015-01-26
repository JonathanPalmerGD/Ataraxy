using UnityEngine;
using System.Collections;

/// <summary>
/// Creates wandering behaviour for a CharacterController.
/// </summary>
[RequireComponent(typeof(Rigidbody))]
public class EdgeWander : MonoBehaviour
{
	public enum GroundState { Falling, OnGround, NearEdge, NearWall, Turning };
	public GroundState navState;

	public static bool expensiveWallCheck = true;

	public bool rightValid = true;
	public bool leftValid = true;
	public bool turnRight = true;
	public bool turnLeft = true;

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

	public void CheckEnvironment()
	{
		//Are we on the floor
		if (CheckFloor())
		{
			//Is there a ledge ahead of us?
			//Is there a wall ahead of us.
			if(CheckWall() || CheckEdge())
			{
				navState = GroundState.Turning;
				if (!haveNewHeading)
				{
					//Say to get one based on the edges in front of us.
					TurnNewHeading();
				}
			}
			else
			{
				navState = GroundState.OnGround;
			}

		}
		else
		{
			navState = GroundState.Falling;
		}
	}

	public void ApplyMovement()
	{

		counter += Time.deltaTime;
		if (counter > turnTime)
		{
			haveNewHeading = false;
		}

		CurrentVelocity = rigidbody.velocity;
		Vector3 desiredDirection = transform.forward;
		float forceAmt = maxSpeed - rigidbody.velocity.magnitude;

		if (navState == GroundState.OnGround)
		{
			//If we don't have our new heading yet
			if (!haveNewHeading)
			{
				//Move forward
				rigidbody.AddForce(desiredDirection * forceAmt * rigidbody.mass);
			}
			
		}
		else if (navState == GroundState.Falling)
		{
			rigidbody.AddForce(-transform.up * forceAmt * rigidbody.mass);
			//Do nothing
		}
		else if( navState == GroundState.Turning)
		{
			//If we're turning right. Desired direction

			desiredDirection = -transform.forward;

			if (turnRight)
			{
				desiredDirection += transform.right;
			}
			if (turnLeft)
			{
				desiredDirection -= transform.right;
			}
			//Debug.DrawLine(transform.position, transform.position + desiredDirection * 15, Color.blue, 3.0f);

			rigidbody.AddForce(desiredDirection * 3 *  forceAmt * rigidbody.mass);

			transform.eulerAngles = Vector3.Lerp(oldRotation, targetRotation, (counter / turnTime));
			

			//transform.eulerAngles = targetRotation;
			//haveNewHeading = false;
		}
	}

	void FixedUpdate()
	{

		CheckEnvironment();

		ApplyMovement();
		
		
		/*
		Direction = Vector3.zero;

		CheckGroundState();

		if (navState == GroundState.Turning)
		{
			Debug.Log("Turning");
			TurnNewHeading();
			transform.eulerAngles = targetRotation;
			
		}
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

		CurrentVelocity = rigidbody.velocity;
		Vector3 desiredDirection = transform.forward;
		float forceAmt = maxSpeed - rigidbody.velocity.magnitude;


		//get new heading?
		if (navState == GroundState.NearEdge)
		{
			if (!haveNewHeading)
			{
				rigidbody.velocity = Vector3.zero;
				rigidbody.AddForce(-desiredDirection * forceAmt * forceAmt * rigidbody.mass);
				RandomNewHeading();
			}
		}
		else if (navState == GroundState.OnGround)
		{
			if (!haveNewHeading)
			{
				rigidbody.AddForce(desiredDirection * forceAmt * rigidbody.mass);
			}
		}
		else if(navState == GroundState.Falling)
		{
		}
		else if (navState == GroundState.Turning)
		{

		}

		Direction.Normalize();*/

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

	void TurnNewHeading()
	{
		float rotAmt = 15;
		switch (Random.Range(0, 1))
		{
			case 0:
				if (turnLeft)
				{
					//heading -= Random.Range(3, rotAmt); 
					heading -= 35;
				}
				else if (turnRight)
				{
					//heading += Random.Range(3, rotAmt);
					heading += 35;
				}
				else
				{
					RandomNewHeading();
				}
				break;
			case 1:
				if (turnRight)
				{
					//heading += Random.Range(3, rotAmt);
					heading += 35;
				}
				else if (turnLeft)
				{
					//heading -= Random.Range(3, rotAmt);
					heading -= 35;
				}
				else
				{
					RandomNewHeading();
				}
				break;
		}

		oldRotation = transform.eulerAngles;

		targetRotation = new Vector3(0, heading, 0);
		counter = turnTime - .2f;
		haveNewHeading = true;
	}

	

	public bool CheckWall()
	{
		if (expensiveWallCheck)
		{
			#region Left & Right Turn Checkers
			RaycastHit hit;

			Vector3 start = transform.position - transform.up * (transform.localScale.y / 5) + transform.right * (transform.localScale.y / 2 + .5f);
			Vector3 dir = transform.forward * (transform.localScale.y / 2 + 2);
			Ray r = new Ray(start, dir);
			Debug.DrawRay(start, dir, Color.cyan);
			if (Physics.Raycast(start, dir, out hit, (transform.localScale.y / 2 + 2)))
			{
				if (hit.collider.gameObject != gameObject)
				{
					turnRight = false;
				}
				else
				{
					turnRight = true;
				}
			}
			else
			{
				turnRight = true;
			}

			start = transform.position - transform.up * (transform.localScale.y / 5) - transform.right * (transform.localScale.y / 2 + .5f);
			dir = transform.forward * (transform.localScale.y / 2 + 2);
			r = new Ray(start, dir);
			Debug.DrawRay(start, dir, Color.cyan);
			if (Physics.Raycast(start, dir, out hit, (transform.localScale.y / 2 + 2)))
			{
				if (hit.collider.gameObject != gameObject)
				{
					turnLeft = false;
				}
				else
				{
					turnLeft = true;
				}
			}
			else
			{
				turnLeft = true;
			}
			#endregion

			if (!turnRight || !turnLeft)
			{
				return true;
			}
		}
		else
		{
			#region Old Wall Check
			RaycastHit hit;

			Vector3 start = transform.position;
			Vector3 dir = transform.forward * (transform.localScale.y / 2 + 5);
			Ray r = new Ray(start, dir);
			Debug.DrawRay(start, dir, Color.cyan, .1f);
			if (Physics.Raycast(start, dir, out hit, (transform.localScale.y / 2 + 5)))
			{
				if (hit.collider.gameObject != gameObject)
				{
					if (hit.collider.gameObject.tag == "Island")
					{
						return true;
					}
					else
					{
						//Debug.Log("Raycasted something besides island.\n");
						return true;
					}
				}
			}
			#endregion
		}


		return false;
	}

	public bool CheckFloor()
	{
		RaycastHit hit;

		Vector3 start = transform.position;
		Vector3 dir = -transform.up * (transform.localScale.y / 2 + .5f);
		Ray r = new Ray(start, dir);
		Debug.DrawRay(start, dir, Color.magenta, .1f);
		if (Physics.Raycast(start, dir, out hit, (transform.localScale.y / 2 + .5f)))
		{
			if (hit.collider.gameObject.tag == "Island")
			{
				return true;
			}
			else
			{
				Debug.Log("Raycasted something besides island.\n");
			}
		}

		return false;
	}

	public bool CheckEdge()
	{
		RaycastHit hit;

		if (expensiveWallCheck)
		{
			Ray r;

			Vector3 start = transform.position + transform.forward * checkDist + transform.right * (transform.localScale.y / 2 + .5f);
			Vector3 dir = -transform.up * (transform.localScale.y / 2 + checkHeight);
			r = new Ray(start, dir);
			Debug.DrawRay(start, dir, Color.green);
			if (Physics.Raycast(start, dir, out hit, (transform.localScale.y / 2 + checkHeight)))
			{
				rightValid = true;
			}
			else
			{
				rightValid = false;
			}

			start = transform.position + transform.forward * checkDist - transform.right * (transform.localScale.y / 2 + .5f);
			r = new Ray(start, dir);
			Debug.DrawRay(start, dir, Color.green);
			if (Physics.Raycast(start, dir, out hit, (transform.localScale.y / 2 + checkHeight)))
			{
				leftValid = true;
			}
			else
			{
				leftValid = false;
			}

			if (rightValid && leftValid)
			{
				return false;
			}
		}
		else
		{
			#region Old Edge
			RaycastHit[] hits;

			Vector3 start = transform.position + transform.forward * checkDist;
			Vector3 dir = -transform.up * (transform.localScale.y / 2 + checkHeight);
			Ray r = new Ray(start, dir);
			//Debug.DrawRay(start, dir, Color.green);
			hits = Physics.RaycastAll(start, dir, (transform.localScale.y / 2 + checkHeight));
			for (int i = 0; i < hits.Length; i++)
			{
				//Debug.Log(hits[i].collider.gameObject.tag);
				if (hits[i].collider.gameObject.tag == "Island")
				{
					//Debug.Log("Floor ahead");
					return false;
				}
			}
			//Perform a check to see if we're on a platform?
			#endregion
		}
		return true;
	}

	public void CheckGroundState()
	{
		if (CheckFloor())
		{
			if (CheckWall())
			{
				navState = GroundState.Turning;
			}
			else if (navState == GroundState.Turning)
			{
				haveNewHeading = false;
				navState = GroundState.OnGround;
			}
			Debug.Log("test");
			if (CheckEdge())
			{
				navState = GroundState.NearEdge;
			}
			else
			{
				navState = GroundState.OnGround;
			}
		}
		else
		{
			navState = GroundState.Falling;
		}
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
}