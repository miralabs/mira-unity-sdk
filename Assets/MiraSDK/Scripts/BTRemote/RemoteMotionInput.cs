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

using System;
using System.Collections;
using System.Collections.Generic;
using System.Runtime.InteropServices;
using UnityEngine;
using AOT;

public class RemoteMotionInput : RemoteTouchInput
{
    // Events
    public delegate void RemoteMotionInputEventHandler(RemoteMotionInput motionInput, EventArgs e);
    public event RemoteMotionInputEventHandler OnValueChanged;

    public RemoteOrientationInput orientation { get; private set; }
    public RemoteMotionSensorInput acceleration { get; private set; }
    public RemoteMotionSensorInput rotationRate { get; private set; }

    internal RemoteMotionInput()
    {
        this.orientation = new RemoteOrientationInput();
        
        this.acceleration = new RemoteMotionSensorInput();
        this.rotationRate = new RemoteMotionSensorInput();
    }

    internal void update()
    {
        if (OnValueChanged != null)
        {
            OnValueChanged(this, EventArgs.Empty);
        }
    }
}