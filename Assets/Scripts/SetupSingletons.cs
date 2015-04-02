using UnityEngine;
using System.Collections;

public class SetupSingletons : MonoBehaviour 
{
	public int SceneDifficulty = 1;

	void Awake()
	{
		Constants.GameDifficulty = SceneDifficulty;
		TerrainManager.Instance.Awake();
		GameManager.Instance.Awake();
		//UIManager.Instance.Awake();
		UIManager.Instance.Init();
		AudioManager.Instance.Init();
		LootManager.Instance.Awake();
		ModifierManager.Instance.Awake();

		Destroy(gameObject);
	}
	
	void Update() 
	{
	
	}
}
