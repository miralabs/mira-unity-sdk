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

public class ConnectToPreviousRemote : MonoBehaviour {

	Guid lastRemoteID;

	bool activelySearching;

	void OnEnable()
    {
		RemoteManager.Instance.OnRemoteConnected += RemoteConnected;
		RemoteManager.Instance.OnRemoteDisconnected += RemoteDisconnected;
    }
    void OnDisable()
    {
		RemoteManager.Instance.OnRemoteConnected -= RemoteConnected;
		RemoteManager.Instance.OnRemoteDisconnected -= RemoteDisconnected;
    }

	private void RemoteConnected(Remote remote, EventArgs args)
	{
		// Stop the AutoConnectLastRemote Coroutine
		StopAllCoroutines();
		activelySearching = false;
		// RemoteManager.Instance.StopRemoteDiscovery();
		lastRemoteID = remote.identifier;

	}

	private void RemoteDisconnected(Remote remote, EventArgs args)
	{
		StartCoroutine(AutoConnectLastRemote());
	}

	IEnumerator AutoConnectLastRemote()
	{
		// TODO: Display 3D notification

		// Wait so it doesn't interfere with normal pairing protocol
		yield return new WaitForSeconds(5);
		if(activelySearching == false)
		{
			activelySearching = true;
			try
			{
				RemoteManager.Instance.StartRemoteDiscovery((remote) =>
				{
					CheckRemotes();
					Debug.Log("DISPLAY REMOTE IS TRIGGERED");

					remote.OnRefresh += this.RemoteRefreshedEventHandler;
				});
			}
			catch (MiraRemoteException exception)
			{
				Debug.Log("Caught RemotesController exception. " + exception.Message);
			}
		}


	}
	// Whenever a new remote is found, this re-calls the check function
	private void RemoteRefreshedEventHandler(Remote remote, EventArgs e)
    {
        if (this.isActiveAndEnabled && activelySearching == true)
        {
            this.CheckRemotesHandler();
        }
    } 

	private void CheckRemotesHandler()
	{
		StartCoroutine(CheckRemotes());
	}
	// This checks to see if any of the discovered remotes == the last known remote
	private IEnumerator CheckRemotes()
	{
		if (lastRemoteID != null && RemoteManager.Instance.discoveredRemotes.Count > 0)
		{
			for(int i=0; i < RemoteManager.Instance.discoveredRemotes.Count; i++)
			{
				Remote discoveredRemote = RemoteManager.Instance.discoveredRemotes[i];
				if(discoveredRemote.identifier == lastRemoteID && RemoteManager.Instance.connectedRemote == null)
				{
					RemoteManager.Instance.ConnectRemote(discoveredRemote, (exception) => {
					if (exception != null)
					{
						Debug.Log("Caught SendInfo exception. " + exception.Message);
						RemoteManager.Instance.StopRemoteDiscovery();
					}
					else
					{
						Debug.Log("Connected to remote!");
						RemoteManager.Instance.StopRemoteDiscovery();
					}
					});

					yield break;
				}
			}
		} 
		yield return null;
	}
}
