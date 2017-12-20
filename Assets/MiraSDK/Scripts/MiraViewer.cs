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
using Wikitude;

namespace Mira
{
    /// <summary>
    ///  Mira Viewer Initializes the Stereo Camera Rig for Mira Headset Mode and Gyro Controllers respective to Mode selected.
    ///  This Script is not required to be attached on the Game Object and only one instance should exist in the Scene.
    ///  All settings are recommended and should not be changed unless absolutely necessary.
    /// </summary>

    public class MiraViewer : MonoBehaviour
    {
        #region Private Variables

        private float stereoCamFov = 50f;

        // float asymmetricFrustrumSlide = 0.00001f * 5;
        private static GameObject viewer = null;

        #endregion Private Variables

        #region Public Variables

        /// <summary>
        /// GameObject containg the Stereo Camera Rig and GyroController.
        /// </summary>
        public GameObject cameraRig;

        /// <summary>
        ///  Left Eye Camera
        /// </summary>
        public GameObject Left_Eye = null;

        /// <summary>
        ///  Right Eye Camera
        /// </summary>
        public GameObject Right_Eye = null;

        /// <summary>
        /// CameraNames contains the list of cameras in the camerarig.
        /// </summary>
        public enum CameraNames
        {
            Right_Camera = 1,
            Left_Camera = 2,
        }

        #endregion Public Variables

        #region Properties

        public static MiraViewer Instance = null;
       
        #endregion Properties

        #region Unity callbacks

        public void Awake()
        {
            if (Instance == null) Instance = this;
            else if (Instance != this) Destroy(gameObject);
            Application.targetFrameRate = 60;
            gameObject.GetComponent<Transform>().hideFlags = HideFlags.HideInInspector;

            stereoCamFov = MiraArController.Instance.fieldOfView;
        }

        #endregion Unity callbacks

        #region Public Methods

        /// <summary>
        /// Creates the Stereo Cameras individually.
        /// Clipping planes are adjusted according to the set SceneScale
        /// </summary>
        /// <returns>The camera for each eye.</returns>
        /// <param name="name">Type Camera which needs to be created.</param>
        public GameObject CreateCameraforEachEye(CameraNames name)
        {
            Debug.Log("Creating " + name);

            GameObject go = new GameObject(name.ToString(), typeof(Camera));
            Camera eyecamera = go.GetComponent<Camera>();
            eyecamera.clearFlags = CameraClearFlags.SolidColor;
            eyecamera.backgroundColor = new Color(0f, 0f, 0f, 0f);
            eyecamera.orthographic = false;
            eyecamera.fieldOfView = stereoCamFov;

            eyecamera.transform.localPosition = new Vector3(0, 0, 0f);
            eyecamera.depth = 99;
            eyecamera.nearClipPlane = MiraArController.Instance.nearClipPlane * (1 / MiraArController.scaleMultiplier);
            eyecamera.farClipPlane = MiraArController.Instance.farClipPlane * (1 / MiraArController.scaleMultiplier);

            eyecamera.allowHDR = false;
            eyecamera.allowMSAA = false;
            eyecamera.useOcclusionCulling = true;

            go.transform.SetParent(cameraRig.transform, false);
            // AsymmetricFrustum frust = go.AddComponent<AsymmetricFrustum>();

            RenderTexture EyeRenderTexture = new RenderTexture(1024, 1024, 0, RenderTextureFormat.ARGB32);
            EyeRenderTexture.useMipMap = false;
            EyeRenderTexture.wrapMode = TextureWrapMode.Clamp;
            EyeRenderTexture.filterMode = FilterMode.Bilinear;
            EyeRenderTexture.anisoLevel = 0;
            EyeRenderTexture.depth = 24;

            EyeRenderTexture.Create();

            eyecamera.targetTexture = EyeRenderTexture;

            if (name == CameraNames.Left_Camera)
            {
                eyecamera.tag = "MainCamera";
                eyecamera.rect = new Rect(0f, 0f, 1f, 1f);
                eyecamera.transform.localPosition = new Vector3(-MiraArController.Instance.IPD / 2 * 0.1f * (1 / MiraArController.scaleMultiplier), 0f, 0f);
                MiraArController.Instance.leftCam = eyecamera.gameObject;
            }
            else
            {
                // frust.slideX = -asymmetricFrustrumSlide;
                eyecamera.rect = new Rect(0f, 0f, 1f, 1f);

                eyecamera.transform.localPosition = new Vector3(MiraArController.Instance.IPD / 2 * 0.1f * (1 / MiraArController.scaleMultiplier), 0f, 0f);

                MiraArController.Instance.rightCam = eyecamera.gameObject;
            }
            return go;
        }

