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

using Mira;
using System.Collections;
using UnityEngine;
using Wikitude;
using UnityEngine.SceneManagement;

/// <summary>
/// All Mira settings for Wikitude Tracking, including positional offsets, tracking scaling, and image preview scaling
///  This Script should only be used in only when using Image Tracking.
///  The ArCamera should be accessed by the Instance.
/// </summary>
public class MiraWikitudeManager : TransformOverride
{
    #region Public Variables

    /// <summary>
    /// refrence to the Wikitude Camera Script.
    /// </summary>
    [HideInInspector]
    public WikitudeCamera ArCam = null;

    /// <summary>
    /// Set this to the gameObject that you would like to act as the Orientation Tracking Main Parent.
    /// </summary>
    public GameObject headOrientation3DOF;

    /// <summary>
    /// Attach the GameObject that contains the all elements in the scene that do not depend on Tracking.
    /// </summary>
    public ImageTrackable imageTracker;

    #endregion Public Variables

    #region PrivateVariables

    private static MiraWikitudeManager instance = null;
    private float scaleMultiplier;
    private Quaternion rotationalOffset;
    public Vector3 positionalOffset;
    private bool flag = false;

    #endregion PrivateVariables

    #region Properties

    /// <summary>
    /// Any public method or variable should be accessed through the instance of the class.
    /// </summary>
    /// <value>The instance.</value>
    public static MiraWikitudeManager Instance
    {
        get
        {
            if (instance == null)
            {
                instance = Object.FindObjectOfType<MiraWikitudeManager>();
            }
            return instance;
        }
    }

    #endregion Properties

    #region Unity callbacks

	public void OnEnable()
	{
		
			
	}
    public void Awake()
    {
        ArCam = gameObject.GetComponent<WikitudeCamera>();
        //	Instance.InstantiateWikitudeCamera ();
        InstaiateRotationalHandoffManager();
    }

    public void Start()
    {
        scaleMultiplier = 1 / MiraArController.scaleMultiplier;
        rotationalOffset = Quaternion.Euler(-30, 0, 0);
        // positionalOffset = new Vector3(-5.2f, -0.93599f, -5.0f) * scaleMultiplier;
        positionalOffset = new Vector3(-6.006f, -2.846f, -3.689f) * scaleMultiplier;

        if (MiraArController.Instance.isSpectator == false)
        {
            Debug.Log("Using front facing camera");
            Vector2 exposurepoint = new Vector2(0.15f, 0.2f);
            ArCam.ExposeAtPointOfInterest(exposurepoint, CaptureExposureMode.ContinuousAutoExpose);
            ArCam.DevicePosition = CaptureDevicePosition.Front;
        }
        else
        {
		Camera specCam = gameObject.AddComponent<Camera> ();
		specCam.nearClipPlane = MiraArController.Instance.nearClipPlane * (1/MiraArController.Instance.setScaleMultiplier);
            ArCam.DevicePosition = CaptureDevicePosition.Back;
            //ArCam.EnableCameraRendering = true;
            positionalOffset = Vector3.zero;
            rotationalOffset = Quaternion.identity;
            ArCam.enabled = true;

            imageTracker.AutoToggleVisibility = true;
        }

        SanityCheck();
    }

    public void Update()
    {
	if(!gameObject.GetComponent<WikitudeCamera> ().EnableCameraRendering && MiraArController.Instance.isSpectator)
	gameObject.GetComponent<WikitudeCamera> ().EnableCameraRendering = true;
	
    }

    #endregion Unity callbacks

    #region Public Methods

    /// <summary>
    /// Instaiates the rotational handoff manager required for state changes from Tracking founf -> Tracking Loast and vice versa.
    /// </summary>
    public void InstaiateRotationalHandoffManager()
    {
	if (headOrientation3DOF != null) {
            headOrientation3DOF.AddComponent<InverseGyroController>();
            headOrientation3DOF.AddComponent<RotationalTrackingManager>();
        }
        else
        {
            Debug.LogError("No 3DOF Head Orientation object set in the MiraWikitudeManager inspector");
        }
    }

    /// <summary>
    /// Drawable Override is used to transalte th position and rotation of the Tracking Object according to the current state of the game.
    /// Only be used if Static Camera is set true on Wikitude Camera
    /// </summary>
    /// <param name="trackable">Image or Object Trackable.</param>
    /// <param name="target">Identified Image or Object Target.</param>
    /// <param name="position">Tracker Gameobject Position.</param>
    /// <param name="rotation">Tracker GameObject Rotation.</param>
    /// <param name="scale">Tracker GameObject Scale.</param>
    public override void DrawableOverride(Trackable trackable, RecognizedTarget target, ref Vector3 position, ref Quaternion rotation, ref Vector3 scale)
    {
        var imageTarget = target as ImageTarget;
        if (imageTarget != null)
        {
            // var augmentationToCamera = position - wikiCamera.transform.position;
            // float length = augmentationToCamera.magnitude;
            // Vector3 direction = augmentationToCamera.normalized;
            position = imageTarget.PhysicalTargetHeight * scaleMultiplier * position * 0.1f;
        }
    }

    /// <summary>
    /// Camera Overrise is used to transalte th position and rotation of the Mira Ar Camera according to the current state of the game.
    /// </summary>
    /// <param name="trackable">Image or Object Trackable.</param>
    /// <param name="camera">Mira Ar Camera Camera.</param>
    /// <param name="target">Identified Image or Object Target.</param>
    /// <param name="position">Mira Ar Camera  Position.</param>
    /// <param name="rotation">Mira Ar Camera Rotation.</param>
    /// <param name="scale">Mira Ar Camera Scale.</param>
    public override void CameraOverride(Trackable trackable, Transform camera, RecognizedTarget target, ref Vector3 position, ref Quaternion rotation, ref Vector3 scale)
    {
        var imageTarget = target as ImageTarget;
        if (imageTarget != null)
        {
            // Debug.Log("Position:" + position +", Cam Position: " + camera.position);
            position = imageTarget.PhysicalTargetHeight * scaleMultiplier * position * 0.1f + rotation * positionalOffset;
            rotation = rotation * rotationalOffset;
        }
    }

    /// <summary>
    /// Rescales the Tracker preview according to the scale.
    /// </summary>
    /// <param name="trackable">Trackable.</param>
    /// <param name="imageTargetHeight">Image target height.</param>
    /// <param name="position">Position.</param>
    /// <param name="rotation">Rotation.</param>
    /// <param name="scale">Scale.</param>
    public override void ImagePreviewOverride(Trackable trackable, float imageTargetHeight, ref Vector3 position, ref Quaternion rotation, ref Vector3 scale)
    {
        scale = new Vector3(imageTargetHeight, imageTargetHeight, imageTargetHeight) * 0.1f * 1 / MiraArController.Instance.setScaleMultiplier;
    }

    /// <summary>
    /// This Function can be used if you wish to load the tracking after the scene starts by disabling the MyImageTracker gameobject.
    /// </summary>
    public IEnumerator ActivateTracking()
    {
        yield return new WaitForSeconds(5.0f);
        if (imageTracker == null)
            Debug.LogError("No imageTracker set in MiraWikitudeManager inspector");
        else
            imageTracker.gameObject.SetActive(true);
    }

    #endregion Public Methods

    #region Private Variables

    // Make sure there are no conflicting settings
    private void SanityCheck()
    {
        if (MiraArController.Instance.isSpectator && ArCam.StaticCamera)
            Debug.LogError("Incompatible WikitudeCamera Settings - cannot be Spectator with Static Camera");
    }

    #endregion Private Variables
}