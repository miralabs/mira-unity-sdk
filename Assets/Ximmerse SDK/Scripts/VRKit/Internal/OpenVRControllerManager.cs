//=============================================================================
//
// Copyright 2016 Ximmerse, LTD. All rights reserved.
//
//=============================================================================

#if XIM_SDK_PREVIEW
#define OPENVR_SDK
#endif
#if OPENVR_SDK
using Valve.VR;
#endif
using UnityEngine;
using Ximmerse.InputSystem;
using System;

namespace Ximmerse.VR {

	public class OpenVRController:ExternalControllerDevice {

#if OPENVR_SDK
		public int unObjectId;
		public bool isValid=false;
		protected bool m_UseOnNewPoses=false;
		protected SteamVR_Controller.Device m_Device;
		protected TrackedDevicePose_t m_Pose;

		public OpenVRController(string name,int unObjectId):base(name) {
			this.unObjectId=unObjectId;
			this.m_Device=SteamVR_Controller.Input((int)this.unObjectId);
		}

		//
		public override int GetInputState(ref XDevicePlugin.ControllerState state) {
			if(m_Device.connected) {
				Vector2 v2;
				//
				v2=m_Device.GetAxis(EVRButtonId.k_EButton_Axis1);
				state.axes[(int)ControllerAxis.PrimaryTrigger]=v2.x;
				v2=m_Device.GetAxis(EVRButtonId.k_EButton_Axis0);
				state.axes[(int)ControllerAxis.PrimaryThumbX]=v2.x;
				state.axes[(int)ControllerAxis.PrimaryThumbY]=v2.y;
				//
				state.buttons=0;
				if(m_Device.GetHairTrigger()) state.buttons|=(uint)XimmerseButton.Trigger;
				if(m_Device.GetPress(SteamVR_Controller.ButtonMask.Grip)) state.buttons|=(uint)XimmerseButton.Grip;

				if(m_Device.GetTouch(SteamVR_Controller.ButtonMask.Touchpad)) state.buttons|=(uint)XimmerseButton.Touch;
				if(m_Device.GetPress(SteamVR_Controller.ButtonMask.Touchpad)) state.buttons|=(uint)XimmerseButton.Click;

				if(m_Device.GetPress(SteamVR_Controller.ButtonMask.ApplicationMenu)) state.buttons|=(uint)XimmerseButton.App;
				if(m_Device.GetPress(SteamVR_Controller.ButtonMask.System)) state.buttons|=(uint)XimmerseButton.Home;

				//
				TrackedDevicePose_t pose=m_UseOnNewPoses?m_Pose:m_Device.GetPose();
				XDevicePlugin.SetInt(handle,XDevicePlugin.kField_TrackingResultInt,(int)(pose.bPoseIsValid?TrackingResult.PoseTracked:TrackingResult.NotTracked));
				SteamVR_Utils.RigidTransform rt=new SteamVR_Utils.RigidTransform(pose.mDeviceToAbsoluteTracking);
				Vector3 v3=rt.pos;
				state.position[0]= v3.x;
				state.position[1]= v3.y;
				state.position[2]=-v3.z;
				Quaternion q=rt.rot;
				state.rotation[0]=-q.x;
				state.rotation[1]=-q.y;
				state.rotation[2]= q.z;
				state.rotation[3]= q.w;
				//
			}
			return 0;
		}

	public virtual void OnNewPoses(TrackedDevicePose_t[] poses)
	{
		m_UseOnNewPoses=true;
		if (unObjectId == -1)
			return;

		var i = (int)unObjectId;

		isValid = false;
		if (poses.Length <= i)
			return;

		if (!poses[i].bDeviceIsConnected)
			return;

		if (!poses[i].bPoseIsValid)
			return;

		isValid = true;

		m_Pose = poses[i];
	}

#endif

	}

	public class OpenVRControllerManager:MonoBehaviour,IInputModule {
#if OPENVR_SDK

		public static void OnNewPoses(TrackedDevicePose_t[] poses) {
			for(int i=0,imax=s_Controllers.Length;i<imax;++i) {
				if(s_Controllers[i]!=null) {
					s_Controllers[i].OnNewPoses(poses);
				}
			}
		}

#endif

		public static OpenVRController[] s_Controllers=new OpenVRController[2];

		public virtual int InitInput() {
#if OPENVR_SDK
			s_Controllers[0]=new OpenVRController("OpenVRController-0",1);
			s_Controllers[1]=new OpenVRController("OpenVRController-1",2);
			return 0;
#else
			return -1;
#endif
		}

		public virtual int ExitInput() {
			return 0;
		}

		public virtual int UpdateInput() {
			return 0;
		}
	}
}
