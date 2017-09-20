using Mira;
using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

/// <summary>
/// MiraController provides the controller's current state including button input and orientation
/// </summary>
public class MiraController : MonoBehaviour
{
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

    private void validateInstance()
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
        XImmerse = 0
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

    private static MiraUserInput _userInput;

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

    private Transform shoulderRefPt;

    private bool isRotationalMode = false;

    [SerializeField]
    private float angleTilt = -15;

    private void Awake()
    {
        validateInstance();
        assignController();
        assignClickFunctionality();
    }

    private void assignController()
    {
        if (controllerType == ControllerType.XImmerse)
        {
            _userInput = new MiraXImmerseInput();
        }
        if (_userInput.init() != true)
        {
            Debug.Log("controller setup failed");
        }
    }

    private void Start()
    {
        // Reference to main camera transform
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

        // shoulderRefPt = new GameObject("ShoulderRefPt").transform;
        // shoulderRefPt.parent = cameraRig;
        // shoulderRefPt.localPosition = offsetPos;
    }

    private void RotateController()
    {
        if (isRotationalMode)
        {
            // Controller rotation = raw rotation relative to the last centered rotation
            transform.rotation = baseController * _userInput.Orientation * Quaternion.Euler(angleTilt, 0, 0);
        }
        else
        {
            // Set Controller Rotation to the RotationalTracking rotation, then add on the controller raw rotation relative to the controller last centered "base" rotation
            transform.rotation = rotationalTracking.rotation * baseController * _userInput.Orientation * Quaternion.Euler(angleTilt, 0, 0);
        }
    }

    /// <summary>
    /// Recenters the controller
    /// </summary>
    /// <returns>The all.</returns>
    private IEnumerator RecenterAll()
    {
        // base is the Inverse of the Current Rotation
        // Previous solution (commented out below) Leads to weird slant when recentered when controller is rolled
        // baseController = Quaternion.Inverse(_userInput.Orientation);
        // This solution isn't affected by roll. Awww yisss
        Quaternion previousRotation = transform.rotation;
        transform.rotation = _userInput.Orientation;
        Vector3 controllerFw = transform.forward;
        controllerFw.y = 0;
        baseController = Quaternion.Inverse(Quaternion.FromToRotation(Vector3.forward, controllerFw));
        Debug.Log("Recenter");
        // Recenter the gyro as well!
        GyroController.Instance.Recenter(true);
        // camGyro.InitializeCamGyro();
        // inverseGyro.CompletelyResetBaseRotation();
        // Gotta reset it back, so it doesn't jitter all crazy-like.
        transform.rotation = previousRotation;
        // Wait a frame so the following methods have the correct values
        yield return null;
        // Quaternion gyroRotation;
        // if (horizontalOnly)
        // {
        // 	// If horizontal only, set gyroRotation to the rotation from world forward to the current gyroforward
        // 	Vector3 fw = gyro.transform.forward;
        // 	baseHorizontal = fw.y;
        // 	fw.y = 0;
        // 	gyroRotation = Quaternion.FromToRotation(Vector3.forward, fw);

        // }
        // else
        // {
        // 	gyroRotation = gyro.transform.rotation;
        // }
        // // Since this rotation is relative to the previous base rotation, you have to take the inverse of that (since it's an inverse, you get an original rotation).
        // // Add on the current rotation, then add on the inverse of the current gyro rotation
        // offsetRotation =  Quaternion.Inverse(Quaternion.Inverse(offsetRotation) * transform.rotation * Quaternion.Inverse(gyroRotation));
        // yield return null;
    }

    private void FollowHead()
    {
        // rotate the offset position by the camera rotation and then add it to the camera position
        transform.position = cameraRig.position + cameraRig.rotation * offsetPos;
    }

    // void TranslateController()
    // {
    // 	// If TrackPosition == true
    // 	// Set local position to offsetPosition defined above
    // 	// transform.localPosition = offsetPos;
    // 	transform.position = shoulderRefPt.position;
    // 	// Add the controller Arm Model Position (scaled)
    // 	transform.localPosition += (_userInput.Position * sceneScaleMult);

    // }

    public void updateControllerTransform()
    {
        if (shouldFollowHead)
        {
            FollowHead();
        }
        RotateController();
    }

    private void LateUpdate()
    {
        if (MiraController.Instance != null)
        {
            updateControllerTransform();

            if (MiraController.StartButtonPressed)
            {
                StartCoroutine(RecenterAll());
            }

#if UNITY_EDITOR
            // DELETE ME
            if (Input.GetKeyDown(KeyCode.R))
            {
                StartCoroutine(RecenterAll());
            }
#endif
        }
    }

    // Update is called once per frame
    private void Update()
    {
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
			Vector3 dir =  MiraPointerManager.PointerGameObject.transform.position - _instance.gameObject.transform.position;
			return _instance != null ? dir : Vector3.zero;
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