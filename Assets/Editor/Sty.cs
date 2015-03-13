using UnityEngine;
using UnityEditor;
using System.Collections;

public class Sty : MonoBehaviour {

	public static Color LightGreen = new Color( .2f, .8f, .2f);
	public static Color VibrantGreen = new Color( .1f, .9f, .1f);
	public static Color DimGreen = new Color(.65f, .7f, .65f);
	public static Color OffWhite = new Color( .2f, .8f, .2f);
	public static Color BakeColor = new Color( .70f, .50f, .30f);
	public static Color BlueLabel = new Color( .7f, .7f, .8f);

	//We might also want tooltips for the different tabs?
	public static int DrawTabs(int tabSelected, string[] tabNames, string[] tabTooltips = null, Texture2D[] tabIcons = null, int tabsPerRow = 0, EditorWindow edWindow = null){

		bool enableToolTips = false;
		if(tabTooltips != null){
			enableToolTips = true;
		}

		if(tabIcons == null){
			tabIcons = new Texture2D[tabNames.Length];
			for(int i = 0; i < tabNames.Length; i++){
				tabIcons[i] = new Texture2D(1,1);
			}
		}

		int returnToSender = tabSelected;

		EditorGUILayout.BeginHorizontal();
		GUIStyle buttonStyle = GetUnselectedTab(edWindow, tabNames.Length);
		for(int i = 0; i < tabNames.Length; i++){

			if(tabsPerRow != 0){
				if(tabNames.Length > tabsPerRow && (i % tabsPerRow) == 0){
					EditorGUILayout.EndHorizontal();
					EditorGUILayout.BeginHorizontal();
				}
			}
			Rect buttonRect = EditorGUILayout.BeginHorizontal();

			int tabsInCurRow = 1;
			tabsInCurRow = tabsPerRow == 0 ? tabNames.Length : tabsPerRow;
			if(i > tabNames.Length - tabsPerRow){
				tabsInCurRow = tabNames.Length % tabsPerRow;
			}
			if(i == tabSelected){
				buttonStyle = GetSelectedTab(edWindow, tabsInCurRow);
			}
			else{
				buttonStyle = GetUnselectedTab(edWindow, tabsInCurRow);
			}

			string toolTipText = "";
			if(enableToolTips && tabTooltips.Length > i){
				toolTipText = tabTooltips[i];
			}

			if(GUILayout.Button( new GUIContent(tabNames[i], toolTipText), buttonStyle)){
				returnToSender = i;
			}

			if(tabIcons.Length > i){
				GUI.Label(new Rect(buttonRect.x + (buttonStyle.fixedHeight - tabIcons[i].height)/2, buttonRect.y + (buttonStyle.fixedHeight - tabIcons[i].height)/2, tabIcons[i].width, tabIcons[i].height), (Texture)tabIcons[i]);
			}

			EditorGUILayout.EndHorizontal();
		}

		EditorGUILayout.EndHorizontal();

		return returnToSender;
	}

	public static int DrawIconTabs(int tabSelected, Texture2D[] tabIcons, string[] tabTooltips = null, int tabsPerRow = 0, EditorWindow edWindow = null){
		bool enableToolTips = false;
		if(tabTooltips != null){
			enableToolTips = true;
		}
		
		int returnToSender = tabSelected;
		
		EditorGUILayout.BeginHorizontal();
		GUIStyle buttonStyle = GetUnselectedTab(edWindow, tabIcons.Length);
		for(int i = 0; i < tabIcons.Length; i++){
			if(tabIcons[i] != null){
				if(tabsPerRow != 0){
					if((i % tabsPerRow) <= 0){
						EditorGUILayout.EndHorizontal();
						EditorGUILayout.BeginHorizontal();
					}
				}

				Rect buttonRect = EditorGUILayout.BeginHorizontal();

				int tabsInCurRow = 1;
				tabsInCurRow = tabsPerRow == 0 ? tabIcons.Length : tabsPerRow;
				if(i > tabIcons.Length - tabsPerRow){
					tabsInCurRow = tabIcons.Length % tabsPerRow;
				}
				if(i == tabSelected){
					buttonStyle = GetSelectedTab(edWindow, tabsInCurRow);
				}
				else{
					buttonStyle = GetUnselectedTab(edWindow, tabsInCurRow);
				}

				string toolTipText = "";
				if(enableToolTips && tabTooltips.Length > i){
					toolTipText = tabTooltips[i];
				}
				
				if(GUILayout.Button( new GUIContent("", toolTipText), buttonStyle)){
					returnToSender = i;
				}

				if(tabIcons.Length > i){
					GUI.Label(new Rect(buttonRect.x + (buttonStyle.fixedWidth - tabIcons[i].height)/2, buttonRect.y + (buttonStyle.fixedHeight - tabIcons[i].height)/2, tabIcons[i].width, tabIcons[i].height), (Texture)tabIcons[i]);
				}

				EditorGUILayout.EndHorizontal();
			}
		}
		
		EditorGUILayout.EndHorizontal();

		return returnToSender;
	}

