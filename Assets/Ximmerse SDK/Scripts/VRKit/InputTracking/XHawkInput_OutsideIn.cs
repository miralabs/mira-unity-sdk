//=============================================================================
//
// Copyright 2016 Ximmerse, LTD. All rights reserved.
//
//=============================================================================

using System.Runtime.InteropServices;
using UnityEngine;
using UnityEngine.UI;
using Ximmerse.UI;
using Ximmerse.VR;

namespace Ximmerse.InputSystem {

	public class XHawkInput_OutsideIn:XHawkInput {

		#region Fields

		[Header("Outside-In")]
		public GameObject anchorPrefab;
		public bool makeTrackerForward=false;
		
		public bool useAutoRecenter=true;
		public float autoRecenterMinAngle=5.0f;
		public bool useExperimental=true;
		public float alignNeedTime=1.0f;
		public Sprite uiAlignSprite;
		public Sprite uiXHawkSprite;
		public Sprite uiLookAtSprite;

		[System.NonSerialized]protected Transform m_CenterEye;
		[System.NonSerialized]protected VRDevice m_VRDevice;
		[System.NonSerialized]protected PlayAreaHelper m_PlayAreaHelper;
		[System.NonSerialized]protected float m_LastAlignHmdTime=-1.0f;
		[System.NonSerialized]protected Image m_UiAlign;
		[System.NonSerialized]protected UIFade m_UiAlignFade;
		[System.NonSerialized]protected Transform m_UiLookAt;
		[System.NonSerialized]protected UIFade m_UiLookAtFade;

        public GameObject uiAutoRecenter;
        private GameObject goUIAutoRecenter;

        private static bool _wasUserPresent = false;
        private bool _needAutoRecenter = true;
        private float lastHmdYaw = -1.0f;
        private int countSameHmdYaw = 0;

		#endregion Fields

		#region Methods

		public override bool InitInternal(){
			if((XDevicePlugin.GetInt(XDevicePlugin.ID_CONTEXT,XDevicePlugin.kField_CtxDeviceVersionInt,0x4000)&0xF000)!=0x4000) {
				return false;
			}
			//
			if(true) {
				Vector3 position=PlayerPrefsEx.GetVector3("XimmerseTracker[Outside-in].position");
				Vector3 rotation=PlayerPrefsEx.GetVector3("XimmerseTracker[Outside-in].rotation");
				XDevicePlugin.SetTrackerPose(m_Handle,position.y,position.z,rotation.x);
				//
				XDevicePlugin.GetTrackerPose(m_Handle,out position.y,out position.z,out rotation.x);
				//
				int i=m_Controllers.Length;while(i-->0) {
					XDevicePlugin.SetBool(XDevicePlugin.GetInputDeviceHandle(m_Controllers[i].key),XDevicePlugin.kField_IsAbsRotationBool,true);
				}
				//
				//PlayerPrefsEx.SetBool("VRDevice.forceFadeOnBadTracking",true);
				XDevicePlugin.NativeMethods.XDeviceSetDecoderFeature(XDevicePlugin.FEATURE_BLOB_AUTO);
			}
			return base.InitInternal();
		}

