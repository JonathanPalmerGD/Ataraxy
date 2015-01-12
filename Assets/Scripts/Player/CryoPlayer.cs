using UnityEngine;
using System.Collections;

public class CryoPlayer : MonoBehaviour
{
	#region Ice Runner Variables - This one's a doozy
	#region Our camera, motor and prefabs
	public Camera viewCamera;
	private CharacterMotor charMotor;
	public GameObject blockPrefab;
	public GameObject cylinderPrefab;
	public GameObject shieldPrefab;
	#endregion

	#region Our ability information, costs, durations and rates
	public float platformCost		= 15;
	public float platformDuration	= 6.0f;
	public float inclineCost		= 1;
	public float inclineDuration	= 2.0f;
	public float ascendRate			= 18;
	public float shieldCost			= 25;
	public float shieldDuration		= 0.75f;
	public float declineCost		= 1;
	public float declineDuration	= 2.0f;
	#endregion

	#region Ice Information
	public bool resourceBased		= true;
	public bool oldGUI				= false;
	public float maxIce				= 50;
	public float ice				= 50;
	public float refreshTime		= 0;
	public float targetTime			= 3.0f;
	public float rechargeRate		= 2.5f;
	public bool lockoutAbilities	= true;
	public float timeSinceAbility	= 0.0f;
	public float abilityLockout		= .75f;
	public Vector3 jumpPlatform		= new Vector3( 25, 30, 25 );
	#endregion

	#region Abilities unlocked
	public bool declineUnlocked		= true;
	public bool inclineUnlocked		= true;
	public bool shieldUnlocked		= true;
	public bool platformUnlocked	= true;
	#endregion

	#region GUI - textures and positioning
	public Vector2 curScreenSize;
	public Texture2D[] declineTex;
	public Texture2D[] inclineTex;
	public Texture2D[] shieldTex;
	public Texture2D[] platformTex;
	public Texture2D justUsedTex;
	public Texture2D[] iceBarTex;
	private int distFromEdge		= 32;
	private int texSize = 64;
	#endregion

	public bool[] buttonMatrix = { false, false, false, false };
	#endregion

	// Use this for initialization
	void Start()
	{
		charMotor = gameObject.GetComponent<CharacterMotor>();
	}

	void DrawTextUI()
	{
		
	}
	
	void OnGUI()
	{
		curScreenSize = new Vector2(Screen.width, Screen.height);

		//Only display our resources if our ice powers actually USE resources.
		if (resourceBased)
		{
			//if(!oldGUI)
			//	DrawGraphicalUI();
			//else
			//	DrawOldUI();
		}
	}

	// Update is called once per frame
	void Update()
	{
		//Update the buttons that we think are pressed or not
		UpdateButtonMatrix();

		#region Resource Based updating
		if (resourceBased)
		{
			if (refreshTime < targetTime)
			{
				refreshTime = refreshTime + Time.deltaTime;
			}
			if (refreshTime > targetTime)
			{
				if (ice < maxIce)
				{
					restoreIce(10 * rechargeRate * Time.deltaTime);
					//ice = ice + 10 * rechargeRate * Time.deltaTime;
				}
			}
			if (lockoutAbilities)
			{
				timeSinceAbility = timeSinceAbility + Time.deltaTime;
				if (IsInclineUp() || IsDeclineUp())
				{
					timeSinceAbility = 0;
				}
			}
		}
		#endregion
		#region Block Checking
		if (ice > 0 || !resourceBased)
		{ 
			#region R Block
			if ((!resourceBased || ice >= platformCost) && IsPlatform())
			{
				CreatePlatform();
			}
			#endregion
			#region Shift Block
			if ((!resourceBased || ice >= shieldCost) && IsShield())	//Incline block
			{
				CreateShield();
			}
			#endregion
			#region E Block
			if ((!resourceBased || ice >= inclineCost) && IsIncline() && (!lockoutAbilities || timeSinceAbility > abilityLockout))	//Incline block
			{
				CreateIncline();
			}
			#endregion
			#region Q Block
			if ((!resourceBased || ice >= declineCost) && IsDecline() && !IsIncline() && (!lockoutAbilities || timeSinceAbility > abilityLockout))	//Decline block
			{
				CreateDecline();
			}
			#endregion
		}
		#endregion
	}

