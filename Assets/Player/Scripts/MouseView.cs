using UnityEngine;
using System.Collections;

//taken from : http://forum.unity3d.com/threads/a-free-simple-smooth-mouselook.73117/
//Very simple smooth mouselook modifier for the MainCamera in Unity
//by Francis R. Griffiths-Keam - www.runningdimensions.com

[AddComponentMenu("Character/MouseView")]
public class MouseView : MonoBehaviour
{
	public float sensitivityMultiplier = 1f;
    public bool flipY = false;
	Vector2 mouseAbsolute;
	Vector2 smoothMouse;
	
	public Vector2 clampInDegrees = new Vector2(360, 180);
	public Vector2 sensitivity = new Vector2(6f, 6f);
	public Vector2 smoothing = new Vector2(1.5f, 1.5f);
	public Vector2 targetDirection;
	public Vector2 targetCharacterDirection;

    //The controller script thats on the player
    public Controller controller;

	void Start()
	{
        if (GetComponent<Controller>())
        {
            controller = GetComponent<Controller>();
        }
        else
        {
            if (transform.parent.GetComponent<Controller>())
            {
                controller = transform.parent.GetComponent<Controller>();
            }
        }
		targetDirection = transform.localRotation.eulerAngles;
	}
	
	void Update()
	{
		if (!UIManager.Instance.paused)
		{
			float flipMP = 1f;
			if (flipY)
			{
				flipMP = -1f;
			}
			var targetOrientation = Quaternion.Euler(targetDirection);
			var mouseDelta = new Vector2(Input.GetAxisRaw("Mouse X"), Input.GetAxisRaw("Mouse Y") * flipMP) * sensitivityMultiplier;
			mouseDelta = Vector2.Scale(mouseDelta, new Vector2(sensitivity.x * smoothing.x, sensitivity.y * smoothing.y));

			smoothMouse.x = Mathf.Lerp(smoothMouse.x, mouseDelta.x, 1f / smoothing.x);
			smoothMouse.y = Mathf.Lerp(smoothMouse.y, mouseDelta.y, 1f / smoothing.y);

			mouseAbsolute += smoothMouse;

			if (clampInDegrees.x < 360)
			{
				mouseAbsolute.x = Mathf.Clamp(mouseAbsolute.x, -clampInDegrees.x * 0.5f, clampInDegrees.x * 0.5f);
			}
			var xRotation = Quaternion.AngleAxis(-mouseAbsolute.y, targetOrientation * Vector3.right);
			transform.localRotation = xRotation;
			var yRotation = Quaternion.AngleAxis(mouseAbsolute.x, transform.InverseTransformDirection(Vector3.up));
			transform.localRotation *= yRotation;
			if (clampInDegrees.y < 360)
			{
				mouseAbsolute.y = Mathf.Clamp(mouseAbsolute.y, -clampInDegrees.y * 0.5f, clampInDegrees.y * 0.5f);
			}
			transform.localRotation *= targetOrientation;
		}
	}
}