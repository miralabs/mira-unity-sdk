//=============================================================================
//
// Copyright 2016 Ximmerse, LTD. All rights reserved.
//
//=============================================================================

#if UNITY_EDITOR

using UnityEngine;
using UnityEditor;

public class SimplePropertyDrawer:PropertyDrawer {

	#region Static

	protected static GUIContent s_EmptyLabel=new GUIContent();

	public static bool PropertyField(Rect position,SerializedProperty property,string label,float labelWidth=-1f,float propertyWidth=-1f,bool includeChildren=false) {
		Rect positionLabel=position;
		if(labelWidth<0) {
			positionLabel.width-=propertyWidth;
			position.xMin=position.xMax-propertyWidth;
		}
		if(propertyWidth<0) {
			positionLabel.width=labelWidth;
			position.xMin+=labelWidth;
		}
		EditorGUI.LabelField(positionLabel,label);
		return EditorGUI.PropertyField(position,property,s_EmptyLabel,includeChildren);
	}

	#endregion Static

	#region Fields
	
	public string[] fields;
	public int numFields;

	#endregion Fields

	#region Methods

	/// <summary>
	/// 
	/// </summary>
	// Draw the property inside the given rect
	public override void OnGUI(Rect position,SerializedProperty property,GUIContent label) {
		// Using BeginProperty / EndProperty on the parent property means that
		// prefab override logic works on the entire property.
		EditorGUI.BeginProperty(position,label,property);

		// Draw label
		// position=EditorGUI.PrefixLabel (position, GUIUtility.GetControlID (FocusType.Passive), label);

		// Don't make child fields be indented
		int indent = EditorGUI.indentLevel;
		EditorGUI.indentLevel=0;

		float indentOffset=indent*15f;
		position.x+=indentOffset;
		position.width-=indentOffset;

		// Calculate rects
		int i, imax=numFields;
		Rect[] rects=new Rect[imax];
		float padding=4.0f;
		float width=(position.width-padding*(imax-1))/imax;
		for(i=0;i<imax;++i) {
			rects[i].Set(position.x+(width+padding)*i,position.y,width,position.height);
		}

		// Draw fields - passs GUIContent.none to each so they are drawn without labels
		OnDraw(property,rects);
		// Set indent back to what it was
		EditorGUI.indentLevel=indent;

		EditorGUI.EndProperty();
	}

	/// <summary>
	/// 
	/// </summary>
	protected virtual void OnDraw(SerializedProperty property,Rect[] rects) {
		for(int i=0;i<numFields;++i) {
			EditorGUI.PropertyField(rects[i],property.FindPropertyRelative(fields[i]),s_EmptyLabel,false);
		}
	}

	#endregion Methods

}

#endif