		public override bool CreateAnchor() {
			if(anchor!=null) {
				return false;
			}
			//
			anchor=(anchorPrefab==null||!m_IsRequestVR?// in 2D mode.
				new GameObject():
				Object.Instantiate(anchorPrefab)
			).transform;
			anchor.name="TrackerAnchor(Outside-In)";
			anchor.SetParent(trackingSpace);
			//
			UpdateAnchorFromPlugin();
			m_PlayAreaHelper=anchor.GetComponentInChildren<PlayAreaHelper>();
			//
			m_UseAnchorProjection=true;
			VRContext.SetAnchor(VRNode.TrackerDefault,anchor);
			//
			if(m_IsRequestVR&&useAutoRecenter) {
				// Recenter hmd firstly,for aligning rotation pivot of vr device and position pivot.
				// In outside-in case,we never use yaw offset(Recenter() will fix the value) in hmd sdk.
				XDevicePlugin.SetBool(m_HmdInput.handle,XDevicePlugin.kField_IsAbsRotationBool,false);
				VRContext.Recenter(false);
				XDevicePlugin.SetBool(m_HmdInput.handle,XDevicePlugin.kField_IsAbsRotationBool,true);
				//
				if(PlayerPrefsEx.GetBool("VRDevice.forceFadeOnBadTracking")) {
					VRContext.FadeOut(1.0f,-1.0f);
				}
			}

            if(m_CenterEye==null) {
				m_CenterEye=VRContext.GetAnchor(VRNode.CenterEye);
				m_VRDevice=VRContext.currentDevice;
			}

            if(uiAutoRecenter!=null)
            {
                goUIAutoRecenter = Instantiate(uiAutoRecenter) as GameObject;
                Transform originTranformAuto = uiAutoRecenter.transform;
                goUIAutoRecenter.transform.SetParent(VRContext.uiRootVR);
                goUIAutoRecenter.transform.localPosition=originTranformAuto.localPosition;
		        goUIAutoRecenter.transform.localRotation=originTranformAuto.localRotation;
		        goUIAutoRecenter.transform.localScale=originTranformAuto.localScale;
                _needAutoRecenter = true;
            }
			// TODO : Align Notice
			if(alignNeedTime>0.0f) {
				m_UiAlign=new GameObject("Wait for Aligning",typeof(Image),typeof(UIFade)).GetComponent<Image>();
				m_UiAlign.gameObject.SetActive(false);
				//
				m_UiAlign.material=Resources.Load<Material>("UI/MatNotice");
				m_UiAlign.sprite=uiAlignSprite;
				m_UiAlign.type=Image.Type.Filled;
				m_UiAlign.fillMethod=Image.FillMethod.Radial360;
				m_UiAlign.fillOrigin=2;
				m_UiAlign.fillAmount=0.0f;
				//
				RectTransform t=m_UiAlign.GetComponent<RectTransform>();
				t.SetParent(VRContext.uiRootVR);
				t.localPosition=Vector3.forward*0.8f;
				t.localRotation=Quaternion.identity;
				t.localScale=Vector3.one*0.0016f;
				t.SetSizeWithCurrentAnchors(RectTransform.Axis.Horizontal,80f);
				t.SetSizeWithCurrentAnchors(RectTransform.Axis.Vertical,80f);
				//
				m_UiAlignFade=m_UiAlign.GetComponent<UIFade>();
				m_UiAlignFade.durationOut=0.25f;
				m_UiAlignFade.onBecameInvisible.AddListener(()=>{m_UiAlign.gameObject.SetActive(false);});
			}
			// TODO : LookAt Notice
			if(true) {
				m_UiLookAtFade=new GameObject("Look At XHawk",typeof(UIFade),typeof(Image)).GetComponent<UIFade>();
				m_UiLookAtFade.GetComponent<Image>().sprite=uiXHawkSprite;
				m_UiLookAtFade.GetComponent<Image>().material=Resources.Load<Material>("UI/MatNotice");
				m_UiLookAtFade.durationIn=.25f;
				m_UiLookAtFade.durationOut=.16f;
				m_UiLookAtFade.onBecameInvisible.AddListener(()=>{m_UiLookAtFade.gameObject.SetActive(false);});
				m_UiLookAtFade.gameObject.SetActive(false);

				RectTransform t=m_UiLookAtFade.GetComponent<RectTransform>();
				t.SetParent(VRContext.uiRootVR);
				t.localPosition=Vector3.forward*0.85f;
				t.localRotation=Quaternion.identity;
				t.localScale=Vector3.one*0.0016f;
				t.SetSizeWithCurrentAnchors(RectTransform.Axis.Horizontal,80f);
				t.SetSizeWithCurrentAnchors(RectTransform.Axis.Vertical,80f);
				//
				m_UiLookAt=new GameObject("Dummy-LookAt").transform;
				m_UiLookAt.SetParent(t);
				m_UiLookAt.localPosition=Vector3.zero;
				m_UiLookAt.localRotation=Quaternion.identity;
				m_UiLookAt.localScale=Vector3.one;
				//
				t=new GameObject("LookAt",typeof(Image)).GetComponent<RectTransform>();
				t.GetComponent<Image>().sprite=uiLookAtSprite;
				t.GetComponent<Image>().material=Resources.Load<Material>("UI/MatNotice");
				t.SetParent(m_UiLookAt);
				t.localPosition=Vector3.forward*80.0f;
				t.localRotation=Quaternion.Euler(new Vector3(90.0f,0.0f,-90.0f));
				t.localScale=Vector3.one;
				t.SetSizeWithCurrentAnchors(RectTransform.Axis.Horizontal,40f);
				t.SetSizeWithCurrentAnchors(RectTransform.Axis.Vertical,40f);
			}
			return true;
		}

