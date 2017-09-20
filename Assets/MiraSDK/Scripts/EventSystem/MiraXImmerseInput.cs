using UnityEngine;
using Ximmerse.InputSystem;

#if UNITY_EDITOR

using UnityEditor;

#endif

/// <summary>
/// MiraXImmerseInput extends MiraUserInput and calls the XImmerse spesific SDK functions
/// it's used by MiraController if the controllerType is XImmerse
/// </summary>
public class MiraXImmerseInput : MiraUserInput
{
    private ControllerInput controller;

    public bool init()
    {
#if UNITY_EDITOR
        if (EditorUserBuildSettings.activeBuildTarget == BuildTarget.StandaloneWindows ||
           EditorUserBuildSettings.activeBuildTarget == BuildTarget.StandaloneWindows64 ||
           EditorUserBuildSettings.activeBuildTarget == BuildTarget.StandaloneOSXUniversal ||
           EditorUserBuildSettings.activeBuildTarget == BuildTarget.StandaloneOSXIntel ||
           EditorUserBuildSettings.activeBuildTarget == BuildTarget.StandaloneOSXIntel64 ||
           EditorUserBuildSettings.activeBuildTarget == BuildTarget.StandaloneLinuxUniversal ||
           EditorUserBuildSettings.activeBuildTarget == BuildTarget.StandaloneLinux64 ||
           EditorUserBuildSettings.activeBuildTarget == BuildTarget.StandaloneLinux64)
        {
            controller = ControllerInputManager.instance.GetControllerInput(ControllerType.LeftController);
        }
        else
        {
            Debug.Log("Attention: the XImmerse controller requires your Active Build Target to be PC Mac & Linux Standalone");
            Debug.Log("Attention: if you wish to test the controller in the editor please switch your platform in Build Settings");
            return false;
        }
#else
		controller = ControllerInputManager.instance.GetControllerInput(ControllerType.LeftController);
#endif
        if (controller != null)
        {
            return true;
        }
        else
        {
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
            return controller != null ? controller.GetPosition() : Vector3.zero;
        }
    }

    public Quaternion Orientation
    {
        get
        {
            return controller != null ? controller.GetRotation() : Quaternion.identity;
        }
    }

    public Vector3 Gyro
    {
        get
        {
            return controller != null ? controller.GetGyroscope() : Vector3.zero;
        }
    }

    public Vector3 Accel
    {
        get
        {
            return controller != null ? controller.GetAccelerometer() : Vector3.zero;
        }
    }

    public bool TouchHeld
    {
        get
        {
            return controller != null ? controller.GetButton(ControllerRawButton.LeftThumbMove) : false;
        }
    }

    public bool TouchReleased
    {
        get
        {
            return controller != null ? controller.GetButtonUp(ControllerRawButton.LeftThumbMove) : false;
        }
    }

    public bool TouchPressed
    {
        get
        {
            return controller != null ? controller.GetButtonDown(ControllerRawButton.LeftThumbMove) : false;
        }
    }

    public Vector2 TouchPos
    {
        get
        {
            return controller != null ? controller.touchPos : Vector2.zero;
        }
    }

    public bool StartButton
    {
        get
        {
            return controller != null ? controller.GetButton(ControllerButton.Start) : false;
        }
    }

    public bool StartButtonReleased
    {
        get
        {
            return controller != null ? controller.GetButtonUp(ControllerButton.Start) : false;
        }
    }

    public bool StartButtonPressed
    {
        get
        {
            return controller != null ? controller.GetButtonDown(ControllerButton.Start) : false;
        }
    }

    public bool UpButton
    {
        get
        {
            return controller != null ? controller.GetButton(ControllerButton.DpadUp) : false;
        }
    }

    public bool UpButtonReleased
    {
        get
        {
            return controller != null ? controller.GetButtonUp(ControllerButton.DpadUp) : false;
        }
    }

    public bool UpButtonPressed
    {
        get
        {
            return controller != null ? controller.GetButtonDown(ControllerButton.DpadUp) : false;
        }
    }

    public bool DownButton
    {
        get
        {
            return controller != null ? controller.GetButton(ControllerButton.DpadDown) : false;
        }
    }

    public bool DownButtonReleased
    {
        get
        {
            return controller != null ? controller.GetButtonUp(ControllerButton.DpadDown) : false;
        }
    }

    public bool DownButtonPressed
    {
        get
        {
            return controller != null ? controller.GetButtonDown(ControllerButton.DpadDown) : false;
        }
    }

    public bool LeftButton
    {
        get
        {
            return controller != null ? controller.GetButton(ControllerButton.DpadLeft) : false;
        }
    }

    public bool LeftButtonReleased
    {
        get
        {
            return controller != null ? controller.GetButtonUp(ControllerButton.DpadLeft) : false;
        }
    }

    public bool LeftButtonPressed
    {
        get
        {
            return controller != null ? controller.GetButtonDown(ControllerButton.DpadLeft) : false;
        }
    }

    public bool RightButton
    {
        get
        {
            return controller != null ? controller.GetButton(ControllerButton.DpadRight) : false;
        }
    }

    public bool RightButtonReleased
    {
        get
        {
            return controller != null ? controller.GetButtonUp(ControllerButton.DpadRight) : false;
        }
    }

    public bool RightButtonPressed
    {
        get
        {
            return controller != null ? controller.GetButtonDown(ControllerButton.DpadRight) : false;
        }
    }

    public bool TouchpadButton
    {
        get
        {
            if (controller != null)
            {
                if (controller.GetButton(ControllerButton.PrimaryThumb) ||
                    controller.GetButton(ControllerButton.DpadUp) ||
                    controller.GetButton(ControllerButton.DpadDown) ||
                    controller.GetButton(ControllerButton.DpadLeft) ||
                    controller.GetButton(ControllerButton.DpadRight))
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
                if (controller.GetButtonUp(ControllerButton.PrimaryThumb) ||
                    controller.GetButtonUp(ControllerButton.DpadUp) ||
                    controller.GetButtonUp(ControllerButton.DpadDown) ||
                    controller.GetButtonUp(ControllerButton.DpadLeft) ||
                    controller.GetButtonUp(ControllerButton.DpadRight))
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
                if (controller.GetButtonDown(ControllerButton.PrimaryThumb) ||
                    controller.GetButtonDown(ControllerButton.DpadUp) ||
                    controller.GetButtonDown(ControllerButton.DpadDown) ||
                    controller.GetButtonDown(ControllerButton.DpadLeft) ||
                    controller.GetButtonDown(ControllerButton.DpadRight))
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
            return controller != null ? controller.GetButton(ControllerButton.PrimaryTrigger) : false;
        }
    }

    public bool TriggerButtonReleased
    {
        get
        {
            return controller != null ? controller.GetButtonUp(ControllerButton.PrimaryTrigger) : false;
        }
    }

    public bool TriggerButtonPressed
    {
        get
        {
            return controller != null ? controller.GetButtonDown(ControllerButton.PrimaryTrigger) : false;
        }
    }

    public bool BackButton
    {
        get
        {
            return controller != null ? controller.GetButton(ControllerButton.Back) : false;
        }
    }

    public bool BackButtonReleased
    {
        get
        {
            return controller != null ? controller.GetButtonUp(ControllerButton.Back) : false;
        }
    }

    public bool BackButtonPressed
    {
        get
        {
            return controller != null ? controller.GetButtonDown(ControllerButton.Back) : false;
        }
    }
}