	/// <summary>
	/// Pretty self explanatory.
	/// </summary>
	public void DrawGraphicalUI()
	{
		if (declineUnlocked)
		{
			if (ice >= declineCost)
			{
				if (IsDecline())
				{
					GUI.DrawTexture(new Rect(distFromEdge, Screen.height - (distFromEdge + texSize), texSize, texSize), justUsedTex);
				}
				else
				{
					GUI.DrawTexture(new Rect(distFromEdge, Screen.height - (distFromEdge + texSize), texSize, texSize), declineTex[1]);
				}
			}
			else
			{
				GUI.DrawTexture(new Rect(distFromEdge, Screen.height - (distFromEdge + texSize), texSize, texSize), declineTex[0]);
			}
		}
		if (inclineUnlocked)
		{
			if (ice >= inclineCost)
			{
				if (IsIncline())
				{
					GUI.DrawTexture(new Rect(distFromEdge + texSize, Screen.height - (distFromEdge + texSize), texSize, texSize), justUsedTex);
				}
				else
				{
					GUI.DrawTexture(new Rect(distFromEdge + texSize, Screen.height - (distFromEdge + texSize), texSize, texSize), inclineTex[1]);
				}
			}
			else
			{
				GUI.DrawTexture(new Rect(distFromEdge + texSize, Screen.height - (distFromEdge + texSize), texSize, texSize), inclineTex[0]);
			}
		}
		if (platformUnlocked)
		{
			if (ice >= platformCost)
			{
				GUI.DrawTexture(new Rect(distFromEdge + texSize * 2, Screen.height - (distFromEdge + texSize), texSize, texSize), platformTex[1]);
			}
			else
			{
				GUI.DrawTexture(new Rect(distFromEdge + texSize * 2, Screen.height - (distFromEdge + texSize), texSize, texSize), platformTex[0]);
			}
		} 
		if (shieldUnlocked)
		{
			if (ice >= platformCost)
			{
				GUI.DrawTexture(new Rect(distFromEdge + texSize * 3, Screen.height - (distFromEdge + texSize), texSize, texSize), shieldTex[1]);
			}
			else
			{
				GUI.DrawTexture(new Rect(distFromEdge + texSize * 3, Screen.height - (distFromEdge + texSize), texSize, texSize), shieldTex[0]);
			}
		}
		DrawIceBar();
	}

	/// <summary>
	/// The graphical ice bar. Needs a solution to having a bunch of ice.
	/// </summary>
	public void DrawIceBar()
	{
		Rect boxInfo = new Rect(Screen.width / 2 - ((int)(maxIce / 5)) * 20, Screen.height - (distFromEdge + texSize * 2), iceBarTex[0].width, iceBarTex[0].height);

		//For every 5 maxIce the player has
		for (int i = 1; i <= maxIce / 5; i++)
		{
			//If they have that group of 5
			if ((int)ice >= i * 5)
			{
				//Print full
				GUI.DrawTexture(new Rect(boxInfo.x + iceBarTex[5].width * i, boxInfo.y, boxInfo.width, boxInfo.height), iceBarTex[5]);
			}
			else if ((int)ice > (i - 1) * 5)
			{
				//Otherwise print whatever 1/5th they have.
				if ((int)ice - (i - 1) * 5 == 1)
				{
					GUI.DrawTexture(new Rect(boxInfo.x + iceBarTex[1].width * i, boxInfo.y, boxInfo.width, boxInfo.height), iceBarTex[1]);
				}
				else if ((int)ice - (i - 1) * 5 == 2)
				{
					GUI.DrawTexture(new Rect(boxInfo.x + iceBarTex[2].width * i, boxInfo.y, boxInfo.width, boxInfo.height), iceBarTex[2]);
				}
				else if ((int)ice - (i - 1) * 5 == 3)
				{
					GUI.DrawTexture(new Rect(boxInfo.x + iceBarTex[3].width * i, boxInfo.y, boxInfo.width, boxInfo.height), iceBarTex[3]);
				}
				else if ((int)ice - (i - 1) * 5 == 4)
				{
					GUI.DrawTexture(new Rect(boxInfo.x + iceBarTex[4].width * i, boxInfo.y, boxInfo.width, boxInfo.height), iceBarTex[4]);
				}
			}
			else
			{
				//If they don't have more than it or in between, print empty
				GUI.DrawTexture(new Rect(boxInfo.x + iceBarTex[0].width * i, boxInfo.y, boxInfo.width, boxInfo.height), iceBarTex[0]);
			}
		}	
	}

