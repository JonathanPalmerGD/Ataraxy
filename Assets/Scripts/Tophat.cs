using UnityEngine;
using System.Collections;

public class Tophat : NPC 
{
	public override void Start()
	{
		base.Start();
		name = "Top Hat";
		Level = 3;
	}
}
