/// Credit to Bogdan Gochev for licensing this shader code to us. 
/// Usage of the Flat Lighting Shader from this SDK is licensed for Mira applications only.
/// Flat lighting asset: https://www.assetstore.unity3d.com/en/#!/content/67730

using UnityEngine;
using UnityEditor;
using System;

namespace FlatLighting {
	public static class UITools {

		static private GUIStyle _HeaderLabel;
		static public GUIStyle HeaderLabel {
			get	{
				if(_HeaderLabel == null) {
					_HeaderLabel = new GUIStyle(EditorStyles.boldLabel);
					_HeaderLabel.stretchWidth = false;
				}

				return _HeaderLabel;
			}
		}

		static private GUIStyle _GroupHeaderLabel;
		static public GUIStyle GroupHeaderLabel {
			get	{
				if(_GroupHeaderLabel == null) {
					_GroupHeaderLabel = new GUIStyle ("ShurikenModuleTitle") {
						font = (new GUIStyle ("Label")).font,
						border = new RectOffset (15, 7, 4, 4),
						fixedHeight = 22,
						contentOffset = new Vector2 (20f, -2f),
						richText = true
					};
				}

				return _GroupHeaderLabel;
			}
		}

		static private GUIStyle _RichTextLabel;
		static public GUIStyle RichTextLabel {
			get	{
				if(_RichTextLabel == null) {
					_RichTextLabel = new GUIStyle(EditorStyles.label);
					_HeaderLabel.stretchWidth = true;
					_RichTextLabel.richText = true;
					_RichTextLabel.alignment = TextAnchor.MiddleLeft;
				}

				return _RichTextLabel;
			}
		}

		static private GUIStyle _LeftToggleButton;
		static public GUIStyle LeftToggleButton {
			get	{
				if(_LeftToggleButton == null) {
					_LeftToggleButton = new GUIStyle(EditorStyles.miniButtonLeft);
					int left = 0;
					int right = left;
					int top = 2;
					int bottom = 10;
					_LeftToggleButton.margin = new RectOffset(left, right, top, bottom);
				}

				return _LeftToggleButton;
			}
		}

		static private GUIStyle _MiddleToggleButton;
		static public GUIStyle MiddleToggleButton {
			get	{
				if(_MiddleToggleButton == null) {
					_MiddleToggleButton = new GUIStyle(EditorStyles.miniButtonMid);
					int left = 0;
					int right = left;
					int top = 2;
					int bottom = 10;
					_MiddleToggleButton.margin = new RectOffset(left, right, top, bottom);
				}

				return _MiddleToggleButton;
			}
		}

		static private GUIStyle _RightToggleButton;
		static public GUIStyle RightToggleButton {
			get	{
				if(_RightToggleButton == null) {
					_RightToggleButton = new GUIStyle(EditorStyles.miniButtonRight);
					int left = 0;
					int right = left;
					int top = 2;
					int bottom = 10;
					_RightToggleButton.margin = new RectOffset(left, right, top, bottom);
				}

				return _RightToggleButton;
			}
		}

		static private GUIStyle _LineStyle;
		static public GUIStyle LineStyle {
			get	{
				if(_LineStyle == null) {
					_LineStyle = new GUIStyle();
					_LineStyle.normal.background = EditorGUIUtility.whiteTexture;
					_LineStyle.stretchWidth = true;
				}

				return _LineStyle;
			}
		}

		static private GUIStyle _VGroupStyle;
		static public GUIStyle VGroupStyle {
			get	{
				if(_VGroupStyle == null) {
					_VGroupStyle = new GUIStyle(EditorStyles.helpBox);
					int left = 5;
					int right = left;
					int top = 10;
					int bottom = top;
					_VGroupStyle.padding = new RectOffset(left, right, top, bottom);
				}

				return _VGroupStyle;
			}
		}

		static private GUIStyle _ButtonWithPadding;
		static public GUIStyle ButtonWithPadding {
			get	{
				if(_ButtonWithPadding == null) {
					_ButtonWithPadding = new GUIStyle(GUI.skin.button);
					int left = 15;
					int right = left;
					int top = 7;
					int bottom = top;
					_ButtonWithPadding.padding = new RectOffset(left, right, top, bottom);
				}

				return _ButtonWithPadding;
			}
		}

		static private GUIStyle _FoldoutHeader;
		static public GUIStyle FoldoutHeader {
			get	{
				if(_FoldoutHeader == null) {
					_FoldoutHeader = new GUIStyle(EditorStyles.foldout);
					_FoldoutHeader.richText = true;
				}

				return _FoldoutHeader;
			}
		}

		static public void DrawSeparatorLine() {
			GUILayout.Space(4);
			GUILine(new Color(.3f,.3f,.3f), 1);
			GUILine(new Color(.9f,.9f,.9f), 1);
			GUILayout.Space(4);
		}

		static public void DrawSeparatorThinLine() {
			GUILayout.Space(2);
			GUILine(new Color(.6f,.6f,.6f), 1);
			GUILine(new Color(.9f,.9f,.9f), 1);
			GUILayout.Space(2);
		}