	//This is obsoleted but good when you have a bunch of mana. I maintained it for that reason
	public void DrawOldUI()
	{
		//Rectangle (Width, Height, xSize, ySize)
		string qMessage = "";
		string eMessage = "";
		string rMessage = "";
		string shiftMessage = "";

		#region Check which abilities are available
		if (ice >= platformCost && platformUnlocked)
		{
			rMessage = "R ";
		}
		if (ice >= shieldCost && shieldUnlocked)
		{
			shiftMessage = "Shift ";
		}
		if (lockoutAbilities && timeSinceAbility >= abilityLockout && ice > inclineCost)
		{
			if (ice >= inclineCost && !IsDecline() && inclineUnlocked)
			{
				eMessage += "E";
			}
			if (ice >= declineCost && !IsIncline() && declineUnlocked)
			{
				qMessage += "Q";
			}
		}
		#endregion

		GUI.Box(new Rect(Screen.width / 2 - 70, Screen.height - 90, 140, 46), ((int)ice).ToString());
		GUI.Label(new Rect(Screen.width / 2 - 65, Screen.height - 87, 10, 28), qMessage);
		GUI.Label(new Rect(Screen.width / 2 + 55, Screen.height - 87, 10, 28), eMessage);
		GUI.Label(new Rect(Screen.width / 2 - 3, Screen.height - 67, 10, 28), rMessage);
		GUI.Label(new Rect(Screen.width / 2 - 65, Screen.height - 67, 30, 28), shiftMessage);
	}

	#region Ability Creation Functions
	public void CreatePlatform()
	{
		//trans.pos - new v3 = (x - 0, y - 1, z - 0)
		Vector3 blockPosition = transform.position - new Vector3(0, 1.45f, 0);

		//Create a new object, that we will destroy after 3 seconds.
		Destroy(Instantiate(blockPrefab, blockPosition, new Quaternion(transform.rotation.x, transform.rotation.y, transform.rotation.z, transform.rotation.w)), platformDuration);

		GetComponent<DetectPad>().ApplyJump(jumpPlatform);

		ice = ice - platformCost;
		refreshTime = 0;
	}

	public void CreateIncline()
	{
		Vector3 blockPosition;
		CharacterMotor charMotor = gameObject.GetComponent<CharacterMotor>();
		if (!Physics.SphereCast(new Ray(transform.position, transform.up), .5f, 1.5f))
		{
			blockPosition = transform.position - new Vector3(0, 1.15f, 0);

			//Create a new object, that we will destroy after 3 seconds.
			Destroy(Instantiate(cylinderPrefab, blockPosition, new Quaternion(transform.rotation.x, transform.rotation.y, transform.rotation.z, transform.rotation.w)), inclineDuration);

			//Move the player upward
			transform.position += Vector3.up * ascendRate * Time.deltaTime;
			
			//Throttle the player's horizontal movement.
			charMotor.movement.velocity.x -= charMotor.movement.velocity.x / 3;
			charMotor.movement.velocity.z -= charMotor.movement.velocity.z / 3;
			
			if (charMotor.movement.velocity.y <= 0)
			{
				//Stop them from falling
				charMotor.movement.velocity.y = 0;
			}
		}
		else
		{
			blockPosition = transform.position - new Vector3(0, 1.15f, 0);

			//Create a new object, that we will destroy after 3 seconds.
			Destroy(Instantiate(cylinderPrefab, blockPosition, new Quaternion(transform.rotation.x, transform.rotation.y, transform.rotation.z, transform.rotation.w)), inclineDuration);

			charMotor.movement.velocity.x -= charMotor.movement.velocity.x / 6;
			charMotor.movement.velocity.z -= charMotor.movement.velocity.z / 6;
			if (charMotor.movement.velocity.y <= 0)
			{
				charMotor.movement.velocity.y = 0;
			}
		}

		ice = ice - inclineCost * Time.deltaTime * 10;
		refreshTime = 0;
	}

	public void CreateDecline()
	{
		//trans.pos - new v3 = (x - 0, y - 1, z - 0)
		Vector3 blockPosition = transform.position - new Vector3(0, 1.16f, 0);

		//Create a new object, that we will destroy after 3 seconds.
		Destroy(Instantiate(cylinderPrefab, blockPosition, new Quaternion(transform.rotation.x, transform.rotation.y, transform.rotation.z, transform.rotation.w)), declineDuration);

		//CharacterMotor charMotor = gameObject.GetComponent<CharacterMotor>();

		//transform.position += Vector3.up * .1f;
		//if (charMotor.movement.velocity.y < 0)
		//{
		//    charMotor.movement.velocity.y = charMotor.movement.velocity.y / 10;
		//}

		ice = ice - declineCost * Time.deltaTime * 10;
		refreshTime = 0;
	}

