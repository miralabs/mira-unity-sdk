// Copyright (c) Mira Labs, Inc., 2017. All rights reserved.
//
// Downloading and/or using this MIRA SDK is under license from MIRA,
// and subject to all terms and conditions of the Mira SDK License Agreement,
// found here: https://www.mirareality.com/Mira_SDK_License_Agreement.pdf
//
// By downloading this SDK, you agree to the Mira SDK License Agreement.
//
// This SDK may only be used in connection with the development of
// applications that are exclusively created for, and exclusively available
// for use with, MIRA hardware devices. This SDK may only be commercialized
// in the U.S. and Canada, subject to the terms of the License.

using UnityEngine;


#if UNITY_EDITOR

using UnityEditor;

#endif

/// <summary>
/// MiraBTRemoteInput extends MiraUserInput and calls the BTController spesific SDK functions
/// it's used by MiraController if the controllerType is PrismRemote
/// </summary>
public class MiraBTRemoteInput
{
    //private ControllerInput controller;
	private RemoteBase controller;

    private VirtualRemote _virtualRemote = null;
    public VirtualRemote virtualRemote { get {return _virtualRemote; } }

    private Remote _connectedRemote;
    public Remote connectedRemote;

    public bool InitVirtual()
    {
		if (controller == null)
        {
            _virtualRemote = new VirtualRemote();
            controller = _virtualRemote;
            // Initialize with zero values to avoid nulls
            _virtualRemote.UpdateVirtualMotion(new Utils.serializableBTRemote(Vector3.zero, Vector3.zero, Vector3.zero));
            _virtualRemote.UpdateVirtualButtons(new Utils.serializableBTRemoteButtons(false, false, false));
            _virtualRemote.UpdateVirtualTouchpad(new Utils.serializableBTRemoteTouchPad(false, false, Vector2.zero, false, false, false, false));
            
            return true;
        }
        else
        {
            Debug.Log("Controller already initialized");
            return false;
        }

    }

    public bool init()
    {

		controller =  RemoteManager.Instance.connectedRemote;
        

		if (controller != null)
        {
            Debug.Log("Controller Initialized");
            return true;
        }
        else
        {
            Debug.Log("Controller Initialization Failed");
            return false;
        }
    }

    /// <summary>
    /// You should call MiraController.Transform directly, this controller does not provide a position
    /// </summary>
    /// <value> always returns null</value>
    public Transform Transform
    {
        get
        {
            return null;
        }
    }

    public Vector3 Position
    {
        get
        {

	return controller != null ? Vector3.zero : Vector3.zero;
		}
    }

    public Quaternion Orientation
    {
        get
        {
            //change to motion.orientation
            // Debug.Log("Getting Orientation, controller = " + controller + " name = " + controller.name + "values =" + controller.motion.rotationRate.x);
			return controller != null ? Quaternion.Euler (new Vector3 (-controller.motion.orientation.pitch,controller.motion.orientation.yaw,controller.motion.orientation.roll)) : Quaternion.identity;
        }
    }

    public Vector3 Gyro
    {
        get
        {
            //gyro 
			return controller != null ? new Vector3 (-controller.motion.orientation.pitch,controller.motion.orientation.yaw,controller.motion.orientation.roll) : Vector3.zero; // not sure
        }
    }

    public Vector3 Accel
    {
        get
        {
			return controller != null ? new Vector3 (controller.motion.acceleration.x, controller.motion.acceleration.y, controller.motion.acceleration.z): Vector3.zero;
        }
    }

    public bool TouchHeld
    {
        get
        {
			return controller != null ? controller.touchPad.touchActive.isPressed : false;
        }
    }

    public bool TouchReleased
    {
        get
        {
			return controller != null ? controller.touchPad.touchActive.OnReleased : false;
        }
    }

    public bool TouchPressed
    {
        get
        {
			return controller != null ?  controller.touchPad.touchActive.OnPressed : false;
        }
    }

    public Vector2 TouchPos
    {
        get
        {
			return controller != null ? new Vector2 ((float)controller.touchPad.xAxis.value,(float)controller.touchPad.yAxis.value) : Vector2.zero;
        }
    }

    public bool StartButton
    {
        get
        {
			return controller != null ? controller.homeButton.isPressed : false;
          
        }
    }

