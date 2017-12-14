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

using System;
using System.Collections;
using System.Collections.Generic;
using System.Runtime.InteropServices;
using UnityEngine;
using AOT;

internal class NativeBridge
{
    /* Objective-C */

    // MRRemoteManager
#if UNITY_IPHONE
    [DllImport("__Internal")]
#else
    [DllImport ("MiraRemote-macOS")]
#endif
    private static extern void MRRemoteManagerBluetoothState(string identifier, MRIntCallback callback);

    #if UNITY_IPHONE
    [DllImport("__Internal")]
#else
    [DllImport ("MiraRemote-macOS")]
#endif
    private static extern void MRRemoteManagerAutomaticallyConnectsToPreviousConnectedRemote(string identifier, MRBoolCallback callback);

    #if UNITY_IPHONE
    [DllImport("__Internal")]
#else
    [DllImport ("MiraRemote-macOS")]
#endif
    private static extern void MRRemoteManagerSetAutomaticallyConnectsToPreviousConnectedRemote(string identifier, bool value, MRBoolCallback callback);

#if UNITY_IPHONE
    [DllImport("__Internal")]
#else
    [DllImport ("MiraRemote-macOS")]
#endif
    private static extern void MRRemoteManagerStart();

#if UNITY_IPHONE
    [DllImport("__Internal")]
#else
    [DllImport ("MiraRemote-macOS")]
#endif
    private static extern void MRRemoteManagerStartRemoteDiscovery(string identifier, MRDiscoveredRemoteCallback callback, MRErrorCallback errorCallback);

#if UNITY_IPHONE
    [DllImport("__Internal")]
#else
    [DllImport ("MiraRemote-macOS")]
#endif
    private static extern void MRRemoteManagerStopRemoteDiscovery();

#if UNITY_IPHONE
    [DllImport("__Internal")]
#else
    [DllImport ("MiraRemote-macOS")]
#endif
    private static extern void MRRemoteManagerConnectRemote(string identifier, string remoteIdentifier, MRConnectedRemoteCallback callback, MRErrorCallback errorCallback);

#if UNITY_IPHONE
    [DllImport("__Internal")]
#else
    [DllImport ("MiraRemote-macOS")]
#endif
    private static extern void MRRemoteManagerDisconnectConnectedRemote(string identifier, MRErrorCallback callback);


    // MRRemote
#if UNITY_IPHONE
    [DllImport("__Internal")]
#else
    [DllImport ("MiraRemote-macOS")]
#endif
    private static extern void MRRemoteRefresh(string identifier, string remoteIdentifier, MRConnectedRemoteCallback callback, MRErrorCallback errorCallback);


    // MRRemote Inputs
#if UNITY_IPHONE
    [DllImport("__Internal")]
#else
    [DllImport ("MiraRemote-macOS")]
#endif
    private static extern void MRRemoteButtonInputAddListener(string identifier, string remoteIdentifier, string keyPath, MRBoolCallback callback);
#if UNITY_IPHONE
    [DllImport("__Internal")]
#else
    [DllImport ("MiraRemote-macOS")]
#endif
    private static extern void MRRemoteTouchInputAddListener(string identifier, string remoteIdentifier, string keyPath, MRBoolCallback callback);

#if UNITY_IPHONE
    [DllImport("__Internal")]
#else
    [DllImport ("MiraRemote-macOS")]
#endif
    private static extern void MRRemoteAxisInputAddListener(string identifier, string remoteIdentifier, string keyPath, MRFloatCallback callback);

#if UNITY_IPHONE
    [DllImport("__Internal")]
#else
    [DllImport ("MiraRemote-macOS")]
#endif
    private static extern void MRRemoteTouchPadInputAddListener(string identifier, string remoteIdentifier, string keyPath, MRBoolCallback callback);

#if UNITY_IPHONE
    [DllImport("__Internal")]
#else
    [DllImport ("MiraRemote-macOS")]
#endif
    private static extern void MRRemoteMotionInputAddListener(string identifier, string remoteIdentifier, string keyPath, MREmptyCallback callback);

#if UNITY_IPHONE
    [DllImport("__Internal")]
#else
    [DllImport ("MiraRemote-macOS")]
#endif
    private static extern void MRRemoteMotionSensorInputAddListener(string identifier, string remoteIdentifier, string keyPath, MRRemoteMotionCallback callback);

#if UNITY_IPHONE
    [DllImport("__Internal")]
#else
    [DllImport ("MiraRemote-macOS")]
#endif
    private static extern void MRRemoteOrientationInputAddListener(string identifier, string remoteIdentifier, string keyPath, MRRemoteMotionCallback callback);


