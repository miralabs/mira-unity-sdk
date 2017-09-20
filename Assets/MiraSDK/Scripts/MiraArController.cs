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
using UnityEngine;

namespace Mira
{
    /// <summary>
    /// MiraArController sets up the required settings for all Mira Modes.
    /// This Script should be present in the Scene on the same Gameobject as you Camera used for Tracking if using Image or Object Tracking.(for eg,MiraArCamera, WikitudeCamera).
    /// This Instance of the class should be used to access the public methods or data members.
    /// </summary>
    public class MiraArController : MonoBehaviour
    {
        #region Public Variables

        /// <summary>
        /// Calibrate values, then use these to universally set them at start
        // IPD must always be in MM (for distortion algorithm)
        /// </summary>
        public static float IPD = 63.5f;


        /// <summary>
        /// Refrence to the leftDistortion Camera
        /// </summary>
        [HideInInspector]
        public GameObject leftCam;

        /// <summary>
        /// Refrence to the rightDistortion Camera
        /// </summary>
        [HideInInspector]
        public GameObject rightCam;

        [HideInInspector]
        public GameObject cameraRig;

        /// <summary>
        /// Sets the rotation only (3-DOF orientation tracking only) mode.
        /// </summary>
        public bool isRotationalOnly = false;

        /// <summary>
        /// Sets the Spectator mode
        /// </summary>
        public bool isSpectator = false;

        /// <summary>
        /// The near clip plane in CM
        /// </summary>
        public float nearClipPlane = 0.1f;

        /// <summary>
        /// The far clip plane in CM
        /// </summary>
        public float farClipPlane = 1500f;

        /// <summary>
        /// Default scale is in cm. To go up to meter scale, set scaleMultiplier to 100.
        /// </summary>
        public float setScaleMultiplier = 100f;

        /// <summary>
        /// The scale multiplier scales the entire Mira System, so that it's easy to port physics or animation heavy games
        /// If you are creating a new game, we recoomend that you keep the scaleMultiplier at 100 (meterscale)
        /// </summary>
        public static float scaleMultiplier = 100f;

        #endregion Public Variables

        #region Private Variables

        private float stereoCamFov = 55f;
        private MiraViewer mv;
        private static MiraArController instance = null;

        #endregion Private Variables

        #region Properties

        /// <summary>
        /// Any variable or method for the MiraARController should be accessed through the Instance.
        /// </summary>
        /// <value>The instance.</value>
        public static MiraArController Instance
        {
            get
            {
                if (instance == null)
                    instance = UnityEngine.Object.FindObjectOfType<MiraArController>();
                return instance;
            }
        }

        #endregion Properties

        #region Unity callbacks

        public void Awake()
        {
            cameraRig = gameObject;
            scaleMultiplier = setScaleMultiplier;
            Instance.InitializeARController();
        }

        private void Start()
        {
            // For screen orientation
            Screen.autorotateToPortrait = true;
            Screen.autorotateToLandscapeLeft = true;
            Screen.autorotateToPortraitUpsideDown = false;
            Screen.autorotateToLandscapeRight = false;
            Screen.orientation = ScreenOrientation.AutoRotation;

            // Force screen brightness
#if UNITY_IOS
				MiraiOSBridge.ForceBrightness();
#endif

            SanityCheck();
        }

        #endregion Unity callbacks

        #region Public Functions

        /// <summary>
        /// Initializes the MiraArCamera and MiraViewer.
        /// </summary>
        public void InitializeARController()
        {
            mv = new MiraViewer(stereoCamFov);
            mv.Create();
            if (isSpectator)
            {
                CreateSpecCamRig();
                SwitchToSpectator();
            }
            else
            {
                CreateARCamRig();
                SwitchToARCam();
            }
        }

        #endregion Public Functions

        #region Private Functions

        // Check to make sure there are no conflicting settings
        private void SanityCheck()
        {
            if (isRotationalOnly == true && GetComponent<MiraWikitudeManager>() != null)
            {
                Debug.LogError("You have conflicting settings - There should not be a Mira Wikitude Manager Component in Rotational Only Mode");
            }
            else if (isRotationalOnly == true && GetComponent<Wikitude.WikitudeCamera>() != null)
            {
                Debug.LogError("You have conflicting settings - There should not be a Wikitude Component in Rotational Only Mode");
            }
        }

        private void CreateARCamRig()
        {
            mv.AddGyroControllerToCameras(true, isRotationalOnly);

            DistortionCamera dc = new DistortionCamera(stereoCamFov);
            dc.Create();

            Debug.Log("Mira AR Camera Rig Initialized");
        }

        private void CreateSpecCamRig()
        {
            mv.AddGyroControllerToCameras(false, isRotationalOnly);

            Debug.Log("Spectator Mode Initialized");
        }

        private void SwitchToARCam()
        {
            // Set the Cam Gyro and WikiCam Device Position to correct orientation
            GyroController[] gyros = GameObject.FindObjectsOfType<GyroController>();

            foreach (GyroController gyro in gyros)
            {
                gyro.useFrontCamera = true;
            }
            Debug.Log("Gyros set to Front Camera");
        }

        private void SwitchToSpectator()
        {
            GyroController[] gyros = GameObject.FindObjectsOfType<GyroController>();

            foreach (GyroController gyro in gyros)
            {
                gyro.useFrontCamera = false;
            }
        }

        #endregion Private Functions
    }
}