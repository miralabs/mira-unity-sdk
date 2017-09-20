//=============================================================================
//
// Copyright 2016 Ximmerse, LTD. All rights reserved.
//
//=============================================================================

using System.Runtime.InteropServices;
using UnityEngine;

namespace Ximmerse.VR{

	public class OVRDevice:VRDevice {

		#region Nested Types

		public enum ovrBool
		{
			False = 0,
			True
		}

		public enum ovrNode
		{
			None           = -1,
			EyeLeft        = 0,
			EyeRight       = 1,
			EyeCenter      = 2,
			HandLeft       = 3,
			HandRight      = 4,
			TrackerZero    = 5,
			TrackerOne     = 6,
			TrackerTwo     = 7,
			TrackerThree   = 8,
			Head           = 9,
			Count,
		}

		public enum ovrStep
		{
			Render = -1,
			Physics = 0,
		}

		public enum ovrTrackingOrigin
		{
			EyeLevel       = 0,
			FloorLevel     = 1,
			Count,
		}

		[StructLayout(LayoutKind.Sequential)]
		public struct ovrVector3f
		{
			public float x;
			public float y;
			public float z;
		}

		[StructLayout(LayoutKind.Sequential)]
		public struct ovrQuatf
		{
			public float x;
			public float y;
			public float z;
			public float w;
		}

		[StructLayout(LayoutKind.Sequential)]
		public struct ovrPosef
		{
			public ovrQuatf Orientation;
			public ovrVector3f Position;
		}

		[StructLayout(LayoutKind.Sequential)]
		public struct ovrPoseStatef
		{
			public ovrPosef Pose;
			public ovrVector3f Velocity;
			public ovrVector3f Acceleration;
			public ovrVector3f AngularVelocity;
			public ovrVector3f AngularAcceleration;
			double Time;
		}

		private static class OVRP_0_1_0
		{
			public static readonly System.Version version = new System.Version(0, 1, 0);
		}

		private static class OVRP_0_1_2
		{
			public static readonly System.Version version = new System.Version(0, 1, 2);

			[DllImport(pluginName, CallingConvention = CallingConvention.Cdecl)]
			public static extern ovrPosef ovrp_GetNodePose(ovrNode nodeId);

			[DllImport(pluginName, CallingConvention = CallingConvention.Cdecl)]
			public static extern ovrBool ovrp_SetControllerVibration(uint controllerMask, float frequency, float amplitude);
		}

		private static class OVRP_0_1_3
		{
			public static readonly System.Version version = new System.Version(0, 1, 3);

			[DllImport(pluginName, CallingConvention = CallingConvention.Cdecl)]
			public static extern ovrPosef ovrp_GetNodeVelocity(ovrNode nodeId);

			[DllImport(pluginName, CallingConvention = CallingConvention.Cdecl)]
			public static extern ovrPosef ovrp_GetNodeAcceleration(ovrNode nodeId);
		}

		private static class OVRP_0_5_0
		{
			public static readonly System.Version version = new System.Version(0, 5, 0);
		}

		private static class OVRP_1_0_0
		{
			public static readonly System.Version version = new System.Version(1, 0, 0);

			[DllImport(pluginName, CallingConvention = CallingConvention.Cdecl)]
			public static extern ovrTrackingOrigin ovrp_GetTrackingOriginType();

			[DllImport(pluginName, CallingConvention = CallingConvention.Cdecl)]
			public static extern ovrBool ovrp_SetTrackingOriginType(ovrTrackingOrigin originType);

			[DllImport(pluginName, CallingConvention = CallingConvention.Cdecl)]
			public static extern ovrPosef ovrp_GetTrackingCalibratedOrigin();

			[DllImport(pluginName, CallingConvention = CallingConvention.Cdecl)]
			public static extern ovrBool ovrp_RecenterTrackingOrigin(uint flags);
		}

		private static class OVRP_1_1_0
		{
			public static readonly System.Version version = new System.Version(1, 1, 0);

			[DllImport(pluginName, CallingConvention = CallingConvention.Cdecl)]
			public static extern ovrBool ovrp_GetInitialized();

