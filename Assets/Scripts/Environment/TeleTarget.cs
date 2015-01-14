using UnityEngine;
using System.Collections;

public class TeleTarget : MonoBehaviour 
{
	//The  player's checkpoint
	public GameObject teleTarget;

	//A variety of things that reset the player (teleplanes, death obstacles) will simply set the player's position back to their teletarg. Checkpoints update the teletarget to target themselves.
}
