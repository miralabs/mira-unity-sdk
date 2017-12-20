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

public class MenuItems : EditorWindow
{
	Texture miraLogo;
    [MenuItem("Mira/Settings")]
    private static void Init()
    {
        EditorWindow window = EditorWindow.CreateInstance<MenuItems>();
        window.maxSize = new Vector2(400f, 1000f);
        window.Show(true);
    }

    private Rect buttonRect;
    private Texture2D m_Logo = null;
    void OnEnable()
    {
	m_Logo = (Texture2D)Resources.Load("MiraLogo",typeof(Texture2D));
    }	
    private void OnGUI()
    {
        {
            GUILayout.Label("Mira Unity Settings", EditorStyles.boldLabel);
		    GUILayout.Label(m_Logo);
            // Accelorometer Frequency

            GUI.enabled = true;
            GUILayout.Label("Accelerometer Frquency" + " (current = " + PlayerSettings.accelerometerFrequency + ")");

            if (PlayerSettings.accelerometerFrequency == 60)
                GUI.enabled = false;

            if (GUILayout.Button("Use Recomended (60 mhz)", GUILayout.Width(250)))
                PlayerSettings.accelerometerFrequency = 60;

            GUILayout.Space(5);

            GUI.enabled = true;
            GUILayout.Label("Current Bundle Id" + " = " + PlayerSettings.GetApplicationIdentifier(BuildTargetGroup.iOS));

            if (PlayerSettings.GetApplicationIdentifier(BuildTargetGroup.iOS) == "com.mirareality.example")
                GUI.enabled = false;

            if (GUILayout.Button("Use Recomended (com.mirareality.example)", GUILayout.Width(250)))
                PlayerSettings.SetApplicationIdentifier(BuildTargetGroup.iOS, "com.mirareality.example");

            GUILayout.Space(5);

            GUI.enabled = true;
            GUILayout.Label("Current Camera Usage Description" + " = " + PlayerSettings.iOS.cameraUsageDescription);

            if (PlayerSettings.iOS.cameraUsageDescription == "Computer Vision")
                GUI.enabled = false;

            if (GUILayout.Button("Use Recomended (Computer Vison)", GUILayout.Width(250)))
                PlayerSettings.iOS.cameraUsageDescription = "Computer Vision";

            GUILayout.Space(5);

            GUI.enabled = true;
            bool isMetal = false;
            foreach(UnityEngine.Rendering.GraphicsDeviceType graphicsType in PlayerSettings.GetGraphicsAPIs(BuildTarget.iOS))
            {
                if(graphicsType == UnityEngine.Rendering.GraphicsDeviceType.Metal)
                    isMetal = true;
            }
            GUILayout.Label("Graphics APIs contains metal:" + " = " + isMetal);
            
            if (isMetal == false)
                GUI.enabled = false;

            if (GUILayout.Button("Use Recomended (Remove Metal)", GUILayout.Width(250)))
            {
                UnityEngine.Rendering.GraphicsDeviceType[] graphicsAPIs = new UnityEngine.Rendering.GraphicsDeviceType[]{UnityEngine.Rendering.GraphicsDeviceType.OpenGLES2};;
                PlayerSettings.SetGraphicsAPIs(BuildTarget.iOS, graphicsAPIs);
            }
    
            GUILayout.Space(5);

            


            GUI.enabled = true;
            GUILayout.Label("UI Orientation" + " = " + PlayerSettings.defaultInterfaceOrientation);

            if (PlayerSettings.defaultInterfaceOrientation == UIOrientation.AutoRotation)
                GUI.enabled = false;

            if (GUILayout.Button("Use Recomended (AutoRotate)", GUILayout.Width(250)))
                PlayerSettings.defaultInterfaceOrientation = UIOrientation.AutoRotation;

            GUILayout.Space(5);

            GUI.enabled = true;
            GUILayout.Label("Configure Mira AutoRotation (Highly Recommended)");

            if (PlayerSettings.allowedAutorotateToLandscapeLeft == true &&
                PlayerSettings.allowedAutorotateToLandscapeRight == false &&
                PlayerSettings.allowedAutorotateToPortrait == true &&
                PlayerSettings.allowedAutorotateToPortraitUpsideDown == false)
                GUI.enabled = false;

            if (GUILayout.Button("Use Recomended (Allow)", GUILayout.Width(250)))
            {
                PlayerSettings.allowedAutorotateToLandscapeLeft = true;
                PlayerSettings.allowedAutorotateToLandscapeRight = false;
                PlayerSettings.allowedAutorotateToPortrait = true;
                PlayerSettings.allowedAutorotateToPortraitUpsideDown = false;
            }
                

            GUILayout.Space(5);

            GUI.enabled = true;
            GUILayout.Label("Current Min iOS OS Version" + " = " + PlayerSettings.iOS.targetOSVersionString);

            if (PlayerSettings.iOS.targetOSVersionString == "8.0")
                GUI.enabled = false;

            if (GUILayout.Button("Use Recomended (8.0)", GUILayout.Width(250)))
                PlayerSettings.iOS.targetOSVersionString = "8.0";

            // GUILayout.Space(15);
            // GUI.enabled = true;
            // if (GUILayout.Button("Apply All Recomended Settings", GUILayout.Width(390)))
            // {
            //     PlayerSettings.accelerometerFrequency = 60;
            //     PlayerSettings.SetApplicationIdentifier(BuildTargetGroup.iOS, "com.mirareality.example");
            //     PlayerSettings.iOS.cameraUsageDescription = "Computer Vision";
            //     PlayerSettings.defaultInterfaceOrientation = UIOrientation.AutoRotation;
            //     PlayerSettings.allowedAutorotateToLandscapeLeft = true;
            //     PlayerSettings.iOS.targetOSVersionString = "8.0";
            // }
		GUILayout.Space(15);
		GUILayout.Label ("Mira SDK 0.7.1.", EditorStyles.boldLabel);
        }
    }

    [MenuItem("Mira/Support")]
    private static void NewMenuOption()
    {
        Application.OpenURL("https://developer.mirareality.com/docs/getting-started/");
    }

    [MenuItem("Mira/Check for SDK Updates")]
    private static void UpdateMiraSDK()
    {
		Application.OpenURL("https://github.com/miralabs/mira-unity-sdk");
    }

}