    public bool StartButtonReleased
    {
        get
        {
			return controller != null ? controller.homeButton.OnReleased : false;
               
        }
    }

    public bool StartButtonPressed
    {
        get
        {
			return controller != null ? controller.homeButton.OnPressed : false;
           
        }
    }

    public bool UpButton
    {
        get
        {
			return controller != null ? controller.touchPad.up.isActive && controller.touchPad.button.isPressed : false;
        }
    }

    public bool UpButtonReleased
    {
        get
        {
			return controller != null ? controller.touchPad.up.isActive  && controller.touchPad.button.OnReleased : false;
        }
    }

    public bool UpButtonPressed
    {
        get
        {
			return controller != null ? controller.touchPad.up.isActive  && controller.touchPad.button.OnPressed: false;
        }
    }

    public bool DownButton
    {
        get
        {
			return controller != null ? controller.touchPad.down.isActive  && controller.touchPad.button.isPressed: false;
        }
    }

    public bool DownButtonReleased
    {
        get
        {
			return controller != null ? controller.touchPad.down.isActive  && controller.touchPad.button.OnReleased : false;
        }
    }

    public bool DownButtonPressed
    {
        get
        {
			return controller != null ? controller.touchPad.down.isActive  && controller.touchPad.button.OnPressed : false;
        }
    }

    public bool LeftButton
    {
        get
        {
			return controller != null ? controller.touchPad.left.isActive  && controller.touchPad.button.isPressed : false;
        }
    }

    public bool LeftButtonReleased
    {
        get
        {
			return controller != null ? controller.touchPad.left.isActive  && controller.touchPad.button.OnReleased  : false;
        }
    }

    public bool LeftButtonPressed
    {
        get
        {
			return controller != null ? controller.touchPad.left.isActive  && controller.touchPad.button.OnPressed : false;
        }
    }

    public bool RightButton
    {
        get
        {
			return controller != null ? controller.touchPad.right.isActive  && controller.touchPad.button.isPressed : false;
        }
    }

    public bool RightButtonReleased
    {
        get
        {
			return controller != null ? controller.touchPad.right.isActive  && controller.touchPad.button.OnReleased : false;
        }
    }

    public bool RightButtonPressed
    {
        get
        {
			return controller != null ? controller.touchPad.right.isActive  && controller.touchPad.button.OnPressed : false;
        }
    }

    public bool TouchpadButton
    {
        get
        {
            if (controller != null)
            {
				if (controller.touchPad.button.OnHeld)
                {
                    return true;
                }
                else
                {
                    return false;
                }
            }
            else
            {
                return false;
            }
        }
    }

    public bool TouchpadButtonReleased
    {
        get
        {
            if (controller != null)
            {
				if (controller.touchPad.button.OnReleased )
                {
                    return true;
                }
                else
                {
                    return false;
                }
            }
            else
            {
                return false;
            }
        }
    }

    public bool TouchpadButtonPressed
    {
        get
        {
            if (controller != null)
            {
				if (controller.touchPad.button.OnPressed)
                {
                    return true;
                }
                else
                {
                    return false;
                }
            }
            else
            {
                return false;
            }
        }
    }

    public bool TriggerButton
    {
        get
        {
			return controller != null ? controller.trigger.isPressed : false;
        }
    }

    public bool TriggerButtonReleased
    {
        get
        {
			return controller != null ? controller.trigger.OnReleased : false;
        }
    }

    public bool TriggerButtonPressed
    {
        get
        {
			return controller != null ? controller.trigger.OnPressed : false;
        }
    }

    public bool BackButton
    {
        get
        {
			return controller != null ? controller.menuButton.isPressed : false;
            
        }
    }

    public bool BackButtonReleased
    {
        get
        {
			return controller != null ? controller.menuButton.OnReleased : false;
        }
             
    }

    public bool BackButtonPressed
    {
        get
        {
			return controller != null ? controller.menuButton.OnPressed : false;
           
        }
    }



    public void OnControllerDisconnected()
    {
            controller.menuButton.OnDisconnected();
            controller.trigger.OnDisconnected();
            controller.touchPad.button.OnDisconnected();
            controller.homeButton.OnDisconnected();
    }
}