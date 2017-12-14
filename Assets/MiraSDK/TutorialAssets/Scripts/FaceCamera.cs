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

using Mira;
using UnityEngine;

/// <summary>
/// A script with this component will act like a buildboard, always facing the miraCamera
/// </summary>
public class FaceCamera : MonoBehaviour
{
    private Transform stereoCamRig;

    private void Start()
    {
        stereoCamRig = MiraArController.Instance.cameraRig.transform;
    }

    private void Update()
    {
        transform.LookAt(stereoCamRig);
        transform.Rotate(0, 180, 0);
    }
}