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

public class Remote : RemoteBase
{
    public delegate void RemoteRefreshedEventHandler(Remote remote, EventArgs e);
    public event RemoteRefreshedEventHandler OnRefresh;
    
    // NOTE: Make sure to update Reset() and UpdateWithRemote() when changing Remote's properties.
	public Guid identifier;

	public string name { get; internal set; }
	public string productName { get; internal set; }
	public string serialNumber { get; internal set; }
	public string hardwareIdentifier { get; internal set; }
	public string firmwareVersion { get; internal set; }

	public float? batteryPercentage { get; internal set; }
	public int? rssi { get; internal set; }

	public bool isConnected { get; internal set; }
    public bool isPreferred { get; internal set; }

	
	internal Remote(string name, Guid identifier)
    {
		this.name = name;
		this.identifier = identifier;

		this.menuButton = new RemoteButtonInput();
		this.homeButton = new RemoteButtonInput();
		this.trigger = new RemoteButtonInput();

		this.touchPad = new RemoteTouchPadInput();

		this.motion = new RemoteMotionInput();

		/* Inputs */
        
		// Buttons
		NativeBridge.RemoteButtonInputAddListener(this, "menuButton", (pressed) => {
			this.menuButton.isPressed = pressed;
		});
		NativeBridge.RemoteButtonInputAddListener(this, "homeButton", (pressed) => {
            this.homeButton.isPressed = pressed;
        });
		NativeBridge.RemoteButtonInputAddListener(this, "trigger", (pressed) => {
            this.trigger.isPressed = pressed;
        });

		// Touch Pad
		NativeBridge.RemoteTouchPadInputAddListener(this, "touchPad", (active) => {
			this.touchPad.isActive = active;
		});
        // TODO: Determine if code above is still necessary
        NativeBridge.RemoteTouchPadInputAddListener(this, "touchPad", (active) => {
			this.touchPad.touchActive.isPressed = active;
		});
		NativeBridge.RemoteButtonInputAddListener(this, "touchPad.button", (pressed) => {
            this.touchPad.button.isPressed = pressed;
        });
		NativeBridge.RemoteAxisInputAddListener(this, "touchPad.xAxis", (value) => {
			this.touchPad.xAxis.value = value;
		});
		NativeBridge.RemoteAxisInputAddListener(this, "touchPad.yAxis", (value) => {
            this.touchPad.yAxis.value = value;
        });
		NativeBridge.RemoteTouchInputAddListener(this, "touchPad.up", (active) => {
			this.touchPad.up.isActive = active;
		});
		NativeBridge.RemoteTouchInputAddListener(this, "touchPad.down", (active) => {
            this.touchPad.down.isActive = active;
        });
		NativeBridge.RemoteTouchInputAddListener(this, "touchPad.left", (active) => {
            this.touchPad.left.isActive = active;
        });
		NativeBridge.RemoteTouchInputAddListener(this, "touchPad.right", (active) => {
            this.touchPad.right.isActive = active;
        });

		// Motion
		NativeBridge.RemoteMotionInputAddListener(this, "motion", () => {
			this.motion.update();
		});
		NativeBridge.RemoteMotionSensorInputAddListener(this, "motion.acceleration", (x, y, z) => {
            this.motion.acceleration.setMotionSensorValues(x, y, z);
        });
		NativeBridge.RemoteMotionSensorInputAddListener(this, "motion.rotationRate", (x, y, z) => {
            this.motion.rotationRate.setMotionSensorValues(x, y, z);
        });
		NativeBridge.RemoteOrientationInputAddListener(this, "motion.orientation", (pitch, yaw, roll) => {
            this.motion.orientation.setOrientationValues(pitch, yaw, roll);
        });

        /* Notifications */
        NativeBridge.RegisterForRemoteDidRefreshNotification((r) => {
            if (r.identifier != this.identifier)
            {
                return;
            }

            this.UpdateWithRemote(r);
        });
    }

	public void Refresh(Action<MiraRemoteException> action)
	{
		NativeBridge.RemoteRefresh(this, action);
	}

    internal void UpdateWithRemote(Remote remote)
    {
        this.name = remote.name;
        this.identifier = remote.identifier;
        this.productName = remote.productName;
        this.serialNumber = remote.serialNumber;
        this.hardwareIdentifier = remote.hardwareIdentifier;
        this.firmwareVersion = remote.firmwareVersion;
        this.batteryPercentage = remote.batteryPercentage;
        this.rssi = remote.rssi;
        this.isConnected = remote.isConnected;
        this.isPreferred = remote.isPreferred;

        if (OnRefresh != null)
        {
            OnRefresh(this, EventArgs.Empty);
        }
    }

    internal void Reset()
    {
        // Intentionally don't reset name and identifier.
        // this.name = null;
        // this.identifier = null;

        this.productName = null;
        this.serialNumber = null;
        this.hardwareIdentifier = null;
        this.firmwareVersion = null;
        this.batteryPercentage = null;
        this.rssi = null;
        this.isConnected = false;
        this.isPreferred = false;
    }
}
