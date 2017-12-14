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
using UnityEngine.UI;
using System;

public class RemotesController : MonoBehaviour 
{

	public GameObject [] remoteLabels;

	public Color defaultColor;
	public Color highlightColor;

	void OnEnable()
    {
		remoteLabels = GameObject.FindGameObjectsWithTag ("RemoteListItem");
        try
        {
            RemoteManager.Instance.StartRemoteDiscovery((remote) =>
            {
                displayRemotes();
				Debug.Log("DISPLAY REMOTE IS TRIGGERED");

                remote.OnRefresh += this.RemoteRefreshedEventHandler;
            });
        }
        catch (MiraRemoteException exception)
        {
            Debug.Log("Caught RemotesController exception. " + exception.Message);
        }
    }

	public void displayRemotes () 
	{
		for (int i = 0; i < remoteLabels.Length; i++) 
		{
			Text text = remoteLabels[i].GetComponentInChildren<Text>();
			Button button = remoteLabels[i].GetComponent<Button>();
			button.image.color = defaultColor;

			if (RemoteManager.Instance.discoveredRemotes.Count > i) 
			{
                Remote remote = RemoteManager.Instance.discoveredRemotes[i];


                string rssi = "Unknown";

                if (remote.rssi != null)
                {
					if(remote.rssi > -70)
						rssi = "Excellent";
					else if(remote.rssi <= -70 && remote.rssi > -85)
						rssi = "Good";
					else if(remote.rssi <= -85 && remote.rssi > -100)
						rssi = "Fair";
					else
						rssi = "Poor";
                    // rssi = (remote.rssi).ToString();
                }

				if(remote == RemoteManager.Instance.connectedRemote)
				{
					button.image.color = highlightColor;
					rssi = "Already Connected";
				}
				else if(remote.isPreferred == true)
				{
					button.image.color = highlightColor;
					rssi = "Is Preferred";
				}
				

                string textString = "Name: " + remote.name + "\n" +
                // "UUID: " +  remote.identifier.ToString() + "\n" +
                "Signal Strength: " + rssi + "\n";
                // "Preferred: " + remote.isPreferred.ToString();

				text.text = textString;
				button.interactable = true;
			} 
			else 
			{
				text.text = "";
				button.interactable = false;
			}
		}
	}

	public void sendInfo(int remoteIdx) 
	{		
		Remote remote = RemoteManager.Instance.discoveredRemotes[remoteIdx];
		RemoteManager.Instance.ConnectRemote(remote, (exception) => {
			if (exception != null)
			{
				Debug.Log("Caught SendInfo exception. " + exception.Message);
				RemoteManager.Instance.StopRemoteDiscovery();
				SettingsManager.Instance.OpenMainMenu();
			}
			else
			{
				Debug.Log("Connected to remote!");
				RemoteManager.Instance.StopRemoteDiscovery();
				SettingsManager.Instance.ExitSettingsMenu();
			}
		});
	}



    private void RemoteRefreshedEventHandler(Remote remote, EventArgs e)
    {
        if (this.isActiveAndEnabled)
        {
            this.displayRemotes();
        }
    } 
}