		static public void SeparatorBig() {
			GUILayout.Space(10);
			GUILine(new Color(.3f,.3f,.3f), 2);
			GUILayout.Space(1);
			GUILine(new Color(.3f,.3f,.3f), 2);
			GUILine(new Color(.85f,.85f,.85f), 1);
			GUILayout.Space(10);
		}

		static public void GUILine(float height = 2f) {
			GUILine(Color.black, height);
		}

		static public void GUILine(Color color, float height = 2f) {
			Rect position = GUILayoutUtility.GetRect(0f, float.MaxValue, height, height, LineStyle);

			if(Event.current.type == EventType.Repaint) {
				Color orgColor = GUI.color;
				GUI.color = orgColor * color;
				LineStyle.Draw(position, false, false, false, false);
				GUI.color = orgColor;
			}
		}

		public class GUIFlexiSpace : IDisposable {
			public GUIFlexiSpace() {
				GUILayout.FlexibleSpace();
			}

			public void Dispose() {
				GUILayout.FlexibleSpace();
			}
		}

		public class GUIHorizontal : IDisposable {
			public GUIHorizontal() {
				GUILayout.BeginHorizontal(new GUILayoutOption[0]);
			}

			public GUIHorizontal(params GUILayoutOption[] layoutOptions) {
				GUILayout.BeginHorizontal(layoutOptions);
			}

			public GUIHorizontal(GUIStyle style, params GUILayoutOption[] layoutOptions) {
				GUILayout.BeginHorizontal(style, layoutOptions);
			}

			public void Dispose() {
				GUILayout.EndHorizontal();
			}
		}

		public class GUIVertical : IDisposable {
			public GUIVertical() {
				GUILayout.BeginVertical(new GUILayoutOption[0]);
			}

			public GUIVertical(GUIStyle style, params GUILayoutOption[] layoutOptions) {
				GUILayout.BeginVertical(style, layoutOptions);
			}

			public void Dispose() {
				GUILayout.EndVertical();
			}
		}

		public class GUIEnable : IDisposable {
			[SerializeField]
			private bool PreviousState {
				get;
				set;
			}

			public GUIEnable(bool newState)	{
				this.PreviousState = GUI.enabled;
				if (!this.PreviousState) {
					GUI.enabled = false;
					return;
				}
				GUI.enabled = newState;
			}

			public void Dispose() {
				GUI.enabled = this.PreviousState;
			}
		}

		public class GUIDisable : IDisposable {

			public GUIDisable(bool isDisabled)	{
				EditorGUI.BeginDisabledGroup(isDisabled);
			}

			public void Dispose() {
				EditorGUI.EndDisabledGroup();
			}
		}

		static public void ToggleHeader(SerializedProperty boolProperty, GUIContent headerText, string toggleTrue = "ON", string toggleFalse = "OFF") {
			using (new GUIHorizontal()) {
				UITools.Header(headerText);
				UITools.Toggle(boolProperty, toggleTrue, toggleFalse);
			}
		}

		static public void ToggleHeader(ValueWrapper<bool> boolProperty, GUIContent headerText, string toggleTrue = "ON", string toggleFalse = "OFF") {
			using (new GUIHorizontal()) {
				UITools.Header(headerText);
				UITools.Toggle(boolProperty, toggleTrue, toggleFalse);
			}
		}

		static public void ToggleHeader(SerializedProperty firstBoolProperty, SerializedProperty secondBoolProperty, GUIContent headerText,string toggleFirst = "First", string toggleSecond = "Second" , string toggleFalse = "OFF") {
			using (new GUIHorizontal()) {
				UITools.Header(headerText);
				UITools.Toggle(firstBoolProperty, secondBoolProperty, toggleFirst, toggleSecond, toggleFalse);
			}
		}

		static public void ToggleHeader(ValueWrapper<bool> firstBoolProperty, ValueWrapper<bool> secondBoolProperty, GUIContent headerText,string toggleFirst = "First", string toggleSecond = "Second" , string toggleFalse = "OFF") {
			using (new GUIHorizontal()) {
				UITools.Header(headerText);
				UITools.Toggle(firstBoolProperty, secondBoolProperty, toggleFirst, toggleSecond, toggleFalse);
			}
		}

		static public void Toggle(SerializedProperty boolProperty, string toggleTrue = "ON", string toggleFalse = "OFF") {
			using (new GUIHorizontal()) {
				GUIContent buttonText = new GUIContent(toggleTrue);
				Rect headerRect = GUILayoutUtility.GetRect(buttonText, UITools.LeftToggleButton);
				if (GUI.Toggle(headerRect, boolProperty.boolValue, buttonText, UITools.LeftToggleButton)){
					boolProperty.boolValue = true;
				}

				buttonText = new GUIContent(toggleFalse);
				headerRect = GUILayoutUtility.GetRect(buttonText, UITools.RightToggleButton);
				if (GUI.Toggle(headerRect, !boolProperty.boolValue, buttonText, UITools.RightToggleButton)){
					boolProperty.boolValue = false;
				}
			}
		}