	public static bool DrawIconButton(string buttonLabel, Texture2D buttonIcon, EditorWindow edWindow = null, EditorStyles buttonStyle = null){
		bool clicked = false;
		Rect buttonRect = EditorGUILayout.BeginVertical("box");
		
		if (GUI.Button(buttonRect, new GUIContent("", "Tooltip"), GetFoldoutButton())){
			//toggleDropDown = (toggleDropDown ? false : true);
		}
		
		GUILayout.Space(5f);
		EditorGUILayout.BeginHorizontal();
		if(clicked){
			GUILayout.Label("[     ]", GetLargeLabelIcon());
			//GUILayout.Label("[  V  ]", largeLabelIcon);
		}
		else
		{
			GUILayout.Label("  []  ", GetLargeLabelIcon());
		}
		GUILayout.Label(buttonLabel, GetLargeLabel());
		EditorGUILayout.EndHorizontal();
		GUILayout.Space(8f);
		EditorGUILayout.EndVertical();
		return clicked;
	}

	public static GUIStyle GetSelectedTab(EditorWindow edWindow = null, int howManyTabs = 1){
		GUIStyle selectedTab = new GUIStyle(EditorStyles.toolbarButton);
		selectedTab.fontStyle = FontStyle.Bold;
		selectedTab.fixedHeight = 36f;
		selectedTab.fontSize = 18;
		selectedTab.normal = selectedTab.active;
		selectedTab.normal.textColor = LightGreen;
		selectedTab.active.textColor = VibrantGreen;
		selectedTab.onActive = new GUIStyleState();

		if(edWindow != null){
			selectedTab.fixedWidth = edWindow.position.width / howManyTabs;
		}

		return selectedTab;
	}
	
	public static GUIStyle GetUnselectedTab(EditorWindow edWindow = null, int howManyTabs = 1){
		GUIStyle unselectedTab = new GUIStyle(EditorStyles.toolbarButton);
		unselectedTab.fontStyle = FontStyle.Bold;
		unselectedTab.fontSize = 18;
		unselectedTab.fixedHeight = 36f;
		unselectedTab.normal.textColor = DimGreen;

		if(edWindow != null){
			unselectedTab.fixedWidth = edWindow.position.width / howManyTabs;
		}

		return unselectedTab;
	}

	public static GUIStyle GetMainTitle(){
		GUIStyle mainTitle = new GUIStyle(EditorStyles.largeLabel);
		
		mainTitle.fontSize = 20;
		mainTitle.alignment = TextAnchor.MiddleLeft;
		mainTitle.clipping = TextClipping.Overflow;
		
		return mainTitle;
	}

	public static GUIStyle GetDarkwindTitle(){
		GUIStyle title = new GUIStyle(EditorStyles.largeLabel);
		
		title.fontSize = 16;
		title.alignment = TextAnchor.UpperRight;
		title.clipping = TextClipping.Overflow;
		
		return title;
	}
	
	public static GUIStyle GetFoldout(){
		GUIStyle foldOutButton = new GUIStyle(EditorStyles.foldout);
		
		foldOutButton.fontStyle = FontStyle.Bold;
		return foldOutButton;
	}
	
	public static GUIStyle GetLargeLabel(){
		GUIStyle largeLabel = new GUIStyle(EditorStyles.largeLabel);
		largeLabel.fontStyle = FontStyle.Bold;
		return largeLabel;
	}

	public static GUIStyle GetBlueLabel(){
		GUIStyle blueLabel = new GUIStyle(EditorStyles.label);
		if(EditorGUIUtility.isProSkin)
			blueLabel.normal.textColor = BlueLabel;
		return blueLabel;
	}
	
	public static GUIStyle GetSmallLabel(){
		GUIStyle label = new GUIStyle(EditorStyles.label);
		label.alignment = TextAnchor.LowerLeft;
		return label;
	}

