//=============================================================================
//
// Copyright 2016 Ximmerse, LTD. All rights reserved.
//
//=============================================================================

using UnityEngine;
using UnityEditor;
using System.IO;
using System.Collections.Generic;

namespace Ximmerse {

	public class SettingsEditor:EditorWindow {

		#region Static

		[MenuItem("Window/Ximmerse Settings")]
		static void Init() {
			SettingsEditor window=(SettingsEditor)EditorWindow.GetWindow(typeof(SettingsEditor));
			window.Show();
		}

		// TODO : It is a feature which needs a lot of works to do.

		protected static string[] s_VRDeviceTypeNames=null;
		protected static Ximmerse.VR.VRDevice[] s_VRDeviceTypeSources=null;

		protected static string[] s_XimmerseDeviceTypeNames=new string[] {
			"Unknown Device",
			"Arm Model",
			"Inside-Out",
			"Outside-In",
		};
		protected static int [] s_XimmerseDeviceTypeValues=new int[] {
			-1,
			0x1010,
			0x3000,
			0x4000,
		};

		#endregion Static

		protected GUIStyle m_HeaderStyle;
		protected PlayerPrefsEx m_PlayerPrefsEx;
		protected Vector2 m_ScrollPosition;


		protected virtual string GetResourcePath() {
			var ms=MonoScript.FromScriptableObject(this);
			var path=AssetDatabase.GetAssetPath(ms);
			path=System.IO.Path.GetDirectoryName(path);
			return path.Substring(0,path.Length-"Editor".Length);
		}

		protected virtual void OnEnable() {
			if(m_HeaderStyle==null) {
				m_HeaderStyle=new GUIStyle();
				m_HeaderStyle.fontStyle=FontStyle.Bold;
				m_HeaderStyle.fontSize=14;
			}
		}

		protected virtual void OnDisable() {
			PlayerPrefsEx.Unload(m_PlayerPrefsEx);
			m_PlayerPrefsEx=null;
		}

