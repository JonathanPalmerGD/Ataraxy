using UnityEngine;
using System.Collections;

public class SetupSingletons : MonoBehaviour 
{
	public int SceneDifficulty = 1;

	void Awake()
	{
		Constants.GameDifficulty = SceneDifficulty;
		AudioManager.Instance.Init();
		TerrainManager.Instance.Awake();
		GameManager.Instance.Awake();
		//UIManager.Instance.Awake();
		UIManager.Instance.Init();
		LootManager.Instance.Awake();
		ModifierManager.Instance.Awake();

		GameManager.Instance.BeginGameMusic();

		Destroy(gameObject);
	}
	
	void Update() 
	{
	
	}
}
