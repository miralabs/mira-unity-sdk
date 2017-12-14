//
//  MRUnityBridge.h
//  MiraRemote
//
//  Created by Riley Testut on 9/17/17.
//  Copyright © 2017 Mira Labs, Inc. All rights reserved.
//

#include "NSError+MiraRemote.h"

#ifdef __cplusplus
extern "C" {
#endif
    
    // Primitives
    typedef void (*MREmptyCallback)(const char *identifier);
    
    typedef void (*MRBoolCallback)(const char *identifier, BOOL value);
    typedef void (*MRIntCallback)(const char *identifier, int value);
    typedef void (*MRFloatCallback)(const char *identifier, float value);

    // Errors
    typedef void (*MRErrorCallback)(const char *identifier, BOOL success, MRError errorCode, const char *message);
    
    // Remotes
    typedef void (*MRDiscoveredRemoteCallback)(const char *identifier, const char *name, const char *remoteIdentifier, int rssi, bool isPreferred);
    typedef void (*MRConnectedRemoteCallback)(const char *identifier,
                                              const char *name,
                                              const char *remoteIdentifier,
                                              const char *productName,
                                              const char *serialNumber,
                                              const char *hardwareIdentifier,
                                              const char *firmwareVersion,
                                              float batteryPercentage,
                                              int rssi,
                                              bool isConnected,
                                              bool isPreferred);
    
    // Remote Inputs
    typedef void (*MRRemoteMotionCallback)(const char *identifier, float x, float y, float z);
    

    // MRRemoteManager
    void MRRemoteManagerBluetoothState(const char *identifier, MRIntCallback callback);
    
    void MRRemoteManagerAutomaticallyConnectsToPreviousConnectedRemote(const char *identifier, MRBoolCallback callback);
    void MRRemoteManagerSetAutomaticallyConnectsToPreviousConnectedRemote(const char *identifier, BOOL value, MRBoolCallback callback);
    
    void MRRemoteManagerStart();
    
    void MRRemoteManagerStartRemoteDiscovery(const char *identifier, MRDiscoveredRemoteCallback callback, MRErrorCallback errorCallback);
    void MRRemoteManagerStopRemoteDiscovery();
    
    void MRRemoteManagerConnectRemote(const char *identifier, const char *remoteIdentifier, MRConnectedRemoteCallback callback, MRErrorCallback errorCallback);
    void MRRemoteManagerDisconnectConnectedRemote(const char *identifier, MRErrorCallback callback);
    
    // MRRemote
    void MRRemoteRefresh(const char *identifier, const char *remoteIdentifier, MRConnectedRemoteCallback callback, MRErrorCallback errorCallback);
    
    // MRRemote Inputs
    void MRRemoteButtonInputAddListener(const char *identifier, const char *remoteIdentifier, const char *keyPath, MRBoolCallback callback);
    void MRRemoteTouchInputAddListener(const char *identifier, const char *remoteIdentifier, const char *keyPath, MRBoolCallback callback);
    void MRRemoteAxisInputAddListener(const char *identifier, const char *remoteIdentifier, const char *keyPath, MRFloatCallback callback);
    void MRRemoteTouchPadInputAddListener(const char *identifier, const char *remoteIdentifier, const char *keyPath, MRBoolCallback callback);
    void MRRemoteMotionInputAddListener(const char *identifier, const char *remoteIdentifier, const char *keyPath, MREmptyCallback callback);
    void MRRemoteMotionSensorInputAddListener(const char *identifier, const char *remoteIdentifier, const char *keyPath, MRRemoteMotionCallback callback);
    void MRRemoteOrientationInputAddListener(const char *identifier, const char *remoteIdentifier, const char *keyPath, MRRemoteMotionCallback callback);
    
    // Notifications
    void MRRegisterForRemoteDidConnectNotification(const char *identifier, MRConnectedRemoteCallback callback);
    void MRRegisterForRemoteDidDisconnectNotification(const char *identifier, MRDiscoveredRemoteCallback callback);
    void MRRegisterForRemoteDidRefreshNotification(const char *identifier, MRDiscoveredRemoteCallback callback);

    
#ifdef __cplusplus
}
#endif
