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
    /// Creates the camera rig that captures the counter-distortion
    /// </summary>
    public class DistortionCamera : MonoBehaviour
    {
        #region Private Variables


        private float stereoFov;
        private static DistortionCamera instance = null;
        private static Camera m_dcLeft;
        private static Camera m_dcRight;

        #endregion Private Variables

        #region Public Variables
        public Camera dcLeft { get { return m_dcLeft; } }
        public Camera dcRight { get { return m_dcRight; } }

        public DistortionCamera(float fov)
        {
            stereoFov = fov;
        }

        public static DistortionCamera Instance
        {
            get
            {
                if (instance == null)
                {
                    instance = UnityEngine.Object.FindObjectOfType<DistortionCamera>();
                }

                return instance;
            }
        }

        #endregion Public Variables

        #region Public Functions

        /// <summary>
        /// Creates The Distortion Camera Rig.
        /// </summary>
        public void Create()
        {
            if (instance == null && UnityEngine.Object.FindObjectOfType<DistortionCamera>() == null)
            {
                GameObject go = new GameObject("MiraDistortionCamera", typeof(DistortionCamera));
                go.AddComponent<MiraPreRender>();
                AddCamera('R');
                AddCamera('L');
            }
        }

        /// <summary>
        ///
        /// </summary>
        /// <param name="Camera">Name of the Camera thats need to be added Right or Left.</param>
        public void AddCamera(char Camera)
        {
            GameObject go = new GameObject("DistortionCamera" + Camera, typeof(Camera));

            Camera cam = go.GetComponent<Camera>();
            {
                cam.clearFlags = CameraClearFlags.SolidColor;
                cam.backgroundColor = new Color(0f, 0f, 0f, 0f);
                cam.orthographic = true;
                cam.orthographicSize = 29.25f;
        

                cam.nearClipPlane = 0.3f;
                cam.farClipPlane = 99.21f;
                cam.depth = 999;
                if (Camera == 'L')
                {
                    cam.rect = new Rect(0.5f, 0f, 0.5f, 1f);
                    cam.cullingMask = 0;
                }
                else
                {
                    cam.rect = new Rect(0f, 0f, 0.5f, 1f);
                    cam.cullingMask = 0;
                }
            }

            go.AddComponent<MeshFilter>();
            MiraPostRender mpost = go.AddComponent<MiraPostRender>();

            if (Camera == 'L')
            {
                mpost.eye = MiraPostRender.Eye.Left;
                m_dcLeft = cam;
            }
            else
            {
                mpost.eye = MiraPostRender.Eye.Right;
                m_dcRight = cam;
            }

            mpost.InitializeDistortion(stereoFov, MiraArController.Instance.IPD);

            mpost.DistortionMesh();
        }

        #endregion Public Functions
    }
}