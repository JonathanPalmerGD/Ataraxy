using UnityEngine;
using System.Collections;

public class Dimensions : MonoBehaviour
{

	private float radius;
	private float height;

	// Use this for initialization
	void Start ()
	{
		float x = renderer.bounds.extents.x;
		float z = renderer.bounds.extents.z;
		float renderRadius = Mathf.Sqrt (x * x + z * z);
		//Debug.Log ("renderRadius = " + renderRadius);
		
		Mesh mesh = GetComponent<MeshFilter> ().mesh;
		x = mesh.bounds.extents.x * transform.localScale.x;
		z = mesh.bounds.extents.z * transform.localScale.z;
		float meshRadius = Mathf.Sqrt (x * x + z * z);
		//Debug.Log ("meshRadius = " + meshRadius);
		
		height = mesh.bounds.size.y;
		radius = meshRadius;
	}

	public float Radius {
		get { return radius; }
	}
	public float Height {
		get { return height; }
	}
	
}
