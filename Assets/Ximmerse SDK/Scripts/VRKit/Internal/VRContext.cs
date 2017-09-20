//=============================================================================
//
// Copyright 2016 Ximmerse, LTD. All rights reserved.
//
//=============================================================================

#if XIM_SDK_PREVIEW
#define OPENVR_SDK
#endif
using UnityEngine;
using Ximmerse.InputSystem;
#if UNITY_IOS
using AOT;
#endif

namespace Ximmerse.VR{

	public enum VRNode{
		None           = -1,
		LeftEye            ,
		RightEye           ,
		CenterEye          ,
		Head               ,
		LeftHand           ,
		RightHand          ,
		TrackerDefault     ,
		TrackingSpace      ,
		Count              ,
	}

	public enum TrackingOrigin {
		Unknown    = -1,
		EyeLevel       ,
		FloorLevel     ,
		Count          ,
	}


	/// <summary>
	/// VRContext is used to describe the context of VR HMD SDK we used.
	/// </summary>
	public class VRContext:MonoBehaviour {

		#region Nested Types

		[System.Serializable]public class NodeTransformPair:UKeyValuePair<VRNode,Transform>{}
#if UNITY_EDITOR
		[UnityEditor.CustomPropertyDrawer(typeof(NodeTransformPair))]public class NodeTransformPairDrawer:UKeyValuePairDrawer<VRNode,Transform>{}
#endif

		#endregion Nested Types

		#region Static

		/// <summary>
		/// 
		/// </summary>
		protected static VRContext s_Main;

		protected static bool s_MainCached;

		/// <summary>
		/// 
		/// </summary>
		public static VRContext main {
			get {
				if(!s_MainCached) {
					s_MainCached=true;
					if(s_Main==null) {
						//
						s_Main=FindObjectOfType<VRContext>();
						/*
						if(s_Main!=null) {// Create dummy one.
							s_Main=new GameObject("VRContext Dummy",typeof(VRContext),typeof(VRDevice)).GetComponent<VRContext>();
							VRDevice vrdev=s_Main.GetComponent<VRDevice>();
							vrdev.family="Dummy";
						}*/
						//
						if(s_Main!=null) {
							s_Main.InitVRContext();
						}
					}
					//
					if(s_Main!=null) {
						Ximmerse.Log.i("VRContext","Initialization Info : family = \""+((s_Main.vrDevice==null)?"None":s_Main.vrDevice.family)+"\", isPresent = "+isPresent+", refreshRate = "+refreshRate+" Hz.");
					}
				}
				return s_Main;
			}
		}
	
		public static XDevicePlugin.ControllerStateDelegate s_OnHmdUpdate=OnHmdUpdate;
		public static XDevicePlugin.SendMessageDelegate s_OnHmdMessage=OnHmdMessage;

#if UNITY_IOS
		public delegate int OnHmdUpdateDelegate (int which, ref XDevicePlugin.ControllerState state);
		public delegate int OnHmdMessageDelegate(int which, int Msg, int wParam, int lParam);
#endif

		#endregion Static

		#region Static Methods

		protected static bool InScene(Transform t) {
			if(t==null){
				return false;
			}
			// TODO:
			return t.parent!=null;
		}

		protected static void CopyTransform(Transform src,Transform dest) {
			dest.localPosition=src.localPosition;
			dest.localRotation=src.localRotation;
			dest.localScale=src.localScale;
		}
	
		/// <summary>
		/// 
		/// </summary>
		public static Transform GetAnchor(VRNode node) {
			if(main==null) {
				//
				switch(node) {
					case VRNode.CenterEye:
						if(Camera.main!=null) {
							return Camera.main.transform;
						}
					break;
				}
				//
				return null;
			}
			return main.GetAnchor(node,null);
		}
	
		/// <summary>
		/// 
		/// </summary>
		public static void SetAnchor(VRNode node,Transform value) {
			if(main==null) {
				return;
			}
			main.SetAnchor(node,value,true);
		}

		public static Quaternion GetHmdRotation() {
			if(main==null||s_Main.vrDevice==null) {
				return Quaternion.identity;
			}else {
				return s_Main.vrDevice.GetRotation();
			}
		}
	
