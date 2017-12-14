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
using UnityEngine.SceneManagement;
using Mira;
/// <summary>
/// Script Toggles the scene mode everytime the scene is loaded.
/// This script should be executed first i.e. should be the topmost script in the script execution order.
/// </summary>
public class ToggleModes : MonoBehaviour {

	// Use this for initialization.
	void OnEnable () {
		//Sets the Mode according to specataor.
		MiraArController.Instance.isSpectator = DemoSceneManager.isSpectator;
	}
	
	// Update is called once per frame.
	void Update () {
		// Toggling the scene mode and Reloading the scene.
		if (Input.GetMouseButtonDown (0)) {
			Debug.Log ("Input touch");
			DemoSceneManager.isSpectator = !DemoSceneManager.isSpectator;
			SceneManager.LoadScene ( SceneManager.GetActiveScene().name);
		}

	}
}
