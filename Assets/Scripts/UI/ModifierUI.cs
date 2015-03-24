using UnityEngine;
using UnityEngine.UI;
using System.Collections;

public class ModifierUI : MonoBehaviour
{
	//public Canvas root;
	public Image rootBackground;
	public Text multiplierText;
	public Text nameText;
	public Image namePlate;
	public Image multiplierPlate;

	public void SetPlateColor(Color newPlateColor, Color newTextColor)
	{
		namePlate.color = newPlateColor;
		multiplierPlate.color = newPlateColor;
		nameText.color = newTextColor;
		multiplierText.color = newTextColor;
	}
}
