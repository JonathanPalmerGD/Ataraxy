using UnityEngine;
using System.Collections;

public class Checkpoint : MonoBehaviour
{
	//Public means anyone can access this. Private means only this class can.
	//Player is private because we're automatically populating it with the player.
	private GameObject player;
	private ParticleSystem partSys;
	private TeleTarget playerTarg;

	private float collisionRadius = 3.0f;
	//Don't forget to face the checkpoint

	// Use this for initialization
	void Start () 
	{
		//Get the player, put it in our private 'player' shoebox.
		player = GameManager.Instance.playerGO;

		if (player.GetComponent<TeleTarget>() == null)
		{
			player.AddComponent<TeleTarget>();
		}

		playerTarg = player.GetComponent<TeleTarget>();

		//Turn off the particle system. We'll turn it back on when the player collides with it.
		partSys = GetComponent<ParticleSystem>();
		if (partSys != null)
		{
			partSys.enableEmission = false;
		}
	}

	void OnTriggerEnter(Collider collider)
	{
		if (collider.gameObject.tag == "Player")
		{
			if (player.GetComponent<TeleTarget>().teleTarget != gameObject)
			{
				if (partSys != null)
				{
					partSys.enableEmission = true;
				}

				if (playerTarg == null)
				{
					Debug.LogError("Player's TeleTarget script is null.\n");
				}
				else
				{
					if (playerTarg.teleTarget == null)
					{

					}
					else
					{
						Checkpoint c = playerTarg.teleTarget.GetComponent<Checkpoint>();

						if (c != null)
						{
							c.Deactivate();
						}
					}
					player.GetComponent<TeleTarget>().teleTarget = gameObject;
				}
			}
		}
	}

	public void Deactivate()
	{
		if (partSys != null)
		{
			partSys.enableEmission = false;
		}
	}
}
