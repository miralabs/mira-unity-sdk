//=============================================================================
//
// Copyright 2016 Ximmerse, LTD. All rights reserved.
//
//=============================================================================

namespace Ximmerse.InputSystem {

	public partial class XDevicePlugin {

        public const int
            XIM_DK4       = 0x4000,
            XIM_DK4_DIS01 = 0x4001, 
            XIM_CV1       = 0x4010,
            XIM_CV1_HTC   = 0x4011,
            XIM_CV1_SAM   = 0x4012;


        public const int
			ID_CONTEXT=0xFF
		;

		public const int
			// Bool
			kField_CtxCanProcessInputEventBool = 0,
			kField_IsAbsRotationBool           =10,
            kField_IsDeviceSleepBool           =11,
            // Int
            kField_CtxSdkVersionInt            = 0,
			kField_CtxDeviceVersionInt         = 1,
			kField_CtxPlatformVersionInt       = 2,
            kField_CtxLogLevelInt              = 3,
			kField_ErrorCodeInt                = 2,
			kField_ConnectionStateInt          = 3,
			kField_BatteryLevelInt             = 4,
			kField_TrackingResultInt           = 5,
			kField_TrackingOriginInt           = 6,
            kField_FwVersionInt                = 7,
            kField_BlobIDInt                   = 8,
            // Float
        kField_PositionScaleFloat          = 0,
			kField_TrackerHeightFloat          = 1,
			kField_TrackerDepthFloat           = 2,
			kField_TrackerPitchFloat           = 3,
			// Object
			kField_TRSObject                   = 0,
			kField_DeviceInfoObject            = 1,
			kField_AddressObject               = 2,
			kField_DisplayNameObject           = 3,
			kField_ModelNameObject             = 4,
			kField_SerialNumberObject          = 5,
			kField_FirmwareRevisionObject      = 6,
			kField_HardwareRevisionObject      = 7,
			kField_SoftwareRevisionObject      = 8,
			kField_ManufacturerNameObject      = 9,
			// Message
			kMessage_TriggerVibration          = 1,
			kMessage_RecenterSensor            = 2,
			kMessage_SleepMode                 = 3,
			kMessage_UpdateDisplayStrings      = 4,
			kMessage_DeviceOperation           = 5,
            kMessage_ChangeBlobColorTemp       = 6,
            kMessage_ChangeBlobColorDefault    = 7,
            kMessage_SetDeviceName             = 8,
            kMessage_SetModelName              = 9,
            //
			OK                                 = 0
		;

        public const int
            FEATURE_BLOB_AUTO = 0x0001,
            FEATURE_POSITION_PREDICTION = 0x0002
            ;

    }

    public enum LOGLevel
    {
        LOG_VERBOSE = 0,
        LOG_DEBUG = 1,
        LOG_INFO = 2,
        LOG_WARN = 3,
        LOG_ERROR = 4,
        LOG_OFF = 5,
    };

    // Reference from GoogleVR.

    /// <summary>
    /// Represents the device's current connection state.
    /// </summary>
    public enum DeviceConnectionState {
		/// <summary>
		/// Indicates that the device is disconnected.
		/// </summary>
		Disconnected,
		/// <summary>
		/// Indicates that the host is scanning for devices.
		/// </summary>
		Scanning,
		/// <summary>
		/// Indicates that the device is connecting.
		/// </summary>
		Connecting,
		/// <summary>
		/// Indicates that the device is connected.
		/// </summary>
		Connected,
		/// <summary>
		/// Indicates that an error has occurred.
		/// </summary>
		Error,
	};

	[System.Flags]
	public enum TrackingResult{
		NotTracked      =    0,
		RotationTracked = 1<<0,
		PositionTracked = 1<<1,
		PoseTracked     = (RotationTracked|PositionTracked),
		RotationEmulated = 1<<2,
		PositionEmulated = 1<<3,
	};

	public enum XimmerseButton {
		// Standard
		Touch   = ControllerRawButton.LeftThumbMove,
		Click   = DpadClick|DpadUp|DpadDown|DpadLeft|DpadRight,
		App     = ControllerRawButton.Back,
		Home    = ControllerRawButton.Start,
		// Touchpad To Dpad
		DpadUp    = ControllerRawButton.DpadUp,
		DpadDown  = ControllerRawButton.DpadDown,
		DpadLeft  = ControllerRawButton.DpadLeft,
		DpadRight = ControllerRawButton.DpadRight,
		DpadClick = ControllerRawButton.LeftThumb,
		// Gestures
		SwipeUp     = ControllerRawButton.LeftThumbUp,
		SwipeDown   = ControllerRawButton.LeftThumbDown,
		SwipeLeft   = ControllerRawButton.LeftThumbLeft,
		SwipeRight  = ControllerRawButton.LeftThumbRight,
		SlashUp    = ControllerRawButton.RightThumbUp,
		SlashDown  = ControllerRawButton.RightThumbDown,
		SlashLeft  = ControllerRawButton.RightThumbLeft,
		SlashRight = ControllerRawButton.RightThumbRight,
		// Ximmerse Ex
		Trigger = ControllerRawButton.LeftTrigger,
		GripL   = ControllerRawButton.LeftShoulder,
		GripR   = ControllerRawButton.RightShoulder,
		Grip    = GripL|GripR,
	}

	public static class XimmerseExtension {

		public static bool GetButton(this ControllerInput thiz,XimmerseButton  buttonMask) {
			if(thiz==null) {
				return false;
			}
			return thiz.GetButton((uint)buttonMask);
		}

		public static bool GetButtonDown(this ControllerInput thiz,XimmerseButton  buttonMask) {
			if(thiz==null) {
				return false;
			}
			return thiz.GetButtonDown((uint)buttonMask);
		}

		public static bool GetButtonUp(this ControllerInput thiz,XimmerseButton  buttonMask) {
			if(thiz==null) {
				return false;
			}
			return thiz.GetButtonUp((uint)buttonMask);
		}
	}
	
    public enum DeviceOperation {
		Close,
		Open,
	}
}
