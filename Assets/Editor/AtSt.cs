using UnityEngine;
using UnityEditor;
using System.Collections;

public class AtSt : EditorWindow {

	public static Color LightGreen = new Color( .2f, .8f, .2f);
	public static Color VibrantGreen = new Color( .1f, .9f, .1f);
	public static Color DimGreen = new Color(.65f, .7f, .65f);
	public static Color OffWhite = new Color( .2f, .8f, .2f);
	public static Color BakeColor = new Color( .70f, .50f, .30f);
	public static Color BlueLabel = new Color(.7f, .7f, .8f);

	public static Color activeColor = new Color(.0f, .0f, .8f);
	public static Color inactiveColor = new Color(.1f, .1f, .1f);
	public static Color otherColor = new Color(.2f, .0f, .0f);


	//public static Color activeColor = new Color(.3f, .8f, .4f);
	//public static Color inactiveColor = new Color(.7f, .7f, .7f);
	//public static Color otherColor = new Color(.4f, .4f, .9f);

		/*//Active focused and hover don't appear to work at all. Probably needs to force repaints.
			toggleLabel.normal.textColor = activeColor;
			toggleLabel.active.textColor = otherColor;
			toggleLabel.focused.textColor = otherColor;
			toggleLabel.hover.textColor = otherColor;
		}
		else
		{
			toggleLabel.normal.textColor = inactiveColor;
			toggleLabel.active.textColor = otherColor;
			toggleLabel.focused.textColor = otherColor;
			toggleLabel.hover.textColor = otherColor;*/

	public static bool DrawTitleFoldout(bool toggleDropDown, string displayText = "", string toolTip = "")
	{

		Rect buttonRect = EditorGUILayout.BeginVertical();
		buttonRect = new Rect(buttonRect.x, buttonRect.y, buttonRect.width, buttonRect.height + 12);
		if (GUI.Button(buttonRect, new GUIContent("", toolTip), GetFoldoutButton())){
			toggleDropDown = (toggleDropDown ? false : true);
		}
		GUILayout.Space(5f);
		EditorGUILayout.BeginHorizontal();
		GUILayout.Space(-5f);
		if(toggleDropDown){
			GUILayout.Label(((Texture)EditorAssets.ArrowIconDown), GetLargeLabelIcon());
		}
		else
		{
			GUILayout.Label(((Texture)EditorAssets.ArrowIconRight), GetLargeLabelIcon());
		}
		GUILayout.Label(displayText, GetLargeLabel());
		EditorGUILayout.EndHorizontal();
		EditorGUILayout.EndVertical();
		GUILayout.Space(6f);

		return toggleDropDown;
	}

	public static bool DrawFoldout(bool toggleDropDown, string displayText = "", string toolTip = "")
	{

		Rect buttonRect = EditorGUILayout.BeginVertical();
		Rect extraRect = new Rect(buttonRect.x, buttonRect.y, buttonRect.width, buttonRect.height + 8);
		if (GUI.Button(extraRect, new GUIContent("", toolTip), GetSmallFoldoutButton())){
			toggleDropDown = (toggleDropDown ? false : true);
		}

		EditorGUILayout.BeginHorizontal();
		if(toggleDropDown){
			GUILayout.Label(((Texture)EditorAssets.ArrowIconDown), GetSmallLabelIcon());
		}
		else
		{
			GUILayout.Label(((Texture)EditorAssets.ArrowIconRight), GetSmallLabelIcon());
		}
		GUILayout.Space(-14);
		GUILayout.Label(displayText, GetSmallLabel());
		EditorGUILayout.EndHorizontal();
		EditorGUILayout.EndVertical();
		if(toggleDropDown){
			GUILayout.Space(10f);
		}
		return toggleDropDown;
	}

	#region Buttons
	public static bool DrawMinusButton(int size = -1)
	{
		if(size > 0)
		{
			GUIStyle tallerToolbar = new GUIStyle(EditorStyles.toolbarButton);
			tallerToolbar.fixedHeight = size;
			tallerToolbar.fixedWidth = size;
			return GUILayout.Button(EditorAssets.MinusIcon, tallerToolbar, GUILayout.ExpandWidth(false));
		}
		return GUILayout.Button(EditorAssets.MinusIcon, EditorStyles.toolbarButton, GUILayout.ExpandWidth(false));
	}
	
