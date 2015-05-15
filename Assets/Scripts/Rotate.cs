using UnityEngine;
using System.Collections;

public class Rotate : MonoBehaviour
{
	//How fast we rotate
	public float rotationSpeed = 0.01f;
	//Around what?
	public Vector3 rotationAxis = Vector3.up;

	void Update()
	{
		if (!UIManager.Instance.paused)
		{
			transform.Rotate(rotationAxis, rotationSpeed);
		}
	}
}
