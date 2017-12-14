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

using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Wikitude;

namespace Mira
{
    /// <summary>
    /// This script manages rotational handoff when tracking is lost.
    /// When tracking is found, the gyro switches into "InverseGyro" mode, where the gyro is inverted, and the object rotate around the camera rig.
    /// This is more realistic to the user's movement in the real space, and particle effects/physics will behave intuitively
    /// </summary>
    public class RotationalTrackingManager : MonoBehaviour
    {
        /// <summary>
        /// The Mira Ar Camera Rig
        /// </summary>
        private Transform mainCamera;

        /// <summary>
        /// Head Pivot Transform
        /// </summary>
        private Transform cameraRig;

        /// <summary>
        /// When true, the cameraRig is in rotational mode (not tracking)
        /// </summary>
        private bool isRotational = false;

        /// <summary>
        /// The buffer where the camera's last positions are stored
        /// When tracking is lost, the oldest values in the buffer are averaged, and the camera moves to that "stable position"
        /// </summary>
        private Queue<Vector3> positionBuffer;

        /// <summary>
        /// The rotation buffer is identical to the positionBuffer, but stores the cam Rotations.
        /// </summary>
        private Queue<Quaternion> rotationBuffer;

        /// <summary>
        /// The rate (in seconds) at which cam rotation/position values are written to the buffers.
        /// </summary>
        private float delay = 0.1f;

        /// <summary>
        /// The length of the buffer
        /// </summary>
        private int bufferSize = 20;

        // The number of values from the buffer that aren't included in the average.
        // (the most recent values are ommitted, due to noise when tracking is lost).
        private int bufferDiscardLast = 5;

        /// <summary>
        /// Start position of CameraRig before tracking (in CM, it will scale accordingly)
        /// </summary>
        private Vector3 camRigStartPosition = new Vector3(0f, 29.6f, -31.5f);

        private bool isSpectator = false;

        private void Start()
        {
            WikitudeCamera wikiCam = MiraWikitudeManager.Instance.ArCam;
            if (wikiCam)
            {
                bool isStaticCamera = wikiCam.StaticCamera;

                if (isStaticCamera)
                {
                    // Deactivate handoff for Static Camera
                    MiraArController.Instance.cameraRig.GetComponent<CamGyro>().camGyroActive = true;
                    GetComponent<InverseGyroController>().enabled = false;
                    this.enabled = false;
                }
            }

            InitializeEvents();
            // Reference the main Camera
            mainCamera = MiraWikitudeManager.Instance.gameObject.transform;
            // Initialize the positionBuffer
            positionBuffer = new Queue<Vector3>(bufferSize);
            // Initialize the rotationBuffer
            rotationBuffer = new Queue<Quaternion>(bufferSize);

            // Initialize Rotational Setup
            StartCoroutine(RotationalSetup());
            // Initialize Start Position
            camRigStartPosition *= (1 / MiraArController.scaleMultiplier);
            // Initialize head pivot
            CamGyro[] sceneCamGyros = GameObject.FindObjectsOfType<CamGyro>();
            if (sceneCamGyros.Length > 1)
            {
                foreach (CamGyro camGyro in sceneCamGyros)
                {
                    if (camGyro.gameObject.name == "HeadPivotContainer")
                        cameraRig = camGyro.gameObject.transform;
                }
            }

            cameraRig = GameObject.FindObjectOfType<CamGyro>().transform;
            isSpectator = MiraArController.Instance.isSpectator;
        }

        private void InitializeEvents()
        {
            ImageTrackable[] trackers = GameObject.FindObjectsOfType<ImageTrackable>();
            foreach (ImageTrackable thisTracker in trackers)
            {
                thisTracker.OnImageRecognized.AddListener(OnTrackingFoundHandoff);
                thisTracker.OnImageLost.AddListener(OnTrackingLostHandoff);
            }
        }

        private void TestEventFound(ImageTarget target)
        {
            Debug.Log(target.Name + " Found");
            Debug.Log(target.ID.ToString() + " ID");
            Debug.Log(target.Scale + " Scale");
            Debug.Log(target.ComputeCameraDistanceToTarget().ToString() + " Cam Dist To Target");
        }

        private void TestEventLost(ImageTarget target)
        {
            Debug.Log(target.Name + " Lost");
            Debug.Log(target.ID.ToString() + " ID");
            Debug.Log(target.Scale + " Scale");
            Debug.Log(target.ComputeCameraDistanceToTarget().ToString() + " Cam Dist To Target");
        }

        private IEnumerator RotationalSetup()
        {
            yield return null;
            // Assume that currently not tracking
            // mainCamera.GetComponent<MiraCameraDriver>().cameraIsTracking = false;
            // As a result, disable the InverseGyroController
            GetComponent<InverseGyroController>().enabled = false;
            // Enable the CamGyro on the MainCamera
            cameraRig.GetComponent<CamGyro>().enabled = true;
            // Reset the camera gyro

            yield return null;
            cameraRig.GetComponent<CamGyro>().camGyroActive = true;
            // Setup is currently rotational
            isRotational = true;
            // If the setup is NOT rotational only (will involve tracking)
            if (MiraArController.Instance.isRotationalOnly == false)
            {
                // Set the camera & rotationally tracked to a neutral position
                if (isSpectator)
                {
                    mainCamera.localPosition = Vector3.zero;
                    mainCamera.localRotation = Quaternion.identity;

                    transform.position = mainCamera.position;
                    transform.rotation = mainCamera.rotation;
                }
                else
                {
                    mainCamera.position = camRigStartPosition;
                    transform.position = camRigStartPosition;

                    mainCamera.rotation = Quaternion.identity;
                    transform.rotation = Quaternion.identity;
                }
            }
            else
            {
                mainCamera.position = Vector3.zero;
                mainCamera.rotation = Quaternion.identity;
            }

            yield return null;

            cameraRig.GetComponent<CamGyro>().ResetCamGyro();

            // Parent the rotationally tracked objects to the world
            transform.SetParent(null);
            transform.rotation = Quaternion.identity;
        }

