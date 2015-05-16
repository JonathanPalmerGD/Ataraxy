using UnityEngine;
using System.Collections;

public class DamageZone : MonoBehaviour 
{
	public float counter = 0.0f;
	public float damageEvery = 0.75f;
	public float damageAmt = .75f;
	public float collisionRadius = 15.0f;
	public bool dangerous = true;

	void Update() 
	{
		counter += Time.deltaTime;
		
		if (dangerous)
		{
			if (counter >= damageEvery)
			{
				Vector3 playerPos = GameManager.Instance.playerGO.transform.position;

				//This could become a trigger volume, but it's fine as a sphere point check.
				float distanceBetween = Vector3.Distance(playerPos, transform.position);

				if (collisionRadius > distanceBetween)
				{
					counter = 0;
					GameManager.Instance.player.AdjustHealth(-damageAmt);
				}
			}
		}
	}
}