        /// <summary>
        /// Adds the gyro controller to camera rig.
        /// The appropriate behavior is chosen with respect to the mode(MiraHeadset Or Spectator).
        /// </summary>
        /// <param name="notSpectator">If set to <c>false</c>Cam Gyro is set in Spectator mode</param>
        /// <param name="isRotationalOnly">If set to <c>true</c> Parenting function is set to rotational mode and InverseGyro will no be used.</param>
        public void AddGyroControllerToCameras(bool notSpectator, bool isRotationalOnly)
        {
            cameraRig = MiraArController.Instance.cameraRig;

            // If spectator mode, check to see if wikiCamera has camgyro. If not, add it, and set to spec.
            if (notSpectator)
            {
                if (cameraRig.GetComponent<CamGyro>() == null)
                    cameraRig.AddComponent<CamGyro>();
            }
            else
            {
                if (MiraArController.Instance.GetComponent<CamGyro>() == null)
                    MiraArController.Instance.gameObject.AddComponent<CamGyro>();
                MiraArController.Instance.GetComponent<CamGyro>().isSpectator = true;
            }

            // Set Parents, offsets, and all that good stuff
            SetParenting(notSpectator, isRotationalOnly);
        }

        /// <summary>
        /// Iniailizes Pre Renderer
        /// </summary>
        public void AddPreRenderer()
        {
            GameObject go = new GameObject("PreRenderer");
            go.transform.SetParent(viewer.transform);
        }

        /// <summary>
        /// Iniailizes Post Renderer
        /// </summary>
        public void AddPostRenderer()
        {
            GameObject go = new GameObject("PostRenderer");
            go.transform.SetParent(viewer.transform);
        }

        #endregion Public Methods

        #region Private Methods

        /// <summary>
        ///Manges the Handoff between states TrackingFound -> TrackigLost..
        /// </summary>
        /// <param name="notSpectator">If set to <c>true</c> not spectator.</param>
        /// <param name="isRotationalOnly">If set to <c>true</c> is rotational only.</param>
        private void SetParenting(bool notSpectator, bool isRotationalOnly)
        {
            // Regular AR Cam rig
            if (notSpectator)
            {
                // Set Correct headPivot offsets and transforms
                cameraRig.transform.rotation = MiraArController.Instance.cameraRig.transform.rotation;
                cameraRig.transform.SetParent(MiraArController.Instance.cameraRig.transform);

                if (!isRotationalOnly)
                {
                    WikitudeCamera wikiCamera = FindObjectOfType<WikitudeCamera>();
                    if (wikiCamera.StaticCamera)
                    {
                        wikiCamera.transform.SetParent(cameraRig.transform);
                        wikiCamera.transform.position = MiraArController.Instance.transform.position;
                        wikiCamera.transform.rotation = MiraArController.Instance.transform.rotation;
                    }
                }
                cameraRig.transform.SetParent(null);
            }
            else
            {
                if (!isRotationalOnly)
                {
                    WikitudeCamera wikiCamera = FindObjectOfType<WikitudeCamera>();
                    cameraRig.transform.rotation = wikiCamera.transform.rotation;
                    cameraRig.transform.SetParent(wikiCamera.transform);
                    cameraRig.transform.localPosition = Vector3.zero;

                    MiraArController.Instance.transform.SetParent(cameraRig.transform);
                    MiraArController.Instance.transform.localPosition = Vector3.zero;
                    MiraArController.Instance.transform.localRotation = Quaternion.identity;
                }
            }

            if (notSpectator)
            {
                cameraRig.transform.SetParent(MiraArController.Instance.transform);

                // Create AR Cam
                if (Left_Eye == null)
                {
                    Right_Eye = CreateCameraforEachEye(CameraNames.Right_Camera);
                    Left_Eye = CreateCameraforEachEye(CameraNames.Left_Camera);
                }
            }
        }

        #endregion Private Methods
    }
}