	public static bool DrawArrowButton()
	{
		return GUILayout.Button(EditorAssets.ArrowIcon, EditorStyles.toolbarButton, GUILayout.ExpandWidth(false));
	}
	
	public static bool DrawPlusButton()
	{
		return GUILayout.Button(EditorAssets.PlusIcon, EditorStyles.toolbarButton, GUILayout.ExpandWidth(false));
	}
	
	public static void DrawLabel(string labelText, float minWidth = 105)
	{
		GUILayout.Label(labelText, AtSt.GetSubTitleLabel(), GUILayout.ExpandWidth(false), GUILayout.MinWidth(105));
	}

	/// <summary>
	/// Label Wrapper - takes a Rect to offset padding of the label by. Left/X, Top/Y, Right/Width, Bottom/Height format for the Rect
	/// </summary>
	/// <param name="labelText">Label text.</param>
	/// <param name="offset">Offset.</param>
	/// <param name="minWidth">Minimum width.</param>
	public static void DrawLabel(string labelText, Rect offset, float minWidth = 105)
	{
		GUILayout.Label(labelText, AtSt.GetSubTitleLabel(offset), GUILayout.ExpandWidth(false), GUILayout.MinWidth(105));
	}

	public static bool DrawButton(Texture image, params GUILayoutOption[] options)
	{

		GUIStyle buttonStyle = EditorStyles.toolbarButton;
		return GUILayout.Button(image, buttonStyle, GUILayout.ExpandWidth(false));
	}
	
	public static bool DrawButton(string content, params GUILayoutOption[] options)
	{
		GUIStyle buttonStyle = EditorStyles.toolbarButton;
		return GUILayout.Button(content, buttonStyle, GUILayout.ExpandWidth(false));
	}
	
	public static bool DrawButton(GUIContent content, params GUILayoutOption[] options)
	{

		GUIStyle buttonStyle = EditorStyles.toolbarButton;
		return GUILayout.Button(content, buttonStyle, GUILayout.ExpandWidth(false));
	}
	#endregion
	
	#region Toggle
	public static bool DrawToggle(bool value, params GUILayoutOption[] options)
	{
		Rect buttonRect = EditorGUILayout.BeginVertical();

		GUILayout.Space(6f);

		if (GUI.Button(buttonRect, new GUIContent("", ""), GetToggleLabelButton(value))){
			value = (value ? false : true);
		}
		GUIStyle label = new GUIStyle(GetToggleLabel(value));
		if(value){
			GUILayout.Label(((Texture)EditorAssets.toggleActiveIcon), label, GUILayout.Width(16));
		}
		else
		{
			GUILayout.Label(((Texture)EditorAssets.toggleInactiveIcon), label, GUILayout.Width(16));
		}
		EditorGUILayout.EndVertical();

		return value;
	}
	
	public static bool DrawToggle(bool value, string text, params GUILayoutOption[] options)
	{
		return DrawToggle(value, new GUIContent(text), options);
	}

	public static void DrawToggleIcon(bool value, GUIStyle label)
	{
		if(value){
			GUILayout.Label(((Texture)EditorAssets.toggleActiveIcon), label, GUILayout.Width(16));
		}
		else
		{
			GUILayout.Label(((Texture)EditorAssets.toggleInactiveIcon), label, GUILayout.Width(16));
		}
		GUILayout.Space(-4);
	}

	public static bool DrawToggle(bool value, Texture image, params GUILayoutOption[] options)
	{
		Rect buttonRect = EditorGUILayout.BeginVertical();
		
		if (GUI.Button(buttonRect, new GUIContent("", ""), GetToggleLabelButton(value))){
			value = (value ? false : true);
		}
		GUIStyle label = new GUIStyle(GetToggleLabel(value));
		
		EditorGUILayout.BeginHorizontal();
		DrawToggleIcon(value, label);
		GUILayout.Label(image, label);
		EditorGUILayout.EndHorizontal();
		EditorGUILayout.EndVertical();
		
		return value;
	}
	
