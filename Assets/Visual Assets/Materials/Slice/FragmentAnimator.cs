using UnityEngine;
using System.Collections;

public class FragmentAnimator : MonoBehaviour
{
	private float perComplete = 1f;
	private float lerpDur = 1.5f;
	public bool opening = false;

	private float counter = 0;
	
	void Update() 
	{
		if (opening)
		{
			perComplete = Mathf.Lerp(0, 1, counter / lerpDur);
		}
		else
		{
			perComplete = Mathf.Lerp(1, 0, counter / lerpDur);
		}
		counter += Time.deltaTime;

		if (Input.GetKeyDown(KeyCode.Period))
		{
			opening = !opening;
			counter = 0;
		}
		if (Input.GetKeyDown(KeyCode.Comma))
		{
			
		}

		renderer.material.SetFloat("_PerComplete", perComplete);
	}
}
