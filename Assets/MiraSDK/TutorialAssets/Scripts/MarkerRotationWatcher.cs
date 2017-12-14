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
using Mira;

/// <summary>
/// Once tracking is found, it evaluates the change in rotation, and drives a lerp or animation. Once the rotation has reached a threshold, it can fire an event
/// </summary>
public class MarkerRotationWatcher : MonoBehaviour {
	[HideInInspector]
	Transform mainCamera;

	/// <summary>
	/// The delta dot product, the amount of rotational change from the original tracking position
	/// </summary>
	public float deltaDotProduct;
	/// <summary>
	///  How much rotation is necessary to complete the lerp or animation
	/// </summary>
	public float necessaryRotation = -0.85f;
	public float dot;

	/// <summary>
	/// Events that are called when rotation is complete
	/// </summary>
	public delegate void RotateAction();
	public static event RotateAction OnRotated;

	private Vector3 startVector;
	private bool firstAngle = true;
	private float counter = 0;

	private bool hasRotated = false;

	// Use this for initialization
	void Start () {
		
		// Refrencing the Wikitude Camera
		mainCamera = MiraWikitudeManager.Instance.transform;
	}
	
	// Update is called once per frame
	void Update () {

		Vector3 camFw = mainCamera.forward;
		camFw.y = 0;
		camFw = camFw.normalized;

		if (firstAngle) {
			startVector = camFw;
			firstAngle = false;
		} else {
			dot = Vector3.Dot (startVector, camFw);
			deltaDotProduct = dot;

			if (dot < -0.9f && !hasRotated) {
				counter += Time.deltaTime;
				print (counter);

				if (counter > 0.5f) {
					if (OnRotated != null) {
						hasRotated = true;
						OnRotated ();
					}
				}
			} else {
				counter = 0;
			}
		}
	}


}
