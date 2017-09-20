using UnityEngine;

/// <summary>
/// Accesses the phone's gyroscope data
/// </summary>
public class GyroController : MonoBehaviour
{
    #region [Singleton Instance]

    private static GyroController instance = null;

    public static GyroController Instance
    {
        get
        {
            if (instance == null)
            {
                instance = new GameObject().AddComponent<GyroController>();
                instance.name = "GyroController";
            }
            return instance;
        }
    }

    #endregion [Singleton Instance]

    #region [Private fields]

    public Quaternion gyroRotation = Quaternion.identity;

    private bool gyroEnabled = true;

    [Range(0, 1)]
    /// <summary>
    /// Determines the level of smoothing applied to gyroscope data
    /// </summary>
    public float lowPassFilterFactor = 1f;

    private readonly Quaternion baseIdentity = Quaternion.Euler(90, 0, 0);
    private readonly Quaternion landscapeRight = Quaternion.Euler(0, 0, 90);
    private readonly Quaternion landscapeLeft = Quaternion.Euler(0, 0, -90);
    private readonly Quaternion upsideDown = Quaternion.Euler(0, 0, 180);
    private readonly Quaternion frontCamera = Quaternion.AngleAxis(180, Vector3.up);

    private Quaternion cameraBase = Quaternion.identity;
    private Quaternion calibration = Quaternion.identity;
    private Quaternion baseOrientation = Quaternion.Euler(90, 0, 0);
    private Quaternion baseOrientationRotationFix = Quaternion.identity;

    private Quaternion referenceRotation = Quaternion.identity;

    private bool inverseGyroSetup = true;

    /// <summary>
    /// Whether or not to display the debug GUI.
    /// </summary>
    public bool debugGUI = false;

    /// <summary>
    /// Orientation matches back-facing camera when false, and front-facing camera when true.
    /// </summary>
    public bool useFrontCamera = true;

    private Quaternion rotationOffset;

    // private MiraRemoteClient remote;

    #endregion [Private fields]

    #region [Unity events]

    protected void Awake()
    {
        Application.targetFrameRate = 60;
    }

    protected Quaternion GetAttitude()
    {
#if UNITY_EDITOR

        return Input.gyro.attitude;
#else
        return Input.gyro.attitude;
#endif
    }

    protected void Start()
    {
        rotationOffset = Quaternion.Euler(-30, 0, 0);
        Input.gyro.enabled = true;
        AttachGyro();
    }

    protected void LateUpdate()
    {
#if UNITY_EDITOR
        if (UnityEditor.EditorApplication.isRemoteConnected == false)

            gyroEnabled = false;
        else
            gyroEnabled = true;
#endif
        if (!gyroEnabled)
            return;
        if (inverseGyroSetup)
        {
            gyroRotation = Quaternion.Slerp(gyroRotation,
                cameraBase * (ConvertRotation(referenceRotation * GetAttitude()) * GetRotFix()) * (useFrontCamera ? frontCamera : Quaternion.identity) * rotationOffset, lowPassFilterFactor);
        }
    }

    protected void OnGUI()
    {
        if (!debugGUI)
            return;

        GUILayout.Label("Orientation: " + Screen.orientation);
        GUILayout.Label("Calibration: " + calibration);
        GUILayout.Label("Camera base: " + cameraBase);
        GUILayout.Label("input.gyro.attitude: " + GetAttitude());
        GUILayout.Label("transform.rotation: " + transform.rotation);
        GUILayout.Label("front-facing: " + useFrontCamera);

        if (GUILayout.Button("On/off gyro: " + Input.gyro.enabled, GUILayout.Height(100)))
        {
            Input.gyro.enabled = !Input.gyro.enabled;
        }

        if (GUILayout.Button("On/off gyro controller: " + gyroEnabled, GUILayout.Height(100)))
        {
            if (gyroEnabled)
            {
                DetachGyro();
            }
            else
            {
                AttachGyro();
            }
        }

        if (GUILayout.Button("Update gyro calibration (Horizontal only)", GUILayout.Height(80)))
        {
            UpdateCalibration(true);
        }

        if (GUILayout.Button("Reset base orientation", GUILayout.Height(80)))
        {
            ResetBaseOrientation();
        }

        if (GUILayout.Button("Reset camera rotation", GUILayout.Height(80)))
        {
            Recenter(true);
        }

        if (GUILayout.Button("Update camera base rotation (Horizontal only)", GUILayout.Height(80)))
        {
            Recenter(true);
        }

        if (GUILayout.Button("Toggle front facing", GUILayout.Height(80)))
        {
            useFrontCamera = !useFrontCamera;
        }

        if (GUILayout.Button("Close", GUILayout.Height(80)))
        {
            debugGUI = false;
        }
    }

