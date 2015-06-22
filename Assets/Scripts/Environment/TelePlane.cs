using UnityEngine;
using System.Collections;

public class TelePlane : MonoBehaviour 
{
	//This class wants to go onto a flat plane with a texture on it. If the player falls below this plane, it will teleport the player back to the player's TeleTarget.
	//TeleTarget can be automatically updated when the player reaches Checkpoints.

	//Don't forget to turn off your plane collider.

	//So the plane can store a reference to the player.
	private GameObject player;

	public bool sendToStart = false;

	public AudioClip contactClipToPlay;
	public Vector3 startPos;
	public Quaternion startRot;

	// Use this for initialization
	void Start () 
	{
		//Fill our shoebox with a reference to the player.
		player = GameManager.Instance.playerGO;
		if (player.GetComponent<TeleTarget>() == null)
		{
			player.AddComponent<TeleTarget>();
		}

		startPos = player.transform.position;
		startRot = player.transform.rotation;
	}
	
	// Update is called once per frame
	void Update () 
	{
		//If the player is below the plane
		if(transform.position.y > player.transform.position.y)
		{
			if (contactClipToPlay != null)
			{
				player.GetComponent<AudioSource>().clip = contactClipToPlay;
				player.GetComponent<AudioSource>().Play();
			}

			//If the target object exists
			if (player.GetComponent<TeleTarget>() != null && player.GetComponent<TeleTarget>().lastCheckpoint != null)
			{
				//Put the player there.
				player.transform.position = player.GetComponent<TeleTarget>().lastCheckpoint.transform.position;
				player.transform.rotation = player.GetComponent<TeleTarget>().lastCheckpoint.transform.rotation;

				//Stop their movement
				player.GetComponent<Rigidbody>().velocity = Vector3.zero;
				//player.GetComponent<CharacterMotor>().SetVelocity(new Vector3(0, 0, 0));


				GameManager.Instance.player.FellBelow();
			}
			else
			{
				player.transform.position = startPos;
				player.transform.rotation = startRot;
				player.GetComponent<Rigidbody>().velocity = Vector3.zero;
				//player.GetComponent<CharacterMotor>().SetVelocity(new Vector3(0, 0, 0));


				GameManager.Instance.player.FellBelow();
			}
		}
	}
}
