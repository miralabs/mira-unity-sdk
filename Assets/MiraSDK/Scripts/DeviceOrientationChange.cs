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
using UnityEngine;

/// <summary>
/// Fires events when a device orientation change is detected.
/// Useful to check when the phone has been inserted into the headset.
/// </summary>
public class DeviceOrientationChange : MonoBehaviour
{
    /// <summary>
    /// Event fired when device orientation changes
    /// </summary>
    public static event Action<Vector2> OnResolutionChange;

    /// <summary>
    /// How often to check for a change in device orientation
    /// </summary>
    public static float CheckDelay = 0.01f;

    private static Vector2 resolution;
    private static bool isAlive = true;

    private void Start()
    {
        StartCoroutine(CheckForChange());
    }

    /// <summary>
    /// While (isAlive == true), checks to see if the current device orientation is different than the previous device orientation
    /// </summary>
    /// <returns></returns>

    private IEnumerator CheckForChange()
    {
        resolution = new Vector2(Screen.width, Screen.height);

        while (isAlive)
        {
            if (resolution.x != Screen.width || resolution.y != Screen.height)
            {
                resolution = new Vector2(Screen.width, Screen.height);
                if (OnResolutionChange != null) OnResolutionChange(resolution);
            }

            yield return new WaitForSeconds(CheckDelay);
        }
    }

    private void OnDestroy()
    {
        isAlive = false;
    }
}