		protected virtual void OnGUI () {
			var resourcePath=GetResourcePath();
#if !(UNITY_5_0)
			var logo=AssetDatabase.LoadAssetAtPath<Texture2D>(resourcePath+"GameAssets/Textures/ui_xim_logo_01.png");
#else
			var logo=Resources.LoadAssetAtPath<Texture2D>(resourcePath + "GameAssets/Textures/ui_xim_logo_01.png");
#endif
			EditorGUILayout.Space();
			var rect=GUILayoutUtility.GetRect(position.width,40,GUI.skin.box);
			if(logo) {
				GUI.DrawTexture(rect,logo,ScaleMode.ScaleToFit);
			}
			EditorGUILayout.Space();
			EditorGUILayout.BeginHorizontal();
				if(GUILayout.Button("User Manual")) {
					Application.OpenURL("https://github.com/Ximmerse/SDK");
				}
				if(GUILayout.Button("API Reference")) {
					Application.OpenURL("https://github.com/Ximmerse/SDK/tree/master/Unity");
				}
				if(GUILayout.Button("FAQ")) {
					Application.OpenURL("https://github.com/Ximmerse/SDK/blob/master/Document/DK04/FAQ_EN.md");
				}
				//if(GUILayout.Button("Get Help")) {
				//}
				//if(GUILayout.Button("Check for Updates")) {
				//}
			EditorGUILayout.EndHorizontal();
			//
			if(m_PlayerPrefsEx==null) {
				m_PlayerPrefsEx=Resources.Load<PlayerPrefsEx>("PlayerPrefsEx-Default");
				PlayerPrefsEx.Load(m_PlayerPrefsEx);
			}
			if(m_PlayerPrefsEx==null) {
				return;
			}
			//
			m_ScrollPosition=EditorGUILayout.BeginScrollView(m_ScrollPosition);
			//
			DrawSeparator();
			EditorGUI.BeginChangeCheck();
				EditorGUILayout.LabelField("Devices",m_HeaderStyle);
				BeginGroup();
					EditorGUILayout.BeginHorizontal();
						string vrFamily="Unknown";
						//
						if(s_VRDeviceTypeNames==null) {
							BuildVRDeviceTypes();
						}
						if(s_VRDeviceTypeNames.Length>1) {
							int vrOldId=GetVRDeviceTypeIndex();
							int vrNewId=EditorGUILayout.Popup("VR Device",vrOldId,s_VRDeviceTypeNames);
							vrFamily=s_VRDeviceTypeNames[vrNewId];
							if(vrOldId!=vrNewId) {
								SetVRDeviceTypeIndex(vrNewId);
							}
							//
							if(GUILayout.Button("Select",GUILayout.Width(56f))) {
								if(s_VRDeviceTypeSources[vrNewId]!=null) {
									Selection.objects=new Object[1]{s_VRDeviceTypeSources[vrNewId].gameObject};
								}
							}
							if(GUILayout.Button("Refresh",GUILayout.Width(56f))) {
								BuildVRDeviceTypes();
							}
						}else{
							EditorGUILayout.LabelField("VR Device",GUILayout.Width(146f));
							if(GUILayout.Button("Refresh")) {
								BuildVRDeviceTypes();
							}
						}
					EditorGUILayout.EndHorizontal();
					if(vrFamily!="Unknown"){
					BeginGroup();
						if(DrawToggle("Use Fade","VRDevice.useFade",true)) {
							DrawColorField("Fade Color","VRDevice.fadeColor",Color.black);
							DrawSlider("Fade Time","VRDevice.fadeTime",2.0f,0f,5f);
						}
						if(DrawToggle("Use Reticle","VRDevice.useReticle",true)) {
							DrawObjectField<Object>("Reticle Object","VRDevice.reticleObject",null);
						}
						if(DrawToggle("Fade On Bad Tracking","VRDevice.forceFadeOnBadTracking",false)){
							BeginGroup();
								DrawColorField("Fade Color","VRDevice.badFadeColor",new Color(.305882365f,.352941185f,.392156899f,1.0f));
								DrawSlider("Fade Time","VRDevice.badFadeTime",0.5f,0f,5f);
							EndGroup();
						}
					EndGroup();}
					switch(DrawIntPopup("Ximmerse Device","XimmerseDevice.type",0x4000,s_XimmerseDeviceTypeNames,s_XimmerseDeviceTypeValues)) {
						case 0x1010:
				EndGroup();
							DrawSeparator();
							EditorGUILayout.LabelField("Arm Model",m_HeaderStyle);
							BeginGroup();
								DrawVector3Field("Shoulder Position","ArmModel.shoulderPosition",new Vector3(0.19f, -0.19f, -0.03f));
								DrawVector3Field("Elbow Position","ArmModel.elbowPosition",new Vector3(0.195f, -0.5f, -0.075f));
								DrawVector3Field("Wrist Position","ArmModel.wristPosition",new Vector3(0.0f, 0.0f, 0.25f));
								DrawVector3Field("Pointer Position","ArmModel.pointerPosition",new Vector3(0.0f, -0.009f, 0.099f));
							EndGroup();
						break;
						case 0x3200:
							BeginGroup();
								DrawToggle("Fix Shake(Experimental)","XimmerseTracker[Inside-out].fixShake",false);
								DrawSlider("M2P Latency","XimmerseTracker[Inside-out].m2pLatency",0.0f,0.0f,1.0f);
							EndGroup();
				EndGroup();
						break;
						case 0x4000:
							BeginGroup();
								DrawVector3Field("Position","XimmerseTracker[Outside-in].position",new Vector3(0,1.675f,1.5f));
								DrawVector3Field("Rotation","XimmerseTracker[Outside-in].rotation",new Vector3(15f,0,0));
							EndGroup();
				EndGroup();
						break;
						default:
				EndGroup();
						break;
					}
			if(EditorGUI.EndChangeCheck()) {
				EditorUtility.SetDirty(m_PlayerPrefsEx);
			}
			//
			if(hasPlayAreaFeature) {
				DrawSeparator();
				DrawPlayAreaFeature();
			}
			//
			DrawSeparator();
			EditorGUI.BeginChangeCheck();
				EditorGUILayout.LabelField("Debug*",m_HeaderStyle);
				BeginGroup();
					DrawEnumPopup<InputSystem.LOGLevel>("Log Level","xdevice.logLevel",(int)InputSystem.LOGLevel.LOG_DEBUG);
					bool oldValue=PlayerPrefsEx.GetBool("xdevice.noServicesInAndroid",false);
					bool newValue=DrawToggle("No Services In Android","xdevice.noServicesInAndroid",false);
					if(!oldValue&&newValue) {
						//
						Debug.Log("Please edit \"Assets\\Plugins\\Android\\AndroidManifest.xml\" manually.");
					}
				EndGroup();
			if(EditorGUI.EndChangeCheck()) {
				EditorUtility.SetDirty(m_PlayerPrefsEx);
			}
			//
			EditorGUILayout.EndScrollView();
		}