	public static GUIStyle GetLargeLabelIcon(){
		GUIStyle largeLabelIcon = new GUIStyle(EditorStyles.largeLabel);
		largeLabelIcon.fixedWidth = 48;
		largeLabelIcon.padding = new RectOffset(12, 0, 0, 0);
		largeLabelIcon.fontStyle = FontStyle.Bold;
		if(EditorGUIUtility.isProSkin)
			largeLabelIcon.normal.textColor = LightGreen;
		return largeLabelIcon;
	}
	
	public static GUIStyle GetSmallLabelIcon(){
		GUIStyle largeLabelIcon = new GUIStyle(EditorStyles.largeLabel);
		largeLabelIcon.fixedWidth = 36;
		largeLabelIcon.padding = new RectOffset(6, 0, -2, 0);
		largeLabelIcon.fontStyle = FontStyle.Bold;
		if(EditorGUIUtility.isProSkin)
			largeLabelIcon.normal.textColor = Color.white;
		return largeLabelIcon;
	}

	
	public static GUIStyle GetLoudLabel(){
		GUIStyle largeLabelIcon = new GUIStyle(EditorStyles.largeLabel);
		largeLabelIcon.fontStyle = FontStyle.Bold;
		largeLabelIcon.fontSize = 24;
		largeLabelIcon.alignment = TextAnchor.MiddleCenter;
		if(EditorGUIUtility.isProSkin)
			largeLabelIcon.normal.textColor = BakeColor;
		return largeLabelIcon;
	}
	
	public static GUIStyle GetFoldoutButton(){
		GUIStyle foldoutButton = new GUIStyle(EditorStyles.toolbarButton);
		foldoutButton.fixedHeight = 36f;
		return foldoutButton;
	}

	public static GUIStyle GetSmallFoldoutButton(){
		GUIStyle smallFoldoutButton = new GUIStyle(EditorStyles.toolbarButton);
		smallFoldoutButton.fixedHeight = 24f;
		return smallFoldoutButton;
	}
	
	public static GUIStyle GetToggleButton(){
		GUIStyle toggleButton = new GUIStyle(EditorStyles.toolbarButton);
		toggleButton.fixedHeight = 24f;
		return toggleButton;
	}
	
	public static GUIStyle GetLargeButton(){
		GUIStyle largeButton = new GUIStyle(EditorStyles.toolbarButton);
		largeButton.fixedHeight = 42f;
		return largeButton;
	}

	public static GUIStyle GetToggleButtonAndTextField(){
		GUIStyle compoundButton = new GUIStyle(EditorStyles.toolbarButton);
		compoundButton.fixedHeight = 24f;
		return compoundButton;
	}

	public static GUIStyle GetUniformButton(Rect edWindow){
		GUIStyle uniformButton = new GUIStyle(EditorStyles.toolbarButton);
		uniformButton.fontSize = 12;
		uniformButton.fixedHeight = 24;
		//uniformButton.fixedWidth = (edWindow.width / 2) - EditorGUI.indentLevel * 2;
		// = TextAnchor.MiddleCenter;
		return uniformButton;
	}

	public static GUIStyle ThinButtonStyle(){
		GUIStyle thinButton = new GUIStyle(EditorStyles.toolbarButton);
		thinButton.fontStyle = FontStyle.Bold;
		thinButton.fixedHeight = 18f;
		return thinButton;
	}

	public static GUIStyle GetEnumStyleButton(){
		GUIStyle enumStyleButton = new GUIStyle(EditorStyles.toolbarDropDown);
		enumStyleButton.onActive.background = ThinButtonStyle().onActive.background;
		enumStyleButton.fixedHeight = 18f;
		return enumStyleButton;
	}

	public static GUIStyle GetObjectField(){
		GUIStyle objectField = new GUIStyle(EditorStyles.objectField);
		objectField.padding = new RectOffset(0,0,0,0);
		objectField.border = new RectOffset(0,0,0,0);
		return objectField;
	}