		public override void UpdateAnchor() {
			if(useAutoRecenter) {
				TryAlignHmd();
			}
			// TODO : Align Notice
			if(m_LastAlignHmdTime>=0.0f) {
				if(m_UiAlign!=null) {
					m_UiAlign.fillAmount=Mathf.Clamp01((Time.time-m_LastAlignHmdTime)/alignNeedTime);
					if(m_UiAlign.fillAmount>=1.0) {
						m_UiAlignFade.FadeOut();
						m_LastAlignHmdTime=-1.0f;
					}
				}
			}else
			// TODO : LookAt Notice
			if(m_IsRequestVR) {
				if((XDevicePlugin.GetNodePosition(m_Handle,0,2,null)&TrackingResult.PositionTracked)==0) {
					//
					Vector3 lookAt=m_UiLookAt.parent.InverseTransformPoint(anchor.position);lookAt.z=0.0f;
					m_UiLookAt.localRotation=Quaternion.LookRotation(lookAt,Vector3.back);
					//
					if(!m_UiLookAtFade.isFadingIn) {
						m_UiLookAtFade.FadeIn();
					}
				}else if(m_UiLookAtFade.alpha>0.0f&&!m_UiLookAtFade.isFadingOut) {
					m_UiLookAtFade.FadeOut();
				}
			}
		}

		public override void Recenter() {
			//
			if(useAutoRecenter) {
				AlignHmd();
				return;
			}
			// Legacy method.
			RecenterAllControllers(0.0f);
			//
			if((ControllerInputManager.GetButtonDown(ControllerType.LeftController,(uint)XimmerseButton.Home)&&ControllerInputManager.GetButton(ControllerType.RightController,(uint)XimmerseButton.Home))||
				(ControllerInputManager.GetButton(ControllerType.LeftController,(uint)XimmerseButton.Home)&&ControllerInputManager.GetButtonDown(ControllerType.RightController,(uint)XimmerseButton.Home))
			){
				XDevicePlugin.SendMessage(m_Handle,XDevicePlugin.kMessage_RecenterSensor,1,0);
				UpdateAnchorFromPlugin();
				//
				if(m_PlayAreaHelper!=null) {
					m_PlayAreaHelper.OnTrackerRecenter();
				}
			}
		}

		public override Vector3 GetLocalPosition(int node) {
			Vector3 position=Vector3.zero;
			if(node==m_Controllers[2].value) {
				//
				if(useExperimental) {
					//
					UpdateState();
					//
					XDevicePlugin.GetSmoothPosition(2,ref position);
					//
					position.Scale(sensitivity);
					if(m_UseAnchorProjection&&anchor!=null) {
						position=m_AnchorMatrix.MultiplyPoint3x4(position);
					}
				}else {
					position=base.GetLocalPosition(node);
				}
				//
				if(m_CenterEye==null) {
					m_CenterEye=VRContext.GetAnchor(VRNode.CenterEye);
					m_VRDevice=VRContext.currentDevice;
				}
				if(m_CenterEye!=null&&m_VRDevice!=null) {
					Quaternion rotation=m_VRDevice.GetRotation();
					//
					if(useAutoRecenter) {
						rotation=Quaternion.AngleAxis(m_YawOffset,Vector3.up)*rotation;
						m_VRDevice.yawOffset=m_YawOffset;
					}else {
					}
					//
					position-=rotation*(m_VRDevice.neckToEye+m_VRDevice.outsideInMarkPose.position);
				}
			}else {
				//
				if(m_EnableEmulatedHand) {
					if((XDevicePlugin.GetNodePosition(m_Handle,0,node,null)&TrackingResult.PositionTracked)==0) {
						return GetEmulatedHandPosition(node);
					}
				}
				//
				if(useExperimental) {
					//
					UpdateState();
					//
					XDevicePlugin.GetSmoothPosition(node,ref position);
					//
					position.Scale(sensitivity);
					if(m_UseAnchorProjection&&anchor!=null) {
						position=m_AnchorMatrix.MultiplyPoint3x4(position);
					}
				}else {
					position=base.GetLocalPosition(node);
				}
				//
			}
			return position;
		}

		[System.NonSerialized]protected float[] m_UpdateAnchorFromPluginTRS=new float[3+4+3];
		public virtual void UpdateAnchorFromPlugin(){
			XDevicePlugin.GetObject(m_Handle,XDevicePlugin.kField_TRSObject,m_UpdateAnchorFromPluginTRS,0);
			if(true) {
				anchor.localPosition=new Vector3(
					 m_UpdateAnchorFromPluginTRS[0],
					 m_UpdateAnchorFromPluginTRS[1],
					-m_UpdateAnchorFromPluginTRS[2]
				);
				anchor.localRotation=new Quaternion(
					-m_UpdateAnchorFromPluginTRS[3],
					-m_UpdateAnchorFromPluginTRS[4],
					 m_UpdateAnchorFromPluginTRS[5],
					 m_UpdateAnchorFromPluginTRS[6]
				);
				//
				if(makeTrackerForward) {
				if(trackingSpace!=null) {
					trackingSpace.localRotation=Quaternion.Euler(0.0f,-(anchor.localRotation.eulerAngles.y+180.0f),0.0f);
				}}
			}
		}

