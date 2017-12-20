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

using Mira;
using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.XR.iOS;

/// <summary>
/// MiraController provides the controller's current state including button input and orientation
/// </summary>
public class MiraController : MonoBehaviour
{
	#if UNITY_EDITOR
	private MiraLivePreviewEditor _mrc;
	#endif
    // static singleton property
    private static MiraController _instance;

    public static MiraController Instance
    {
        get
        {
            if (_instance == null)
            {
                _instance = GameObject.FindObjectOfType<MiraController>();
                if (_instance == null)
                {
                    Debug.Log("Warning: no MiraController was found, returning null");
                }
            }
            return _instance;
        }
    }

    private void ValidateInstance()
    {
        if (_instance != null && _instance != this)
        {
            Debug.Log("There were multiple MiraController instances in your scene, destroying one");
            Destroy(this.gameObject);
        }
        else
        {
            _instance = this;
        }
    }

    /// <summary>
    /// Controller type defines what type of user input should we expect
    /// </summary>
    public enum ControllerType
    {
        PrismRemote = 0,
        MiraVirtual = 1,
    }

    public enum Handedness
    {
        Right = 0,
        Left = 1
    }

    public Handedness handedness = Handedness.Right;

    /// <summary>
    /// The type of the controller.
    /// </summary>
    public ControllerType controllerType;

    /// <summary>
    /// ClickChoices defines what button(s) are eligible for click events.
    /// </summary>
    public enum ClickChoices
    {
        TouchpadButton = 0,
        TriggerButton = 1,
        BothTouchpadAndTrigger = 2
    }

    /// <summary>
    ///  WhatButtonIsClick defines what button should trigger a click event, currently this cannot be changed at runtime.
    /// </summary>
    public ClickChoices WhatButtonIsClick;

    public MiraBTRemoteInput userInput {get {return _userInput; } }

    private static MiraBTRemoteInput _userInput;

    /// <summary>
    /// The stereo camera rig.
    /// </summary>
    private Transform cameraRig;

    /// <summary>
    /// The rotational tracking game object's transform
    /// </summary>
    private Transform rotationalTracking;

    /// <summary>
    /// Does the controller also track Position?
    /// </summary>
    private bool trackPosition = false;

    private bool shouldFollowHead = true;

    /// <summary>
    /// The scene scale multiplier, setting lower than 100 dampens arm model positions
    /// </summary>
    private float sceneScaleMult = 10f;

    /// <summary>
    /// Offset of Arm Model in X (in CM)
    /// </summary>
    [SerializeField]
    private float armOffsetX = 0f;

    /// <summary>
    /// Offset of Arm Model in Y (in CM)
    /// </summary>
    [SerializeField]
    private float armOffsetY = -40f;

    /// <summary>
    /// Offset of Arm Model in Z (in CM)
    /// </summary>
    [SerializeField]
    private float armOffsetZ = -25f;

    //
    /// <summary>
    /// The base controller representing the last raw controller rotation when centered
    /// </summary>
    private Quaternion baseController;

    /// <summary>
    /// The offset position, stores the base arm offset (pivot location) when not using the arm model
    /// </summary>
    private Vector3 offsetPos;

    private bool isRotationalMode = false;

    [SerializeField]
    private float angleTiltX = 15;
    [SerializeField]
    private float angleTiltY = -5;

    private GameObject returnToHome;

    private Image radialTimer;


    void OnEnable()
    {
        RemoteManager.Instance.OnRemoteConnected += RemoteConnected;
		RemoteManager.Instance.OnRemoteDisconnected += RemoteDisconnected;
    }
    void OnDisable()
    {
        RemoteManager.Instance.OnRemoteConnected -= RemoteConnected;
		RemoteManager.Instance.OnRemoteDisconnected -= RemoteDisconnected;
    }
    private void Awake()
    {
        ValidateInstance();
        controllerType = ControllerType.PrismRemote;
        _userInput = new MiraBTRemoteInput();

    }

    public void assignController()
    {
		
        if (_userInput.init() != true)
        {
            Debug.Log("controller setup failed");
        }
    }

