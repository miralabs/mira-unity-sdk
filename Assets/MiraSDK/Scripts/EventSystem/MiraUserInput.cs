using UnityEngine;

/// <summary>
/// MiraUserInput defines possible user inputs that can be provided by any number of input devices
/// and unified into one input system by MiraController, see MiraXImmerseInput for an example implementation
/// </summary>
internal interface MiraUserInput
{
    bool init();

    Transform Transform
    {
        get;
    }

    Vector3 Position
    {
        get;
    }

    Quaternion Orientation
    {
        get;
    }

    Vector3 Gyro
    {
        get;
    }

    Vector3 Accel
    {
        get;
    }

    bool TouchHeld
    {
        get;
    }

    bool TouchReleased
    {
        get;
    }

    bool TouchPressed
    {
        get;
    }

    Vector2 TouchPos
    {
        get;
    }

    bool StartButton
    {
        get;
    }

    bool StartButtonReleased
    {
        get;
    }

    bool StartButtonPressed
    {
        get;
    }

    bool TouchpadButton
    {
        get;
    }

    bool TouchpadButtonReleased
    {
        get;
    }

    bool TouchpadButtonPressed
    {
        get;
    }

    bool TriggerButton
    {
        get;
    }

    bool TriggerButtonReleased
    {
        get;
    }

    bool TriggerButtonPressed
    {
        get;
    }

    bool BackButton
    {
        get;
    }

    bool BackButtonReleased
    {
        get;
    }

    bool BackButtonPressed
    {
        get;
    }

    bool UpButton
    {
        get;
    }

    bool UpButtonReleased
    {
        get;
    }

    bool UpButtonPressed
    {
        get;
    }

    bool DownButton
    {
        get;
    }

    bool DownButtonReleased
    {
        get;
    }

    bool DownButtonPressed
    {
        get;
    }

    bool LeftButton
    {
        get;
    }

    bool LeftButtonReleased
    {
        get;
    }

    bool LeftButtonPressed
    {
        get;
    }

    bool RightButton
    {
        get;
    }

    bool RightButtonReleased
    {
        get;
    }

    bool RightButtonPressed
    {
        get;
    }
}