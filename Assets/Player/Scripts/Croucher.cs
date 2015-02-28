using UnityEngine;

[AddComponentMenu("Character/Croucher")]
[RequireComponent(typeof(CapsuleCollider))]
public class Croucher : MonoBehaviour {

	CapsuleCollider capsule;
	public float crouchSmooth = 10f;
    //Capsule collider height when crouching
    public float capsuleHeight = 1.4f;
    //Capsule collider center when crouching
    public float capsuleCenter = 0.3f;
    //Character y position when crouching
    public float crouchedCharacterY = 0f;

    //The root object that contains all graphical elements of the player
	public Transform character;
    //The root object that contains the camera
	public Transform mCamera;

    //The global position regardless of the crouching state
    public float globalYPosition = 0f;
    //The amount of units the character already crouches
    public float amountGoingToCrouch = 0.5f;
    [HideInInspector]
    public float defHeight;
    [HideInInspector]
    public float defCenterY;
	float defCharacterY;
	float defCameraY;

	public bool crouching = false;

	void Start()
	{
		capsule = GetComponent<CapsuleCollider>();

        defCameraY = mCamera.localPosition.y;
		defHeight = capsule.height;
		defCenterY = capsule.center.y;
		defCharacterY = character.localPosition.y;
	}
	void Update()
	{
		if(crouching)
        {
            globalYPosition = Mathf.Lerp(globalYPosition, capsule.transform.position.y + amountGoingToCrouch, Time.deltaTime * crouchSmooth);
            mCamera.localPosition = Vector3.Lerp(mCamera.localPosition, new Vector3(0f, defCameraY + crouchedCharacterY, 0f), Time.deltaTime * crouchSmooth);

			character.localPosition = Vector3.Lerp(character.localPosition, new Vector3(0f,crouchedCharacterY,0f), Time.deltaTime*crouchSmooth);
			capsule.height = Mathf.Lerp(capsule.height, capsuleHeight, Time.deltaTime*crouchSmooth);
			capsule.center = Vector3.Lerp(capsule.center, new Vector3(0f,capsuleCenter,0f), Time.deltaTime*crouchSmooth);
		}
		else
        {
            globalYPosition = Mathf.Lerp(globalYPosition, capsule.transform.position.y, Time.deltaTime * crouchSmooth);
            mCamera.localPosition = Vector3.Lerp(mCamera.localPosition, new Vector3(0f, defCameraY, 0f), Time.deltaTime * crouchSmooth);

			character.localPosition = Vector3.Lerp(character.localPosition, new Vector3(0f,defCharacterY,0f), Time.deltaTime*crouchSmooth);
			capsule.height = Mathf.Lerp(capsule.height, defHeight, Time.deltaTime*crouchSmooth);
			capsule.center = Vector3.Lerp(capsule.center, new Vector3(0f,defCenterY,0f), Time.deltaTime*crouchSmooth);
		}
	}
}
