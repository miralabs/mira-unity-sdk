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

public class RemoteButtonInput 
{
    // Events
    public delegate void RemoteButtonInputEventHandler(RemoteButtonInput touchInput, EventArgs e);
    //Button Events that will be trigger internally
    public event RemoteButtonInputEventHandler OnPressChanged;
    public event RemoteButtonInputEventHandler OnHeldState;
    public event RemoteButtonInputEventHandler OnPresseddState;
    public event RemoteButtonInputEventHandler OnReleasedState;

    private int m_PrevFrameCount=-1;

    private bool m_PrevState = false;

    private bool m_State = false;

    //
    private bool _isPressed = false;
    private bool _onPressed = false;
    private bool _onReleased = false;
    private bool _onHeld = false;
	// private bool _lastframe = false;

    //Button Properties
    //on Pressed detrmines in change from false to true;
    public bool OnPressed
    {
        get {
            UpdateState();
            CheckOnPressed();  
            return _onPressed; }
	}
    // value of the button in previous frame.
    // public bool LastFrame {
    //     get{ return _lastframe; }
    //     set{ _lastframe = value; }
    // }
    //On Released determines the cahnge from true to false;
    public bool OnReleased
    {
        get
        { 
        UpdateState();
        CheckOnReleased();    
        return _onReleased; }
    }
    //On Held determines if its currently pressed.
    public bool OnHeld
    {
        get { 
            UpdateState();
            return isPressed; 
            }
    }
    // the main value which gets updated from the framework.
     public bool isPressed
    {
        get
        {
            UpdateState();
            return _isPressed;
        }
        internal set
        {
			//_onReleased = false;
			//_onPressed = false;

            if (value == _isPressed)
            {
                return;
            }
			// else if(value == false)
			// {
			// 	_onReleased = true;
			// }
			// else
			// {
			// 	_onPressed = true;

			// }

            _isPressed = value;
           
       
            if (OnPressChanged != null)
            {
                OnPressChanged(this, EventArgs.Empty);
            }
        }
    }

    public virtual void UpdateState() {
			if(Time.frameCount!=m_PrevFrameCount)
            {
				m_PrevFrameCount=Time.frameCount;
				m_PrevState=m_State;
				//
				m_State = _isPressed;
			}
		}

   
    // IEnumerator CheckOnEveryFrame()
	// {
    //     yield return new WaitForEndOfFrame();
	// 	_lastframe = _isPressed;
	// }
	 
    // Compares Button Values with the last frame to identify if an event needs to be triggered
    void CheckOnPressed()
    {
        _onPressed = false;
        if(m_PrevState == false && m_State == true)
		_onPressed = true;

    }

    // Compares Button Values with the last frame to identify if an event needs to be triggered
    void CheckOnReleased()
    {
        _onReleased = false;
		 if(m_PrevState == true && m_State == false)
		_onReleased = true;
    }

    // Resets the Value of the Buttons to make sure they dnt fire events after BTRemote disconnects
    public void OnDisconnected()
    {
         _isPressed = false;
         _onPressed = false;
         _onReleased = false;
         _onHeld = false;
	     // _lastframe = false;
         m_State = false;
         m_PrevState = false;
    }
    
    internal RemoteButtonInput()
    {
    }

}