    // Notifications
#if UNITY_IPHONE
    [DllImport("__Internal")]
#else
    [DllImport ("MiraRemote-macOS")]
#endif
    private static extern void MRRegisterForRemoteDidConnectNotification(string identifier, MRConnectedRemoteCallback callback);

#if UNITY_IPHONE
    [DllImport("__Internal")]
#else
    [DllImport ("MiraRemote-macOS")]
#endif
    private static extern void MRRegisterForRemoteDidDisconnectNotification(string identifier, MRDiscoveredRemoteCallback callback);

#if UNITY_IPHONE
    [DllImport("__Internal")]
#else
    [DllImport ("MiraRemote-macOS")]
#endif
    private static extern void MRRegisterForRemoteDidRefreshNotification(string identifier, MRDiscoveredRemoteCallback callback);


    /* Delegates */
    private delegate void MREmptyCallback(string identifier);

    private delegate void MRBoolCallback(string identifier, bool value);
    private delegate void MRIntCallback(string identifier, int value);
    private delegate void MRFloatCallback(string identifier, float value);

    private delegate void MRErrorCallback(string identifier, bool success, int errorCode, string message);

    private delegate void MRDiscoveredRemoteCallback(string identifier, string name, string remoteIdentifier, int rssi, bool isPreferred);
    private delegate void MRConnectedRemoteCallback(string identifier,
                                                    string name,
                                                    string remoteIdentifier,
                                                    string productName,
                                                    string serialNumber,
                                                    string hardwareIdentifier,
                                                    string firmwareVersion,
                                                    float batteryPercentage,
                                                    int rssi,
                                                    bool isConnected,
                                                    bool isPreferred);

    private delegate void MRRemoteMotionCallback(string identifier, float x, float y, float z);

    /* Actions */
    private static Dictionary<string, Action> emptyActions = new Dictionary<string, Action>();

    private static Dictionary<string, Action<bool>> boolActions = new Dictionary<string, Action<bool>>();
    private static Dictionary<string, Action<int>> intActions = new Dictionary<string, Action<int>>();
    private static Dictionary<string, Action<float>> floatActions = new Dictionary<string, Action<float>>();

    private static Dictionary<string, Action<MiraRemoteException>> exceptionActions = new Dictionary<string, Action<MiraRemoteException>>();

    private static Dictionary<string, Action<Remote>> remoteActions = new Dictionary<string, Action<Remote>>();

    private static Dictionary<string, Action<float, float, float>> remoteMotionActions = new Dictionary<string, Action<float, float, float>>();


    /* Internal Functions */
    internal static BluetoothState RemoteManagerBluetoothState()
    {
        if (Application.platform != RuntimePlatform.IPhonePlayer && Application.platform != RuntimePlatform.OSXPlayer)
        {
            return BluetoothState.Unknown;
        }

        string identifier = Guid.NewGuid().ToString();

        BluetoothState bluetoothState = BluetoothState.Unknown;
        intActions[identifier] = (state) => {
            bluetoothState = (BluetoothState)state;
        };
        
        MRRemoteManagerBluetoothState(identifier, IntCallback);

        return bluetoothState;
    }

    internal static bool RemoteManagerAutomaticallyConnectsToPreviousConnectedRemote()
    {
        if (Application.platform != RuntimePlatform.IPhonePlayer && Application.platform != RuntimePlatform.OSXPlayer)
        {
            return true;
        }

        string identifier = Guid.NewGuid().ToString();

        bool value = true;
        boolActions[identifier] = (v) => {
            value = v;
        };

        MRRemoteManagerAutomaticallyConnectsToPreviousConnectedRemote(identifier, BoolCallback);

        return value;
    }