			[DllImport(pluginName, CallingConvention = CallingConvention.Cdecl, EntryPoint = "ovrp_GetVersion")]
			private static extern System.IntPtr _ovrp_GetVersion();
			public static string ovrp_GetVersion() { return Marshal.PtrToStringAnsi(_ovrp_GetVersion()); }

			[DllImport(pluginName, CallingConvention = CallingConvention.Cdecl, EntryPoint = "ovrp_GetNativeSDKVersion")]
			private static extern System.IntPtr _ovrp_GetNativeSDKVersion();
			public static string ovrp_GetNativeSDKVersion() { return Marshal.PtrToStringAnsi(_ovrp_GetNativeSDKVersion()); }

			[DllImport(pluginName, CallingConvention = CallingConvention.Cdecl)]
			public static extern ovrBool ovrp_GetTrackingOrientationSupported();

			[DllImport(pluginName, CallingConvention = CallingConvention.Cdecl)]
			public static extern ovrBool ovrp_GetTrackingOrientationEnabled();

			[DllImport(pluginName, CallingConvention = CallingConvention.Cdecl)]
			public static extern ovrBool ovrp_SetTrackingOrientationEnabled(ovrBool value);

			[DllImport(pluginName, CallingConvention = CallingConvention.Cdecl)]
			public static extern ovrBool ovrp_GetTrackingPositionSupported();

			[DllImport(pluginName, CallingConvention = CallingConvention.Cdecl)]
			public static extern ovrBool ovrp_GetTrackingPositionEnabled();

			[DllImport(pluginName, CallingConvention = CallingConvention.Cdecl)]
			public static extern ovrBool ovrp_SetTrackingPositionEnabled(ovrBool value);

			[DllImport(pluginName, CallingConvention = CallingConvention.Cdecl)]
			public static extern ovrBool ovrp_GetNodePresent(ovrNode nodeId);

			[DllImport(pluginName, CallingConvention = CallingConvention.Cdecl)]
			public static extern ovrBool ovrp_GetNodeOrientationTracked(ovrNode nodeId);

			[DllImport(pluginName, CallingConvention = CallingConvention.Cdecl)]
			public static extern ovrBool ovrp_GetNodePositionTracked(ovrNode nodeId);

			[DllImport(pluginName, CallingConvention = CallingConvention.Cdecl)]
			public static extern ovrBool ovrp_GetUserPresent();

			[DllImport(pluginName, CallingConvention = CallingConvention.Cdecl)]
			public static extern float ovrp_GetUserIPD();

			[DllImport(pluginName, CallingConvention = CallingConvention.Cdecl)]
			public static extern ovrBool ovrp_SetUserIPD(float value);

			[DllImport(pluginName, CallingConvention = CallingConvention.Cdecl)]
			public static extern float ovrp_GetUserEyeDepth();

			[DllImport(pluginName, CallingConvention = CallingConvention.Cdecl)]
			public static extern ovrBool ovrp_SetUserEyeDepth(float value);

			[DllImport(pluginName, CallingConvention = CallingConvention.Cdecl)]
			public static extern float ovrp_GetUserEyeHeight();

			[DllImport(pluginName, CallingConvention = CallingConvention.Cdecl)]
			public static extern ovrBool ovrp_SetUserEyeHeight(float value);
		}

		private static class OVRP_1_3_0
		{
			public static readonly System.Version version = new System.Version(1, 3, 0);
		}

		private static class OVRP_1_8_0
		{
			public static readonly System.Version version = new System.Version(1, 8, 0);

			[DllImport(pluginName, CallingConvention = CallingConvention.Cdecl)]
			public static extern ovrBool ovrp_Update2(int stateId, int frameIndex, double predictionSeconds);

			[DllImport(pluginName, CallingConvention = CallingConvention.Cdecl)]
			public static extern ovrPosef ovrp_GetNodePose2(int stateId, ovrNode nodeId);

