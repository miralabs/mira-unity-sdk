//=============================================================================
//
// Copyright 2016 Ximmerse, LTD. All rights reserved.
//
//=============================================================================

#if UNITY_EDITOR
using UnityEditor;
using UnityEngine;
#endif

namespace Ximmerse {

public class UKeyValuePair<TKey,TValue>{
	public TKey key;
	public TValue value;
}

#if UNITY_EDITOR

public class UKeyValuePairDrawer<TKey,TValue>:SimplePropertyDrawer{

	public override void OnGUI(Rect position,SerializedProperty property,GUIContent label) {
		if(fields==null) {
			numFields=2;
			fields=new string[2] {"key","value"};
		}
		base.OnGUI(position,property,label);
	}
}

#endif

}
