using UnityEngine;
using System.Collections;

public class TargetingVisual : MonoBehaviour 
{
	public Entity parentEntity;

	//Targetting Display
	Vector3 startPoint;
	Vector3 targetingDir = Vector3.zero;
	public Vector3 currentTrackPos;
	private GameObject trackingObject;

	GameObject target;

	//% of target's velocity to consider
	float LeadPercentage = 0.0f;

	//The distance the length 
	float lineDistance = 30f;

	//The distance the line extends beyond the target.
	float extendBeyond = 5f;

	//How quickly the targetting follows.
	float trackingStrength = 8f;

	public Color startLineColor;
	public Color endLineColor;
	public LineRenderer lineRend;

	void Start()
	{
		lineRend = GetComponent<LineRenderer>();
		if (lineRend == null)
		{
			lineRend = gameObject.AddComponent<LineRenderer>();
		}

		lineRend.material = new Material(Shader.Find("Particles/Additive"));

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
		UpdateLinePoints();
		//SetupLineRenderer(startPoint, startPoint+targettingDir*lineDistance)
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

		//Every frame, move the current position at speed of N towards the target.
		trackingObject.transform.position += dirFromTrackPosToTarget * trackingStrength * Time.deltaTime;
		//Draw line to the current position.

		Vector3 dirToTarget = transform.position - (trackingObject.transform.position);
		dirToTarget.Normalize();

		lineRend.SetPosition(1, transform.position - dirToTarget * lineDistance);
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
		lineRend.SetPosition(0, startPoint);

		Vector3 targPos = target.transform.position;
		if (target.rigidbody != null)
		{
			targPos += target.rigidbody.velocity * 3f;
		}

		AdvanceTargeting(targPos);
	}

	void UpdateLineColor()
	{
		lineRend.SetColors(startLineColor, endLineColor);
	}
}
