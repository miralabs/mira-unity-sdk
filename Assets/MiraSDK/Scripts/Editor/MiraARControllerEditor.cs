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

using UnityEditor;
using UnityEngine;
using Wikitude;

namespace Mira
{
    [CustomEditor(typeof(MiraArController))]
    public class MiraARControllerEditor : Editor
    {

        private static readonly string[] _dontIncludeMe = new string[]{"setScaleMultiplier"};
        
        public override void OnInspectorGUI()
        {
            
 
            GUILayout.BeginVertical();
            serializedObject.Update();
            

            bool externalOperation = false;
            if(MiraArController.Instance.defaultScale)
            {
                DrawPropertiesExcluding(serializedObject, _dontIncludeMe);
                MiraArController.Instance.setScaleMultiplier = 100;
                GUILayout.Space(6f);
                GUILayout.Label("\t *** Mira System is Meter Scale ***");
                GUILayout.Space(16f);
            }

            if(MiraArController.Instance.defaultScale != true)
            {
                this.DrawDefaultInspector();

                GUILayout.Label("\t *** Note: 1 unit = " + MiraArController.Instance.setScaleMultiplier + " cm");
                GUILayout.Label("\t *** Note: 1 unit = " + MiraArController.Instance.setScaleMultiplier / 100 + " m");
                GUILayout.Space(16f);

            }

			if (MiraArController.Instance.isRotationalOnly) {
				if (FindObjectOfType<WikitudeCamera> ())
					GUILayout.Label ("Warning: WikitudeCamera should not exist in RotationalOnly Mode", EditorStyles.boldLabel);
                if (MiraArController.Instance.isSpectator)
                    GUILayout.Label ("Warning: Cannot have Spectator Rotational Mode", EditorStyles.boldLabel);
			} else {
                if (FindObjectOfType<WikitudeCamera>() == null)
                {

                    GUILayout.Label (" ** Warning ** ", EditorStyles.boldLabel);
                    GUILayout.Label ("Script is set to track,");
                    GUILayout.Label ("but no Wikitude cam found.");
                    GUILayout.Label ("Check the right prefab");
                    GUILayout.Space(6f);
                    GUILayout.Label ("~ This warning is a haiku ~");
                    
                }
                else
                {  
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
			}
            if (GUILayout.Button("Get Support"))
            {
                Application.OpenURL("https://www.mirareality.com/developers/");
            }
            GUILayout.EndVertical();
            if (externalOperation)
            {
                EditorGUIUtility.ExitGUI();
            }

            serializedObject.ApplyModifiedProperties();

            
        }
    }
}