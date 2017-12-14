using UnityEngine;
using Wikitude;

public class ExposureController : MonoBehaviour
{
    public WikitudeCamera WikiCamera;

    private void Start()
    {
        WikiCamera.DevicePosition = CaptureDevicePosition.Front;
    }

    private void Update()
    {
        if (Input.GetMouseButtonUp(0))
        {
            // This works for landscape right orientation
            // See exposurePointOfInterest documentation for more information
            Vector2 exposePosition = new Vector2(Input.mousePosition.x / Screen.width, Input.mousePosition.y / Screen.height);
            WikiCamera.ExposeAtPointOfInterest(exposePosition, CaptureExposureMode.ContinuousAutoExpose);
        }
    }
}