		// Taken from http://blog.csdn.net/zhuangyou123/article/details/38660189
		protected virtual T[] LoadAllAssets<T>(string path,string searchPattern,bool retVal=true,List<T> objList=null) where T : Object {
			string[] directoryEntries;
			if(objList==null) objList=new List<T>();
			try{
				//
				directoryEntries=Directory.GetFiles(path,searchPattern);
				for(int i=0,imax=directoryEntries.Length;i<imax;i++){
					T tempTex=AssetDatabase.LoadAssetAtPath(directoryEntries[i],typeof(T)) as T;
					if(tempTex!=null){
						objList.Add(tempTex);
					}
				}
				//
				directoryEntries=Directory.GetDirectories(path);
				for(int i=0,imax=directoryEntries.Length;i<imax;i++){
					LoadAllAssets<T>(directoryEntries[i],searchPattern,false,objList);
				}
			}catch(DirectoryNotFoundException) {
				Debug.Log("The path encapsulated in the "+path+"Directory object does not exist.");
			}
			if(retVal&&objList.Count>0)
				return objList.ToArray();
			return null;
		}

		protected virtual void BuildVRDeviceTypes() {
			var list=LoadAllAssets<Ximmerse.VR.VRDevice>("Assets/","VRDevice-*.prefab");
			if(list==null) {
				list=new Ximmerse.VR.VRDevice[0];
			}
			//
			int i=0,imax=list.Length+1;
			s_VRDeviceTypeNames=new string[imax];
			s_VRDeviceTypeSources=new Ximmerse.VR.VRDevice[imax];
			//
			s_VRDeviceTypeSources[i]=null;
			s_VRDeviceTypeNames[i]="Unknown";
			++i;
			//
			for(;i<imax;++i) {
				s_VRDeviceTypeSources[i]=list[i-1].GetComponent<Ximmerse.VR.VRDevice>();
				if(s_VRDeviceTypeSources[i]==null) {
					s_VRDeviceTypeNames[i]="Unknown";
				} else {
#if UNITY_ANDROID
					s_VRDeviceTypeNames[i]=s_VRDeviceTypeSources[i].androidTargetName;
#else
					s_VRDeviceTypeNames[i]=s_VRDeviceTypeSources[i].family;
#endif
				}
			}
		}

		protected virtual int GetVRDeviceTypeIndex() {
			Ximmerse.VR.VRDevice d=null;
			var vrDeviceSource = PlayerPrefsEx.GetObject ("VRDevice.source", null);
			GameObject go = vrDeviceSource as GameObject;

			if(go!=null) {
				d=go.GetComponent<Ximmerse.VR.VRDevice>();
			}
			//
			for(int i=0,imax=s_VRDeviceTypeSources.Length;i<imax;++i) {
				if(s_VRDeviceTypeSources[i]==d) {
					return i;
				}
			}
			return 0;
		}

		protected virtual void SetVRDeviceTypeIndex(int id) {
			Ximmerse.VR.VRDevice d=s_VRDeviceTypeSources[id];
			PlayerPrefsEx.SetObject("VRDevice.source",d==null?null:d.gameObject);
			//
#if UNITY_ANDROID
			if(d==null) {
				/*
				if(File.Exists(Application.dataPath + "/Plugins/Android/AndroidManifest.xml") &&
					EditorUtility.DisplayDialog("Change To UnityManaged","Remove AndroidManifest.xml?","Yes,do it","No,Thanks")) {
					File.Delete(Application.dataPath + "/Plugins/Android/AndroidManifest.xml");
					AssetDatabase.Refresh();
				}
				*/
			} else {
				/*
				if(d.androidManifest!=null){
					if(EditorUtility.DisplayDialog("Change To " + d.androidTargetName,"Replace AndroidManifest.xml?","Yes,do it","No,Thanks")) {
						File.WriteAllText(Application.dataPath + "/Plugins/Android/AndroidManifest.xml",d.androidManifest.text);
						AssetDatabase.Refresh();
					}
				}
				*/
			}
#endif
			if(d==null) {
			}else {
				PlayerSettings.virtualRealitySupported=d.useUnityVR;
			}
		}

