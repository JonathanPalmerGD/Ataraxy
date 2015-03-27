using UnityEngine;
using System.Collections;

public class SetupSingletons : MonoBehaviour 
{

	void Awake()
	{
		TerrainManager.Instance.Awake();
		GameManager.Instance.Awake();
		UIManager.Instance.Awake();
		LootManager.Instance.Awake();
		ModifierManager.Instance.Awake();

		Destroy(gameObject);
	}
	
	void Update() 
	{
	
	}
}
