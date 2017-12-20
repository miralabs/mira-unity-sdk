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

using UnityEngine;

namespace Mira
{
    /// <summary>
    /// This retains 3-DOF orientation tracking during 6-DOF marker tracking, by inverting the gyroscope, and applying it to the HeadTracking gameObject.
    /// Because the marker is always stationary, at the world origin, there is no difference between the user walking around the marker, and the user reminaing stationary and spinning the marker.
    /// The Inverse gyro counters this, by keeping rotationally tracked elements in the proper location
    /// </summary>
    public class InverseGyroController : MonoBehaviour
    {
        private Transform stereoCamRig;

        private void Start()
        {
            stereoCamRig = MiraArController.Instance.gameObject.transform;
            transform.position = stereoCamRig.position;

            this.enabled = false;
        }

        private void LateUpdate()
        {
            transform.position = stereoCamRig.position;
            transform.rotation = stereoCamRig.rotation * Quaternion.Inverse(GyroController.Instance.gyroRotation);
        }
    }
}