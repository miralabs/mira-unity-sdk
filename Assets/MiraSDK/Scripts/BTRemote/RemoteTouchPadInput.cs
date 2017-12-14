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

public class RemoteTouchPadInput: RemoteTouchInput
{
    // Events
    public delegate void RemoteTouchPadInputEventHandler(RemoteTouchPadInput touchPadInput, EventArgs e);
    public event RemoteTouchPadInputEventHandler OnValueChanged;

    public RemoteButtonInput button { get; private set; }

    public RemoteButtonInput touchActive { get; private set; }

    public RemoteAxisInput xAxis { get; private set; }
    public RemoteAxisInput yAxis { get; private set; }

    public RemoteTouchInput up { get; private set; }
    public RemoteTouchInput down { get; private set; }
    public RemoteTouchInput left { get; private set; }
    public RemoteTouchInput right { get; private set; }

    internal RemoteTouchPadInput()
    {
        this.button = new RemoteButtonInput();
        this.touchActive = new RemoteButtonInput();

        this.xAxis = new RemoteAxisInput();
        this.yAxis = new RemoteAxisInput();

        this.up = new RemoteTouchInput();
        this.down = new RemoteTouchInput();
        this.left = new RemoteTouchInput();
        this.right = new RemoteTouchInput();
    }

    public override bool isActive
    {
        get
        {
            return base.isActive;
        }
        internal set
        {
            base.isActive = value;

            if (OnValueChanged != null)
            {
                // Call this even if isActive didn't change, since other properties might have changed.
                OnValueChanged(this, EventArgs.Empty);
            }
        }
    }
}