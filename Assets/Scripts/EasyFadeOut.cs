using UnityEngine;
 
[RequireComponent(typeof(AudioSource))]
 
public class EasyFadeOut : MonoBehaviour 
{
	public float fadeDuration = 3.5f;
	public bool removeObject = false;
 
	void FixedUpdate()
	{
		if (GetComponent<AudioSource>().volume > 0)
		{
			GetComponent<AudioSource>().volume = GetComponent<AudioSource>().volume - (Time.deltaTime / (fadeDuration + 1));
		}
		else
		{
			if(removeObject)
			{
				Destroy(gameObject);
			}
			else
			{
				Destroy (this);
			}
		}
	}
}