			[DllImport(pluginName, CallingConvention = CallingConvention.Cdecl)]
			public static extern ovrPosef ovrp_GetNodeVelocity2(int stateId, ovrNode nodeId);

			[DllImport(pluginName, CallingConvention = CallingConvention.Cdecl)]
			public static extern ovrPosef ovrp_GetNodeAcceleration2(int stateId, ovrNode nodeId);
		}

		private static class OVRP_1_12_0
		{
			public static readonly System.Version version = new System.Version(1, 12, 0);

			[DllImport(pluginName, CallingConvention = CallingConvention.Cdecl)]
			public static extern float ovrp_GetAppFramerate();

			[DllImport(pluginName, CallingConvention = CallingConvention.Cdecl)]
			public static extern ovrPoseStatef ovrp_GetNodePoseState(ovrStep stepId, ovrNode nodeId);
		}

		private static class OVRP_1_14_0
		{
			public static readonly System.Version version = new System.Version(1, 14, 0);
		}

		#endregion Nested Types

		#region Statics
		
		private const string pluginName = "OVRPlugin";
		private static System.Version _versionZero = new System.Version(0, 0, 0);
		public static readonly System.Version wrapperVersion = OVRP_1_14_0.version;

		private static System.Version _version;
		public static System.Version version
		{
			get {
				if (_version == null)
				{
					try
					{
						string pluginVersion = OVRP_1_1_0.ovrp_GetVersion();

						if (pluginVersion != null)
						{
							// Truncate unsupported trailing version info for System.Version. Original string is returned if not present.
							pluginVersion = pluginVersion.Split('-')[0];
							_version = new System.Version(pluginVersion);
						}
						else
						{
							_version = _versionZero;
						}
					}
					catch
					{
						_version = _versionZero;
					}

					// Unity 5.1.1f3-p3 have OVRPlugin version "0.5.0", which isn't accurate.
					if (_version == OVRP_0_5_0.version)
						_version = OVRP_0_1_0.version;

					if (_version > _versionZero && _version < OVRP_1_3_0.version)
						throw new System.PlatformNotSupportedException("Oculus Utilities version " + wrapperVersion + " is too new for OVRPlugin version " + _version.ToString () + ". Update to the latest version of Unity.");
				}

				return _version;
			}
		}

		private static ovrBool ToBool(bool b)
		{
			return (b) ? ovrBool.True : ovrBool.False;
		}

		public static ovrTrackingOrigin GetTrackingOriginType()
		{
			return OVRP_1_0_0.ovrp_GetTrackingOriginType();
		}

		public static bool SetTrackingOriginType(ovrTrackingOrigin originType)
		{
			return OVRP_1_0_0.ovrp_SetTrackingOriginType(originType) == ovrBool.True;
		}

		public static bool UpdateNodePhysicsPoses(int frameIndex, double predictionSeconds)
		{
			if (version >= OVRP_1_8_0.version)
				return OVRP_1_8_0.ovrp_Update2((int)ovrStep.Physics, frameIndex, predictionSeconds) == ovrBool.True;

			return false;
		}

		public static ovrPosef GetNodePose(ovrNode nodeId, ovrStep stepId)
		{
			if (version >= OVRP_1_12_0.version)
				return OVRP_1_12_0.ovrp_GetNodePoseState (stepId, nodeId).Pose;
		
			if (version >= OVRP_1_8_0.version && stepId == ovrStep.Physics)
				return OVRP_1_8_0.ovrp_GetNodePose2(0, nodeId);
		
			return OVRP_0_1_2.ovrp_GetNodePose(nodeId);
		}

		public static ovrVector3f GetNodeVelocity(ovrNode nodeId, ovrStep stepId)
		{
			if (version >= OVRP_1_12_0.version)
				return OVRP_1_12_0.ovrp_GetNodePoseState (stepId, nodeId).Velocity;
		
			if (version >= OVRP_1_8_0.version && stepId == ovrStep.Physics)
				return OVRP_1_8_0.ovrp_GetNodeVelocity2(0, nodeId).Position;
		
			return OVRP_0_1_3.ovrp_GetNodeVelocity(nodeId).Position;
		}

