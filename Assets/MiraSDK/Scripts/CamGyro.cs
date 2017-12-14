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
    /// Applies the gyroScope data to the camera when tracking is not active
    /// </summary>
    public class CamGyro : MonoBehaviour
    {
        [HideInInspector]
        /// <summary>
        /// Is the MiraArController currently set to spectator mode?
        /// </summary>
        public bool isSpectator = false;

        /// <summary>
        /// Stores the camera and gyro base rotations when tracking is lost (so current gyro data can be applied relative to that base orientation)
        /// </summary>
        private Quaternion camGyroOffset = Quaternion.identity;

        /// <summary>
        /// is the camera rotation currently being driven by the gyro? When camera rotation is driven by tracking, this is false
        /// </summary>
        [HideInInspector]
        public bool camGyroActive = false;

        private void Awake()
        {
            // Reference to main GyroController
            if (GyroController.Instance == null)
            {
                Debug.Log("There needs to be a gyroController!");
            }
            // initialize camGyroOffset
            camGyroOffset = Quaternion.identity;
        }

        private void Start()
        {
            if (MiraArController.Instance.isRotationalOnly)
                camGyroActive = true;
        }

        private void LateUpdate()
        {

            if (camGyroActive && !isSpectator)
            {
                // if(GyroController.Instance)
                transform.rotation = camGyroOffset * GyroController.Instance.gyroRotation;
            }
        }

        /// <summary>
        /// ResetCamGyro is called by RotationalTrackingManager right after tracking is lost, and the camera rotation is snapped to the averageBuffer value.
        /// It counters the disparity between the camera rotation derived by tracking, and the camera rotation the gyroScope reports.
        /// The first part of quaternion rotates to the trackingLost position.
        /// Inverting the gyro at this point makes it the baseline/reference point for future movement.
        /// in the update, the current gyro rotation is added back in. Essentially the camera's rotation will match the same rotation as when tracking was lost.
        /// The future gyro rotations will be relative to this new baseline rotation.
        /// </summary>
        public void ResetCamGyro()
        {
            camGyroOffset = transform.rotation * Quaternion.Inverse(GyroController.Instance.gyroRotation);
        }
    }
}