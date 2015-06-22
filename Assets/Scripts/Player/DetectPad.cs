using UnityEngine;
using System.Collections;

public class DetectPad : MonoBehaviour
{
	private Ray playerRay;
	public AudioClip jumpNoise;
	public bool rayCast;
	public RaycastHit hitInfo;
	public CharacterMotor charMotor;
	public float jumpRayHeight;

	// Use this for initialization
	void Start()
	{
		charMotor = GameObject.FindGameObjectWithTag("Player").GetComponent<CharacterMotor>();
	}

	// Update is called once per frame
	void Update()
	{
		// Cast a sphere downward to see if it hits anything. This should reset the player's jumps even on edges/inclines.
		playerRay = new Ray(transform.position, -1 * transform.up);

		//Raycast down
		rayCast = Physics.SphereCast(playerRay, 0.5f, out hitInfo, jumpRayHeight);
		
		//If we raycast with something that has a jumppad
		if (rayCast && hitInfo.collider.gameObject.tag == "Jumppad")
		{
			CharacterMotor charMotor = gameObject.GetComponent<CharacterMotor>();
			
			//Play a jump noise
			PlayJumpNoise();

			Jumppad jumppad = hitInfo.collider.gameObject.GetComponent<Jumppad>();

			Vector3 jumpVel = new Vector3(charMotor.transform.forward.x * 20.0f, jumppad.jumpVel.y, charMotor.transform.forward.z * 20.0f);
			
			//Jump in the direction the pad says to.
			charMotor.SetVelocity(jumpVel);
		}
	}

	/// <summary>
	/// This is a method that does the effect of the jump pad. That way we can have things trigger the jump even when we don't actually raycast into a jumppad
	/// </summary>
	/// <param name="jumpVel"></param>
	public void ApplyJump(Vector3 jumpVel)
	{
		PlayJumpNoise();

		jumpVel = new Vector3(charMotor.transform.forward.x * jumpVel.x, jumpVel.y, charMotor.transform.forward.z * jumpVel.z);
		charMotor.SetVelocity(jumpVel);
	}

	public void PlayJumpNoise()
	{
		GetComponent<AudioSource>().clip = jumpNoise;
		GetComponent<AudioSource>().Play();
	}
}
