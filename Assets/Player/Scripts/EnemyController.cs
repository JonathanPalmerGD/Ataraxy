using UnityEngine;
using System.Collections.Generic;
using System.Linq;

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
	//public GameObject target;
	public Vector3 targetPosition;
	public Island lastLocation;

	public Stack<PathNode> curPath;
	public PathNode nextNode;
	public PathNode lastNode;

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

	#region [Mega Region] Footsteps, Speed, Running & Jumping Variables
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
	public float destDistance = 5;
	public enum GroundState { Falling, OnGround, Turning, Jumping, Stopped };
	public GroundState navState;
	public bool haveNewHeading = false;

	public bool ignoreJumpHeight;
	#endregion

	#region Player Knowledge
	[Header("Edge & Entity Detection")]
	public float trackingStrength = .5f;
	#endregion

	#region Private variables
	bool grounded;
	bool nearDestination;
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
				if (nextNode.transform.position.y - 3 > transform.position.y)
				{
					Debug.Log("Vertical Jump\n");
					ApplyJump(true);
				}
				else
				{
					ApplyJump(false);
				}
			}
		}
		else
		{
			//If we have jumps left AND we're falling
			if(jumpsDone < maxJumps-1)
			{
				bool jumpedYet = false;

				//Jump to Ascend
				if (!jumpedYet && nextNode.transform.position.y - 3 > transform.position.y && myRB.velocity.y < 0)
				{
					Debug.Log("Vertical Jump\n");
					ApplyJump(true);
					jumpedYet = true;
				}
				//If it is far away.
				if (!jumpedYet && !nearDestination && myRB.velocity.y < -10)
				{
					ApplyJump(false);
				}
			}
		}
	}
	void ApplyJump(bool vertical = false)
	{
		jumpsDone++;
		lastAiredPos = transform.position.y;
		myRB.velocity = new Vector3(myRB.velocity.x * (vertical ? 0.15f : 1), CalculateJumpVerticalSpeed(vertical), myRB.velocity.z * (vertical ? 0.15f : 1));
	}
	float CalculateJumpVerticalSpeed(bool vertical)
	{
		return Mathf.Sqrt(jumpHeight * 20f * (vertical ? 1.5f : 1));
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
		curPath = new Stack<PathNode>();

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
		if (Input.GetKeyDown(KeyCode.Insert))
		{
			toggleView = !toggleView;
		}
		#region When to Check
		if (Time.time > nextCheck)
		{
			nextCheck = Time.time + (1f / checksPerSecond);
			grounded = IsGrounded();
			moving = IsMoving();
			canStand = CanStand();
			nearDestination = isNearDestination();
			CheckEnvironment();
			CheckInvalidPath();
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
			//target = GameManager.Instance.playerGO;
		}
		#endif
		#endregion
		//Island targNearIsland = TerrainManager.Instance.FindIslandNearTarget(target);
		/*
		if (targNearIsland != null)
		{
			Debug.DrawLine(target.transform.position, targNearIsland.transform.position);
		}*/
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
		if (nextNode != null)
		{
			FaceTarget(nextNode.transform.position);
		}
		else
		{
			GetNewDestination();
		}
		#endregion

		#region Calculate Steering Force
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
		#endregion

		#region Arrival Condition - Get new Destination?
		if (nearDestination && navState != GroundState.Falling)
		{
			GetNewDestination();
		}
		#endregion

		#region Debug Display Path
		DisplayNearestNode();
		DisplayDestinationNode();
		DisplayPath(curPath);
		#endregion

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
			#region Add Force to Rigidbody
			if (grounded)
			{
				myRB.AddForce(velocityChange, ForceMode.VelocityChange);
			}
			else
			{
				//If we're near our destination, let the character move normally?
				if (nearDestination)
				{
					//Debug.Log("Near Destintaion Slow." + velocityChange + " \n");
					myRB.AddForce(velocityChange, ForceMode.VelocityChange);
				}
					//If in mid air then only change movement speed if actually trying to move
				else if (input.x != 0 || input.z != 0)
				{
					myRB.AddForce(velocityChange * 15f, ForceMode.Acceleration);
				}

			}
			#endregion
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
		//Debug.Log
		Vector3 steering = Vector3.forward;
		if (navState == GroundState.OnGround || navState == GroundState.Jumping)
		{
			float distFromDest = CheckDestinationDistance();
			
			steering = Vector3.forward;

			//Check distance to destination. If less distance, dampen this
			if (distFromDest < walkSpeed * .75f )
			{
				//Debug.DrawLine(transform.position, transform.position + Vector3.up * 15, Color.blue, 3.0f);
				steering += Vector3.back * 0.3f;
				return steering;
			}

			if (navState == GroundState.Jumping)
			{
				//If our velocity is forward, jump
				//JUMP!
				Jump();
			}
			
		}
		else if (navState == GroundState.Falling)
		{
			//If we have a target
			if (nextNode != null)
			{
				Jump();

				float distFromDest = CheckDestinationDistance();
				//Going to move this to function extensions later.
				float newDistFromDest = Constants.CheckXZDistance(transform.position + rigidbody.velocity.normalized, nextNode.transform.position);
				
				
				//Debug.Log(distFromDest + "\t\t\t" + newDistFromDest + "\n");
				if (distFromDest < newDistFromDest)
				{
					steering = Vector3.zero;
					return steering;
				}
			}
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
		else if(navState == GroundState.Stopped)
		{
			steering = Vector3.zero;
		}


		return steering.normalized;
	}

	void FaceTarget(Vector3 targetToFace)
	{
		//if (navState != GroundState.Falling)
		//{
			//Create a vector3 that represents the target on our plane.
			Vector3 xzPosition = new Vector3(targetToFace.x, transform.position.y, targetToFace.z);

			//Get the destination rotation
			Quaternion targRotation = Quaternion.LookRotation(xzPosition - transform.position);

			//Find a small value to turn about
			float turnAmount = Mathf.Min(trackingStrength * 1 / checksPerSecond, 1);

			//Set our rotation partially towards our goal.
			transform.rotation = Quaternion.Lerp(transform.rotation, targRotation, turnAmount);
		//}
	}

	void CheckInvalidPath()
	{
		if (lastNode != null && lastLocation != null && nextNode != null)
		{
			//If our previous path appears invalid	
			if (lastNode.island != lastLocation && nextNode.island != lastLocation)
			{
				Debug.Log("Forcing new path\n");
				//Force us to find a new one?
				curPath = new Stack<PathNode>();
				GetNewDestination();
			}
		}
	}

	float CheckDestinationDistance()
	{
		if (nextNode != null)
		{
			Vector2 posFlat = new Vector2(transform.position.x, transform.position.z);
			Vector2 nodePosFlat = new Vector2(nextNode.transform.position.x, nextNode.transform.position.z);

			//Find distance to the position.
			return Vector2.Distance(posFlat, nodePosFlat);
		}
		return float.MaxValue;
	}

	bool toggleView = true;
	void UpdateTarget()
	{
		if (toggleView)
		{
			if (curPath.Count < 1)
			{
				//curPath = TerrainManager.Instance.FindPathToRandomNeighborIsland(lastLocation, lastLocation.NearestNode(transform.position));

				if (GameManager.Instance.player.GetComponent<Controller>().lastLocation != null)
				{
					//Debug.Log(GameManager.Instance.player.GetComponent<Controller>().lastLocation + "\n" + lastLocation);
					if (GameManager.Instance.player.GetComponent<Controller>().lastLocation != lastLocation)
					{
						//Find our nearest node.
						PathNode pathStartLocation = lastLocation.NearestNode(transform.position);

						//Find a path from current location to the target.
						Stack<PathNode> newPath = TerrainManager.Instance.PathToIsland(lastLocation, GameManager.Instance.player.GetComponent<Controller>().lastLocation, 120);

						//Stack<PathNode> newPath = TerrainManager.Instance.FindPathToIsland(pathStartLocation, GameManager.Instance.player.GetComponent<Controller>().lastLocation, 90);

						/*List<PathNode> newPathReverse = newPath.ToList();
						newPath = new Stack<PathNode>();
						for (int i = 0; i < newPathReverse.Count; i++)
						{
							newPath.Push(newPathReverse[i]);
						}*/

						
						if (newPath.Count > 0)
						{
							Debug.Log("New Path Acquired: " + newPath.Count + "\n");
						}
						curPath = newPath;
					}
				}
				//Get a new path to the target.
			}
		}
		if (curPath.Count > 0)
		{
			
			//Update the last node we were at.
			lastNode = nextNode;

			//Set our nextNode destination to the top of the stack.
			nextNode = curPath.Pop();

			//Say we aren't near a node anymore.
			nearDestination = false;
		}
		else
		{
			nextNode = null;
		}
	}

	void GetNewDestination()
	{
		if(lastLocation != null)
		{
			UpdateTarget();
		}
	}

	void GetNewPath()
	{
		//Get a random adjacent island.
		//Ask TerrainManager for a path to that island?
	}

	void DisplayPath(Stack<PathNode> p)
	{
		if (p != null && p.Count > 0)
		{
			List<PathNode> pnList = new List<PathNode>();

			if (lastNode != null)
			{
				pnList.Add(lastNode);
			}
			pnList.Add(nextNode);
			pnList.AddRange(p.ToList());

			//Debug.DrawLine(transform.position, pnList[0].transform.position + Vector3.up, Color.black, 0.2f);


			for (int i = 1; i < pnList.Count; i++)
			{
				Vector3 firstPos = pnList[i - 1].transform.position;
				Vector3 secondPos = pnList[i].transform.position;
				//Debug.DrawLine(pnList[i - 1].transform.position + Vector3.up * i * 2, pnList[i].transform.position + Vector3.up * i * 2, Color.green, 15.0f);
				Debug.DrawLine(firstPos + Vector3.up * i * 2, secondPos + Vector3.up * i * 2, Color.green);
				Debug.DrawLine(firstPos + Vector3.up * i * 2, firstPos + Vector3.up * (i-1) * 2, Color.red);
				
			}
		}
	}

	/// <summary>
	/// Use this to display the node we are pathing to next
	/// </summary>
	void DisplayDestinationNode()
	{
		if (nextNode != null)
		{
			Debug.DrawLine(transform.position + Vector3.up * 3, nextNode.transform.position + Vector3.up * 3, Color.magenta);
		}
	}

	/// <summary>
	/// Use this to display the node we are closest to.
	/// </summary>
	void DisplayNearestNode()
	{
		if (lastLocation != null)
		{
			PathNode n = lastLocation.NearestNode(transform.position);

			Debug.DrawLine(transform.position, n.transform.position, Color.white);
		}
	}
	#endregion

	#region Environment Checking
	public void CheckEnvironment()
	{
		#region [Debug Code] Dot Product Checking
		/*if (target != null)
		{
			Vector3 forward = transform.TransformDirection(Vector3.forward);
			Vector3 toOther = target.transform.position - transform.position;

			float dotProduct = Vector3.Dot(forward.normalized, toOther.normalized);

			Debug.Log("Forward: " + forward.normalized + "\t\t\t" + toOther.normalized + "\nDot Product: " + dotProduct + "\n");

			//Debug.DrawLine(transform.position, transform.position + transform.forward * 10, Color.blue, 8.0f);
		}*/
		#endregion

		//If we have a next node we're going to
		if (nextNode != null)
		{
			//Are we on the floor. CheckFloor sets our CurLocation island as well
			if (grounded)
			{
				//Is there a ledge ahead of us?
				//Is there a wall ahead of us.
				bool wall = CheckWall();
				bool edge = CheckEdge();
				#region Wall Check
				if (wall)
				{
					navState = GroundState.Turning;
					if (!haveNewHeading)
					{
						//Say to get one based on the edges in front of us.
						//TurnNewHeading();
					}
				}
				#endregion
				#region Edge Check
				else if (edge)
				{
					//That way we can easily fail out to just turning around.
					bool goingToJump = false;

					#region NextNode Pathing Setup

					if (nextNode.island != lastLocation)
					{
						//Check if our target is in the direction we're facing.

						Vector3 forward = transform.TransformDirection(Vector3.forward);
						Vector3 toOther = nextNode.transform.position - transform.position;
						if (ignoreJumpHeight)
						{
							Vector3 targetHeightless = new Vector3(nextNode.transform.position.x, transform.position.y, nextNode.transform.position.z);

							toOther = targetHeightless - transform.position;
						}
						float dotProduct = Vector3.Dot(forward.normalized, toOther.normalized);
						//Debug.DrawLine(transform.position, transform.position + transform.forward * 10, Color.blue, 8.0f);

						//We want to check that we are facing in the right direction and that we're moving forward.
						float velocityDotProduct = Vector3.Dot(rigidbody.velocity.normalized, toOther.normalized);

						if (dotProduct > .99f && velocityDotProduct > .7f)
						{
							//Debug.Log("Dot Product: " + dotProduct + "\nVelocity Dot Product: " + velocityDotProduct);
							goingToJump = true;
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

					#endregion


					#region [Old Code] Single Path Node Setup
					/*
				//If our target isn't on our island
				if (target.tag == "PathNode")
				{
					PathNode pn = target.GetComponent<PathNode>();
					if (pn.island != lastLocation)
					{
						//Check if our target is in the direction we're facing.
						
						Vector3 forward = transform.TransformDirection(Vector3.forward);
						Vector3 toOther = target.transform.position - transform.position;
						if (ignoreJumpHeight)
						{
							Vector3 targetHeightless = new Vector3(target.transform.position.x, transform.position.y, target.transform.position.z);

							toOther = targetHeightless - transform.position;
						}
						float dotProduct = Vector3.Dot(forward.normalized, toOther.normalized);
						Debug.DrawLine(transform.position, transform.position + transform.forward * 10, Color.blue, 8.0f);

						//We want to check that we are facing in the right direction and that we're moving forward.
						float velocityDotProduct = Vector3.Dot(rigidbody.velocity.normalized, toOther.normalized);

						if (dotProduct > .97f && velocityDotProduct > .5f)
						{
							//Debug.Log("Dot Product: " + dotProduct + "\nVelocity Dot Product: " + velocityDotProduct);
							goingToJump = true;
						}
					}
				}*/
					#endregion
				}
				#endregion
				#region No Wall? No Edge? To the Ground State
				else
				{
					//In this state, we will navigate towards the target.
					navState = GroundState.OnGround;
				}
				#endregion
			}
			#region Not Grounded? To the Falling State with you
			else
			{
				navState = GroundState.Falling;
			}
			#endregion
		}
		#region If nextNode == Null - Enter Stopped GroundState
		else
		{
			//Try to acquire new destination?
			//GetNewDestination();
			navState = GroundState.Stopped;
		}
		#endregion
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
	public bool isNearDestination()
	{
		if (CheckDestinationDistance() < destDistance)
		{
			return true;
		}
		return false;
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
					lastLocation = hit.collider.gameObject.GetComponent<Island>();
				}
				return true;
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
				lastLocation = hit.collider.gameObject.GetComponent<Island>();
				return true;
			}
			else
			{
				lastLocation = null;
				return true;
				//Debug.Log("Raycasted something besides island.\n");
			}
		}

		return false;
	}

	#endregion
}