    internal static void RemoteManagerSetAutomaticallyConnectsToPreviousConnectedRemote(bool value)
    {
        if (Application.platform != RuntimePlatform.IPhonePlayer && Application.platform != RuntimePlatform.OSXPlayer)
        {
            return;
        }

        string identifier = Guid.NewGuid().ToString();

        boolActions[identifier] = (v) => {
            // Do nothing
        };

        MRRemoteManagerSetAutomaticallyConnectsToPreviousConnectedRemote(identifier, value, BoolCallback);
    }

    internal static void RemoteManagerStart()
    {
        if (Application.platform != RuntimePlatform.IPhonePlayer && Application.platform != RuntimePlatform.OSXPlayer)
        {
            return;
        }

        MRRemoteManagerStart();
    }

    internal static void RemoteManagerStartRemoteDiscovery(Action<Remote> action)
    {
        if (Application.platform != RuntimePlatform.IPhonePlayer && Application.platform != RuntimePlatform.OSXPlayer)
        {
            return;
        }

        string identifier = "discovery-" + Guid.NewGuid().ToString();

        MiraRemoteException discoveryException = null;

        remoteActions[identifier] = (remote) => {
            action(remote);

            // Remove exception action, but this action is called once per discovered device, so can't remove this action yet.
            exceptionActions.Remove(identifier);
        };
        exceptionActions[identifier] = (exception) => {
            discoveryException = exception;
        };

        MRRemoteManagerStartRemoteDiscovery(identifier, DiscoveredRemoteCallback, ErrorCallback);

        if (discoveryException != null)
        {
            remoteActions.Remove(identifier);
            exceptionActions.Remove(identifier);

            throw discoveryException;
        }
    }

    internal static void RemoteManagerStopRemoteDiscovery()
    {
        if (Application.platform != RuntimePlatform.IPhonePlayer && Application.platform != RuntimePlatform.OSXPlayer)
        {
            return;
        }

        MRRemoteManagerStopRemoteDiscovery();

        // Remove all discovery actions.
        Dictionary<string, Action<Remote>> tempRemoteActions = new Dictionary<string, Action<Remote>>(remoteActions);
        foreach (KeyValuePair<string, Action<Remote>> entry in tempRemoteActions)
        {
            if (entry.Key.StartsWith("discovery"))
            {
                remoteActions.Remove(entry.Key);
            }
        }
    }

    internal static void RemoteManagerConnectRemote(Remote remote, Action<MiraRemoteException> action)
    {
        if (Application.platform != RuntimePlatform.IPhonePlayer && Application.platform != RuntimePlatform.OSXPlayer)
        {
            return;
        }

        string identifier = Guid.NewGuid().ToString();

        remoteActions[identifier] = (connectedRemote) =>
        {
            remote.UpdateWithRemote(connectedRemote);

            action(null);

            remoteActions.Remove(identifier);
            exceptionActions.Remove(identifier);
        };
        exceptionActions[identifier] = (exception) =>
        {
            action(exception);

            remoteActions.Remove(identifier);
            exceptionActions.Remove(identifier);
        };

        MRRemoteManagerConnectRemote(identifier, remote.identifier.ToString(), ConnectedRemoteCallback, ErrorCallback);
    }

    internal static void RemoteManagerDisconnectConnectedRemote(Action<MiraRemoteException> action)
    {
        if (Application.platform != RuntimePlatform.IPhonePlayer && Application.platform != RuntimePlatform.OSXPlayer)
        {
            return;
        }

        string identifier = Guid.NewGuid().ToString();

        exceptionActions[identifier] = (exception) =>
        {
            action(exception);

            exceptionActions.Remove(identifier);
        };

        MRRemoteManagerDisconnectConnectedRemote(identifier, ErrorCallback);
    }

    internal static void RemoteRefresh(Remote remote, Action<MiraRemoteException> action)
    {
        if (Application.platform != RuntimePlatform.IPhonePlayer && Application.platform != RuntimePlatform.OSXPlayer)
        {
            return;
        }

        string identifier = Guid.NewGuid().ToString();

        remoteActions[identifier] = (refreshedRemote) => {
            remote.UpdateWithRemote(refreshedRemote);

            action(null);

            remoteActions.Remove(identifier);
        };
        exceptionActions[identifier] = (exception) => {
            action(exception);

            exceptionActions.Remove(identifier);
        };

        MRRemoteRefresh(identifier, remote.identifier.ToString(), ConnectedRemoteCallback, ErrorCallback);
    }