	/// <summary>
	/// Draws a dropdown menu. Index is for different icons?
	/// </summary>
	/// <returns><c>true</c>, returns true when opened, <c>false</c> returns true when closed.</returns>
	/// <param name="toggleDropDown">The boolean we care about</param>
	/// <param name="name">Display name.</param>
	/// <param name="index">Icon index. Has no effect currently.</param>
	public static bool DrawDropDown(string displayText, bool toggleDropDown, string toolTip = null){
		
		Rect buttonRect = EditorGUILayout.BeginVertical("box");

		if (GUI.Button(buttonRect, new GUIContent("", toolTip), GetFoldoutButton())){
			toggleDropDown = (toggleDropDown ? false : true);
		}
		
		GUILayout.Space(5f);
		EditorGUILayout.BeginHorizontal();
		if(toggleDropDown){
			GUILayout.Label("[     ]", GetLargeLabelIcon());
			//GUILayout.Label("[  V  ]", largeLabelIcon);
		}
		else
		{
			GUILayout.Label("  []  ", GetLargeLabelIcon());
		}
		GUILayout.Label(displayText, GetLargeLabel());
		EditorGUILayout.EndHorizontal();
		GUILayout.Space(8f);
		EditorGUILayout.EndVertical();
		return toggleDropDown;
	}
	
	public static bool DrawSmallDropDown(string displayText, bool toggleDropDown, string toolTip = null){
		Rect buttonRect = EditorGUILayout.BeginVertical("box");

		if (GUI.Button(buttonRect, new GUIContent("", toolTip), GetSmallFoldoutButton())){
			toggleDropDown = (toggleDropDown ? false : true);
		}
		
		EditorGUILayout.BeginHorizontal();
		if(toggleDropDown){
			GUILayout.Label("[     ]", GetSmallLabelIcon());
			//GUILayout.Label("[ V ]", labelIcon);
		}
		else
		{
			GUILayout.Label("  []  ", GetSmallLabelIcon());
		}
		GUILayout.Label(displayText, GetSmallLabel());
		EditorGUILayout.EndHorizontal();
		GUILayout.Space(0f);
		EditorGUILayout.EndVertical();
		return toggleDropDown;
	}

	public static bool DrawToggleBar(string displayText, bool toggleDropDown, string toolTip = null){

		Rect buttonRect = EditorGUILayout.BeginVertical("box");

		if (GUI.Button(buttonRect, new GUIContent("", toolTip), GetSmallFoldoutButton())){
			toggleDropDown = (toggleDropDown ? false : true);
		}
		
		EditorGUILayout.BeginHorizontal();
		if(toggleDropDown){

			GUILayout.Label("[ X ]", GetSmallLabelIcon());
		}
		else
		{
			GUI.enabled = false;
			GUILayout.Label("[    ]", GetSmallLabelIcon());
			GUI.enabled = true;
		}
		GUILayout.Label(displayText, GetSmallLabel());
		EditorGUILayout.EndHorizontal();
		EditorGUILayout.EndVertical();

		return toggleDropDown;
	}

	public static bool DrawToggleBarAndTextField(string displayText, bool toggleDropDown, string textField, string toolTip = null){
		
		Rect buttonRect = EditorGUILayout.BeginVertical("box");

		if (GUI.Button(buttonRect, new GUIContent("", toolTip), GetToggleButtonAndTextField())){
			toggleDropDown = (toggleDropDown ? false : true);
		}
		
		EditorGUILayout.BeginHorizontal();
		if(toggleDropDown){
			EditorGUILayout.TextField("[ X ]  " + displayText, textField, GetBlueLabel());
		}
		else
		{
			GUI.enabled = false;
			EditorGUILayout.TextField("[    ]  " + displayText, textField, EditorStyles.label);
			GUI.enabled = true;
		}
		EditorGUILayout.EndHorizontal();
		EditorGUILayout.EndVertical();

		return toggleDropDown;
	}

	public static bool DrawLargeButton(string displayText, string toolTip = null){
		bool returnToSender = false;
		Rect buttonRect = EditorGUILayout.BeginVertical("box");

		if (GUI.Button(buttonRect, new GUIContent("", toolTip), GetLargeButton())){
			returnToSender = true;
		}

		GUILayout.Label(displayText, GetLoudLabel());

		EditorGUILayout.EndVertical();
		
		return returnToSender;
	}

	public static void DrawGuiDivider(){
		
		GUILayout.Space(1f);
		
		if (Event.current.type == EventType.Repaint)
		{
			Texture2D tex = EditorGUIUtility.whiteTexture;
			Rect rect = GUILayoutUtility.GetLastRect();
			GUI.color = new Color(0f, 0f, 0f, 0.25f);
			GUI.DrawTexture(new Rect(0f, rect.yMin + 6f, Screen.width, 4f), tex);
			GUI.DrawTexture(new Rect(0f, rect.yMin + 6f, Screen.width, 1f), tex);
			GUI.DrawTexture(new Rect(0f, rect.yMin + 9f, Screen.width, 1f), tex);
			GUI.color = Color.white;
		}
	}
}
