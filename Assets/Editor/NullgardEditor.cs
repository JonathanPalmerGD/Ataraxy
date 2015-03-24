using UnityEngine;
using UnityEditor;
using System.Collections;

//[CustomEditor(typeof(Nullgard), true)]
public class NullgardEditor : Editor
{
	bool CustomView = false;
	public override void OnInspectorGUI()
	{
		//Nullgard nullgard = (Nullgard)target;

		CustomView = EditorGUILayout.Foldout(CustomView, "Nullgard Custom Inspector");
		if(CustomView)
		{


		}
		else
		{
			EditorGUILayout.Space();
			EditorGUILayout.Space();
			base.OnInspectorGUI();
		}
	}
}
