using UnityEngine;
using System.Collections;

public class Projectile : MonoBehaviour 
{
	private Allegiance faction;
	public Allegiance Faction
	{
		get { return faction; }
		set { faction = value; }
	}

	void Start() 
	{
	
	}
	
	void Update() 
	{
	
	}

	void OnTriggerEnter(Collider collider)
	{
		string cTag = collider.gameObject.tag;
		if (cTag == "Entity" || cTag == "Player" || cTag == "Enemy" || cTag == "Island")
		{
			Entity e = collider.gameObject.GetComponent<Entity>();
			if (e != null)
			{
				if (e.Faction == Faction)
				{
					Debug.Log("Projectile collided with same Faction as firing source.\n");
				}
				else
				{

				}
			}
		}
		
	}
}