		/// <summary>
		/// Center tracking to the current position and orientation of the HMD.
		/// </summary>
		public static void Recenter(bool fireEvent) {
			if(main!=null){
				//s_Main.vrDevice.Recenter();
			}
			if(fireEvent){if(main!=null){if(s_Main.onRecenter!=null) {
				s_Main.onRecenter.Invoke();
			}}}
		}
	
		public static void FadeIn(float delay=0.0f,float duration=-1.0f) {
			if(main!=null){
				if(s_Main.fadeFx!=null) {
					s_Main.fadeFx.FadeIn(delay,duration);
				}
			}
		}
	
		public static void FadeOut(float delay=0.0f,float duration=-1.0f) {
			if(main!=null){
				if(s_Main.fadeFx!=null) {
					s_Main.fadeFx.FadeOut(delay,duration);
				}
			}
		}
#if UNITY_IOS
		[MonoPInvokeCallback(typeof(OnHmdUpdateDelegate))]
#endif
		public static int OnHmdUpdate(int which, ref XDevicePlugin.ControllerState state){
            if(s_Main!=null) {
				Quaternion q=(s_Main.vrDevice==null)?Quaternion.identity:s_Main.vrDevice.GetRotation();
				int i=0;
				state.rotation[i]=-q[i];++i;
				state.rotation[i]=-q[i];++i;
				state.rotation[i]= q[i];++i;
				state.rotation[i]= q[i];++i;
				return 0;
			}else {
				return -1;
			}
		}

#if UNITY_IOS
		[MonoPInvokeCallback(typeof(OnHmdMessageDelegate))]
#endif
		public static int OnHmdMessage(int which, int Msg, int wParam, int lParam){
			switch(Msg) {
				case XDevicePlugin.kMessage_RecenterSensor:
					if(main!=null) {
						s_Main.vrDevice.Recenter();
					}
				break;
			}
			return 0;
		}

		/// <summary>
		/// Successfully detected a VR device in working order.
		/// </summary>
		public static bool isPresent {
			get {
				if(main==null) {
					return false;
				}
				return s_Main.vrDevice.isPresent;
			}
		}

		/// <summary>
		/// Refresh rate of the display in Hertz.
		/// </summary>
		public static float refreshRate {
			get {
				if(main==null) {
					return Application.targetFrameRate;
				}
				return s_Main.vrDevice.refreshRate;
			}
		}
	
		public static TrackingOrigin trackingOrigin {
			get {
				if(main==null) {
					return TrackingOrigin.Unknown;
				}
				return s_Main.trackingOriginType;
			}
		}
	
		public static Transform uiRootVR{
			get {
				Transform centerEye=GetAnchor(VRNode.CenterEye);
				if(centerEye==null){
					return null;
				}else {
                    if(s_Main!=null && s_Main.m_UiRootVR==null) {
						s_Main.m_UiRootVR=centerEye.Find("UIRoot-VR") as Transform;
						if(s_Main.m_UiRootVR==null) {
							s_Main.m_UiRootVR=new GameObject("UIRoot-VR",typeof(Canvas)).transform;
							//
							s_Main.m_UiRootVR.SetParent(centerEye);
							s_Main.m_UiRootVR.localPosition=Vector3.zero;
							s_Main.m_UiRootVR.localRotation=Quaternion.identity;
							s_Main.m_UiRootVR.localScale=Vector3.one;
							//
							Canvas canvas=s_Main.m_UiRootVR.GetComponent<Canvas>();
							canvas.renderMode=RenderMode.WorldSpace;
						}
					}
					return s_Main.m_UiRootVR;
				}
			}
		}
	
		public static VRDevice currentDevice {
			get {
				if(main!=null) {
					return s_Main.vrDevice;
				}
				return null;
			}
		}

		#endregion Static Methods

		#region Fields

		public TrackingOrigin trackingOriginType=TrackingOrigin.EyeLevel;
		public VRDevice vrDevice;
	
		/// <summary>
		/// 
		/// </summary>
		[SerializeField]protected NodeTransformPair[] m_Anchors=new NodeTransformPair[] {
			new NodeTransformPair{key=VRNode.TrackingSpace},
			new NodeTransformPair{key=VRNode.CenterEye}
		};

