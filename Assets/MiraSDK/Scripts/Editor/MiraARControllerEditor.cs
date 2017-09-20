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


using UnityEditor;
using UnityEngine;
using Wikitude;

namespace Mira
{
    [CustomEditor(typeof(MiraArController))]
    public class MiraARControllerEditor : Editor
    {
        public override void OnInspectorGUI()
        {
            GUILayout.BeginVertical();
            this.DrawDefaultInspector();

            bool externalOperation = false;

            GUILayout.Label("\t *** Note: 1 unit = " + MiraArController.Instance.setScaleMultiplier + " cm");
            GUILayout.Label("\t *** Note: 1 unit = " + MiraArController.Instance.setScaleMultiplier / 100 + " m");
            GUILayout.Space(16f);

			if (MiraArController.Instance.isRotationalOnly) {
				if (FindObjectOfType<WikitudeCamera> ())
					GUILayout.Label ("Warning: WikitudeCamera should not exist in RotationalOnly Mode");
			} else {
				if (FindObjectOfType<WikitudeCamera> ().DesiredCameraFramerate == CaptureDeviceFramerate.Framerate_60) {
					//GUILayout.Label ("Not Compatible With all Devicess");
					FindObjectOfType<WikitudeCamera> ().DesiredCameraFramerate = CaptureDeviceFramerate.Auto;
				}
				if (FindObjectOfType<WikitudeCamera> ().DesiredCameraResolution == CaptureDeviceResolution.FullHD)
					FindObjectOfType<WikitudeCamera> ().DesiredCameraResolution = CaptureDeviceResolution.Auto;

				if (FindObjectOfType<WikitudeCamera> ().IgnoreTrackableScale == false)
					FindObjectOfType<WikitudeCamera> ().IgnoreTrackableScale = true;

				if (FindObjectOfType<WikitudeCamera> ().StaticCamera == true)
					FindObjectOfType<WikitudeCamera> ().StaticCamera = false;

				if (FindObjectOfType<WikitudeCamera> ().EnableInputPlugin)
					FindObjectOfType<WikitudeCamera> ().EnableInputPlugin = false;
			}
            if (GUILayout.Button("Get Support"))
            {
                Application.OpenURL("http://developers.miralabs.io/");
            }
            GUILayout.EndVertical();
            if (externalOperation)
            {
                EditorGUIUtility.ExitGUI();
            }
        }
    }
}