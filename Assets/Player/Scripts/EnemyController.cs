﻿using UnityEngine;
using System.Collections.Generic;

[AddComponentMenu("Character/Controller")]
[RequireComponent(typeof(Rigidbody))]

/*

 * Public functions

 * Footstep();
 * IsGrounded();
 * IsMoving();
 * CanStand();
 * Jump();

*/
public class EnemyController : MonoBehaviour
{
	public GameObject target;
	public Vector3 targetPosition;
	public Island curLocation;

	public bool controllerAble = true;
	public Transform camera;    //The root object that contains the camera
	public float checksPerSecond = 10f; //The amount of times per second to run IsGrounded(), CanStand() and IsMoving()

	#region Falling
	[Header("Falling")]
	public float fallThreshold = 3f;    //How many units to fall to start counting as damage
	public float fallDamageMultiplier = 3f;
	public float maxFallSpeed = 20f;
	#endregion

	//Croucher croucher;
	public CapsuleCollider capsule;

	#region Footsteps
	[Header("Footsteps")]
	public AudioSource audioSource;
	public AudioClip footstepSound;
	public float footstepSpeed = 1.5f;
	#endregion

	#region Speed
	[Header("Speed")]
	public float currentSpeed;
	public float rotationSpeed;
	[Space(10f)]
	public float walkSpeed = 5f;
	public float crouchSpeed = 2.5f;
	public float climbSpeed = 5.5f;
	public float runSpeed = 7.5f;
	#endregion

	#region Running
	[Header("Running")]
	public bool ableToRun = false;
	public int stamina = 100;
	public float staminaRegenPerSecond = 15f;
	float nextStamina;
	[HideInInspector]
	public int maxStamina = 100;
	#endregion

	#region Jumping
	[Header("Jumping")]
	public bool canJump = true;
	public float jumpHeight = 2f;   //in unity units
	public int maxJumps = 2;
	public float airAccelerator = 0.5f;
	public float groundAccelerator = 1.5f;
	#endregion

	#region Edge & Entity Detection
	[Header("Edge & Entity Detection")]
	public bool rightValid = true;
	public bool leftValid = true;
	public bool turnRight = true;
	public bool turnLeft = true;
	private Vector3 checkDistance;
	public float checkHeight;
	public float checkDist;
	public enum GroundState { Falling, OnGround, Turning, Jumping };
	public GroundState navState;
	public bool haveNewHeading = false;
	#endregion

	#region Player Knowledge
	[Header("Edge & Entity Detection")]
	public float trackingStrength = .5f;
	#endregion

	#region Private variables
	bool grounded;
	float nextCheck;
	bool canStand;
	float speed;
	float acceleration;
	int jumpsDone = 0;
	float jumpedYPos;
	float landedYPos;
	bool lastGrounded;
	bool moving;
	bool lastCrouching;
	float nextFootstep;
	Rigidbody myRB;
	float lastAiredPos;
	Vector3 velocityChange;
	#endregion

	#region Hidden variables
	[HideInInspector]
	public bool crouching;
	//This can be used to alter the speed from another script
	[HideInInspector]
	public float speedMultiplier = 1f;
	#endregion

	#region Movement Methods
	public void Jump()
	{
		if(!canJump)
		{
			return;
		}
		if(grounded)
		{
			//Normal jumping if the player can stand
			if(canStand)
			{
				jumpsDone = 0;
				lastAiredPos = transform.position.y;
				myRB.velocity = new Vector3(myRB.velocity.x, CalculateJumpVerticalSpeed(), myRB.velocity.z);
			}
		}
		else
		{
			//Double jumping in mid air if possible
			if(jumpsDone < maxJumps-1)
			{
				jumpsDone++;
				lastAiredPos = transform.position.y;
				myRB.velocity = new Vector3(myRB.velocity.x, CalculateJumpVerticalSpeed(), myRB.velocity.z);
			}
		}
	}
	float CalculateJumpVerticalSpeed()
	{
		return Mathf.Sqrt(jumpHeight * 20f);
	}
	public void Footstep()
	{
		//networkView.RPC("Step", RPCMode.All);
		Step();
	}
	[RPC]
	void Step()
	{
		if(audioSource && footstepSound)
		{
			audioSource.pitch = Random.Range(0.9f, 1.1f);
			audioSource.volume = 0.8f;
			audioSource.maxDistance = 15f;
			audioSource.PlayOneShot(footstepSound);
		}
	}
	[RPC]
	void CrouchState(bool newCrouch)
	{
		//croucher.crouching = newCrouch;
	}
	#endregion

