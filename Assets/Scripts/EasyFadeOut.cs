using UnityEngine;
 
[RequireComponent(typeof(AudioSource))]
 
public class EasyFadeOut : MonoBehaviour 
{
	public float fadeDuration = 3.5f;
	public bool removeObject = false;
 
	void FixedUpdate()
	{
		if (audio.volume > 0)
		{
			audio.volume = audio.volume - (Time.deltaTime / (fadeDuration + 1));
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