    #endregion [Unity events]

    #region [Public methods]

    /// <summary>
    /// Attaches gyro controller to the transform.
    /// </summary>
    public void AttachGyro()
    {
        gyroEnabled = true;
        ResetBaseOrientation();
        UpdateCalibration(true);
        UpdateCameraBaseRotation(true);
        RecalculateReferenceRotation();
    }

    /// <summary>
    /// Detaches gyro controller from the transform
    /// </summary>
    public void DetachGyro()
    {
        gyroEnabled = false;
    }

    #endregion [Public methods]

    #region [Private methods]

    /// <summary>
    /// Update the gyro calibration.
    /// </summary>
    private void UpdateCalibration(bool onlyHorizontal)
    {
        if (onlyHorizontal)
        {
            var fw = (GetAttitude()) * (-Vector3.forward);
            fw.z = 0;
            if (fw == Vector3.zero)
            {
                calibration = Quaternion.identity;
            }
            else
            {
                calibration = (Quaternion.FromToRotation(baseOrientationRotationFix * Vector3.up, fw));
            }
        }
        else
        {
            calibration = GetAttitude();
        }
    }

    /// <summary>
    /// Update the camera base rotation.
    /// </summary>
    /// <param name='onlyHorizontal'>
    /// Only y rotation.
    /// </param>
    private void UpdateCameraBaseRotation(bool onlyHorizontal)
    {
        if (onlyHorizontal)
        {
            if (gyroRotation == Quaternion.identity)
            {
                cameraBase = Quaternion.identity;
            }
            else
            {
                var fw = gyroRotation * Vector3.forward;
                fw.y = 0;
                gyroRotation =
                     (ConvertRotation(referenceRotation * GetAttitude()) * GetRotFix()) * (useFrontCamera ? frontCamera : Quaternion.identity) * rotationOffset;
                cameraBase = Quaternion.FromToRotation(Vector3.forward, fw);
            }
        }
        else
        {
            //cameraBase = transform.rotation;
        }
    }

    /// <summary>
    /// Recenter the camera's orientation
    /// </summary>
    /// <param name='onlyHorizontal'>
    /// Only y rotation.
    /// </param>
    public void Recenter(bool onlyHorizontal)
    {
        if (onlyHorizontal)
        {
            Quaternion previousRotation = gyroRotation;

            Quaternion pureGyroRotation =
                    (ConvertRotation(referenceRotation * GetAttitude()) * GetRotFix()) * (useFrontCamera ? frontCamera : Quaternion.identity) * rotationOffset;
            Vector3 fw = pureGyroRotation * Vector3.forward;
            fw.y = 0;
            if (fw == Vector3.zero)
            {
                cameraBase = Quaternion.identity;
            }
            else
            {
                cameraBase = (Quaternion.FromToRotation(fw, Vector3.forward));
            }

            gyroRotation = previousRotation;

            // }
        }
        // else {
        //     cameraBase = Quaternion.Inverse(Quaternion.Inverse(cameraBase) * transform.rotation);
        // }
    }

    /// <summary>
    /// Converts the rotation from right handed to left handed.
    /// </summary>
    /// <returns>
    /// The result rotation.
    /// </returns>
    /// <param name='q'>
    /// The rotation to convert.
    /// </param>
    private static Quaternion ConvertRotation(Quaternion q)
    {
        return new Quaternion(q.x, q.y, -q.z, -q.w);
    }

    /// <summary>
    /// Gets the rot fix for different orientations.
    /// </summary>
    /// <returns>
    /// The rot fix.
    /// </returns>
    private Quaternion GetRotFix()
    {
#if UNITY_3_5
    if (Screen.orientation == ScreenOrientation.Portrait)
        return Quaternion.identity;

    if (Screen.orientation == ScreenOrientation.LandscapeLeft || Screen.orientation == ScreenOrientation.Landscape)
        return landscapeLeft;

    if (Screen.orientation == ScreenOrientation.LandscapeRight)
        return landscapeRight;

    if (Screen.orientation == ScreenOrientation.PortraitUpsideDown)
        return upsideDown;

    return Quaternion.identity;
#else
        return Quaternion.identity;

#endif
    }

    /// <summary>
    /// Recalculates reference system.
    /// </summary>
    private void ResetBaseOrientation()
    {
        baseOrientationRotationFix = GetRotFix();
        baseOrientation = baseOrientationRotationFix * baseIdentity;
    }

    /// <summary>
    /// Recalculates reference rotation.
    /// </summary>
    private void RecalculateReferenceRotation()
    {
        referenceRotation = Quaternion.Inverse(baseOrientation) * Quaternion.Inverse(calibration);
    }

    #endregion [Private methods]
}