		protected virtual bool hasPlayAreaFeature {
			get {
				return PlayerPrefsEx.GetInt("XimmerseDevice.type",0)==0x4000;
			}
		}

		protected virtual void DrawPlayAreaFeature() {
			EditorGUILayout.LabelField("Play Area",m_HeaderStyle);
			EditorGUI.BeginChangeCheck();
			if(DrawToggle("Enabled","PlayArea.enabled",true)) {
				//
			BeginGroup();
				PlayAreaRenderer prefab=null;
				GameObject go=PlayerPrefsEx.GetObject("PlayAreaRenderer") as GameObject;
				if(go==null) {
					go=EditorGUILayout.ObjectField("Prefab",null,typeof(GameObject),true) as GameObject;
					if(go==null) {
			EndGroup();
						return;
					}else {
						PlayerPrefsEx.SetObject("PlayAreaRenderer",go);
					}
				}
				if(go!=null) {
					prefab=go.GetComponent<PlayAreaRenderer>();
				}
				if(prefab==null) {
			EndGroup();
						return;
				}
				//
                DrawToggle("Show Camera Model", "PlayArea.showCameraModel", true);
                //EditorGUILayout.HelpBox("",MessageType.Info);
                DrawSlider("Warning Distance","PlayArea.warningDistance",0.5f    ,0.01f ,1.0f);
				if(DrawToggle("Draw Ground","PlayArea.drawGround",true)) {
					if(EditorGUI.EndChangeCheck()) {
						EditorUtility.SetDirty(m_PlayerPrefsEx);
					}
				BeginGroup();
				EditorGUI.BeginChangeCheck();
					prefab.groundColor=EditorGUILayout.ColorField("Color",prefab.groundColor);
					prefab.borderThickness=EditorGUILayout.Slider("Thickness",prefab.borderThickness,0.01f ,1.0f);
				if(EditorGUI.EndChangeCheck()) EditorUtility.SetDirty(prefab);
				EndGroup();
				}else {
					if(EditorGUI.EndChangeCheck()) {
						EditorUtility.SetDirty(m_PlayerPrefsEx);
					}
				}
				EditorGUI.BeginChangeCheck();
				if(DrawToggle("Draw Wall","PlayArea.drawWall",true)) {
				if(EditorGUI.EndChangeCheck()) {
					EditorUtility.SetDirty(m_PlayerPrefsEx);
				}
				BeginGroup();
				EditorGUI.BeginChangeCheck();
					prefab.wallColor=EditorGUILayout.ColorField("Color",prefab.wallColor);
					prefab.thickness=EditorGUILayout.Slider("Thickness",prefab.thickness,0.001f,1.0f);
					prefab.cellSize=EditorGUILayout.Slider("Cell Size",prefab.cellSize,0.01f,1.0f);
					prefab.emptySize=EditorGUILayout.Slider("Empty Size",prefab.emptySize,0.01f,1.0f);
					prefab.height=EditorGUILayout.Slider("Height",prefab.height,1.0f,3.0f);
				if(EditorGUI.EndChangeCheck()) EditorUtility.SetDirty(prefab);
				EndGroup();
				}
				EditorGUI.BeginChangeCheck();
				if(DrawToggle("Draw Plane","PlayArea.drawPlane",true)) {
				if(EditorGUI.EndChangeCheck()) {
					EditorUtility.SetDirty(m_PlayerPrefsEx);
				}
				BeginGroup();
				EditorGUI.BeginChangeCheck();
					prefab.planeColor=EditorGUILayout.ColorField("Color",prefab.planeColor);
				if(EditorGUI.EndChangeCheck()) EditorUtility.SetDirty(prefab);
				EndGroup();
				}
			EndGroup();
			}
		}

		#region Editor GUI Extension

		//protected static System.Collections.Generic.List<int>
		// s_IndentLevels=new System.Collections.Generic.List<int>();

