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
using System;
using Mira;

/// <summary>
/// Manages the settings menu, that allows the user to change remotes, return to the mira home app, etc
/// </summary>
public class SettingsManager : MonoBehaviour {
	public GameObject MainSettingsMenu;
	public GameObject RemoteMenu;

	public GameObject connectedNotification;
	public GameObject disconnectedNotification;

	public static SettingsManager Instance;

	public RemotesController remotesController;

	public GameObject settingsButton;

	void Awake() 
	{
            // Check if instance already exists, destroy any imposters, and keep it alive during scene changes
            if (Instance == null) Instance = this;
            else if (Instance != this) Destroy(gameObject);
            // DontDestroyOnLoad(gameObject);
    }

	void OnEnable()
	{
		RemoteManager.Instance.OnRemoteConnected += RemoteConnected;
		RemoteManager.Instance.OnRemoteDisconnected += RemoteDisconnected;
		ExitSettingsMenu();
	}

	void OnDisable()
	{
		RemoteManager.Instance.OnRemoteConnected -= RemoteConnected;
		RemoteManager.Instance.OnRemoteDisconnected -= RemoteDisconnected;
	}


	public void OpenControllerMenu()
	{
		// Dope transition here to Controller Screen
		RemoteMenu.SetActive(true);
		MainSettingsMenu.SetActive(false);
		remotesController.displayRemotes();
	}

	public void OpenMainMenu()
	{
		RemoteMenu.SetActive(false);
		MainSettingsMenu.SetActive(true);
	}
	public void GoToMiraHome()
	{
	}

	void RemoteConnected(Remote remote, EventArgs args)
	{
		// if(MiraController.Instance != null)
		// 	MiraController.Instance.NewControllerConnected();
		StartCoroutine(RemoteConnectedNotification());
	}
	void RemoteDisconnected(Remote remote, EventArgs args)
	{
		// if(MiraController.Instance != null)
		// 	MiraController.Instance.ControllerDisconnected();
		StartCoroutine(RemoteDisconnectedNotification());
	}
	public IEnumerator RemoteConnectedNotification()
	{
		connectedNotification.SetActive(true);
		yield return new WaitForSeconds(3f);
		connectedNotification.SetActive(false);

	}
	public IEnumerator RemoteDisconnectedNotification()
	{
		disconnectedNotification.SetActive(true);
		yield return new WaitForSeconds(3f);
		disconnectedNotification.SetActive(false);

	}
	public void ExitSettingsMenu()
	{
		RemoteMenu.SetActive(false);
		MainSettingsMenu.SetActive(false);
		// Re-enables MiraInputModule (temporarily), and hides this menu
		if(MiraArController.Instance!=null)
			MiraArController.Instance.ToggleSettingsMenu(false);
		else
			settingsButton.SetActive(true);

	}
}
