using UnityEngine;
using System.Collections;

public class SliceAnimator : MonoBehaviour
{
	private float sOpenness = 1f;
	private float sClipFrequency = .5f;
	private float lerpDur = 1.5f;
	public bool opening = false;

	private float counter = 0;
	
	void Update() 
	{
		if (opening)
		{
			sOpenness = Mathf.Lerp(.15f, 1, counter / lerpDur);
		}
		else
		{
			sOpenness = Mathf.Lerp(1, .15f, counter / lerpDur);
		}
		counter += Time.deltaTime;

		if (Input.GetKeyDown(KeyCode.Comma))
		{
			opening = !opening;
			counter = 0;
		}
		if (Input.GetKeyDown(KeyCode.Period))
		{
			sClipFrequency++;

			if (sClipFrequency > 9)
			{
				sClipFrequency = 1;
			}
		}
		if (Input.GetKeyDown(KeyCode.Slash))
		{
			sClipFrequency += 2;

			if (sClipFrequency > 9)
			{
				sClipFrequency = 1;
			}
		}

		renderer.material.SetFloat("_Openness", sOpenness);
		//renderer.material.SetFloat("_Offset",( Time.realtimeSinceStartup / 10) %1);
		renderer.material.SetFloat("_Frequency", sClipFrequency);
	}
}
