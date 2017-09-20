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

namespace Ximmerse.VR {

	[RequireComponent(typeof(Camera))]
	public class OpenVRDevice:VRDevice {

#if OPENVR_SDK
		public enum EIndex
		{
			None = -1,
			Hmd = (int)OpenVR.k_unTrackedDeviceIndex_Hmd,
			Device1,
			Device2,
			Device3,
			Device4,
			Device5,
			Device6,
			Device7,
			Device8,
			Device9,
			Device10,
			Device11,
			Device12,
			Device13,
			Device14,
			Device15
		}
#endif

		public override void InitDevice(VRContext context) {
			useUnityVR=true;
			UnityEngine.VR.InputTracking.Recenter();
#if UNITY_5_6_OR_NEWER
			UnityEngine.VR.InputTracking.disablePositionalTracking=true;
#else
#endif
			trackingOriginType=TrackingOrigin.EyeLevel;
			//
#if OPENVR_SDK
			ETrackedPropertyError e=ETrackedPropertyError.TrackedProp_Success;
			for(int i=(int)EIndex.Device1,iend=(int)EIndex.Device15;i<=iend;++i) {
				if(OpenVR.System.GetInt32TrackedDeviceProperty((uint)i,ETrackedDeviceProperty.Prop_DeviceClass_Int32,ref e)==(int)ETrackedDeviceClass.GenericTracker) {
					index=(EIndex)i;
					break;
				}
			}
#endif
			//
			base.InitDevice(context);
		}

		protected override void Update() {
			//
			m_Input=null;
			base.Update();
			//
#if UNITY_5_6_OR_NEWER
#else
			m_CenterEyeAnchor.localPosition=Vector3.zero;
#endif
		}

#if OPENVR_SDK
		public EIndex index;
		public bool isValid = false;

		private void OnNewPoses(TrackedDevicePose_t[] poses)
		{
			if (index == EIndex.None)
				return;

			var i = (int)index;

			isValid = false;
			if (poses.Length <= i)
				return;

			if (!poses[i].bDeviceIsConnected)
				return;

			if (!poses[i].bPoseIsValid)
				return;

			isValid = true;

			var pose = new SteamVR_Utils.RigidTransform(poses[i].mDeviceToAbsoluteTracking);

			//
			m_HeadPosition=pose.pos;
			yawOffset=pose.rot.eulerAngles.y;
		}
// Taken from SteamVR_UpdatePoses.cs

//#if !(UNITY_5_6)
		void Awake()
		{
			var camera = GetComponent<Camera>();
			camera.stereoTargetEye = StereoTargetEyeMask.None;
			camera.clearFlags = CameraClearFlags.SolidColor;
			camera.useOcclusionCulling = false;
			camera.cullingMask = 0;
			camera.depth = -9999;
		}
//#endif
		void OnPreCull()
		{
			var compositor = OpenVR.Compositor;
			if (compositor != null)
			{
				var render = SteamVR_Render.instance;
				compositor.GetLastPoses(render.poses, render.gamePoses);
				OpenVRControllerManager.OnNewPoses(render.poses);
				OnNewPoses(render.poses);
			}
		}
#endif

	}
}
