using UnityEngine;
using System.Collections;

public class TargetingVisual : MonoBehaviour 
{
	public Entity parentEntity;

	//Targetting Display
	Vector3 startPoint;
	public Vector3 targetingDir = Vector3.zero;
	public Vector3 currentTrackPos;
	private GameObject trackingObject;

	GameObject target;

	bool leadTarget = false;

	//% of target's velocity to consider
	float LeadPercentage = 0.0f;

	//The distance the length 
	float lineDistance = 30f;

	//The distance the line extends beyond the target.
	float extendBeyond = 5f;

	//How quickly the targetting follows.
	float trackingStrength = 8f;

	public Color startLineColor = Color.black;
	public Color endLineColor = Color.red;
	public LineRenderer lineRend;

	public bool lockedOn = false;

	/// <summary>
	/// Move the tracking object towards the player
	/// </summary>
	public bool UpdateTracking = true;

	/// <summary>
	/// Move closer to the player or return home.
	/// </summary>
	public bool KnowledgeOfPlayer = true;
	/*public bool DisableTrack
	{
		set { disableTrack = value; }
		get { return disableTrack; }
	}*/

	void Start()
	{
		lineRend = GetComponent<LineRenderer>();
		if (lineRend == null)
		{
			lineRend = gameObject.AddComponent<LineRenderer>();
		}

		lineRend.material = new Material(Shader.Find("Particles/Alpha Blended"));

		lineRend.SetVertexCount(2);
		lineRend.SetWidth(.05f, .05f);

		trackingObject = new GameObject();
		trackingObject.transform.position = transform.position;
		trackingObject.name = "Tracking Object";
		trackingObject.transform.SetParent(transform);

		target = GameManager.Instance.playerGO;

		startPoint = transform.position;
		UpdateLineColor();
		UpdateLinePoints();
		currentTrackPos = transform.position;
	}

	void OnDestroy()
	{
		GameObject.Destroy(trackingObject);
	}

	void Update()
	{
		/*if (UpdateTracking)
			Debug.DrawLine(transform.position + Vector3.up * 5, transform.position + Vector3.up * 8, Color.blue, .1f);
		if (KnowledgeOfPlayer)
			Debug.DrawLine(transform.position + Vector3.up * 8, transform.position + Vector3.up * 11, Color.cyan, .1f);*/

		if(Input.GetKeyDown(KeyCode.Period))
		{
			Debug.Log("Toggle " + KnowledgeOfPlayer + " \n");
			KnowledgeOfPlayer = !KnowledgeOfPlayer;
		}

		startPoint = transform.position;
	}

	void FixedUpdate()
	{
		UpdateLinePoints();
	}

	public float counter = 0;
	void AdvanceTargeting(Vector3 targetToFace)
	{
		if (Input.GetKeyDown(KeyCode.F1))
		{
			counter = 0;
		}

		#region Direct Target
		/*Vector3 dirToTarget = transform.position - (targetToFace + Vector3.up);
		dirToTarget.Normalize();

		lineRend.SetPosition(1, transform.position - dirToTarget * lineDistance);*/
		#endregion

		#region Recovery Targetting (catches up quick, locks on slowly)
		//Old value for reference
		//float trackingStrength = .01f;
		/*float turnAmount = Mathf.Min(trackingStrength, 1);

		Vector3 dirToTarget = transform.position - (targetToFace + Vector3.up);

		dirToTarget = new Vector3(Mathf.Lerp(targetingDir.x, dirToTarget.x, turnAmount), Mathf.Lerp(targetingDir.y, dirToTarget.y, turnAmount), Mathf.Lerp(targetingDir.z, dirToTarget.z, turnAmount));
		targetingDir = dirToTarget;
		dirToTarget.Normalize();

		lineRend.SetPosition(1, transform.position - dirToTarget * lineDistance);*/
		#endregion

		#region Tracking Object
		//This approach uses the creation of tracking object to home towards the player (managed here).
		//The visual is created targeting the game object.
		//The result is the object moves towards the player
		//CON: The projectile jumps over the player. Should make it so if it is close enough, it snaps to the player.

		Vector3 targPos = trackingObject.transform.position;

		//Vector3 of a current position.
		Vector3 dirFromTrackPosToTarget = targetToFace - targPos;
		dirFromTrackPosToTarget.Normalize();

		Vector3 futurePos = trackingObject.transform.position + dirFromTrackPosToTarget * trackingStrength * Time.deltaTime;

		float futureDistToTarget = Vector3.Distance(targetToFace, futurePos);
		float curDistToTarget = Vector3.Distance(targetToFace, trackingObject.transform.position);

		if (futureDistToTarget < curDistToTarget)
		{
			//Every frame, move the current position at speed of N towards the target.
			trackingObject.transform.position += dirFromTrackPosToTarget * trackingStrength * Time.deltaTime;
		}
		else
		{
			trackingObject.transform.position = targetToFace;
			lockedOn = true;
		}
		//Draw line to the current position.

		targetingDir = transform.position - (trackingObject.transform.position);
		float targetDist = targetingDir.magnitude;
		targetingDir.Normalize();

		if (targetDist > lineDistance)
		{
			lineRend.SetPosition(1, transform.position - targetingDir * lineDistance);
		}
		else
		{
			lineRend.SetPosition(1, transform.position - targetingDir * (targetDist));
		}
		#endregion

		#region Constant Locking Speed Attempt
		//Vector3 of a current position.
		/*Vector3 dirFromTrackPosToTarget = targetToFace - currentTrackPos;
		dirFromTrackPosToTarget.Normalize();

		//Every frame, move the current position at speed of N towards the target.
		currentTrackPos += dirFromTrackPosToTarget * trackingStrength * Time.deltaTime;
		//Draw line to the current position.

		Vector3 dirToTarget = transform.position - (targetToFace + Vector3.up);

		dirToTarget = -dirFromTrackPosToTarget;

		lineRend.SetPosition(1, transform.position - dirToTarget * lineDistance);
		*/
		#endregion

		#region Accelerated Locking Speed
		//I want it to have a locking speed
		//Shorter distances means it will lock completely faster, as opposed to over a set interval.

		/*counter++;
		float turnAmount = counter / 900;

		Vector3 dirToTarget = transform.position - (targetToFace + Vector3.up);

		dirToTarget = new Vector3(Mathf.Lerp(targetingDir.x, dirToTarget.x, turnAmount), Mathf.Lerp(targetingDir.y, dirToTarget.y, turnAmount), Mathf.Lerp(targetingDir.z, dirToTarget.z, turnAmount));
		if(targetingDir == dirToTarget)
		{
			counter = 0;
		}
		targetingDir = dirToTarget;
		dirToTarget.Normalize();

		lineRend.SetPosition(1, transform.position - dirToTarget * lineDistance);*/
		#endregion
	}

	void UpdateLinePoints()
	{
		//We always want to update this because it is based on our position.
		lineRend.SetPosition(0, startPoint);

		if (UpdateTracking)
		{
			Vector3 targPos = target.transform.position;

			if (leadTarget)
			{
				if (target.rigidbody != null)
				{
					targPos += target.rigidbody.velocity * 3f;
				}
			}
			if (KnowledgeOfPlayer)
			{
				AdvanceTargeting(targPos);
			}
			else
			{
				AdvanceTargeting(transform.position);
			}
		}
	}

	public void UpdateLineColor()
	{
		lineRend.SetColors(startLineColor, endLineColor);
	}

	public void UpdateLineColor(Color newStartColor, Color newEndColor)
	{
		startLineColor = newStartColor;
		endLineColor = newEndColor;
		UpdateLineColor();
	}
}
