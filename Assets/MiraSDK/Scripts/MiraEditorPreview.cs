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
    /// Simulate tracking using the mouse and keyboard in the editor
    /// Click and drag the mouse to look around the scene (as a replacement for the Unity Remote App)
    /// Use "WASD" to orbit the camera around the tracker
    /// </summary>
    public class MiraEditorPreview : MonoBehaviour
    {
#if UNITY_EDITOR
        private Vector2 _mouseAbsolute;
        private Vector2 _smoothMouse;

        private Vector2 clampInDegrees = new Vector2(360, 180);
        private Vector2 sensitivity = new Vector2(2, 2);
        private Vector2 smoothFactor = new Vector2(3, 3);
        private Vector2 targetDirection;
        private Vector2 targetCharacterDirection;

        private float moveAmount = 1;
        private float rotationAmount = 2f;

        private Quaternion additionalCamRotation = Quaternion.identity;
        private Quaternion verticalOffset = Quaternion.identity;

        private void Start()
        {
            // Set target direction to the camera's initial orientation.
            targetDirection = transform.localRotation.eulerAngles;

            // Set target direction for the character body to its inital state.

            moveAmount *= (1 / MiraArController.scaleMultiplier);
        }

        private void CheckCameraMovement()
        {
            if (Input.GetKey(KeyCode.W))
            {
                MoveCamera(Vector3.forward);
            }
            if (Input.GetKey(KeyCode.S))
            {
                MoveCamera(Vector3.back);
            }
            if (Input.GetKey(KeyCode.A))
            {
                RotateCamera(Vector3.up);
            }
            if (Input.GetKey(KeyCode.D))
            {
                RotateCamera(Vector3.down);
            }
            // if(Input.GetKey(KeyCode.A))
            // {
            // 	MoveCamera(Vector3.left);
            // }
            // if(Input.GetKey(KeyCode.D))
            // {
            // 	MoveCamera(Vector3.right);
            // }
        }

        private void MoveCamera(Vector3 direction)
        {
            transform.Translate(direction * moveAmount, Space.Self);
        }

        private void RotateCamera(Vector3 direction)
        {
            transform.position = (Quaternion.AngleAxis(rotationAmount, direction) * transform.position);
            additionalCamRotation *= Quaternion.AngleAxis(rotationAmount, direction);
            transform.rotation = additionalCamRotation;
        }

        private void Update()
        {
            CheckCameraMovement();

            if (Input.GetMouseButton(0))
            {
                // Allow the script to clamp based on a desired target value.
                var targetOrientation = Quaternion.Euler(targetDirection);
                var targetCharacterOrientation = Quaternion.Euler(targetCharacterDirection);

                // Get raw mouse input for a cleaner reading on more sensitive mice.
                var mouseDelta = new Vector2(Input.GetAxisRaw("Mouse X"), -Input.GetAxisRaw("Mouse Y"));

                // Scale input against the sensitivity setting and multiply that against the smoothFactor value.
                mouseDelta = Vector2.Scale(mouseDelta, new Vector2(sensitivity.x * smoothFactor.x, sensitivity.y * smoothFactor.y));

                // Interpolate mouse movement over time to apply smoothFactor delta.
                _smoothMouse.x = Mathf.Lerp(_smoothMouse.x, mouseDelta.x, 1f / smoothFactor.x);
                _smoothMouse.y = Mathf.Lerp(_smoothMouse.y, mouseDelta.y, 1f / smoothFactor.y);

                // Find the absolute mouse movement value from point zero.
                _mouseAbsolute += _smoothMouse;

                // Clamp and apply the local x value first, so as not to be affected by world transforms.
                if (clampInDegrees.x < 360)
                    _mouseAbsolute.x = Mathf.Clamp(_mouseAbsolute.x, -clampInDegrees.x * 0.5f, clampInDegrees.x * 0.5f);

                // Then clamp and apply the global y value.
                if (clampInDegrees.y < 360)
                    _mouseAbsolute.y = Mathf.Clamp(_mouseAbsolute.y, -clampInDegrees.y * 0.5f, clampInDegrees.y * 0.5f);
                transform.rotation = additionalCamRotation;
                verticalOffset = Quaternion.AngleAxis(-_mouseAbsolute.y, targetOrientation * Vector3.right);
                transform.localRotation *= Quaternion.AngleAxis(-_mouseAbsolute.y, targetOrientation * Vector3.right) * targetOrientation;

                var yRotation = Quaternion.AngleAxis(_mouseAbsolute.x, transform.InverseTransformDirection(Vector3.up));
                transform.localRotation *= yRotation;
            }
        }

#endif
    }
}