//=============================================================================
//
// Copyright 2016 Ximmerse, LTD. All rights reserved.
//
//=============================================================================

using UnityEngine;
using Ximmerse.InputSystem;

namespace Ximmerse.VR{

	[System.Serializable]
	public class Pose3D {
		public Vector3 position;
		public Vector3 eulerAngles;
		public Quaternion rotation {
			get {
				return Quaternion.Euler(eulerAngles);
			}
			set {
				eulerAngles=value.eulerAngles;
			}
		}
	}

	public class VRDevice:MonoBehaviour {

		#region Fields

		[Header("VR")]
		public string family;
		public TrackingOrigin trackingOriginType=TrackingOrigin.EyeLevel;
		[Tooltip("Vector from the tracking origin to the neck pivot point.")]
		public Vector3 neckPosition;
		[Tooltip("Vector from the neck pivot point to the point between the eyes.")]
		public Vector3 neckToEye;

		[Header("Editor")]
		public string androidTargetName;
		public bool useUnityVR=false;

		[Header("For Ximmerse")]
		public Pose3D inOutsideMarkPose;
		public Pose3D outsideInMarkPose;
		
		[System.NonSerialized]public float yawOffset;
		[System.NonSerialized]protected ControllerInput m_Input;
		[System.NonSerialized]protected Transform m_CenterEyeAnchor,m_EyeMover;
		[System.NonSerialized]protected Vector3 m_HeadPosition;
		[System.NonSerialized]protected int m_Handle;

		#endregion Fields

		#region Unity Messages

		protected virtual void Start() {
			m_Input=ControllerInputManager.GetInput(ControllerType.Hmd);
		}

		protected virtual void Update() {
			if(m_Input!=null&&m_Input.positionTracked) {
				m_HeadPosition=m_Input.GetPosition();
			}
			//
			if(m_EyeMover!=null) {
				m_EyeMover.localRotation=Quaternion.AngleAxis(yawOffset,Vector3.up);
				m_EyeMover.localPosition=m_HeadPosition+(m_EyeMover.localRotation*m_CenterEyeAnchor.localRotation)*neckToEye;
			}
		}

		#endregion Unity Messages

		#region Methods

		/// <summary>
		/// Initialize this device.
		/// </summary>
		public virtual void InitDevice(VRContext context) {
			m_Handle=XDevicePlugin.GetInputDeviceHandle("VRDevice");
		}

		public virtual void OnVRContextInited(VRContext context) {
			m_CenterEyeAnchor=context.GetAnchor(VRNode.CenterEye,null);
			m_HeadPosition=Vector3.zero;
			yawOffset=0.0f;
			//
			if(m_CenterEyeAnchor!=null) {
				m_EyeMover=new GameObject("EyeMover").transform;

				m_EyeMover.SetParent(m_CenterEyeAnchor.parent);
				m_EyeMover.localPosition=Vector3.zero;
				m_EyeMover.localRotation=Quaternion.identity;
				m_EyeMover.localScale=Vector3.one;

				m_CenterEyeAnchor.SetParent(m_EyeMover);
				m_CenterEyeAnchor.localPosition=Vector3.zero;
				m_CenterEyeAnchor.localRotation=Quaternion.identity;
				m_CenterEyeAnchor.localScale=Vector3.one;
			}
		}

		public virtual Quaternion GetRotation() {
			if(useUnityVR) {
				return UnityEngine.VR.InputTracking.GetLocalRotation(UnityEngine.VR.VRNode.CenterEye);
			}else if(m_CenterEyeAnchor!=null) {
				return m_CenterEyeAnchor.localRotation;
			}
			return Quaternion.identity;
		}

		/// <summary>
		/// Successfully detected a VR device in working order.
		/// </summary>
		public virtual bool isPresent {
			get{
				if(useUnityVR) {
					return UnityEngine.VR.VRDevice.isPresent;
				}
				return false;
			}
		}

	    /// <summary>
	    /// True if the user is currently wearing the display.
	    /// </summary>
	    public virtual bool isUserPresent
	    {
		    get {
				return true;
		    }
	    }

		/// <summary>
		/// Refresh rate of the display in Hertz.
		/// </summary>
		public virtual float refreshRate {
			get{
				if(useUnityVR) {
					return UnityEngine.VR.VRDevice.refreshRate;
				}
				return 60.0f;
			}
		}

		/// <summary>
		/// Center tracking to the current position and orientation of the HMD.
		/// </summary>
		public virtual void Recenter() {
			//
			if(XDevicePlugin.GetBool(m_Handle,XDevicePlugin.kField_IsAbsRotationBool,false)) {
				return;
			}
			//
			if(useUnityVR) {
				UnityEngine.VR.InputTracking.Recenter();
			}
		}

		#endregion Methods

	}

}