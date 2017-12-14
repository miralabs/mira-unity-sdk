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

public class RemoteMotionSensorInput
{
    // Events
    public delegate void RemoteMotionSensorInputEventHandler(RemoteMotionSensorInput motionSensorInput, EventArgs e);
    public event RemoteMotionSensorInputEventHandler OnValueChanged;

    public float x { get; private set; }
    public float y { get; private set; }
    public float z { get; private set; }

    internal RemoteMotionSensorInput()
    {
    }

    internal void setMotionSensorValues(float x, float y, float z)
    {
        // Update all at once to prevent multiple events.
        this.x = x;
        this.y = y;
        this.z = z;

        if (OnValueChanged != null)
        {
            OnValueChanged(this, EventArgs.Empty);
        }
    }
    public void setMotionSensorVector(Vector3 sensorVal)
    {
        this.x = sensorVal.x;
        this.y = sensorVal.y;
        this.z = sensorVal.z;
    }
    public Vector3 getMotionSensorVector()
    {
        return new Vector3(this.x, this.y, this.z);
    }
}