		#endregion Methods

		#region Natives

		[DllImport(XDevicePlugin.LIB_XDEVICE,CallingConvention=CallingConvention.Cdecl)]
		public static extern float XDeviceGetHmdYaw(float[] rotation);

		#endregion Natives

		#region Experimentals

		protected bool m_IsPaused;

		protected virtual void OnApplicationFocus(bool hasFocus) {
			OnApplicationPause(!hasFocus);
		}

		protected virtual void OnApplicationPause(bool pauseStatus) {
			if(m_IsPaused!=pauseStatus) {
				m_IsPaused=pauseStatus;
				//
				XDevicePlugin.setHMDRotation(Quaternion.identity);
				if(m_IsPaused) {// OnPause()
				}else {// OnResume()
					AlignHmd();
				}
			}
		}

		protected float m_YawOffset=0.0f;
		
		public virtual float GetRawYaw() {
            if (VRContext.currentDevice == null)
                return 0f;
			Quaternion rotation=VRContext.currentDevice.GetRotation();
			return rotation.eulerAngles.y;
		}

		protected float[] m_TempHmdRotation=new float[4];
        private float getHmdYaw()
        {
            float yaw = -1;
#if (UNITY_ANDROID && !UNITY_EDITOR)
			yaw=XDeviceGetHmdYaw(m_TempHmdRotation);
#else
			Quaternion rotation=VRContext.GetHmdRotation();
			m_TempHmdRotation[0]=-rotation.x;
			m_TempHmdRotation[1]=-rotation.y;
			m_TempHmdRotation[2]= rotation.z;
			m_TempHmdRotation[3]= rotation.w;
			//
			yaw=XDeviceGetHmdYaw(m_TempHmdRotation);
#endif
            return yaw;
        }

		public virtual float GetBetterYaw() {
            float yaw = getHmdYaw();
			//
			if(yaw>=0
				&&(XDevicePlugin.GetNodePosition(m_Handle,0,2,null)&TrackingResult.PositionTracked)!=0) {
				return 360.0f-yaw;
			}else {
				return GetRawYaw()+m_YawOffset;
			}
		}

		protected int m_CheckHmdSleepFrameCount=-1,m_ForceRecenterHmdFrameCount=-1;
		protected Quaternion m_CheckHmdSleepPrevRotation=Quaternion.identity;
		protected bool m_CheckHmdSleepValue=false;
		protected float m_CheckHmdSleepStartTime=-1.0f;

		public virtual bool IsHmdSleep() {
			if(m_CheckHmdSleepFrameCount!=Time.frameCount) {
				m_CheckHmdSleepFrameCount=Time.frameCount;
				//
				if(m_CenterEye==null) {
					return false;
				}else{
					Quaternion q=m_CenterEye.rotation;
					Vector3 offset=q*Vector3.forward-m_CheckHmdSleepPrevRotation*Vector3.forward;
					bool prevValue=m_CheckHmdSleepValue;
					m_CheckHmdSleepValue=offset.sqrMagnitude<=(0.01f*0.01f);
					if(!prevValue&&m_CheckHmdSleepValue) {
						m_CheckHmdSleepStartTime=Time.realtimeSinceStartup;
					}
					//
					m_CheckHmdSleepPrevRotation=q;
				}
			}
			return m_CheckHmdSleepValue;
		}

		public virtual float GetHmdSleepTime() {
			//
			if(GetTime()<=1.0f||(Time.frameCount==m_ForceRecenterHmdFrameCount)) {
				return 1.0f;
			}
			if(IsHmdSleep()) {
				return Time.realtimeSinceStartup-m_CheckHmdSleepStartTime;
			}else {
				return 0.0f;
			}
		}

		public virtual float GetAutoRecenterMinAngle() {
			//
			if(GetTime()<=.75f||(Time.frameCount==m_ForceRecenterHmdFrameCount)) {
				return 0.0f;
			}
			//
			return autoRecenterMinAngle;
		}

		protected float m_GameStartTime=0.0f;
		protected bool m_IsHmdRecentering=false;

		public virtual float GetTime() {
			return Time.timeSinceLevelLoad-m_GameStartTime;
		}