		public static ovrVector3f GetNodeAngularVelocity(ovrNode nodeId, ovrStep stepId)
		{
			if (version >= OVRP_1_12_0.version)
				return OVRP_1_12_0.ovrp_GetNodePoseState(stepId, nodeId).AngularVelocity;

			return new ovrVector3f(); //TODO: Convert legacy quat to vec3?
		}

		public static ovrVector3f GetNodeAcceleration(ovrNode nodeId, ovrStep stepId)
		{
			if (version >= OVRP_1_12_0.version)
				return OVRP_1_12_0.ovrp_GetNodePoseState (stepId, nodeId).Acceleration;
		
			if (version >= OVRP_1_8_0.version && stepId == ovrStep.Physics)
				return OVRP_1_8_0.ovrp_GetNodeAcceleration2(0, nodeId).Position;
		
			return OVRP_0_1_3.ovrp_GetNodeAcceleration(nodeId).Position;
		}

		public static ovrVector3f GetNodeAngularAcceleration(ovrNode nodeId, ovrStep stepId)
		{
			if (version >= OVRP_1_12_0.version)
				return OVRP_1_12_0.ovrp_GetNodePoseState(stepId, nodeId).AngularAcceleration;

			return new ovrVector3f(); //TODO: Convert legacy quat to vec3?
		}

		public static bool GetNodePresent(ovrNode nodeId)
		{
			return OVRP_1_1_0.ovrp_GetNodePresent(nodeId) == ovrBool.True;
		}

		public static bool GetNodeOrientationTracked(ovrNode nodeId)
		{
			return OVRP_1_1_0.ovrp_GetNodeOrientationTracked(nodeId) == ovrBool.True;
		}

		public static bool GetNodePositionTracked(ovrNode nodeId)
		{
			return OVRP_1_1_0.ovrp_GetNodePositionTracked(nodeId) == ovrBool.True;
		}

		public static bool userPresent { get { return OVRP_1_1_0.ovrp_GetUserPresent() == ovrBool.True; } }

		public static bool rotation
		{
			get { return OVRP_1_1_0.ovrp_GetTrackingOrientationEnabled() == ovrBool.True; }
			set { OVRP_1_1_0.ovrp_SetTrackingOrientationEnabled(ToBool(value)); }
		}

		public static bool position
		{
			get { return OVRP_1_1_0.ovrp_GetTrackingPositionEnabled() == ovrBool.True; }
			set { OVRP_1_1_0.ovrp_SetTrackingPositionEnabled(ToBool(value)); }
		}

		public static float eyeDepth
		{
			get { return OVRP_1_1_0.ovrp_GetUserEyeDepth(); }
			set { OVRP_1_1_0.ovrp_SetUserEyeDepth(value); }
		}

		public static float eyeHeight
		{
			get { return OVRP_1_1_0.ovrp_GetUserEyeHeight(); }
			set { OVRP_1_1_0.ovrp_SetUserEyeHeight(value); }
		}

		#endregion Statics

		#region Fields


		#endregion Fields

		#region Methods

		public override void InitDevice(VRContext context) {
			useUnityVR=true;
			UnityEngine.VR.InputTracking.Recenter();
			trackingOriginType=TrackingOrigin.EyeLevel;
			SetTrackingOriginType(ovrTrackingOrigin.EyeLevel);
			//
			neckToEye.Set(0.0f,0.075f,eyeDepth);
			position=false;
			//
			base.InitDevice(context);
		}

		public override void OnVRContextInited(VRContext context) {
			base.OnVRContextInited(context);
		}

		public override Quaternion GetRotation() {
			UpdateNodePhysicsPoses(0,0d);
			var pose=GetNodePose(ovrNode.EyeCenter,ovrStep.Physics);
			return new Quaternion(-pose.Orientation.x,-pose.Orientation.y,pose.Orientation.z,pose.Orientation.w);
		}

	    public override bool isUserPresent
	    {
		    get {
               return userPresent;
		    }
	    }

		#endregion Methods

	}

}