    private void Start()
    {
        
        Debug.Log("Creating Bluetooth Remote Input");	
        assignClickFunctionality();		

        cameraRig = MiraArController.Instance.cameraRig.transform;
          // Rotational tracking transform
        if (MiraArController.Instance.isRotationalOnly == false)
        {
            rotationalTracking = MiraWikitudeManager.Instance.headOrientation3DOF.transform;

            isRotationalMode = false;
        }
        else
        {
            isRotationalMode = true;
        }

        StartCoroutine(StartController());
    }
    private IEnumerator StartController()
    {
        

        sceneScaleMult *= (1 / MiraArController.scaleMultiplier);
        if (handedness == Handedness.Left)
        {
            armOffsetX = -armOffsetX;
        }

        // Initialize baseController rotation as identity
        baseController = Quaternion.identity;

        // Scale arm offsets to scene scale
        armOffsetX *= (1 / MiraArController.scaleMultiplier);
        armOffsetY *= (1 / MiraArController.scaleMultiplier);
        armOffsetZ *= (1 / MiraArController.scaleMultiplier);

        offsetPos = new Vector3(armOffsetX, armOffsetY, armOffsetZ);

        // If not using the arm model, set position to offsetPos
        if (trackPosition == false)
        {
            transform.localPosition = offsetPos;
        }

        returnToHome = Instantiate(Resources.Load("RadialHomeTimer", typeof(GameObject))) as GameObject;
        // homeRadialTimer.transform.localScale = (100 * (1/MiraArController.scaleMultiplier)) * Vector3.one;
        // homeRadialTimer.transform.SetParent(MiraArController.Instance.cameraRig.transform);
        // Vector3 relativePosition = (100 * (1/MiraArController.scaleMultiplier)) * new Vector3(0,0,0.7f);
        // homeRadialTimer.transform.localPosition = relativePosition;
        radialTimer = returnToHome.GetComponentInChildren<Image>();

        returnToHome.SetActive(false);
        //radialTimer.gameObject.SetActive(false);


        // shoulderRefPt = new GameObject("ShoulderRefPt").transform;
        // shoulderRefPt.parent = cameraRig;
        // shoulderRefPt.localPosition = offsetPos;

        StartCoroutine(DelayedRecenter());

        yield return null;
        
    }

    void RemoteConnected(Remote remote, EventArgs args)
    {
        Debug.Log("Controller MRRemote connected");
		//YK moving from Awake to make sure Controller Type can be set
		assignController();
    }
    void RemoteDisconnected(Remote remote, EventArgs args)
    {
        MiraController._userInput.OnControllerDisconnected();
    }
    IEnumerator DelayedRecenter()
    {
        yield return new WaitForSeconds(0.5f);
        RecenterAll();

    }


    private void RotateController()
    {
        if (isRotationalMode)
        {
            // Controller rotation = raw rotation relative to the last centered rotation
            transform.rotation = baseController * _userInput.Orientation * Quaternion.Euler(angleTiltX, angleTiltY, 0);
        }
        else
        {
            // Set Controller Rotation to the RotationalTracking rotation, then add on the controller raw rotation relative to the controller last centered "base" rotation
            transform.rotation = rotationalTracking.rotation * baseController * _userInput.Orientation * Quaternion.Euler(angleTiltX, angleTiltY, 0);
        }
    }

    /// <summary>
    /// Recenters the controller
    /// </summary>
    /// <returns>The all.</returns>
    public void RecenterAll()
    {

        Quaternion previousRotation = transform.rotation;
        transform.rotation = _userInput.Orientation;
        Vector3 controllerFw = transform.forward;
        controllerFw.y = 0;
        baseController = Quaternion.Inverse(Quaternion.FromToRotation(Vector3.forward, controllerFw));
        Debug.Log("Recenter");
        GyroController.Instance.Recenter(true);
     
        transform.rotation = previousRotation;

       
     
    }

    private void FollowHead()
    {
        // rotate the offset position by the camera rotation and then add it to the camera position
        transform.position = cameraRig.position + cameraRig.rotation * offsetPos;
    }


    public void updateControllerTransform()
    {
        if (shouldFollowHead)
        {
            FollowHead();
        }
        RotateController();
    }

    IEnumerator ReturnToHome()
    {
        float timer = 0f;
        bool animStarted = false;
        float animProgress = 0f;
        while(MiraController.StartButton)
        {
            
            timer += Time.deltaTime;
            if(timer > 0.5f && animStarted == false)
            {
                // Start Anim (Instantiate radial timer here)
                animStarted = true;
                returnToHome.SetActive(true);
            }
            if(animStarted == true)
            {
                // anim progress goes from 0 to 1 from 0.5 seconds to 2 seconds
                animProgress = (timer - 0.5f)/1.5f;
                // Radial animation timer here
                radialTimer.fillAmount = animProgress;
    
            }
            
            if(timer >= 2)
            {
                // Go back to Home
                Debug.Log("Returning to Mira Home App");
                timer = 0;
            }

            yield return null;
        }
    }