		protected static void BeginGroup(float value=14.0f) {
			EditorGUILayout.BeginHorizontal();
				GUILayout.Space(value);
				EditorGUILayout.BeginVertical();
			//int indent=EditorGUI.indentLevel+(int)value;
			//EditorGUI.indentLevel=indent;
			//s_IndentLevels.Add(indent);
		}

		protected static void EndGroup() {
				EditorGUILayout.EndVertical();
			EditorGUILayout.EndHorizontal();
			//int i=s_IndentLevels.Count-1;
			//if(i>=0) {
			//	EditorGUI.indentLevel=s_IndentLevels[i];
			//	s_IndentLevels.RemoveAt(i);
			//}
		}

		/// <summary>
		/// Draw a visible separator in addition to adding some padding.
		/// </summary>
		protected static void DrawSeparator() {
			GUILayout.Space(12f);

			if(Event.current.type==EventType.Repaint) {
				Texture2D tex = EditorGUIUtility.whiteTexture;
				Rect rect = GUILayoutUtility.GetLastRect();
				GUI.color=new Color(0f,0f,0f,0.25f);
				GUI.DrawTexture(new Rect(0f,rect.yMin+6f,Screen.width,4f),tex);
				GUI.DrawTexture(new Rect(0f,rect.yMin+6f,Screen.width,1f),tex);
				GUI.DrawTexture(new Rect(0f,rect.yMin+9f,Screen.width,1f),tex);
				GUI.color=Color.white;
			}
		}

		protected static bool DrawToggle(string label,string key,bool value) {
			value=PlayerPrefsEx.GetBool(key,value);
			bool newValue=EditorGUILayout.Toggle(label,value);
			if(newValue!=value) {
				PlayerPrefsEx.SetBool(key,newValue);
			}
			return newValue;
		}

		protected static string DrawPopup(string label,string key,string value,string[] displayedOptions) {
			value=PlayerPrefsEx.GetString(key,value);
			//
			int intValue=System.Array.IndexOf(displayedOptions,value);
			if(intValue<0){intValue=0;}
			//
			int newValue=EditorGUILayout.Popup(label,intValue,displayedOptions);
			if(newValue!=intValue) {
				PlayerPrefsEx.SetString(key,displayedOptions[newValue]);
			}
			return displayedOptions[newValue];
		}

		protected static int DrawIntPopup(string label,string key,int value,string[] displayedOptions,int[] optionValues) {
			value=PlayerPrefsEx.GetInt(key,value);
			int newValue=EditorGUILayout.IntPopup(label,value,displayedOptions,optionValues);
			if(newValue!=value) {
				PlayerPrefsEx.SetInt(key,newValue);
			}
			return newValue;
		}

		protected static int DrawEnumPopup<T>(string label,string key,int value) {
			string[] displayedOptions=System.Enum.GetNames(typeof(T));
			int[] optionValues=(int[])System.Enum.GetValues(typeof(T));
			return DrawIntPopup(label,key,value,displayedOptions,optionValues) ;
		}

		protected static Vector3 DrawVector3Field(string label,string key,Vector3 value) {
			value=PlayerPrefsEx.GetVector3(key,value);
			Vector3 newValue=EditorGUILayout.Vector3Field(label,value);
			if(newValue!=value) {
				PlayerPrefsEx.SetVector3(key,newValue);
			}
			return newValue;
		}

		protected static Color DrawColorField(string label,string key,Color value) {
			value=PlayerPrefsEx.GetColor(key,value);
			Color newValue=EditorGUILayout.ColorField(label,value);
			if(newValue!=value) {
				PlayerPrefsEx.SetColor(key,newValue);
			}
			return newValue;
		}

		protected static float DrawSlider(string label,string key,float value,float min,float max) {
			value=PlayerPrefsEx.GetFloat(key,value);
			float newValue=EditorGUILayout.Slider(label,value,min,max);
			if(newValue!=value) {
				PlayerPrefsEx.SetFloat(key,newValue);
			}
			return newValue;
		}

		protected static T DrawObjectField<T>(string label,string key,T value) where T:Object{
			value=PlayerPrefsEx.GetObject(key,value) as T;
			T newValue=EditorGUILayout.ObjectField(label,value,typeof(T),false) as T;
			if(newValue!=value) {
				PlayerPrefsEx.SetObject(key,newValue);
			}
			return newValue;
		}

		#endregion Editor GUI Extension

	}

}