    internal static void RemoteButtonInputAddListener(Remote remote, string keyPath, Action<bool> action)
    {
        if (Application.platform != RuntimePlatform.IPhonePlayer && Application.platform != RuntimePlatform.OSXPlayer)
        {
            return;
        }

        string identifier = Guid.NewGuid().ToString();

        boolActions[identifier] = action;

        MRRemoteButtonInputAddListener(identifier, remote.identifier.ToString(), keyPath, BoolCallback);
    }

    internal static void RemoteTouchInputAddListener(Remote remote, string keyPath, Action<bool> action)
    {
        if (Application.platform != RuntimePlatform.IPhonePlayer && Application.platform != RuntimePlatform.OSXPlayer)
        {
            return;
        }

        string identifier = Guid.NewGuid().ToString();

        boolActions[identifier] = action;

        MRRemoteTouchInputAddListener(identifier, remote.identifier.ToString(), keyPath, BoolCallback);
    }

    internal static void RemoteAxisInputAddListener(Remote remote, string keyPath, Action<float> action)
    {
        if (Application.platform != RuntimePlatform.IPhonePlayer && Application.platform != RuntimePlatform.OSXPlayer)
        {
            return;
        }

        string identifier = Guid.NewGuid().ToString();

        floatActions[identifier] = action;

        MRRemoteAxisInputAddListener(identifier, remote.identifier.ToString(), keyPath, FloatCallback);
    }

    internal static void RemoteTouchPadInputAddListener(Remote remote, string keyPath, Action<bool> action)
    {
        if (Application.platform != RuntimePlatform.IPhonePlayer && Application.platform != RuntimePlatform.OSXPlayer)
        {
            return;
        }

        string identifier = Guid.NewGuid().ToString();

        boolActions[identifier] = action;

        MRRemoteTouchPadInputAddListener(identifier, remote.identifier.ToString(), keyPath, BoolCallback);
    }

    internal static void RemoteMotionInputAddListener(Remote remote, string keyPath, Action action)
    {
        if (Application.platform != RuntimePlatform.IPhonePlayer && Application.platform != RuntimePlatform.OSXPlayer)
        {
            return;
        }

        string identifier = Guid.NewGuid().ToString();

        emptyActions[identifier] = action;

        MRRemoteMotionInputAddListener(identifier, remote.identifier.ToString(), keyPath, EmptyCallback);
    }

    internal static void RemoteMotionSensorInputAddListener(Remote remote, string keyPath, Action<float, float, float> action)
    {
        if (Application.platform != RuntimePlatform.IPhonePlayer && Application.platform != RuntimePlatform.OSXPlayer)
        {
            return;
        }

        string identifier = Guid.NewGuid().ToString();

        remoteMotionActions[identifier] = action;

        MRRemoteMotionSensorInputAddListener(identifier, remote.identifier.ToString(), keyPath, RemoteMotionCallback);
    }

    internal static void RemoteOrientationInputAddListener(Remote remote, string keyPath, Action<float, float, float> action)
    {
        if (Application.platform != RuntimePlatform.IPhonePlayer && Application.platform != RuntimePlatform.OSXPlayer)
        {
            return;
        }

        string identifier = Guid.NewGuid().ToString();

        remoteMotionActions[identifier] = action;

        MRRemoteMotionSensorInputAddListener(identifier, remote.identifier.ToString(), keyPath, RemoteMotionCallback);
    }

    internal static void RegisterForRemoteDidConnectNotification(Action<Remote> action)
    {
        if (Application.platform != RuntimePlatform.IPhonePlayer && Application.platform != RuntimePlatform.OSXPlayer)
        {
            return;
        }
        
        string identifier = Guid.NewGuid().ToString();

        remoteActions[identifier] = action;

        MRRegisterForRemoteDidConnectNotification(identifier, ConnectedRemoteCallback);
    }