    IEnumerator RecenterAnim()
    {
        float animTime = 0.5f;
        Transform reticle = MiraReticle.Instance.transform;
        Material reticleVis = reticle.GetComponent<Renderer>().material;
        Color originalColor = new Color();
        Color fadeColor = new Color();
        if(reticleVis)
        {
            originalColor = reticleVis.GetColor("_ColorMult");
            originalColor.a = 1;
            fadeColor = originalColor;

        }

        float timer = 0f;
        while(timer < animTime)
        {
            MiraReticle.Instance.externalMultiplier = 1 + (timer * 50);

            if(reticleVis != null)
            {
                reticleVis.SetColor("_ColorMult", fadeColor);
                fadeColor.a = 1 - (timer/animTime);
            }

            timer += Time.deltaTime;
            yield return null;
        }
        MiraReticle.Instance.externalMultiplier = 1;
        if(reticleVis)
        {
            reticleVis.SetColor("_ColorMult", originalColor);
        }
        
    }

    private void LateUpdate()
    {
        
        if (MiraController.Instance != null)
        {
           
            updateControllerTransform();

            if (MiraController.StartButtonPressed)
            {
                StartCoroutine(ReturnToHome());
            }

            if (MiraController.StartButtonReleased)
            {
                RecenterAll();
                StartCoroutine(RecenterAnim());
                returnToHome.SetActive(false);
                
            }



#if UNITY_EDITOR
            // DELETE ME
            if (Input.GetKeyDown(KeyCode.R))
            {
                RecenterAll();
            }
#endif

            //Updating LastFrame
            // MiraController._userInput.UpdateLastFrameButtonData();
        }
        
    }



	/// <summary>
	/// Gets the direction from the controller to the reticle, 
	/// this is often better than the raw forward direction of the controller for representing pointing.
	/// </summary>
	/// <value>The direction from the controller to the reticle.</value>
	public static Vector3 Direction
	{
		get
		{
            if(_instance != null)
			    return MiraPointerManager.PointerGameObject.transform.position - _instance.gameObject.transform.position;
			else
                return Vector3.zero;
		}
	}

    /// <summary>
    /// Gets the transform of the controller game object
    /// </summary>
    /// <value>The position.</value>
    public static Transform Transform
    {
        get
        {
            return _instance != null ? _instance.gameObject.transform : null;
        }
    }

    /// <summary>
    /// Gets the position
    /// </summary>
    /// <value>The position.</value>
    public static Vector3 Position
    {
        get
        {
            return _instance != null ? _instance.gameObject.transform.position : Vector3.zero;
        }
    }

    /// <summary>
    /// Gets the orientation of the controller as a quaternion
    /// </summary>
    /// <value>The orientation.</value>
    public static Quaternion Orientation
    {
        get
        {
            return _instance != null ? _instance.gameObject.transform.rotation : Quaternion.identity;
        }
    }

    /// <summary>
    /// Gets the gyroscope reading
    /// </summary>
    /// <value>The gyroscope reading</value>
    public static Vector3 Gyro
    {
        get
        {
            return _instance != null ? _userInput.Gyro : Vector3.zero;
        }
    }

    /// <summary>
    /// Gets the accelerometer reading
    /// </summary>
    /// <value>The accelerometer reading</value>
    public static Vector3 Accel
    {
        get
        {
            return _instance != null ? _userInput.Gyro : Vector3.zero;
        }
    }

    /// <summary>
    /// Gets a value indicating whether the user is touching the touchpad
    /// </summary>
    /// <value><c>true</c> if user is touching touchpad; otherwise, <c>false</c>.</value>
    public static bool TouchHeld
    {
        get
        {
            return _instance != null ? _userInput.TouchHeld : false;
        }
    }

    /// <summary>
    /// Gets a value indicating whether the user just stopped touching the touchpad
    /// </summary>
    /// <value><c>true</c> if user just stopped touching touchpad; otherwise, <c>false</c>.</value>
    public static bool TouchReleased
    {
        get
        {
            return _instance != null ? _userInput.TouchReleased : false;
        }
    }

    /// <summary>
    /// Gets a value indicating whether the user just started touching the touchpad
    /// </summary>
    /// <value><c>true</c> if user just started touching touchpad; otherwise, <c>false</c>.</value>
    public static bool TouchPressed
    {
        get
        {
            return _instance != null ? _userInput.TouchPressed : false;
        }
    }