		public virtual void AlignHmd() {
			if(m_IsHmdRecentering){
				return;
			}
			m_IsHmdRecentering=true;
			//
			if(PlayerPrefsEx.GetBool("VRDevice.forceFadeOnBadTracking")) {
				VRContext.main.fadeFx.onBecameVisible.AddListener(AlignHmdDelayed);
				VRContext.FadeIn(0.0f,0.15f);
			}else {
				m_ForceRecenterHmdFrameCount=Time.frameCount+1;
				m_IsHmdRecentering=false;
			}
		}

		public virtual void AlignHmdDelayed() {
			if(PlayerPrefsEx.GetBool("VRDevice.forceFadeOnBadTracking")) {
				m_GameStartTime=Time.timeSinceLevelLoad;
				//
				VRContext.main.fadeFx.onBecameVisible.RemoveListener(AlignHmdDelayed);
				VRContext.main.fadeFx.onBecameInvisible.AddListener(AlignHmdFinished);
				VRContext.FadeOut(1.0f,0.15f);
				//
				StartCoroutine(RecenterAllControllersDelayed(0.99f,GetRawYaw()+m_YawOffset));
			}
		}

		public virtual void AlignHmdFinished() {
			if(PlayerPrefsEx.GetBool("VRDevice.forceFadeOnBadTracking")) {
				VRContext.main.fadeFx.onBecameInvisible.RemoveListener(AlignHmdFinished);
				m_IsHmdRecentering=false;
			}
		}

		public virtual void TryAlignHmd() {
            if (VRContext.currentDevice == null)
                return;
            
			float curYaw=GetRawYaw()+m_YawOffset,betterYaw=GetBetterYaw();
			float yawOffset=Mathf.Repeat(betterYaw-curYaw,360.0f);
			//
			if(yawOffset>180.0f) {yawOffset=360.0f-yawOffset;}
			//

            // HMD mounted events
	        if (_wasUserPresent && !VRContext.currentDevice.isUserPresent)
	        {
		        //HMDUnmounted
	        }
	        if (!_wasUserPresent && VRContext.currentDevice.isUserPresent)
	        {
		        //HMDMounted
                _needAutoRecenter = true;
                goUIAutoRecenter.SetActive(true);
	        }
		    _wasUserPresent = VRContext.currentDevice.isUserPresent;

            // if not getHmdYaw, countSameHmdYaw++
            if(getHmdYaw()==lastHmdYaw)
                countSameHmdYaw++;
            else
                countSameHmdYaw=0;
            lastHmdYaw = getHmdYaw();

             
            // Untill lookat camera,do AutoRecenter
			if(_needAutoRecenter && GetHmdSleepTime()>0.5f && (getHmdYaw()>320.0f||(getHmdYaw()<40.0f&&getHmdYaw()>0.0f))&&countSameHmdYaw<20) {
                if(yawOffset>=GetAutoRecenterMinAngle())
                {
                    m_YawOffset=betterYaw-GetRawYaw();
				    // TODO : Align Notice
					m_LastAlignHmdTime=Time.time;
					if(m_UiAlign!=null) {
						m_UiAlign.fillAmount=0.0f;
						m_UiAlign.gameObject.SetActive(true);
					}
                }
                _needAutoRecenter = false;
                goUIAutoRecenter.SetActive(false);
			}

			// Call AlignHmd() without UIFade.
			if(Time.frameCount==m_ForceRecenterHmdFrameCount) {
				if(XDeviceGetHmdYaw(m_TempHmdRotation)<0.0f) {// Next frame.
					m_ForceRecenterHmdFrameCount=Time.frameCount+1;
					betterYaw=0.0f;
				}else {// Done.
					RecenterAllControllers(curYaw);
				}
			}else {
				if(XDeviceGetHmdYaw(m_TempHmdRotation)<0.0f) {// No result.
				//	betterYaw=0.0f;
				}
			}

			// Tell the plugin hmd's yaw value.
            if(_needAutoRecenter || !_wasUserPresent)
            {
                XDevicePlugin.setHMDRotation(Quaternion.AngleAxis(0,Vector3.up));
            }
            else
            {
                //XDevicePlugin.setHMDRotation(Quaternion.AngleAxis(curYaw,Vector3.up));
                Quaternion rotationVR=VRContext.currentDevice.GetRotation();
                Vector3 eulerVR = rotationVR.eulerAngles;
                eulerVR.y = curYaw;
                XDevicePlugin.setHMDRotation(Quaternion.Euler(eulerVR));
            }
		}

		#endregion Experimentals

	}
}
