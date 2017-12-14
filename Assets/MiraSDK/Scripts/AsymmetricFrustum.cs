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
    [ExecuteInEditMode]
    /// <summary>
    /// Creates an asymmetric frustum on the stereo cameras
    /// </summary>
    public class AsymmetricFrustum : MonoBehaviour
    {
        [HideInInspector]
        public float left;

        [HideInInspector]
        public float right;

        [HideInInspector]
        public float top;

        [HideInInspector]
        public float bottom;

        [HideInInspector]
        public float near;

        [HideInInspector]
        public float far;

        // public float whereverYouAre;
        // public float iBelieveThatMyHeartWillGoOooooonnn;
        private float fov;

        /// <summary>
        /// The near clipping plane that the asymmetric frustum value was calibrated using.!-- Do not touch this, plz
        /// </summary>
        private float calibratedCamNear = 0.001f;

        // float calibratedCamFar = 15;
        /// <summary>
        /// The slideX value quantifies the amount of asymmetry
        /// </summary>
        [HideInInspector]
        public float slideX = 0f;

        /// <summary>
        /// Is this the left camera?
        /// </summary>
        public bool isLeftCam = false;

        /// <summary>
        /// Reference to gameObject's camera
        /// </summary>
        private Camera cam;

        /// <summary>
        /// Is the asymmetricFrustum active
        /// </summary>
        public bool asymmetricFrust = true;

        private void Start()
        {
            cam = gameObject.GetComponent<Camera>();
            near = cam.nearClipPlane;
            far = cam.farClipPlane;
            fov = cam.fieldOfView;
            SetFrustByFov();
        }

        /// <summary>
        /// Sets the correct asymmetric frust, to match the FOV of the stereo camera
        /// </summary>
        public void SetFrustByFov()
        {
            Debug.Log("Camera Far Clip: " + far);
            float y = Mathf.Tan(0.5f * fov * Mathf.Deg2Rad) * near;
            top = y;
            bottom = -y;
            float x = cam.aspect * y;
            right = x;
            left = -x;

            UpdateFrust();
        }

        /// <summary>
        /// Incrases or decreases the frustum slide, and refreshes the frustum
        /// </summary>
        /// <param name="amt">The amount to shift the frustum</param>
        public void SlideFrust(float amt)
        {
            if (isLeftCam)
            {
                slideX -= amt;
            }
            else
            {
                slideX += amt;
            }
            UpdateFrust();
        }

        /// <summary>
        /// Toggles the asymmetric frustum on and off
        /// </summary>
        public void ToggleFrust()
        {
            if (asymmetricFrust)
            {
                cam.ResetProjectionMatrix();
                asymmetricFrust = false;
            }
            else
            {
                SetFrustByFov();
                asymmetricFrust = true;
            }
        }

        /// <summary>
        /// Frefeshes da frustum, if you make any changes to the slideX value, you will need to call this
        /// </summary>
        public void UpdateFrust()
        {
            Matrix4x4 m = PerspectiveOffCenter(calibratedCamNear, left, right, bottom, top, near, far, slideX * (1 / MiraArController.scaleMultiplier) * 100, gameObject);
            cam.projectionMatrix = m;
        }

        private void SetParams(float l, float r)
        {
            left = l;
            right = r;
        }

        private static Matrix4x4 PerspectiveOffCenter(float calibratedNear, float left, float right, float bottom, float top, float near, float far, float slideX, GameObject thisGO)
        {
            float calibratedSlideX = near / calibratedNear * slideX;
            Debug.Log("SlideX: " + slideX);
            left = left + calibratedSlideX;
            right = right + calibratedSlideX;

            thisGO.GetComponent<AsymmetricFrustum>().SetParams(left, right);

            // x is the ratio of the near clip plane to the total distance between right and left points
            // Because of calibratedNear and scaling above, X and Y will always be the same, even when changing slideX
            float x = 2.0F * near / (right - left);
            float y = 2.0F * near / (top - bottom);

            // a is a measure of the asymmetry in the x direction
            // if a is negative, the asymmetry is to the left, if positive, frustum is leaning to the right
            // a does not change with changes to the near or far clip, but does change when you slide right and left
            float a = (right + left) / (right - left);
            float b = (top + bottom) / (top - bottom);

            // c and d represent the ratio between the far and near clip plane
            float c = -(far + near) / (far - near);
            float d = -(2.0F * far * near) / (far - near);

            // e, you do you
            float e = -1.0F;
            Matrix4x4 m = new Matrix4x4();
            m[0, 0] = x;
            m[0, 1] = 0;
            m[0, 2] = a;
            m[0, 3] = 0;
            m[1, 0] = 0;
            m[1, 1] = y;
            m[1, 2] = b;
            m[1, 3] = 0;
            m[2, 0] = 0;
            m[2, 1] = 0;
            m[2, 2] = c;
            m[2, 3] = d;
            m[3, 0] = 0;
            m[3, 1] = 0;
            m[3, 2] = e;
            m[3, 3] = 0;
            return m;
        }
    }
}