	public static bool DrawToggle(bool value, GUIContent content, params GUILayoutOption[] options)
	{		
		Rect buttonRect = EditorGUILayout.BeginVertical();
		
		if (GUI.Button(buttonRect, new GUIContent("", ""), GetToggleLabelButton(value))){
			value = (value ? false : true);
		}
		GUIStyle label = new GUIStyle(GetToggleLabel(value));
		
		EditorGUILayout.BeginHorizontal();
		DrawToggleIcon(value, label);
		GUILayout.Label(content, label);
		EditorGUILayout.EndHorizontal();
		EditorGUILayout.EndVertical();
		
		return value;
	}
	#endregion
	
	#region DrawPopup
	public static int DrawPopup(int selectedIndex, string[] displayedOptions, params GUILayoutOption[] options)
	{
		return EditorGUILayout.Popup(selectedIndex, displayedOptions, GetPopup(), options);
	}
	
	public static int DrawPopup(int selectedIndex, string[] displayedOptions, GUIStyle style, params GUILayoutOption[] options)
	{
		return EditorGUILayout.Popup(selectedIndex, displayedOptions, style, options);
	}
	
	public static int DrawPopup(int selectedIndex, GUIContent[] displayedOptions, params GUILayoutOption[] options)
	{
		return EditorGUILayout.Popup(selectedIndex, displayedOptions, GetPopup(), options);
	}
	
	public static int DrawPopup(int selectedIndex, GUIContent[] displayedOptions, GUIStyle style, params GUILayoutOption[] options)
	{
		return EditorGUILayout.Popup(selectedIndex, displayedOptions, style, options);
	}
	
	public static int DrawPopup(string label, int selectedIndex, string[] displayedOptions, params GUILayoutOption[] options)
	{
		return EditorGUILayout.Popup(label, selectedIndex, displayedOptions, GetPopup(), options);
	}
	
	public static int DrawPopup(string label, int selectedIndex, string[] displayedOptions, GUIStyle style, params GUILayoutOption[] options)
	{
		return EditorGUILayout.Popup(label, selectedIndex, displayedOptions, style, options);
	}
	
	public static int DrawPopup(GUIContent label, int selectedIndex, GUIContent[] displayedOptions, params GUILayoutOption[] options)
	{
		return EditorGUILayout.Popup(label, selectedIndex, displayedOptions, GetPopup(), options);
	}
	
	public static int DrawPopup(GUIContent label, int selectedIndex, GUIContent[] displayedOptions, GUIStyle style, params GUILayoutOption[] options)
	{
		return EditorGUILayout.Popup(label, selectedIndex, displayedOptions, style, options);
	}
	#endregion

	#region EnumPopup
	public static System.Enum DrawEnumPopup(System.Enum selectedIndex, params GUILayoutOption[] options)
	{
		System.Enum enumValue = EditorGUILayout.EnumPopup(selectedIndex, GetEnumPopup(), options);
		return enumValue;
	}


	public static System.Enum DrawEnumPopup(string text, System.Enum selectedIndex, params GUILayoutOption[] options)
	{
		System.Enum enumValue = EditorGUILayout.EnumPopup(text, selectedIndex, GetEnumPopup(), options);
		return enumValue;
	}
	#endregion
	
	#region GUIStyles
	public static GUIStyle GetTitleLabel(){
		GUIStyle titleLabel = new GUIStyle(EditorStyles.largeLabel);
		titleLabel.fontSize = 18;
		return titleLabel;
	}
	
	public static GUIStyle GetToggleStyle(bool value)
	{
		GUIStyle toggleStyle = new GUIStyle(EditorStyles.toggle);

		toggleStyle.normal.textColor = inactiveColor;
		toggleStyle.active.textColor = activeColor;
		toggleStyle.focused.textColor = activeColor;
		toggleStyle.focused.textColor = activeColor;

		return toggleStyle;
	}

	//A bit obsolete
	public static GUIStyle GetSubTitleLabel(){

		GUIStyle subtitleLabel = new GUIStyle(EditorStyles.boldLabel);

		subtitleLabel.padding = new RectOffset(subtitleLabel.padding.left, subtitleLabel.padding.right, subtitleLabel.padding.top - 4, subtitleLabel.padding.bottom);
		return subtitleLabel;
	}

