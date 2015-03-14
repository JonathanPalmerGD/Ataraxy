using UnityEngine;
using UnityEngine.UI;
using System.Collections;

public class Crosshair : MonoBehaviour 
{
	private int crosshairIndex = 3;
	/// <summary>
	/// Accessor for Crosshair Index. 
	/// Returns Normally.
	/// Set checks validity. Will Debug.LogError if invalid.
	/// </summary>
	public int CrosshairIndex
	{
		get {return crosshairIndex;}
		set
		{
			if (value < crosshairs.Length && value >= 0)
			{
				crosshairIndex = value;
				RefreshCrosshair();
			}
			else
			{
				Debug.LogError("Incorrect crosshair index: " + value + " out of " + crosshairs.Length + " possible crosshairs.\n");
			}
		}
	}


	//Needs code for adjusting the size of the crosshair.

	/// <summary>
	/// Stores the crosshair sprites of all available crosshairs.
	/// Will be used for different weapons having separate crosshairs.
	/// </summary>
	public Sprite[] crosshairs;
	private Color crosshairColor = Color.white;
	/// <summary>
	/// The color the crosshair will be set to.
	/// Gets and Sets normally.
	/// </summary>
	public Color CrosshairColor
	{
		get { return crosshairColor; }
		set { crosshairColor = value; }
	}

	void Start()
	{
		//Debug.Log(AssetDatabase.GetAssetPath(crosshairs[2]) + '\n');
		crosshairs = Resources.LoadAll<Sprite>("Crosshairs/");

		//Debug.Log(crosshairs.Length + '\n');

		if (crosshairs == null || crosshairs.Length < 1)
		{
			Debug.LogError("Failure to load Crosshairs.\n");
		}
		else
		{
			RefreshCrosshair();
		}
	}

	public void SetCrosshairSize(Vector2 size)
	{
		Image img = this.GetComponent<Image>();
		img.rectTransform.sizeDelta = new Vector2(size.x, size.y);	
	}

	void Update()
	{
		if (Input.GetKeyDown(KeyCode.Minus))
		{
			Image img = this.GetComponent<Image>();
			img.rectTransform.sizeDelta = new Vector2(img.rectTransform.rect.width / 2, img.rectTransform.rect.height / 2);	
		}
		if (Input.GetKeyDown(KeyCode.Equals))
		{
			Image img = this.GetComponent<Image>();
			img.rectTransform.sizeDelta = new Vector2(img.rectTransform.rect.width * 2, img.rectTransform.rect.height * 2);
		}
	}

	/// <summary>
	/// Checks for valid index. If successful will update this GameObject's Image component to have the correct crosshair sprite.
	/// </summary>
	public void RefreshCrosshair()
	{
		if (crosshairIndex < crosshairs.Length && crosshairIndex >= 0)
		{
			Image img = this.GetComponent<Image>();
			img.sprite = crosshairs[crosshairIndex];
			img.color = crosshairColor;
		}
	}
}
