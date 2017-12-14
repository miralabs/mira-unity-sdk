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

public class RemoteOrientationInput
{
    // Events
    public delegate void RemoteOrientationInputEventHandler(RemoteOrientationInput orientationInput, EventArgs e);
    public event RemoteOrientationInputEventHandler OnValueChanged;

    public float pitch { get; private set; }
    public float yaw { get; private set; }
    public float roll { get; private set; }

    internal RemoteOrientationInput()
    {
    }

    internal void setOrientationValues(float pitch, float yaw, float roll)
    {
        // Update all at once to prevent multiple events.
        this.pitch = pitch;
        this.yaw = yaw;
        this.roll = roll;

        if (OnValueChanged != null)
        {
            OnValueChanged(this, EventArgs.Empty);
        }
    }

    public void setOrientationVector(Vector3 pyr)
    {
        // pyr = pitch yall roll
        this.pitch = pyr.x;
        this.yaw = pyr.y;
        this.roll = pyr.z;
    }

    public Vector3 getOrientationVector()
    {
        return new Vector3(this.pitch, this.yaw, this.roll);
    }
}