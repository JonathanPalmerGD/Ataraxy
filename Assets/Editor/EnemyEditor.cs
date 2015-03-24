using UnityEngine;
using UnityEditor;
using System.Collections;

[CustomEditor(typeof(Enemy), true)]
public class EnemyEditor :  Editor
{	
	[SerializeField]
	bool CustomView = false;
	public override void OnInspectorGUI()
	{
		#region Editor Asset Loading
		if (EditorAssets.ArrowIcon == null)
		{
			EditorAssets.ArrowIcon = Resources.Load("checkboxOff") as Texture2D;
			EditorAssets.ArrowIconDown = Resources.Load("checkboxOn") as Texture2D;
			EditorAssets.ArrowIconRight = Resources.Load("checkboxOff") as Texture2D;
			EditorAssets.MinusIcon = Resources.Load("checkboxOff") as Texture2D;
			EditorAssets.PlusIcon = Resources.Load("checkboxOn") as Texture2D;
			EditorAssets.toggleActiveIcon = Resources.Load("checkboxOn") as Texture2D;
			EditorAssets.toggleInactiveIcon = Resources.Load("checkboxOff") as Texture2D;
		}
		#endregion

		Enemy enemy = (Enemy)target;

		EditorGUILayout.Space();
		string titleText = CustomView ? "View Default Inspector" : "View Enemy Inspector";

		CustomView = AtSt.DrawTitleFoldout(CustomView, titleText);
		//CustomView = EditorGUILayout.Foldout(CustomView, "Enemy Custom Inspector");
		if (CustomView)
		{
			DrawCustomView(enemy);
			EditorGUILayout.Space();
			EditorGUILayout.Space();
		}
		else
		{
			EditorGUILayout.Space();
			EditorGUILayout.Space();
			base.OnInspectorGUI();
		}
	}

	public void DrawCustomView(Enemy enemy)
	{

		enemy.state = (Enemy.EnemyState)AtSt.DrawEnumPopup("Enemy State", enemy.state);
		//AtSt.DrawLabel("State Timer:\t" + enemy.stateTimer);
		enemy.stateTimer = EditorGUILayout.FloatField("State Timer", enemy.stateTimer);
		enemy.XpReward = EditorGUILayout.FloatField("XP Reward", enemy.XpReward);
		enemy.CanSeePlayer = AtSt.DrawToggle(enemy.CanSeePlayer, "Can See Player");
		//enemy.weapon = (Weapon)EditorGUILayout.ObjectField("Weapon", enemy.weapon, typeof(Weapon));
	}
}