		public bool canRecenter=true;
		public UnityEngine.Events.UnityAction onRecenter;

		[System.NonSerialized]protected bool m_IsInited=false;
		[System.NonSerialized]protected float m_LastHomeButtonPressedTime;

		[System.NonSerialized]protected Transform m_UiRootVR;
		[System.NonSerialized]protected Ximmerse.UI.UIFade m_UIFade;

		#endregion Fields

		#region Unity Messages

		/// <summary>
		/// 
		/// </summary>
		protected virtual void Awake() {
			if(s_Main==null) {
				s_Main=this;
			}else if(s_Main!=this) {
				Ximmerse.Log.e("VRContext","Only one instance can be run!!!");
			}
			InitVRContext();
		}
	
		/// <summary>
		/// 
		/// </summary>
		protected virtual void Update(){
			if(canRecenter) {
				// PC : Push the keyboard for debug.
				if(Input.GetKeyDown(KeyCode.R)) {
					Recenter(true);
				}
				// Mobile VR : Hold the Home button for a while.
				if(ControllerInputManager.GetButtonUp(ControllerType.Controller,(uint)XimmerseButton.Home)){
					m_LastHomeButtonPressedTime=-1.0f;
				}else if(m_LastHomeButtonPressedTime>0.0f&&(Time.time-m_LastHomeButtonPressedTime)>=1.0f) {
					m_LastHomeButtonPressedTime=-1.0f;
					Recenter(true);
				}else if(ControllerInputManager.GetButtonDown(ControllerType.Controller,(uint)XimmerseButton.Home)){
					m_LastHomeButtonPressedTime=Time.time;
				}
			}

            //setHMDRotation();
		}

        public void setHMDRotation()
        {
            Quaternion qua = Quaternion.identity;
            if(main!=null) {
				qua=(s_Main.vrDevice==null)?Quaternion.identity:s_Main.vrDevice.GetRotation();
			}
            //Debug.Log("setHMDRotation:"+qua);
            XDevicePlugin.setHMDRotation(qua);
        }
	
		/// <summary>
		/// 
		/// </summary>
		protected virtual void OnDestroy(){
			s_MainCached=false;
		}

		#endregion Unity Messages

		#region Methods

		/// <summary>
		/// 
		/// </summary>
		public virtual void InitVRContext() {
			//
			if(m_IsInited) {
				return;
			}
			m_IsInited=true;
			// Init Hmd SDK.
			bool needClone=true;
			if(vrDevice==null) {
#if (UNITY_EDITOR || UNITY_STANDALONE)&&OPENVR_SDK
				vrDevice=new GameObject("OpenVRDevice",typeof(OpenVRDevice)).GetComponent<OpenVRDevice>();
				needClone=false;
#else
				GameObject go=PlayerPrefsEx.GetObject("VRDevice.source",null) as GameObject;
				if(go==null||(vrDevice=go.GetComponent<VRDevice>())==null) {
				}else{
				}
#endif
			}
			  //
			if(needClone) {
				string name=vrDevice.name;
				vrDevice=Instantiate(vrDevice) as VRDevice;
				vrDevice.name=name;
			}
			  //
			Transform t=vrDevice.transform;
			t.SetParent(transform);
			t.localPosition=Vector3.zero;
			t.localRotation=Quaternion.identity;
			t.localScale=Vector3.one;
			  // The center eye may be replaced in this function.
			vrDevice.InitDevice(this);
			//
			// Check existed anchors 
			Transform trackingSpace=GetAnchor(VRNode.TrackingSpace,null);
			Transform centerEyeAnchor=GetAnchor(VRNode.CenterEye,null);
			  // Auto fill the trackingSpace field if it is null.
			if(trackingSpace==null){if(centerEyeAnchor!=null) {
				SetAnchor(VRNode.TrackingSpace,trackingSpace=centerEyeAnchor.parent);
			}}
			// Instantiate anchors.
			if(trackingSpace!=null) {
				for(int i=0,imax=m_Anchors.Length;i<imax;++i) {
					switch(m_Anchors[i].key) {
						// Ignored anchors.
						case VRNode.TrackingSpace:
						case VRNode.Count:
						break;
						//
						default:
							if(m_Anchors[i].value!=null) {
								if(!InScene(m_Anchors[i].value)) {// This anchor is a prefab.
									Transform copy=Instantiate<Transform>(m_Anchors[i].value);
									copy.SetParent(trackingSpace);
									copy.name=m_Anchors[i].key.ToString()+"Anchor";
									CopyTransform(m_Anchors[i].value,copy);
									//
									m_Anchors[i].value=copy;
								}
							}
						break;
					}
				}
			}
			if(vrDevice.family!="Dummy") {
				if(trackingSpace==null&&centerEyeAnchor==null) {
					Ximmerse.Log.e("VRContext","trackingSpace==null&&centerEyeAnchor==null at InitVRContext()");
				}else if(centerEyeAnchor==null) {
					Ximmerse.Log.e("VRContext","centerEyeAnchor==null at InitVRContext()");
				}
			}
			// Optional :
			if(PlayerPrefsEx.GetBool("VRDevice.useFade",false)) {
				if(fadeFx!=null) {
					fadeFx.FadeOut();
				}
			}
			// Optional :
			if(PlayerPrefsEx.GetBool("VRDevice.useReticle",false)) {
				GameObject go=PlayerPrefsEx.GetObject("VRDevice.reticleObject",null) as GameObject;
				if(go!=null) {
					Transform rawT=go.transform;
					go=Instantiate(go);
					Transform newT=go.transform;
					//
					newT.SetParent(uiRootVR);
					CopyTransform(rawT,newT);
				}
			}
			//
			// Send the initialization message.
			XDevicePlugin.Init();
			vrDevice.OnVRContextInited(this);
			for(int i=0,imax=m_Anchors.Length;i<imax;++i) {
				if(m_Anchors[i].value!=null) {
					m_Anchors[i].value.SendMessage("OnVRContextInited",this,SendMessageOptions.DontRequireReceiver);
				}
			}
		}

