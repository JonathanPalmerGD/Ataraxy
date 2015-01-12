using UnityEngine;
using System.Collections;

public class LerpColor : MonoBehaviour 
{
	public bool changingColor = false;
	public Color oldColor;
	public Color targetColor;
	public float oldIntensity;
	public float targetIntensity;
	public float counter = 0;
	public float changeDuration = 2.5f;

	// Use this for initialization
	void Start () 
	{
		oldColor = light.color;
		oldIntensity = light.intensity;
	}
	
	/// <summary>
	/// Change the color of the color of something over time. Also provides an intensity
	/// </summary>
	void Update () 
	{
		if (changingColor)
		{
			counter += Time.deltaTime;
			if (counter > changeDuration)
			{
				changingColor = false;
				counter = 0;
				light.color = targetColor;
				light.intensity = targetIntensity;
			}
			else
			{
				light.color = new Color(Mathf.Lerp(oldColor.r, targetColor.r, counter / changeDuration), Mathf.Lerp(oldColor.g, targetColor.g, counter / changeDuration), Mathf.Lerp(oldColor.b, targetColor.b, counter / changeDuration), Mathf.Lerp(oldColor.a, targetColor.a, counter / changeDuration));
				light.intensity = Mathf.Lerp(oldIntensity, targetIntensity, counter / changeDuration);
			}
		}
	}

	public void ChangeColor(Color newColor, float newDuration, float newIntensity)
	{
		changingColor = true;
		counter = 0;
		targetIntensity = newIntensity;
		changeDuration = newDuration;
		oldColor = targetColor;
		targetColor = newColor;
	}
}
