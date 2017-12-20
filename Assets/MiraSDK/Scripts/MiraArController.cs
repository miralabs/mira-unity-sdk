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
        public float IPD = 63.5f;


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
        /// Is the scene in meter scale?
        /// </summary>
        public bool defaultScale = true;

        /// <summary>
        /// Default scale is in cm. To go up to meter scale, set scaleMultiplier to 100.
        /// </summary>
        [SerializeField]
        public float setScaleMultiplier;

        /// <summary>
        /// The scale multiplier scales the entire Mira System, so that it's easy to port physics or animation heavy games
        /// If you are creating a new game, we recoomend that you keep the scaleMultiplier at 100 (meterscale)
        /// </summary>
        public static float scaleMultiplier = 100;

        public float fieldOfView { get {return stereoCamFov; } }

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


        // Controls the Settings Button
        Texture btnTexture;

        float buttonHeight = 100;
        float buttonWidth = 100;

        GUISkin MiraGuiSkin;

        UnityEngine.EventSystems.MiraInputModule miraInputModule;

        bool GUIEnabled = true;

        private GameObject settingsMenu;

        void OnGUI()
        {
            if(GUIEnabled)
            {
                GUI.skin = MiraGuiSkin;
                if(GUI.Button (new Rect(Screen.width/2 - buttonWidth/2, 0, buttonWidth, buttonHeight), btnTexture))
                {   
                    Debug.Log("Settings Menu Enabled");
                    ToggleSettingsMenu(true);
                }

            }
        
        }

        public void ToggleSettingsMenu(bool enable)
        {
            // settingsMenu.SetActive(enable);
            if(enable)
                settingsMenu.GetComponent<SettingsManager>().OpenMainMenu();
            // else
            //     settingsMenu.GetComponent<SettingsManager>().SettingsMenuClose();
            if(miraInputModule != null)
                miraInputModule.enabled = !enable;
            GUIEnabled = !enable;
        }

        void SetupSettingsMenu()
        {
            settingsMenu = Instantiate((GameObject)Resources.Load("SettingsMenuCanvas", typeof(GameObject)));
            // settingsMenu.SetActive(false);
            // ToggleSettingsMenu(false);
            MiraGuiSkin = (GUISkin)Resources.Load("MiraGuiSkin", typeof(GUISkin));
            btnTexture = (Texture)Resources.Load("SettingsIcon", typeof(Texture));

            // Find instances of MiraInputModule
            UnityEngine.EventSystems.MiraInputModule[] miraInputModules = FindObjectsOfType<UnityEngine.EventSystems.MiraInputModule>();
            // Make sure there's only one Mira Input Module, otherwise settings menu will break
            if(miraInputModules.Length > 0)
            {
                if(miraInputModules[0].enabled == true)
                    miraInputModule = miraInputModules[0];
            }
            if(miraInputModules.Length > 1)
            {
                for(int i=1; i<miraInputModules.Length; i++)
                {  
                    if(miraInputModule == null && miraInputModules[i].enabled == true)
                        miraInputModule = miraInputModules[i];
                    miraInputModules[i].enabled = false;
                }
            }
            
        }

        public void Awake()
        {
            if(isSpectator == false)
                DeviceOrientation();
            cameraRig = gameObject;
            scaleMultiplier = setScaleMultiplier;
            Instance.InitializeARController();
            
            SetupSettingsMenu();

        }

        private void Start()
        {

            // Force screen brightness
    #if UNITY_IOS
                MiraiOSBridge.ForceBrightness();
    #endif

            SanityCheck();
        }

        

        void DeviceOrientation()
        {

            // For screen orientation
            Screen.orientation = ScreenOrientation.LandscapeLeft;

            // Screen.autorotateToPortrait = false;
            // Screen.autorotateToLandscapeLeft = true;
            // Screen.autorotateToPortraitUpsideDown = false;
            // Screen.autorotateToLandscapeRight = false;
        }

        #endregion Unity callbacks

        #region Public Functions

        public void UpdateIPD(float newIPD)
        {
            IPD = newIPD; 
            float camOffset = newIPD/2 * 0.1f * (1 / scaleMultiplier);
            MiraViewer.Instance.Left_Eye.transform.localPosition = new Vector3(-camOffset, 0, 0);
            MiraViewer.Instance.Right_Eye.transform.localPosition = new Vector3(camOffset, 0, 0);

            MiraPostRender distortionL = DistortionCamera.Instance.dcLeft.GetComponent<MiraPostRender>();
            MiraPostRender distortionR = DistortionCamera.Instance.dcRight.GetComponent<MiraPostRender>();

            distortionL.RecalculateDistortion();
            distortionR.RecalculateDistortion();

        }

        /// <summary>
        /// Initializes the MiraArCamera and MiraViewer.
        /// </summary>
        public void InitializeARController()
        {
            mv = gameObject.AddComponent<MiraViewer>();
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