		/// <summary>
		/// 
		/// </summary>
		public virtual Transform GetAnchor(VRNode node,Transform defaultValue) {
			for(int i=0,imax=m_Anchors.Length;i<imax;++i) {
				if(m_Anchors[i].key==node) {
					return m_Anchors[i].value;
				}
			}
			return defaultValue;
		}

		/// <summary>
		/// 
		/// </summary>
		public virtual void SetAnchor(VRNode node,Transform value,bool canAdd) {
			int i=0,imax=m_Anchors.Length;
			for(;i<imax;++i) {
				if(m_Anchors[i].key==node) {
					m_Anchors[i].value=value;
					return;
				}
			}
			//
			if(canAdd) {
				NodeTransformPair[] newAnchors=new NodeTransformPair[imax+1];
				i=imax;
				while(i-->0) {
					newAnchors[i]=m_Anchors[i];
				}
				newAnchors[imax]=new NodeTransformPair{key=node,value=value};
				m_Anchors=newAnchors;
			}
		}
	
		/// <summary>
		/// Center tracking to the current position and orientation of the HMD.
		/// </summary>
		public virtual void Recenter() {
			Recenter(true);
		}

		// TODO :

		public virtual Ximmerse.UI.UIFade fadeFx {
			get {
				if(uiRootVR==null) {
					return null;
				}
				if(m_UIFade==null) {
					RectTransform t=uiRootVR.Find("VRCameraFade") as RectTransform;
					if(t==null){
						t=new GameObject("VRCameraFade",typeof(UnityEngine.UI.Image),typeof(Ximmerse.UI.UIFade)).GetComponent<RectTransform>();
					}
					//
					t.GetComponent<UnityEngine.UI.Image>().color=PlayerPrefsEx.GetColor("VRDevice.fadeColor",Color.black);
					//
					m_UIFade=t.GetComponent<Ximmerse.UI.UIFade>();
					m_UIFade.durationIn=m_UIFade.durationOut=PlayerPrefsEx.GetFloat("VRDevice.fadeTime",2.0f);
					//
					t.SetParent(uiRootVR);
					t.localPosition=new Vector3(0,0,0.11f);
					t.localRotation=Quaternion.identity;
					t.localScale=Vector3.one*0.0055f;
					t.sizeDelta=new Vector2(100,100);
				}
				return m_UIFade;
			}
		}

		#endregion Methods

	}

}