	#region Start, Update and FixedUpdate
	void Start()
	{
		//target = GameManager.Instance.playerGO;
		GetNewDestination();
		//croucher = GetComponent<Croucher>();
		capsule = GetComponent<CapsuleCollider>();

		myRB = GetComponent<Rigidbody>();
		myRB.freezeRotation = true;

		maxStamina = stamina;

		grounded = IsGrounded();
		lastGrounded = grounded;

		//If this is networked, make sure that the rigidbody is kinematic is true for the
		//people with a !networkView.isMine (myRB.isKinematic = true;)
	}
	void Update()
	{
		#region When to Check
		if (Time.time > nextCheck)
		{
			nextCheck = Time.time + (1f / checksPerSecond);
			grounded = IsGrounded();
			moving = IsMoving();
			canStand = CanStand();
			CheckEnvironment();
		}
		#endregion
		#region Landing or leaving ground
		if (lastGrounded != grounded)
		{
			//This sound will play when jumping or when landing
			if (grounded)
			{
				Footstep();
				nextFootstep = Time.time + (0.2f);
			}
			else
			{
				if (nextFootstep < Time.time - (0.05f))
				{
					Footstep();
				}
			}
			lastGrounded = grounded;
			if (lastGrounded == true)
			{
				landedYPos = transform.position.y;
				if (jumpedYPos > landedYPos)
				{
					float distanceFell = jumpedYPos - landedYPos;
					if (distanceFell > fallThreshold)
					{
						if (distanceFell * fallDamageMultiplier > 1.5f)
						{
							distanceFell -= fallThreshold;
							//Here is where you will do your fall damage calculation
							//playerHealth -= Mathf.RoundToInt(distanceFell * fallDamageMultiplier);
						}
					}
				}
			}
			else
			{
				lastAiredPos = transform.position.y;
				jumpsDone = 0;
			}
		}
		#endregion
		#region Minor Jump Checking Cleanup
		if (!grounded)
		{
			if (transform.position.y > lastAiredPos)
			{
				lastAiredPos = transform.position.y;
			}
			else
			{
				jumpedYPos = lastAiredPos;
			}
		}
		#endregion
		#region Stamina regeneration for running
		if (ableToRun)
		{
			if (stamina < maxStamina && Time.time > nextStamina)
			{
				nextStamina = Time.time + (1f / staminaRegenPerSecond);
				stamina += 1;
			}
		}
		#endregion
		#region Footstep sounds when moving
		if (moving && Time.time > nextFootstep && grounded)
		{
			float mp = Random.Range(0.8f, 1.2f);
			nextFootstep = Time.time + ((3.5f / currentSpeed) * mp) / footstepSpeed;
			Footstep();
		}
		#endregion
		#region Where the jump function happens
		if (Input.GetButtonDown("Jump") && controllerAble)
		{
			//Jump();
		}
		#endregion
		#region Pathing Dev Keys
		#if UNITY_EDITOR
		if (Input.GetKeyDown(KeyCode.I))
		{
			GetNewDestination();
		}
		if (Input.GetKeyDown(KeyCode.K))
		{
			target = GameManager.Instance.playerGO;
		}
		#endif
		#endregion
	}
	void FixedUpdate ()
	{
		currentSpeed = Mathf.Round(myRB.velocity.magnitude);
		speed = walkSpeed;

		#region Crouch Handling
		if (lastCrouching != crouching)
		{
			lastCrouching = crouching;
			//This can be an rpc call
			//networkView.RPC("CrouchState", RPCMode.All, crouching);
			CrouchState(crouching);
		}
		#endregion
		#region Crouching Input
		/*if (Input.GetKey(KeyCode.LeftControl) || !canStand)
		{
			if (grounded)
			{
				speed = crouchSpeed * speedMultiplier;
			}

			crouching = true;
		}*/
		#endregion
		#region Running Input
		else if (Input.GetKey(KeyCode.LeftShift) && canStand && stamina > 0 && ableToRun)
		{
			//Running
			if (grounded)
			{
				stamina -= 1;
				speed = runSpeed * speedMultiplier;
			}
			else
			{
				speed = walkSpeed * speedMultiplier;
			}
			crouching = false;
		}
		else
		{
			if(!canStand)
			{
				if (grounded)
				{
					speed = crouchSpeed * speedMultiplier;
				}
				crouching = true;
			}
			else
			{
				speed = walkSpeed * speedMultiplier;
				
				crouching = false;
			}
		}
		#endregion

		#region Ground Acceleration
		if (grounded)
		{
			if(controllerAble)
			{
				acceleration = groundAccelerator;
			}
		}
		else
		{
			if(controllerAble)
			{
				acceleration = airAccelerator;
			}
			else
			{
				acceleration = 0.1f;
			}
		}
		#endregion

		#region Rotation of Enemy
		if (target != null)
		{
			FaceTarget(target.transform.position);
		}
		else
		{
			GetNewDestination();
		}
		#endregion

		Vector3 input = CalcSteering();
		//Vector3 input = Vector3.right + Vector3.forward;
		//Debug.DrawLine(transform.position, (transform.position + transform.forward * 10), Color.green);
		Vector3 targetVelocity = input;
		targetVelocity = transform.TransformDirection(targetVelocity) * speed;

		velocityChange = targetVelocity;
		velocityChange = (targetVelocity - myRB.velocity);
		velocityChange.x = Mathf.Clamp(velocityChange.x, -acceleration, acceleration);
		velocityChange.z = Mathf.Clamp(velocityChange.z, -acceleration, acceleration);
		velocityChange.y = 0f;
		//Debug.DrawLine(transform.position + Vector3.up * 2, (transform.position + targetVelocity * 10) + Vector3.up * 2, Color.blue);

		CheckDestination();
		DisplayNearestNode();
		DisplayDestinationNode();

		#region Controllable
		if (controllerAble)
		{
			#region Speed limit for diagonal walking
			if (targetVelocity.sqrMagnitude > (speed * speed))
			{
				targetVelocity = targetVelocity.normalized * speed;
			}
			#endregion
			#region Speed limit for falling
			if (myRB.velocity.y < -maxFallSpeed)
			{
				myRB.velocity = new Vector3(myRB.velocity.x, -maxFallSpeed, myRB.velocity.z);
			}
			#endregion
			if (grounded)
			{
				myRB.AddForce(velocityChange, ForceMode.VelocityChange);
			}
			else
			{
				//If in mid air then only change movement speed if actually trying to move
				if (input.x != 0 || input.z != 0)
				{
					myRB.AddForce(velocityChange * 15f, ForceMode.Acceleration);
				}
			}
		}
		else
		{
			//If the player isnt supposed to move, the player movement on x and z axis is 0
			targetVelocity = Vector3.zero;
			myRB.velocity = new Vector3(0, myRB.velocity.y, 0);
		}
		#endregion
	}
	#endregion