    internal static void RegisterForRemoteDidDisconnectNotification(Action<Remote> action)
    {
        if (Application.platform != RuntimePlatform.IPhonePlayer && Application.platform != RuntimePlatform.OSXPlayer)
        {
            return;
        }

        string identifier = Guid.NewGuid().ToString();

        remoteActions[identifier] = action;

        MRRegisterForRemoteDidDisconnectNotification(identifier, DiscoveredRemoteCallback);
    }

    internal static void RegisterForRemoteDidRefreshNotification(Action<Remote> action)
    {
        if (Application.platform != RuntimePlatform.IPhonePlayer && Application.platform != RuntimePlatform.OSXPlayer)
        {
            return;
        }

        string identifier = Guid.NewGuid().ToString();

        remoteActions[identifier] = action;

        MRRegisterForRemoteDidRefreshNotification(identifier, DiscoveredRemoteCallback);
    }

    private static Remote RemoteForRemoteIdentifier(Guid identifier, string name)
    {
        if (RemoteManager.Instance.connectedRemote != null)
        {
            if (RemoteManager.Instance.connectedRemote.identifier == identifier)
            {
                return RemoteManager.Instance.connectedRemote;
            }
        }

        foreach (Remote r in RemoteManager.Instance.discoveredRemotes)
        {
            if (r.identifier == identifier)
            {
                return r;
            }
        }

        Remote remote = new Remote(name, identifier);
        return remote;
    }

    /* Callback Implementations */
    [MonoPInvokeCallback(typeof(MREmptyCallback))]
    private static void EmptyCallback(string identifier)
    {
        Action action = emptyActions[identifier];
        action();
    }

    [MonoPInvokeCallback(typeof(MRBoolCallback))]
    private static void BoolCallback(string identifier, bool value)
    {
        Action<bool> action = boolActions[identifier];
        action(value);
    }

    [MonoPInvokeCallback(typeof(MRIntCallback))]
    private static void IntCallback(string identifier, int value)
    {
        Action<int> action = intActions[identifier];
        action(value);
    }

    [MonoPInvokeCallback(typeof(MRFloatCallback))]
    private static void FloatCallback(string identifier, float value)
    {
        Action<float> action = floatActions[identifier];
        action(value);
    }

    [MonoPInvokeCallback(typeof(MRErrorCallback))]
    private static void ErrorCallback(string identifier, bool success, int errorCode, string message)
    {
        Action<MiraRemoteException> action = exceptionActions[identifier];

        MiraRemoteException exception = success ? new MiraRemoteException(message, (MiraRemoteErrorCode)errorCode) : null;
        action(exception);
    }

    [MonoPInvokeCallback(typeof(MRDiscoveredRemoteCallback))]
    private static void DiscoveredRemoteCallback(string identifier, string name, string remoteIdentifier, int rssi, bool isPreferred)
    {
        Action<Remote> action = remoteActions[identifier];
        
        Remote remote = RemoteForRemoteIdentifier(new Guid(remoteIdentifier), name);
        remote.isPreferred = isPreferred;
        
        if (rssi != 0)
        {
            remote.rssi = rssi;
        }
        else
        {
            remote.rssi = null;
        }

        action(remote);
    }

    [MonoPInvokeCallback(typeof(MRConnectedRemoteCallback))]
    private static void ConnectedRemoteCallback(string identifier, string name, string remoteIdentifier, string productName, string serialNumber, string hardwareIdentifier, string firmwareVersion, float batteryPercentage, int rssi, bool isConnected, bool isPreferred)
    {
        Action<Remote> action = remoteActions[identifier];

        Remote remote = RemoteForRemoteIdentifier(new Guid(remoteIdentifier), name);
        remote.productName = productName;
        remote.serialNumber = serialNumber;
        remote.hardwareIdentifier = hardwareIdentifier;
        remote.firmwareVersion = firmwareVersion;
        remote.batteryPercentage = batteryPercentage;
        remote.isConnected = isConnected;
        remote.isPreferred = isPreferred;

        if (rssi != 0)
        {
            remote.rssi = rssi;
        }
        else
        {
            remote.rssi = null;
        }

        action(remote);
    }

    [MonoPInvokeCallback(typeof(MRRemoteMotionCallback))]
    private static void RemoteMotionCallback(string identifier, float x, float y, float z)
    {
        Action<float, float, float> action = remoteMotionActions[identifier];
        action(x, y, z);
    }
}