        private IEnumerator BufferPosition()
        {
            while (true)
            {
                yield return new WaitForEndOfFrame();
                // Until the buffer is full, add (Enqueue) the position and rotation
                // When the buffer matches the correct size, delete (Dequeue) the oldest value, and add the newest value
                if (positionBuffer.Count == bufferSize)
                {
                    positionBuffer.Enqueue(mainCamera.position);
                    positionBuffer.Dequeue();
                    rotationBuffer.Enqueue(mainCamera.rotation);
                    rotationBuffer.Dequeue();
                }
                else
                {
                    positionBuffer.Enqueue(mainCamera.position);
                    rotationBuffer.Enqueue(mainCamera.rotation);
                }
                // Wait for delay, so queue is not overloaded with the same values
                yield return new WaitForSeconds(delay);
            }
        }

#if UNITY_EDITOR

        private void Update()
        {
            // This is a way to preview the handoff in the editor:
            if (Input.GetKeyDown(KeyCode.Space))
            {
                if (isRotational == false)
                {
                    TrackingLost();
                    Debug.Log("TrackingHandoffLost");
                }
                else
                {
                    TrackingFound();
                    Debug.Log("TrackingHandoffFound");
                }
            }
        }

#endif

        /// <summary>
        /// Function is called when tracking is found - ends the handoff
        /// </summary>
        /// <param name="imgTarget">Image target that was found</param>
        public void OnTrackingFoundHandoff(ImageTarget imgTarget)
        {
            TrackingFound();
        }

        /// <summary>
        /// Function is called when tracking is lost - initiates handoff
        /// </summary>
        /// <param name="imgTarget"></param>
        public void OnTrackingLostHandoff(ImageTarget imgTarget)
        {
            TrackingLost();
        }

        public void RemoteTrackingEvents(bool found)
        {
            if(found)
            {
                TrackingFound();
            }
            else
            {
                TrackingLost();
            }
        }

        private void TrackingLost()
        {
            // When tracking is lost, stop all coroutines, and switch the camera to Gyroscope mode
            StopAllCoroutines();
            StartCoroutine(SwitchToCameraGyro());
        }

        private void TrackingFound()
        {
            // When Tracker is found:

            // Start buffering position
            StartCoroutine(BufferPosition());
            // Parent rotationally tracked objects back to mainCamera
            transform.SetParent(cameraRig);
            // Zero out the position
            transform.position = cameraRig.position;
            // Match cameraRig rotation to mainCamera rotation
            cameraRig.rotation = mainCamera.rotation;
            // Enable the inverse gyro Controller
            GetComponent<InverseGyroController>().enabled = true;
            // Turn off the CamGyro
            cameraRig.GetComponent<CamGyro>().camGyroActive = false;
            cameraRig.GetComponent<CamGyro>().enabled = false;

            isRotational = false;
        }

        private IEnumerator SwitchToCameraGyro()
        {
            // Average the buffer, and store that value as the offsetPosition
            Vector3 offsetPosition = AverageBufferPosition();
            // Notifies the camera scaler that tracking is not active (so it stops performing the update)
            // mainCamera.GetComponent<MiraCameraDriver>().cameraIsTracking = false;
            // Set the mainCamera position to the (stable) averaged buffer position
            mainCamera.position = offsetPosition;
            // Set the mainCamera rotation to the (stable) averaged buffer rotation
            mainCamera.rotation = AverageBufferRotation();
            // Deactivate the Inverse Gyro Controller (Gyro will be applied directly)
            GetComponent<InverseGyroController>().enabled = false;
            // Make sure CamGyro is enabled (Applies gyro directly to camera, with offsets)
            cameraRig.GetComponent<CamGyro>().enabled = true;
            // Reset Cam Gyro to set those offsets
            cameraRig.GetComponent<CamGyro>().ResetCamGyro();
            // Wait a frame, so offsets aren't influenced by next update
            yield return null;
            // Activate CamGyro
            cameraRig.GetComponent<CamGyro>().camGyroActive = true;
            // Setup is currently rotational
            isRotational = true;
            // Wait a frame for all camera offsets to be applied
            yield return null;
            // Parent RotationalTracking to Camera
            transform.SetParent(null);
        }

        private Vector3 AverageBufferPosition()
        {
            Vector3 average = new Vector3();
            System.Collections.IEnumerator enBuffer = positionBuffer.GetEnumerator();
            float count = 0;
            int limit = bufferSize - bufferDiscardLast;

            while (enBuffer.MoveNext())
            {
                if (count < limit)
                {
                    average += (Vector3)enBuffer.Current;
                }
                count += 1;
            }

            return new Vector3(average.x / limit, average.y / limit, average.z / limit);
        }

        private Quaternion AverageBufferRotation()
        {
            float w = 0f;
            float x = 0f;
            float y = 0f;
            float z = 0f;
            System.Collections.IEnumerator rotBuffer = rotationBuffer.GetEnumerator();

            int limit = bufferSize - bufferDiscardLast;
            float count = 0;

            while (rotBuffer.MoveNext())
            {
                if (count < limit)
                {
                    Quaternion temp = (Quaternion)rotBuffer.Current;
                    w += temp.w;
                    x += temp.x;
                    y += temp.y;
                    z += temp.z;
                }
                count += 1;
            }

            return new Quaternion(x / limit, y / limit, z / limit, w / limit);
        }
    }
}