	#region Pathing
	Vector3 CalcSteering()
	{
		Vector3 steering = Vector3.forward;
		if (navState == GroundState.OnGround || navState == GroundState.Jumping)
		{
			float distFromDest = CheckDestinationDistance();
			
			steering = Vector3.forward;

			//Check distance to destination. If less distance, dampen this
			if (distFromDest < walkSpeed * .75f )
			{
				//Debug.DrawLine(transform.position, transform.position + Vector3.up * 15, Color.blue, 3.0f);
				steering += Vector3.back * 0.7f;
				//acceleration = .2f;
			}

			if (navState == GroundState.Jumping)
			{
				//JUMP!
				Jump();
			}
			
		}
		else if (navState == GroundState.Falling)
		{
			return Vector3.zero;
		}
		else if (navState == GroundState.Turning)
		{
			if (leftValid && !rightValid)
			{
				steering += Vector3.left * 2; 
			}
			if (rightValid && !leftValid)
			{
				steering += Vector3.right * 2;
			}
			if (!rightValid && !leftValid)
			{
				steering += Vector3.back * 2;
			}
		}


		return steering.normalized;
	}


	void FaceTarget(Vector3 targetToFace)
	{
		if (navState != GroundState.Falling)
		{
			//Create a vector3 that represents the target on our plane.
			Vector3 xzPosition = new Vector3(targetToFace.x, transform.position.y, targetToFace.z);

			//Get the destination rotation
			Quaternion targRotation = Quaternion.LookRotation(xzPosition - transform.position);

			//Find a small value to turn about
			float turnAmount = Mathf.Min(trackingStrength * 1 / checksPerSecond, 1);

			//Set our rotation partially towards our goal.
			transform.rotation = Quaternion.Lerp(transform.rotation, targRotation, turnAmount);
		}
	}