    /// <summary>
    /// Gets the touch position on the touchpad
    /// </summary>
    /// <value>The touch position.</value>
    public static Vector2 TouchPos
    {
        get
        {
            return _instance != null ? _userInput.TouchPos : Vector2.zero;
        }
    }

    /// <summary>
    /// Gets a value indicating whether the user is holding down the StartButton
    /// </summary>
    /// <value><c>true</c> if user is holding down the StartButton; otherwise, <c>false</c>.</value>
    public static bool StartButton
    {
        get
        {
            return _instance != null ? _userInput.StartButton : false;
        }
    }

    /// <summary>
    /// Gets a value indicating whether the user just stopped pressing the StartButton
    /// </summary>
    /// <value><c>true</c> if user just stopped pressing the StartButton; otherwise, <c>false</c>.</value>
    public static bool StartButtonReleased
    {
        get
        {
            return _instance != null ? _userInput.StartButtonReleased : false;
        }
    }

    /// <summary>
    /// Gets a value indicating whether the user just started pressing the StartButton
    /// </summary>
    /// <value><c>true</c> if user just started pressing the StartButton; otherwise, <c>false</c>.</value>
    public static bool StartButtonPressed
    {
        get
        {
            return _instance != null ? _userInput.StartButtonPressed : false;
        }
    }

    /// <summary>
    /// Gets a value indicating whether the user is holding down the ClickButton, this could be Touchpad, Trigger, or Both
    /// </summary>
    /// <value><c>true</c> if user is holding down the ClickButton; otherwise, <c>false</c>.</value>
    public static bool ClickButton
    {
        get
        {
            if (_instance != null)
            {
                for (int i = 0; i < evaluateClickButton.Count; i++)
                {
                    if (evaluateClickButton[i]())
                    {
                        return true;
                    }
                }
                return false;
            }
            else
            {
                return false;
            }
        }
    }

    /// <summary>
    /// Gets a value indicating whether the user just stopped pressing the ClickButton, this could be Touchpad, Trigger, or Both
    /// </summary>
    /// <value><c>true</c> if user just stopped pressing the ClickButton; otherwise, <c>false</c>.</value>
    public static bool ClickButtonReleased
    {
        get
        {
            if (_instance != null)
            {
                for (int i = 0; i < evaluateClickButtonReleased.Count; i++)
                {
                    if (evaluateClickButtonReleased[i]())
                    {
                        return true;
                    }
                }
                return false;
            }
            else
            {
                return false;
            }
        }
    }

    /// <summary>
    /// Gets a value indicating whether the user just started pressing the ClickButton , this could be Touchpad, Trigger, or Both
    /// </summary>
    /// <value><c>true</c> if user just started pressing the ClickButton; otherwise, <c>false</c>.</value>
    public static bool ClickButtonPressed
    {
        get
        {
            if (_instance != null)
            {
                for (int i = 0; i < evaluateClickButtonPressed.Count; i++)
                {
                    if (evaluateClickButtonPressed[i]())
                    {
                        return true;
                    }
                }
                return false;
            }
            else
            {
                return false;
            }
        }
    }

    /// <summary>
    /// Gets a value indicating whether the user is holding down the TouchpadButton
    /// </summary>
    /// <value><c>true</c> if user is holding down the TriggerButton; otherwise, <c>false</c>.</value>
    public static bool TouchpadButton
    {
        get
        {
            return _instance != null ? _userInput.TouchpadButton : false;
        }
    }

    /// <summary>
    /// Gets a value indicating whether the user just stopped pressing the TouchpadButton
    /// </summary>
    /// <value><c>true</c> if user just stopped pressing the TriggerButton; otherwise, <c>false</c>.</value>
    public static bool TouchpadButtonReleased
    {
        get
        {
            return _instance != null ? _userInput.TouchpadButtonReleased : false;
        }
    }

    /// <summary>
    /// Gets a value indicating whether the user just started pressing the TouchpadButton
    /// </summary>
    /// <value><c>true</c> if user just started pressing the TriggerButton; otherwise, <c>false</c>.</value>
    public static bool TouchpadButtonPressed
    {
        get
        {
            return _instance != null ? _userInput.TouchpadButtonPressed : false;
        }
    }