		static public void ToggleWithLabel(ValueWrapper<bool> boolProperty, GUIContent label, string toggleTrue = "ON", string toggleFalse = "OFF") {
			using (new GUIHorizontal()) {
				EditorGUILayout.LabelField(label);
				UITools.Toggle(boolProperty, toggleTrue, toggleFalse);
			}
		}

		static public void ToggleWithLabel(SerializedProperty boolProperty, GUIContent label, string toggleTrue = "ON", string toggleFalse = "OFF") {
			using (new GUIHorizontal()) {
				EditorGUILayout.LabelField(label);
				UITools.Toggle(boolProperty, toggleTrue, toggleFalse);
			}
		}

		static public void Toggle(ValueWrapper<bool> boolProperty, string toggleTrue = "ON", string toggleFalse = "OFF") {
			using (new GUIHorizontal()) {
				GUIContent buttonText = new GUIContent(toggleTrue);
				Rect headerRect = GUILayoutUtility.GetRect(buttonText, UITools.LeftToggleButton);
				if (GUI.Toggle(headerRect, boolProperty, buttonText, UITools.LeftToggleButton)){
					boolProperty.Value = true;
				}

				buttonText = new GUIContent(toggleFalse);
				headerRect = GUILayoutUtility.GetRect(buttonText, UITools.RightToggleButton);
				if (GUI.Toggle(headerRect, !boolProperty, buttonText, UITools.RightToggleButton)){
					boolProperty.Value = false;
				}
			}
		}

		static public void Toggle(SerializedProperty firstBoolProperty, SerializedProperty secondBoolProperty, string toggleFirst = "First", string toggleSecond = "Second" , string toggleFalse = "OFF") {
			using (new GUIHorizontal()) {
				GUIContent buttonText = new GUIContent(toggleFirst);
				Rect headerRect = GUILayoutUtility.GetRect(buttonText, UITools.LeftToggleButton);
				if (GUI.Toggle(headerRect, firstBoolProperty.boolValue, buttonText, UITools.LeftToggleButton)){
					firstBoolProperty.boolValue = true;
					secondBoolProperty.boolValue = false;
				}

				buttonText = new GUIContent(toggleSecond);
				headerRect = GUILayoutUtility.GetRect(buttonText, UITools.MiddleToggleButton);
				if (GUI.Toggle(headerRect, secondBoolProperty.boolValue, buttonText, UITools.MiddleToggleButton)){
					firstBoolProperty.boolValue = false;
					secondBoolProperty.boolValue = true;
				}

				buttonText = new GUIContent(toggleFalse);
				headerRect = GUILayoutUtility.GetRect(buttonText, UITools.RightToggleButton);
				if (GUI.Toggle(headerRect, !firstBoolProperty.boolValue && !secondBoolProperty.boolValue, buttonText, UITools.RightToggleButton)){
					firstBoolProperty.boolValue = false;
					secondBoolProperty.boolValue = false;
				}
			}
		}

		static public void Toggle(ValueWrapper<bool> firstBoolProperty, ValueWrapper<bool> secondBoolProperty, string toggleFirst = "First", string toggleSecond = "Second" , string toggleFalse = "OFF") {
			using (new GUIHorizontal()) {
				GUIContent buttonText = new GUIContent(toggleFirst);
				Rect headerRect = GUILayoutUtility.GetRect(buttonText, UITools.LeftToggleButton);
				if (GUI.Toggle(headerRect, firstBoolProperty, buttonText, UITools.LeftToggleButton)){
					firstBoolProperty.Value = true;
					secondBoolProperty.Value = false;
				}

				buttonText = new GUIContent(toggleSecond);
				headerRect = GUILayoutUtility.GetRect(buttonText, UITools.MiddleToggleButton);
				if (GUI.Toggle(headerRect, secondBoolProperty, buttonText, UITools.MiddleToggleButton)){
					firstBoolProperty.Value = false;
					secondBoolProperty.Value = true;
				}

				buttonText = new GUIContent(toggleFalse);
				headerRect = GUILayoutUtility.GetRect(buttonText, UITools.RightToggleButton);
				if (GUI.Toggle(headerRect, !firstBoolProperty && !secondBoolProperty, buttonText, UITools.RightToggleButton)){
					firstBoolProperty.Value = false;
					secondBoolProperty.Value = false;
				}
			}
		}

		static public void Header(GUIContent headerLabelText) {
			EditorGUILayout.LabelField(headerLabelText, HeaderLabel);
		}

		static public bool GroupHeader(GUIContent headerLabelText, bool isDisplayed)
		{
			bool display = isDisplayed;
			GUILayout.Label(headerLabelText, GroupHeaderLabel);
			Event e = Event.current;
			if (e.type == EventType.MouseDown &&
			    GUILayoutUtility.GetLastRect().Contains (e.mousePosition))
			{
				display = !isDisplayed;
				e.Use ();
			}

			return display;
		}
	}
}