	float CheckDestinationDistance()
	{
		if (target != null)
		{
			Vector2 posFlat = new Vector2(transform.position.x, transform.position.z);
			Vector2 nodePosFlat = new Vector2(target.transform.position.x, target.transform.position.z);

			//Find distance to the position.
			return Vector2.Distance(posFlat, nodePosFlat);
		}
		return float.MaxValue;
	}

	void CheckDestination()
	{
		if (CheckDestinationDistance() < 5)
		{
			GetNewDestination();
		}
	}

	void GetNewDestination()
	{
		if(curLocation != null)
		{
			PathNode n = curLocation.GetRandomNode(curLocation.NearestNode(transform.position), true);
			target = n.gameObject;
		}
	}

	/// <summary>
	/// Use this to display the node we are pathing to next
	/// </summary>
	void DisplayDestinationNode()
	{
		if (target != null)
		{
			Debug.DrawLine(transform.position + Vector3.up * 3, target.transform.position + Vector3.up * 3, Color.magenta, 1 / checksPerSecond);
		}
	}

	/// <summary>
	/// Use this to display the node we are closest to.
	/// </summary>
	void DisplayNearestNode()
	{
		if (curLocation != null)
		{
			PathNode n = curLocation.NearestNode(transform.position);

			Debug.DrawLine(transform.position, n.transform.position, Color.white, 1 / checksPerSecond);
		}
	}
	#endregion

	#region Environment Checking
	public void CheckEnvironment()
	{
		//Are we on the floor. CheckFloor sets our CurLocation island as well
		if (grounded)
		{
			//Is there a ledge ahead of us?
			//Is there a wall ahead of us.
			bool wall = CheckWall();
			bool edge = CheckEdge();
			if (wall)
			{
				navState = GroundState.Turning;
				if (!haveNewHeading)
				{
					//Say to get one based on the edges in front of us.
					//TurnNewHeading();
				}
			}
			else if(edge)
			{
				//That way we can easily fail out to just turning around.
				bool goingToJump = false;

				if (target == null)
				{
					GetNewDestination();
				}

				//If our target isn't on our island
				if (target.tag == "PathNode")
				{
					PathNode pn = target.GetComponent<PathNode>();
					if (pn.island != curLocation)
					{
						//Check if our target is in the direction we're facing.
						
						Vector3 forward = transform.TransformDirection(Vector3.forward);
						Vector3 toOther = target.transform.position - transform.position;

						float dotProduct = Vector3.Dot(forward, toOther);

						Debug.Log("Dot Product: " + dotProduct + "\n");

						Debug.DrawLine(transform.position, transform.position + transform.forward * 10, Color.blue, 8.0f);

						if (dotProduct > 5f)
						{
							goingToJump = true;
						}
					}
				}

				if (goingToJump)
				{
					navState = GroundState.Jumping;
				}
				else
				{
					navState = GroundState.Turning;
					if (!haveNewHeading)
					{
						//Say to get one based on the edges in front of us.
						//TurnNewHeading();
					}
				}
			}
			else
			{
				//In this state, we will navigate towards the target.
				navState = GroundState.OnGround;
			}

		}
		else
		{
			navState = GroundState.Falling;
		}
	}
	public bool CheckWall()
	{
		#region Left & Right Turn Checkers
		RaycastHit hit;

		Vector3 start = transform.position - transform.up * (transform.localScale.y / 5) + transform.right * (transform.localScale.y / 2 + .5f);
		Vector3 dir = transform.forward * (transform.localScale.y / 2 + 2);
		Ray r = new Ray(start, dir);
		Debug.DrawRay(start, dir, Color.cyan, 1 / checksPerSecond);
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
		Debug.DrawRay(start, dir, Color.cyan, 1 / checksPerSecond);
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

		return false;
	}
	public bool CheckEdge()
	{
		RaycastHit hit;	
		Ray r;

		Vector3 start = transform.position + transform.forward * checkDist + transform.right * (transform.localScale.y / 2 + .5f);
		Vector3 dir = -transform.up * (transform.localScale.y / 2 + checkHeight);
		r = new Ray(start, dir);
		Debug.DrawRay(start, dir, Color.green, 1 / checksPerSecond);
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
		Debug.DrawRay(start, dir, Color.green, 1 / checksPerSecond);
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
	
		return true;
	}
	#endregion

