using UnityEngine;
using System.Collections;

public class SetupSingletons : MonoBehaviour 
{

	void Awake()
	{
		TerrainManager.Instance.Awake();
		GameManager.Instance.Awake();
		UIManager.Instance.Awake();
		TerrainManager.Instance.Awake();

		Destroy(gameObject);
	}
	
	void Update() 
	{
	
	}
}
