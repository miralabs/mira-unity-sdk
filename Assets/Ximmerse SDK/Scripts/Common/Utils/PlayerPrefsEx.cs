//=============================================================================
//
// Copyright 2016 Ximmerse, LTD. All rights reserved.
//
//=============================================================================

using System.Collections.Generic;
using UnityEngine;
using Ximmerse;

public class PlayerPrefsEx:MonoBehaviour {

	#region Nested Types

	public class MyDictionary<KeyValuePair,TValue> where KeyValuePair:UKeyValuePair<string,TValue>{

		[SerializeField]protected KeyValuePair[] m_Data=new KeyValuePair[0];
		[System.NonSerialized]public Dictionary<string,TValue> data=null;

		protected virtual void EnsureDataBuilt(){
			if(data==null) {
				int i=0,imax=m_Data.Length;
				data=new Dictionary<string, TValue>(imax);
				for(;i<imax;++i) {
					if(data.ContainsKey(m_Data[i].key)) {
#if UNITY_EDITOR
						UnityEditor.ArrayUtility.RemoveAt(ref m_Data,i);
						//
						--i;--imax;
#endif
					}else {
						data.Add(m_Data[i].key,m_Data[i].value);
					}
				}
			}
		}

		public virtual TValue Get(string key,TValue defaultValue=default(TValue)) {
			EnsureDataBuilt();
			//
			if(data.ContainsKey(key)) {
				return data[key];
			}
			//
			return defaultValue;
		}

		public virtual void Set(string key,TValue value) {
			EnsureDataBuilt();
			//
			if(data.ContainsKey(key)) {
				data[key]=value;
#if UNITY_EDITOR
				int i=System.Array.FindIndex(m_Data,(x)=>(x.key==key));
				m_Data[i].value=value;
#endif
			}else {
				data.Add(key,value);
#if UNITY_EDITOR
				KeyValuePair pair=System.Activator.CreateInstance<KeyValuePair>();
				pair.key=key;
				pair.value=value;
				UnityEditor.ArrayUtility.Add(ref m_Data,pair);
#endif
			}
		}

		public virtual bool Exists(string key) {
			EnsureDataBuilt();
			//
			return data.ContainsKey(key);
		}

		public virtual void Remove(string key) {
			EnsureDataBuilt();
			//
			if(data.ContainsKey(key)) {
				data.Remove(key);
#if UNITY_EDITOR
				int i=System.Array.FindIndex(m_Data,(x)=>(x.key==key));
				UnityEditor.ArrayUtility.RemoveAt(ref m_Data,i);
#endif
			}
		}
	}

#if UNITY_EDITOR

	[UnityEditor.CustomPropertyDrawer(typeof(StringPlayerPrefsExPair))]

	[UnityEditor.CustomPropertyDrawer(typeof(StringIntPair))]
	[UnityEditor.CustomPropertyDrawer(typeof(StringFloatPair))]
	[UnityEditor.CustomPropertyDrawer(typeof(StringStringPair))]
	// Ext
	[UnityEditor.CustomPropertyDrawer(typeof(StringVector3Pair))]
	[UnityEditor.CustomPropertyDrawer(typeof(StringColorPair))]
	[UnityEditor.CustomPropertyDrawer(typeof(StringObjectPair))]
	public class KeyValuePairDrawer:SimplePropertyDrawer {

		public override void OnGUI(Rect position,UnityEditor.SerializedProperty property,GUIContent label) {
			if(fields==null) {
				numFields=2;
				fields=new string[2]{"key","value"};
			}
			base.OnGUI(position,property,label);
		}
	}

#endif

	#endregion Nested Types

	#region Static

	protected static bool s_MainCached;
	protected static PlayerPrefsEx s_Main;

	public static PlayerPrefsEx main {
		get {
			if(!s_MainCached) {
				s_MainCached=true;
				//
				s_Main=Resources.Load<PlayerPrefsEx>("PlayerPrefsEx-Default");
				// By the loaded level.
				if(s_Main.mapPlayerPrefsEx.Get("Scene"+Application.loadedLevel,null)!=null) {
					s_Main=s_Main.LoadPlayerPrefs("Scene"+Application.loadedLevel);
				}
				if(s_Main.mapPlayerPrefsEx.Get(Application.loadedLevelName,null)!=null) {
					s_Main=s_Main.LoadPlayerPrefs(Application.loadedLevelName);
				}

				if(s_Main!=null) {
					if(Application.isPlaying) {
						string name=s_Main.name;
						s_Main=Instantiate(s_Main);// To receive the destroy message.
						s_Main.name=name;
					}
				}else {
					s_Main=new GameObject("PlayerPrefsEx-Dummy").GetComponent<PlayerPrefsEx>();
				}
				//
				Ximmerse.Log.d("PlayerPrefsEx",s_Main.name);
			}
			return s_Main;
		}
	}

#if UNITY_EDITOR

	public static void Load(PlayerPrefsEx playerPrefs) {
		s_MainCached=true;
		s_Main=playerPrefs;
		//
		if(s_Main!=null) {
			s_Main.parent=null;
		}
	}

	public static void Unload(PlayerPrefsEx playerPrefs) {
		s_MainCached=false;
		s_Main=null;
	}

#endif

	#endregion Static

	#region Type Interfaces

