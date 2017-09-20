// Copyright (c) Mira Labs, Inc., 2017. All rights reserved.
// 
// Downloading and/or using this MIRA SDK is under license from MIRA, 
// and subject to all terms and conditions of the Mira Software License,
// found here: www.mirareality.com/sdk-license/
// 
// By downloading this SDK, you agree to the Mira Software License.
//
// This SDK may only be used in connection with the development of
// applications that are exclusively created for, and exclusively available
// for use with, MIRA hardware devices. This SDK may only be commercialized
// in the U.S. and Canada, subject to the terms of the License.
// 
// The MIRA SDK includes software under license from The Apache Software Foundation.

using System.Collections;
using UnityEngine;

namespace Mira
{
    /// <summary>
    /// Detects when phone has been flipped from portrait to landscape
    /// When in portrait, displays a 2D screen
    /// When flipped into landscape, shows the 3D stereo view
    /// </summary>
    public class Transition2D : MonoBehaviour
    {
        /// <summary>
        /// Do you want this functionality to only activate once, then turn off?
        /// This is more performant, as it does not continue to perform orientation checks.
        /// This is useful for displaying the "Please insert your phone into the headset" screen
        /// </summary>
        public bool displayOnce = true;

        private Canvas instructionScreen;
        public Camera twoDCamera;
        private Camera distortionL;
        private Camera distortionR;

        private void OnEnable()
        {
            DeviceOrientationChange.OnResolutionChange += OrientationChecker;
        }

        private void OnDisable()
        {
            DeviceOrientationChange.OnResolutionChange -= OrientationChecker;
        }

        private IEnumerator Start()
        {
            QualitySettings.antiAliasing = 4;
            yield return new WaitForEndOfFrame();
            instructionScreen = transform.GetComponentInChildren<Canvas>();
            distortionL = GameObject.Find("DistortionCameraL").GetComponent<Camera>();
            distortionR = GameObject.Find("DistortionCameraR").GetComponent<Camera>();
            distortionL.gameObject.SetActive(false);
            distortionR.gameObject.SetActive(false);
        }

        private void OrientationChecker(Vector2 resolution)
        {
            if (resolution.x > resolution.y)
            {
                StopAllCoroutines();
                StartCoroutine(TransitionToLandscape());
                Debug.Log("Resolution Changed to Landscape");
            }
            else
            {
                StopAllCoroutines();
                StartCoroutine(TransitionToPortrait());
                Debug.Log("Resolution Changed to Portrait");
            }
        }

        private IEnumerator TransitionToLandscape()
        {
            QualitySettings.antiAliasing = 0;
            distortionL.gameObject.SetActive(true);
            distortionR.gameObject.SetActive(true);
            twoDCamera.gameObject.SetActive(false);
            instructionScreen.gameObject.SetActive(false);
            if (displayOnce)
                gameObject.SetActive(false);
            yield return null;
        }

        private IEnumerator TransitionToPortrait()
        {
            QualitySettings.antiAliasing = 4;
            distortionL.gameObject.SetActive(false);
            distortionR.gameObject.SetActive(false);
            twoDCamera.gameObject.SetActive(true);
            instructionScreen.gameObject.SetActive(true);
            yield return null;
        }
    }
}