    /// <summary>
    /// Gets a value indicating whether the user is holding down the TriggerButton
    /// </summary>
    /// <value><c>true</c> if user is holding down the TriggerButton; otherwise, <c>false</c>.</value>
    public static bool TriggerButton
    {
        get
        {
            return _instance != null ? _userInput.TriggerButton : false;
        }
    }

    /// <summary>
    /// Gets a value indicating whether the user just stopped pressing the TriggerButton
    /// </summary>
    /// <value><c>true</c> if user just stopped pressing the TriggerButton; otherwise, <c>false</c>.</value>
    public static bool TriggerButtonReleased
    {
        get
        {
            return _instance != null ? _userInput.TriggerButtonReleased : false;
        }
    }

    /// <summary>
    /// Gets a value indicating whether the user just started pressing the TriggerButton
    /// </summary>
    /// <value><c>true</c> if user just started pressing the TriggerButton; otherwise, <c>false</c>.</value>
    public static bool TriggerButtonPressed
    {
        get
        {
            return _instance != null ? _userInput.TriggerButtonPressed : false;
        }
    }

    /// <summary>
    /// Gets a value indicating whether the user is holding down the BackButton
    /// </summary>
    /// <value><c>true</c> if user is holding down the BackButton; otherwise, <c>false</c>.</value>
    public static bool BackButton
    {
        get
        {
            return _instance != null ? _userInput.BackButton : false;
        }
    }

    /// <summary>
    /// Gets a value indicating whether the user just stopped pressing the BackButton
    /// </summary>
    /// <value><c>true</c> if user just stopped pressing the BackButton; otherwise, <c>false</c>.</value>
    public static bool BackButtonReleased
    {
        get
        {
            return _instance != null ? _userInput.BackButtonReleased : false;
        }
    }

    /// <summary>
    /// Gets a value indicating whether the user just started pressing the BackButton
    /// </summary>
    /// <value><c>true</c> if user just started pressing the BackButton; otherwise, <c>false</c>.</value>
    public static bool BackButtonPressed
    {
        get
        {
            return _instance != null ? _userInput.BackButtonPressed : false;
        }
    }

    /// <summary>
    /// Gets a value indicating whether the user is holding down the UpButton
    /// </summary>
    /// <value><c>true</c> if user is holding down the UpButton; otherwise, <c>false</c>.</value>
    public static bool UpButton
    {
        get
        {
            return _instance != null ? _userInput.UpButton : false;
        }
    }

    /// <summary>
    /// Gets a value indicating whether the user just stopped pressing the UpButton
    /// </summary>
    /// <value><c>true</c> if user just stopped pressing the UpButton; otherwise, <c>false</c>.</value>
    public static bool UpButtonReleased
    {
        get
        {
            return _instance != null ? _userInput.UpButtonReleased : false;
        }
    }

    /// <summary>
    /// Gets a value indicating whether the user just started pressing the UpButton
    /// </summary>
    /// <value><c>true</c> if user just started pressing the UpButton; otherwise, <c>false</c>.</value>
    public static bool UpButtonPressed
    {
        get
        {
            return _instance != null ? _userInput.UpButtonPressed : false;
        }
    }

    /// <summary>
    /// Gets a value indicating whether the user is holding down the DownButton
    /// </summary>
    /// <value><c>true</c> if user is holding down the DownButton; otherwise, <c>false</c>.</value>
    public static bool DownButton
    {
        get
        {
            return _instance != null ? _userInput.DownButton : false;
        }
    }

    /// <summary>
    /// Gets a value indicating whether the user just stopped pressing the DownButton
    /// </summary>
    /// <value><c>true</c> if user just stopped pressing the DownButton; otherwise, <c>false</c>.</value>
    public static bool DownButtonReleased
    {
        get
        {
            return _instance != null ? _userInput.DownButtonReleased : false;
        }
    }

    /// <summary>
    /// Gets a value indicating whether the user just started pressing the DownButton
    /// </summary>
    /// <value><c>true</c> if user just started pressing the DownButton; otherwise, <c>false</c>.</value>
    public static bool DownButtonPressed
    {
        get
        {
            return _instance != null ? _userInput.DownButtonPressed : false;
        }
    }

    /// <summary>
    /// Gets a value indicating whether the user is holding Left the LeftButton
    /// </summary>
    /// <value><c>true</c> if user is holding Left the LeftButton; otherwise, <c>false</c>.</value>
    public static bool LeftButton
    {
        get
        {
            return _instance != null ? _userInput.LeftButton : false;
        }
    }