	public static GUIStyle GetSubTitleLabel(Rect offset)
	{
		GUIStyle subtitleLabel = new GUIStyle(EditorStyles.boldLabel);
		subtitleLabel.padding = new RectOffset(subtitleLabel.padding.left + (int)offset.x, subtitleLabel.padding.right + (int)offset.width, subtitleLabel.padding.top + (int)offset.y, subtitleLabel.padding.bottom + (int)offset.height);
		//subtitleLabel = new GUIStyle(EditorStyles.boldLabel);
		return subtitleLabel;
	}
	
	public static GUIStyle GetLargeLabel(){
		GUIStyle largeLabel = new GUIStyle(EditorStyles.largeLabel);
		largeLabel.fontStyle = FontStyle.Bold;
		return largeLabel;
	}
	
	public static GUIStyle GetPopup()
	{
		GUIStyle popupStyle = new GUIStyle(EditorStyles.toolbarPopup);
		popupStyle.margin = new RectOffset(popupStyle.margin.left, popupStyle.margin.right, popupStyle.margin.top + 2, popupStyle.margin.bottom);
		return popupStyle;
	}
	
	public static GUIStyle GetEnumPopup()
	{

		GUIStyle enumPopupStyle = new GUIStyle(EditorStyles.toolbarPopup);
		enumPopupStyle.fixedHeight = 15;
		enumPopupStyle.margin = new RectOffset(enumPopupStyle.margin.left, enumPopupStyle.margin.right, enumPopupStyle.margin.top + 2, enumPopupStyle.margin.bottom);
		return enumPopupStyle;
	}

	public static GUIStyle GetSmallFoldoutButton()
	{
		GUIStyle smallFoldoutButton = new GUIStyle(EditorStyles.toolbarButton);
		smallFoldoutButton.fixedHeight = 20f;
		return smallFoldoutButton;
	}

	public static GUIStyle GetFoldoutButton()
	{
		GUIStyle foldoutButton = new GUIStyle(EditorStyles.toolbarButton);
		foldoutButton.fixedHeight = 30f;
		//foldoutButton.fixedWidth = 300;
		return foldoutButton;
	}

	public static GUIStyle GetToggleLabelButton(bool toggled)
	{
		return EditorStyles.label;
		//}

		//If we wanted to have custom backgrounds or something for the toggle labels.
		/*
		GUIStyle toggleLabel = new GUIStyle(EditorStyles.label);

		toggleNormalBackground = (Texture2D)AssetDatabase.LoadAssetAtPath("Assets/Editor/UnityBuild/pale-border-icon.png", typeof(Texture2D));
		toggleActiveBackground = (Texture2D)AssetDatabase.LoadAssetAtPath("Assets/Editor/UnityBuild/minus-icon.png", typeof(Texture2D));
		toggleLabel.normal.background = toggleNormalBackground;
		toggleLabel.focused.background = new Texture2D(2,2);
		toggleLabel.active.background = toggleActiveBackground;

		return toggleLabel;*/
	}

	public static GUIStyle GetToggleLabel(bool toggled){

		GUIStyle toggleLabel = new GUIStyle(EditorStyles.label);
		if(toggled)
		{
			//Active focused and hover don't appear to work at all. Probably needs to force repaints.
			toggleLabel.normal.textColor = activeColor;
			toggleLabel.active.textColor = otherColor;
			toggleLabel.focused.textColor = otherColor;
			toggleLabel.hover.textColor = otherColor;
		}
		else
		{
			toggleLabel.normal.textColor = inactiveColor;
			toggleLabel.active.textColor = otherColor;
			toggleLabel.focused.textColor = otherColor;
			toggleLabel.hover.textColor = otherColor;
		}
		return toggleLabel;
	}
	
	public static GUIStyle GetLargeLabelIcon(){
		GUIStyle largeLabelIcon = new GUIStyle(EditorStyles.largeLabel);
		largeLabelIcon.padding = new RectOffset(12, 0, 0, 0);
		largeLabelIcon.fontStyle = FontStyle.Bold;
		//if(EditorGUIUtility.isProSkin)
		//	largeLabelIcon.normal.textColor = LightGreen;
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

	public static GUIStyle GetSmallLabel(){
		GUIStyle label = new GUIStyle(EditorStyles.label);
		label.alignment = TextAnchor.LowerLeft;
		return label;
	}
	#endregion
}