	[System.Serializable]public class StringPlayerPrefsExPair:UKeyValuePair<string,PlayerPrefsEx>{}
	[System.Serializable]public class PlayerPrefsExDictionary:MyDictionary<StringPlayerPrefsExPair,PlayerPrefsEx>{}
	public PlayerPrefsExDictionary mapPlayerPrefsEx=new PlayerPrefsExDictionary();
	public static PlayerPrefsEx GetPlayerPrefsEx(string key,PlayerPrefsEx defaultValue=null) {
		if(main.mapPlayerPrefsEx.Exists(key)) {
			return main.mapPlayerPrefsEx.Get(key,defaultValue);
		}else if(main.parent!=null) {
			return main.parent.mapPlayerPrefsEx.Get(key,defaultValue);
		}
		return defaultValue;
	}
	public static void SetPlayerPrefsEx(string key,PlayerPrefsEx value) {
		main.mapPlayerPrefsEx.Set(key,value);
	}

	[System.Serializable]public class StringIntPair:UKeyValuePair<string,int>{}
	[System.Serializable]public class IntDictionary:MyDictionary<StringIntPair,int>{}
	public IntDictionary mapInt=new IntDictionary();
	public static int GetInt(string key,int defaultValue=0) {
		if(main.mapInt.Exists(key)) {
			return main.mapInt.Get(key,defaultValue);
		}else if(main.parent!=null) {
			return main.parent.mapInt.Get(key,defaultValue);
		}
		return defaultValue;
	}
	public static void SetInt(string key,int value) {
		main.mapInt.Set(key,value);
	}

	[System.Serializable]public class StringFloatPair:UKeyValuePair<string,float>{}
	[System.Serializable]public class FloatDictionary:MyDictionary<StringFloatPair,float>{}
	public FloatDictionary mapFloat=new FloatDictionary();
	public static float GetFloat(string key,float defaultValue=0) {
		if(main.mapFloat.Exists(key)) {
			return main.mapFloat.Get(key,defaultValue);
		}else if(main.parent!=null) {
			return main.parent.mapFloat.Get(key,defaultValue);
		}
		return defaultValue;
	}
	public static void SetFloat(string key,float value) {
		main.mapFloat.Set(key,value);
	}

	[System.Serializable]public class StringStringPair:UKeyValuePair<string,string>{}
	[System.Serializable]public class StringDictionary:MyDictionary<StringStringPair,string>{}
	public StringDictionary mapString=new StringDictionary();
	public static string GetString(string key,string defaultValue) {
		if(main.mapString.Exists(key)) {
			return main.mapString.Get(key,defaultValue);
		}else if(main.parent!=null) {
			return main.parent.mapString.Get(key,defaultValue);
		}
		return defaultValue;
	}
	public static void SetString(string key,string value) {
		main.mapString.Set(key,value);
	}

	[System.Serializable]public class StringVector3Pair:UKeyValuePair<string,Vector3>{}
	[System.Serializable]public class Vector3Dictionary:MyDictionary<StringVector3Pair,Vector3>{}
	public Vector3Dictionary mapVector3=new Vector3Dictionary();
	public static Vector3 GetVector3(string key,Vector3 defaultValue) {
		if(main.mapVector3.Exists(key)) {
			return main.mapVector3.Get(key,defaultValue);
		}else if(main.parent!=null) {
			return main.parent.mapVector3.Get(key,defaultValue);
		}
		return defaultValue;
	}
	public static void SetVector3(string key,Vector3 value) {
		main.mapVector3.Set(key,value);
	}

	[System.Serializable]public class StringColorPair:UKeyValuePair<string,Color>{}
	[System.Serializable]public class ColorDictionary:MyDictionary<StringColorPair,Color>{}
	public ColorDictionary mapColor=new ColorDictionary();
	public static Color GetColor(string key,Color defaultValue) {
		if(main.mapColor.Exists(key)) {
			return main.mapColor.Get(key,defaultValue);
		}else if(main.parent!=null) {
			return main.parent.mapColor.Get(key,defaultValue);
		}
		return defaultValue;
	}
	public static void SetColor(string key,Color value) {
		main.mapColor.Set(key,value);
	}

	[System.Serializable]public class StringObjectPair:UKeyValuePair<string,Object>{}
	[System.Serializable]public class ObjectDictionary:MyDictionary<StringObjectPair,Object>{}
	public ObjectDictionary mapObject=new ObjectDictionary();
	public static Object GetObject(string key,Object defaultValue=null) {
		if(main.mapObject.Exists(key)) {
			return main.mapObject.Get(key,defaultValue);
		}else if(main.parent!=null) {
			return main.parent.mapObject.Get(key,defaultValue);
		}
		return defaultValue;
	}
	public static void SetObject(string key,Object value) {
		main.mapObject.Set(key,value);
	}

	//

	public static bool GetBool(string key,bool defaultValue=false) {
		return main.mapInt.Get(key,defaultValue?1:0)==1;
	}

	public static void SetBool(string key,bool value) {
		main.mapInt.Set(key,value?1:0);
	}

	public static Vector3 GetVector3(string key) {
		return GetVector3(key,Vector3.zero);
	}
	public static Color GetColor(string key) {
		return GetColor(key,Color.black);
	}

	#endregion Type Interfaces

	#region Misc

	[System.NonSerialized]public PlayerPrefsEx parent;

	protected virtual void OnDestroy() {
		s_Main=null;
		s_MainCached=false;
	}

	public virtual PlayerPrefsEx LoadPlayerPrefs(string key) {
		PlayerPrefsEx ret=mapPlayerPrefsEx.Get(key,null);
		if(ret!=null) {
			ret.parent=this;
		}
		return ret;
	}

	#endregion Misc

}
