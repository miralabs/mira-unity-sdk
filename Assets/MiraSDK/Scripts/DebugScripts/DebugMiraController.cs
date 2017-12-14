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

using System.Text;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;

public class DebugMiraController : MonoBehaviour,
             IPointerEnterHandler, IPointerExitHandler, IPointerDownHandler, IPointerUpHandler, IPointerClickHandler
{
    public Text debugOutputText;
    private string output;

    private bool didPointerEnter = false;
    private bool didPointerExit = false;
    private bool didPointerClick = false;
    private bool didPointerDown = false;
    private bool didPointerUp = false;

    // these OnPointer functions are automatically called when
    // the pointer interacts with a game object that this script is attached to
    public void OnPointerEnter(PointerEventData pointerData)
    {
        didPointerEnter = true;
    }

    public void OnPointerExit(PointerEventData pointerData)
    {
        didPointerExit = true;
    }

    public void OnPointerClick(PointerEventData pointerData)
    {
        didPointerClick = true;
    }

    public void OnPointerDown(PointerEventData pointerData)
    {
        didPointerDown = true;
    }

    public void OnPointerUp(PointerEventData pointerData)
    {
        didPointerUp = true;
    }

    private void Start()
    {
    }

    private void LateUpdate()
    {
        // reset for next frame
        didPointerEnter = false;
        didPointerExit = false;
        didPointerClick = false;
        didPointerDown = false;
        didPointerUp = false;
    }

    private void Update()
    {
        StringBuilder sb = new StringBuilder();

        sb.AppendLine("<b>didPointerEnter</b>:  " + didPointerEnter);
        sb.AppendLine("<b>didPointerExit</b>:  " + didPointerExit);
        sb.AppendLine("<b>didPointerClick</b>:  " + didPointerClick);
        sb.AppendLine("<b>didPointerDown</b>:  " + didPointerDown);
        sb.AppendLine("<b>didPointerUp</b>:  " + didPointerUp);

        sb.AppendLine("<b>Position</b>:  " + MiraController.Position);
        sb.AppendLine("<b>Orientation</b>:  " + MiraController.Orientation);
		sb.AppendLine("<b>Direction</b>:  " + MiraController.Direction);

        sb.AppendLine("<b>Gyroscope</b>:  " + MiraController.Gyro);
        sb.AppendLine("<b>Accel</b>:  " + MiraController.Accel);
        sb.AppendLine("<b>TouchPos</b>:  " + MiraController.TouchPos);

        sb.AppendLine("<b>TouchHeld</b>:  " + MiraController.TouchHeld);
        sb.AppendLine("<b>TouchPressed</b>:  " + MiraController.TouchPressed);
        sb.AppendLine("<b>TouchReleased</b>:  " + MiraController.TouchReleased);

        sb.AppendLine("<b>StartButton</b>:  " + MiraController.StartButton);
        sb.AppendLine("<b>StartButtonPressed</b>:  " + MiraController.StartButtonPressed);
        sb.AppendLine("<b>StartButtonReleased</b>:  " + MiraController.StartButtonReleased);

        sb.AppendLine("<b>ClickButton</b>:  " + MiraController.ClickButton);
        sb.AppendLine("<b>ClickButtonPressed</b>:  " + MiraController.ClickButtonPressed);
        sb.AppendLine("<b>ClickButtonReleased</b>:  " + MiraController.ClickButtonReleased);

        sb.AppendLine("<b>TouchpadButton</b>:  " + MiraController.TouchpadButton);
        sb.AppendLine("<b>TouchpadButtonPressed</b>:  " + MiraController.TouchpadButtonPressed);
        sb.AppendLine("<b>TouchpadButtonReleased</b>:  " + MiraController.TouchpadButtonReleased);

        sb.AppendLine("<b>TriggerButton</b>:  " + MiraController.TriggerButton);
        sb.AppendLine("<b>TriggerButtonPressed</b>:  " + MiraController.TriggerButtonPressed);
        sb.AppendLine("<b>TriggerButtonReleased</b>:  " + MiraController.TriggerButtonReleased);

        sb.AppendLine("<b>BackButton</b>:  " + MiraController.BackButton);
        sb.AppendLine("<b>BackButtonPressed</b>:  " + MiraController.BackButtonPressed);
        sb.AppendLine("<b>BackButtonReleased</b>:  " + MiraController.BackButtonReleased);

        sb.AppendLine("<b>UpButton</b>:  " + MiraController.UpButton);
        sb.AppendLine("<b>UpButtonPressed</b>:  " + MiraController.UpButtonPressed);
        sb.AppendLine("<b>UpButtonReleased</b>:  " + MiraController.UpButtonReleased);

        sb.AppendLine("<b>DownButton</b>:  " + MiraController.DownButton);
        sb.AppendLine("<b>DownButtonPressed</b>:  " + MiraController.DownButtonPressed);
        sb.AppendLine("<b>DownButtonReleased</b>:  " + MiraController.DownButtonReleased);

        sb.AppendLine("<b>LeftButton</b>:  " + MiraController.LeftButton);
        sb.AppendLine("<b>LeftButtonPressed</b>:  " + MiraController.LeftButtonPressed);
        sb.AppendLine("<b>LeftButtonReleased</b>:  " + MiraController.LeftButtonReleased);

        sb.AppendLine("<b>RightButton</b>:  " + MiraController.RightButton);
        sb.AppendLine("<b>RightButtonPressed</b>:  " + MiraController.RightButtonPressed);
        sb.AppendLine("<b>RightButtonReleased</b>:  " + MiraController.RightButtonReleased);

        debugOutputText.text = sb.ToString();
    }
}