    /// <summary>
    /// Gets a value indicating whether the user just stopped pressing the LeftButton
    /// </summary>
    /// <value><c>true</c> if user just stopped pressing the LeftButton; otherwise, <c>false</c>.</value>
    public static bool LeftButtonReleased
    {
        get
        {
            return _instance != null ? _userInput.LeftButtonReleased : false;
        }
    }

    /// <summary>
    /// Gets a value indicating whether the user just started pressing the LeftButton
    /// </summary>
    /// <value><c>true</c> if user just started pressing the LeftButton; otherwise, <c>false</c>.</value>
    public static bool LeftButtonPressed
    {
        get
        {
            return _instance != null ? _userInput.LeftButtonPressed : false;
        }
    }

    /// <summary>
    /// Gets a value indicating whether the user is holding Right the RightButton
    /// </summary>
    /// <value><c>true</c> if user is holding Right the RightButton; otherwise, <c>false</c>.</value>
    public static bool RightButton
    {
        get
        {
            return _instance != null ? _userInput.RightButton : false;
        }
    }

    /// <summary>
    /// Gets a value indicating whether the user just stopped pressing the RightButton
    /// </summary>
    /// <value><c>true</c> if user just stopped pressing the RightButton; otherwise, <c>false</c>.</value>
    public static bool RightButtonReleased
    {
        get
        {
            return _instance != null ? _userInput.RightButtonReleased : false;
        }
    }

    /// <summary>
    /// Gets a value indicating whether the user just started pressing the RightButton
    /// </summary>
    /// <value><c>true</c> if user just started pressing the RightButton; otherwise, <c>false</c>.</value>
    public static bool RightButtonPressed
    {
        get
        {
            return _instance != null ? _userInput.RightButtonPressed : false;
        }
    }

    /// <summary>
    /// you're probably looking for ClickButton,
    /// this is for calculating what that property returns
    /// </summary>
    private static List<Func<bool>> evaluateClickButton
    {
        get;
        set;
    }

    /// <summary>
    /// you're probably looking for ClickButtonPressed,
    /// this is for calculating what that property returns
    /// </summary>
    private static List<Func<bool>> evaluateClickButtonPressed
    {
        get;
        set;
    }

    /// <summary>
    /// you're probably looking for ClickButtonReleased,
    /// this is for calculating what that property returns
    /// </summary>
    private static List<Func<bool>> evaluateClickButtonReleased
    {
        get;
        set;
    }

    // these are only functions wrapping the properties defining the buttons
    // so they can be called via Func's and shouldn't be altered to do anything else
    private bool getTouchPadButton()
    {
        return TouchpadButton;
    }

    private bool getTouchPadButtonPressed()
    {
        return TouchpadButtonPressed;
    }

    private bool getTouchPadButtonReleased()
    {
        return TouchpadButtonReleased;
    }

    private bool getTriggerButton()
    {
        return TriggerButton;
    }

    private bool getTriggerButtonPressed()
    {
        return TriggerButtonPressed;
    }

    private bool getTriggerButtonReleased()
    {
        return TriggerButtonReleased;
    }

    private void assignClickFunctionality()
    {
        if (_userInput != null)
        {
            Debug.Log("Assigning Click Func");
            evaluateClickButton = new List<Func<bool>>();
            evaluateClickButtonPressed = new List<Func<bool>>();
            evaluateClickButtonReleased = new List<Func<bool>>();
            if (WhatButtonIsClick == ClickChoices.TouchpadButton)
            {
                evaluateClickButton.Add(getTouchPadButton);
                evaluateClickButtonPressed.Add(getTouchPadButtonPressed);
                evaluateClickButtonReleased.Add(getTouchPadButtonReleased);
            }
            else if (WhatButtonIsClick == ClickChoices.TriggerButton)
            {
                evaluateClickButton.Add(getTriggerButton);
                evaluateClickButtonPressed.Add(getTriggerButtonPressed);
                evaluateClickButtonReleased.Add(getTriggerButtonReleased);
            }
            else if (WhatButtonIsClick == ClickChoices.BothTouchpadAndTrigger)
            {
                evaluateClickButton.Add(getTouchPadButton);
                evaluateClickButtonPressed.Add(getTouchPadButtonPressed);
                evaluateClickButtonReleased.Add(getTouchPadButtonReleased);
                evaluateClickButton.Add(getTriggerButton);
                evaluateClickButtonPressed.Add(getTriggerButtonPressed);
                evaluateClickButtonReleased.Add(getTriggerButtonReleased);
            }
        }
    }
}
