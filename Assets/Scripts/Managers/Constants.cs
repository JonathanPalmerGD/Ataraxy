using UnityEngine;
using System.Collections;

public static class Constants : object
{
	public static Color healthUsed = new Color(.75f, .75f, .75f, 1f);
	public static Color healthRemaining = new Color(.0f, .75f, .15f, 1.0f);

	public static float targetFade = 3.0f;

	public static float CheckXZDistance(Vector3 firstPos, Vector3 secondPos)
	{
		Vector2 posFlat = new Vector2(firstPos.x, firstPos.z);
		Vector2 nodePosFlat = new Vector2(secondPos.x, secondPos.z);

		//Find distance to the position.
		return Vector2.Distance(posFlat, nodePosFlat);
	}

}
