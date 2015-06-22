using UnityEngine;
using System.Collections;

public class SliceAnimator : MonoBehaviour
{
	private float sOpenness = 1f;
	private float sClipFrequency = 4f;
	private float lerpDur = 1.5f;
	private float maxShieldPer = .0f;
	public bool opening = false;

	private float counter = 0;
	
	void Update() 
	{
		if (opening)
		{
			sOpenness = Mathf.Lerp(maxShieldPer, 1, counter / lerpDur);
		}
		else
		{
			sOpenness = Mathf.Lerp(1, maxShieldPer, counter / lerpDur);
		}
		counter += Time.deltaTime;

		if (Input.GetKeyDown(KeyCode.Comma))
		{
			opening = !opening;
			counter = 0;
		}
		if (Input.GetKeyDown(KeyCode.Period))
		{
			sClipFrequency--;

			if (sClipFrequency < 1)
			{
				sClipFrequency = 10;
			}
		}
		if (Input.GetKeyDown(KeyCode.Slash))
		{
			maxShieldPer += .1f;

			if (maxShieldPer > .5f)
			{
				maxShieldPer = 0;
			}
		}

		GetComponent<Renderer>().material.SetFloat("_Openness", sOpenness);
		//renderer.material.SetFloat("_Offset",( Time.realtimeSinceStartup / 10) %1);
		GetComponent<Renderer>().material.SetFloat("_Frequency", sClipFrequency);
	}
}
