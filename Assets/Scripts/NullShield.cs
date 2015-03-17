using UnityEngine;
using System.Collections;

public class NullShield : MonoBehaviour 
{
	public GameObject fizzlePrefab;

	//Reference to parent
	public Entity Carrier;

	//A reference to our faction so we don't fizzle our own projectiles?
	//It'd be good to design to avoid doing this, but we want the fizzlers to be faction-caring.
	public Allegiance Faction;

	void Start()
	{
		fizzlePrefab = Resources.Load<GameObject>("Projectiles/FizzledProjectile");
	}

	void OnTriggerEnter(Collider other) 
	{
		if(other.tag == "Projectile")
		{
			Projectile proj = other.GetComponent<Projectile>();
			if (proj != null && proj.Faction != Faction)
			{
				
				GameObject fizzler = (GameObject)GameObject.Instantiate(fizzlePrefab, proj.transform.position, Quaternion.identity);
				GameObject.Destroy(fizzler, 1);
				
				//Destroy it
				DestroyProjectile(proj);

			}
			else
			{
				proj = other.transform.parent.GetComponent<Projectile>();
				if (proj != null && proj.Faction != Faction)
				{
					GameObject fizzler = (GameObject)GameObject.Instantiate(fizzlePrefab, proj.transform.position, Quaternion.identity);
					GameObject.Destroy(fizzler, 1);
					DestroyProjectile(proj);
				}
				//else
				//{
					//Debug.LogError("No Projectile parent with a Projectile Tag...?\n");
				//}
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
