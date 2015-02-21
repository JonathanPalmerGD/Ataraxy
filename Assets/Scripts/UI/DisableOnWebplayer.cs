using UnityEngine;
using System.Collections;

public class DisableOnWebplayer : MonoBehaviour 
{
	void Start()
	{
#if UNITY_WEBPLAYER
		gameObject.SetActive(false);
#endif
	}
	
	void Update() 
	{
	
	}
}