	public void CreateShield()
	{
		//trans.pos - new v3 = (x - 0, y - 1, z - 0)
		Vector3 blockPosition = transform.position + 3.0f * viewCamera.transform.forward;// - new Vector3(0, -.5f, 0);

		//Create a new object, that we will destroy after 3 seconds.
		//Destroy(Instantiate(shieldPrefab, blockPosition, new Quaternion(camera.transform.rotation.x, camera.transform.rotation.y, camera.transform.rotation.z, camera.transform.rotation.w)), platformDuration);
		Destroy(Instantiate(shieldPrefab, blockPosition, viewCamera.transform.rotation), shieldDuration);
		//Destroy(Instantiate(shieldPrefab, blockPosition, new Quaternion(transform.rotation.x, transform.rotation.y, transform.rotation.z, transform.rotation.w)), platformDuration);

		ice = ice - shieldCost;
		refreshTime = 0;
	}
	#endregion

	/// <summary>
	/// For updating if the player has certain buttons down or not
	/// </summary>
	public void UpdateButtonMatrix()
	{
		//Decline
		buttonMatrix[0] = IsDecline();
		//Incline
		buttonMatrix[1] = IsIncline();
		//Platform
		buttonMatrix[2] = IsPlatform();
		//Shield
		buttonMatrix[3] = IsShield();
	}

	#region Button Checking
	public bool IsPlatform()
	{
		//if (platformUnlocked && (Input.GetButtonDown("Redirect")))// || Input.GetMouseButtonDown(1)))
		//{
		//	return true;
		//}
		return false;
	}

	public bool IsShield()
	{
		//if (shieldUnlocked && (Input.GetButtonDown("Shield")))// || Input.GetMouseButtonDown(3)))
		//{
		//	return true;
		//}
		return false;
	}

	public bool IsDecline()
	{
		if (declineUnlocked && Input.GetMouseButton(0) && (Input.mousePosition.x > 0 && Input.mousePosition.y > 0 && Input.mousePosition.x < Screen.width && Input.mousePosition.y < Screen.height || !Screen.lockCursor))
		{
			return true;
		}
		/*if (declineUnlocked && (Input.GetButton("Glide")))
		{
			return true;
		}
		if (declineUnlocked && Input.GetAxis("Glide") > .5f)
		{
			return true;
		}*/
		return false;
	}

	public bool IsIncline()
	{
		if (inclineUnlocked && Input.GetMouseButton(4) && (Input.mousePosition.x > 0 && Input.mousePosition.y > 0 && Input.mousePosition.x < Screen.width && Input.mousePosition.y < Screen.height || !Screen.lockCursor))
		{
			return true;
		}
		/*if (inclineUnlocked && (Input.GetButton("Ascend")))
		{
			return true;
		}
		if (inclineUnlocked && Input.GetAxis("Ascend") > .5f)
		{
			return true;
		}*/
		return false;
	}

	public bool IsDeclineUp()
	{
		/*if (declineUnlocked && (Input.GetButtonUp("Glide")))// || Input.GetMouseButtonUp(0)))
		{
			return true;
		}
		else if (declineUnlocked && Input.GetAxisRaw("Glide") < 0)
		{
			return true;
		}*/
		return false;
	}

	public bool IsInclineUp()
	{
		/*if (inclineUnlocked && (Input.GetButtonUp("Ascend")))// || Input.GetMouseButtonUp(0)))
		{
			return true;
		}
		else if (inclineUnlocked && Input.GetAxisRaw("Ascend") < 0)
		{
			return true;
		}*/
		return false;
	}
	#endregion

	#region Ice Access methods - Called by tokens
	public void changeMaxIce(float newMaxIce)
	{
		maxIce = newMaxIce;
		if (ice > maxIce)
		{
			ice = maxIce;
		}
	}

	public void restoreIce(float restoreAmt)
	{
		if (ice < maxIce)
		{
			ice += restoreAmt;
			if (ice > maxIce)
			{
				ice = maxIce;
			}
		}
	}
	#endregion
}