	#region Moving, Standing, Grounded checking
	public bool IsMoving()
	{
		float minSpeed = 0.5f;
		return myRB.velocity.magnitude > minSpeed;
	}
	public bool CanStand()
	{
		//croucher.defHeight is the original height of the capsule collider
		//because when crouching the capsule collider height changes
		//divided by 2 because the cast starts from the center of the player and not the top
		//float castDistance = croucher.defHeight / 2f + 0.1f;

		//Vector3 centerCast = new Vector3(transform.position.x, croucher.globalYPosition, transform.position.z);
		
		//return !Physics.Raycast(centerCast, transform.up, castDistance);
		return true;
	}
	public bool IsGrounded()
	{
		float castRadius = capsule.radius-0.1f;
		float castDistance = capsule.height/2f+1.8f;

		//1 cast in the middle, and 4 more casts on the edges of the collider
		Vector3 leftCast = new Vector3(transform.position.x-castRadius, transform.position.y, transform.position.z);
		Vector3 rightCast = new Vector3(transform.position.x+castRadius, transform.position.y, transform.position.z);
		Vector3 frontCast = new Vector3(transform.position.x, transform.position.y, transform.position.z+castRadius);
		Vector3 backCast = new Vector3(transform.position.x, transform.position.y, transform.position.z-castRadius);
		Vector3 centerCast = transform.position;

		List<Vector3> casts = new List<Vector3>();
		casts.Add(centerCast);
		casts.Add(frontCast);
		casts.Add(backCast);
		casts.Add(leftCast);
		casts.Add(rightCast);

		RaycastHit hit;
		for(int i = 0; i < casts.Count; i++)
		{
			if(Physics.Raycast(casts[i], -transform.up, out hit, castDistance))
			{
				if(hit.collider.gameObject.tag == "Island")
				{
					curLocation = hit.collider.gameObject.GetComponent<Island>();
					return true;
				}
			}
		}
		/* (Physics.Raycast(leftCast, -transform.up, castDistance) || Physics.Raycast(rightCast, -transform.up, castDistance) || 
			Physics.Raycast(frontCast, -transform.up, castDistance) || Physics.Raycast(backCast, -transform.up, castDistance) || 
				Physics.Raycast(centerCast, -transform.up, castDistance));*/

		return false;
	}
	#endregion

	#region Collisions and Triggers
	void OnTriggerStay(Collider what)
	{
		if(what.name == "Ladder")
		{
			myRB.velocity = Vector3.Lerp(myRB.velocity, new Vector3(myRB.velocity.x/4f, climbSpeed, myRB.velocity.z/4f), Time.deltaTime*15f);
		}
	}
	void OnTriggerExit(Collider what)
	{
		if(what.name == "Ladder")
		{
			jumpedYPos = transform.position.y;
		}
	}
	void OnTriggerEnter(Collider what)
	{
		if(what.name == "Death")
		{
			//playerHealth = 0;
		}
		if(what.name == "Safepad")
		{
			jumpedYPos = what.transform.position.y;
		}
	}
	void OnCollisionEnter(Collision what)
	{
		if(what.transform.name == "Safepad")
		{
			jumpedYPos = what.transform.position.y;
		}
	}
	#endregion


	#region Dead Code
	/// <summary>
	/// This is now obsolete. IsGrounded does this better.
	/// </summary>
	/// <returns></returns>
	public bool CheckFloor()
	{
		RaycastHit hit;

		Vector3 start = transform.position;
		Vector3 dir = -transform.up * (capsule.height + 2.2f);
		Ray r = new Ray(start, dir);
		Debug.DrawRay(start, dir, Color.magenta, 1 / checksPerSecond);
		if (Physics.Raycast(start, dir, out hit, (capsule.height + 2.2f)))
		{
			if (hit.collider.gameObject.tag == "Island")
			{
				curLocation = hit.collider.gameObject.GetComponent<Island>();
				return true;
			}
			else
			{
				curLocation = null;
				return true;
				//Debug.Log("Raycasted something besides island.\n");
			}
		}

		return false;
	}

	#endregion
}