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
using Utils;
using UnityEngine;

public class VirtualRemote : RemoteBase
{
	internal VirtualRemote()
    {
		this.menuButton = new RemoteButtonInput();
		this.homeButton = new RemoteButtonInput();
		this.trigger = new RemoteButtonInput();

		this.touchPad = new RemoteTouchPadInput();

		this.motion = new RemoteMotionInput();

    }

    public void UpdateVirtualMotion(serializableBTRemote s_bt)
    {  
        this.motion.acceleration.setMotionSensorVector(s_bt.acceleration);
        this.motion.rotationRate.setMotionSensorVector(s_bt.rotationRate);
        this.motion.orientation.setOrientationVector(s_bt.orientation);


    }
    public void UpdateVirtualButtons(serializableBTRemoteButtons s_bt)
    {
        homeButton.isPressed = s_bt.startButton;
        menuButton.isPressed = s_bt.backButton;
        trigger.isPressed = s_bt.triggerButton;
    

    }
    public void UpdateVirtualTouchpad(serializableBTRemoteTouchPad s_bt)
    {
        this.touchPad.isActive = s_bt.touchActive;
        this.touchPad.button.isPressed = s_bt.touchButton;
        this.touchPad.xAxis.value = s_bt.touchPos.x;
        this.touchPad.yAxis.value = s_bt.touchPos.y;
        this.touchPad.up.isActive = s_bt.upButton;
        this.touchPad.down.isActive = s_bt.downButton;
        this.touchPad.left.isActive = s_bt.leftButton;
        this.touchPad.right.isActive = s_bt.rightButton;

    }


}
