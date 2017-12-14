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
/// <summary>
/// Demo scene manager displaying how to svae data between scenes.
/// </summary>
public class DemoSceneManager : MonoBehaviour {

	#region Variables
	/// <summary>
	/// storeing the current mode of the scene.
	/// </summary>
	[HideInInspector]
	public static  bool isSpectator = true;

	/// <summary>
	/// Making sure scenemaneger stores data between sceen transition and reloads.
	/// </summary>
	private static DemoSceneManager instance = null;

	public static DemoSceneManager Instance
	{
		get { 
			if (instance == null)
				instance = FindObjectOfType<DemoSceneManager> ();
			return instance; }
	}
	#endregion

	#region Unity callbacks

	public void OnEnable() {

		DontDestroyOnLoad (this);	
	}

	#endregion

}
