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
using System.Collections.ObjectModel;

public enum BluetoothState { Unknown, Resetting, Unsupported, Unauthorized, PoweredOff, PoweredOn };

public class RemoteManager : MonoBehaviour	
{
	// Events
	public delegate void RemoteConnectedEventHandler(Remote remote, EventArgs e);
    public event RemoteConnectedEventHandler OnRemoteConnected;

    public delegate void RemoteDisconnectedEventHandler(Remote remote, EventArgs e);
    public event RemoteDisconnectedEventHandler OnRemoteDisconnected;

	public readonly static RemoteManager Instance = new RemoteManager();

	public bool isStarted { get; private set; }
	public bool isDiscoveringRemotes { get; private set; }

	public BluetoothState bluetoothState {
		get {
			return NativeBridge.RemoteManagerBluetoothState();
		}
	}

    public bool automaticallyConnectsToPreviousConnectedRemote { 
        get {
            return NativeBridge.RemoteManagerAutomaticallyConnectsToPreviousConnectedRemote();
        }
        set {
            NativeBridge.RemoteManagerSetAutomaticallyConnectsToPreviousConnectedRemote(value);
        }
     }

	private Remote _connectedRemote;
	public Remote connectedRemote {
		get {
			return _connectedRemote;
		} 
		private set {
			if (_connectedRemote != null)
			{
				_connectedRemote.isConnected = false;
			}

			_connectedRemote = value;

			if (_connectedRemote != null)
            {
                _connectedRemote.isConnected = true;
            }
		}
	}
	
	private List<Remote> _discoveredRemotes = new List<Remote>();
	public ReadOnlyCollection<Remote> discoveredRemotes {
		get {
			return _discoveredRemotes.AsReadOnly();
		}
	}

	private RemoteManager() 
	{
		this.isStarted = false;
		this.isDiscoveringRemotes = false;

		NativeBridge.RegisterForRemoteDidConnectNotification((r) => {
			Remote remote = RemoteForRemoteIdentifier(r.identifier);

			if (remote != null)
			{
                remote.UpdateWithRemote(r);

				this.connectedRemote = remote;
			}
			else
			{
				this.connectedRemote = r;
			}

			if (OnRemoteConnected != null)
			{
				OnRemoteConnected(this.connectedRemote, EventArgs.Empty);
			}
		});

		NativeBridge.RegisterForRemoteDidDisconnectNotification((remote) =>
        {
			Remote previousRemote = this.connectedRemote;
			previousRemote.Reset();

			this.connectedRemote = null;

			if (OnRemoteDisconnected != null)
			{
				OnRemoteDisconnected(previousRemote, EventArgs.Empty);
            }
        });
	}

	void Start()
	{
		if (this.isStarted)
		{
			return;
		}

		this.isStarted = true;

		NativeBridge.RemoteManagerStart();
	}

	public void StartRemoteDiscovery(Action<Remote> action)
	{
		try
		{
			NativeBridge.RemoteManagerStartRemoteDiscovery((Remote remote) => {
				this._discoveredRemotes.Add(remote);
				action(remote);
			});

			this.isDiscoveringRemotes = true;
		}
		catch (MiraRemoteException exception)
		{	
			throw exception;
		}
    }

	public void StopRemoteDiscovery()
	{
		this._discoveredRemotes = new List<Remote>();

		NativeBridge.RemoteManagerStopRemoteDiscovery();

		this.isDiscoveringRemotes = false;
	}

	public void ConnectRemote(Remote remote, Action<MiraRemoteException> action)
	{
		NativeBridge.RemoteManagerConnectRemote(remote, (exception) => {
			if (exception == null)
			{
				this.connectedRemote = remote;
			}

			action(exception);
		});
	}

	public void DisconnectConnectedRemote(Action<MiraRemoteException> action)
	{
		NativeBridge.RemoteManagerDisconnectConnectedRemote((exception) => {
			if (exception == null)
            {
                this.connectedRemote = null;
            }

			action(exception);
		});
	}

	private Remote RemoteForRemoteIdentifier(Guid identifier)
	{
		if (this.connectedRemote != null)
		{
			if (this.connectedRemote.identifier == identifier)
			{
				return this.connectedRemote;
			}
		}

		foreach (Remote remote in this.discoveredRemotes)
		{
			if (remote.identifier == identifier)
			{
				return remote;
			}
		}

		return null;
	}
}
