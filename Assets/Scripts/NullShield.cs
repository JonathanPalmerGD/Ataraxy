using UnityEngine;
using System.Collections;

public class NullShield : MonoBehaviour 
{
	//Reference to parent
	//public Entity Carrier

	void OnTriggerEnter(Collider other) 
	{
		if(other.tag == "Projectile")
		{
			Projectile proj = other.GetComponent<Projectile>();
			if(proj != null)
			{
				//Destroy it
				DestroyProjectile(proj);
			}
			else
			{
				proj = other.transform.parent.GetComponent<Projectile>();
				if(proj != null)
				{
					DestroyProjectile(proj);
				}
				else
				{
					Debug.LogError("No Projectile parent with a Projectile Tag...?\n");
				}
			}
			//Tell it to destroy itself?
		}
	}

	public void DestroyProjectile(Projectile proj)
	{

		//Projectile fizzles itself now.
		//proj.Fizzle();
		//Destroy(proj);
		//Destroy(proj.gameObject, .5f);
	}
}
