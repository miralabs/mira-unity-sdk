//
//  NSError+MiraRemote.h
//  MiraRemote
//
//  Created by Riley Testut on 9/15/17.
//  Copyright © 2017 Mira Labs, Inc. All rights reserved.
//

#import <Foundation/Foundation.h>
#import <CoreBluetooth/CoreBluetooth.h>

/**
 The error domain for all errors produced by the MiraRemote framework.
 */
extern const _Nonnull NSErrorDomain MiraRemoteErrorDomain;

/**
 A key to retrieve the bluetooth state at the time an error occured.
 */
extern const _Nonnull NSErrorUserInfoKey MRBluetoothStateKey;

/**
 The error codes produced by the MiraRemote framework.
 */
typedef NS_ERROR_ENUM(MiraRemoteErrorDomain, MRError)
{
    /**
     An unknown error.
     */
    MRErrorUnknown = 0,
    
    /**
     Bluetooth is unavailable for the device.
     */
    MRErrorBluetoothUnavailable = 1,
    
    /**
     Bluetooth has been disabled for the device.
     */
    MRErrorBluetoothDisabled = 2,
    
    /**
     An attempt to discover remotes was made when a discovery process was already in progress.
     */
    MRErrorAlreadyDiscoveringRemotes = 3,
    
    /**
     There is no connected remote.
     */
    MRErrorNoConnectedRemote = 4,
    
    /**
     The requested remote could not be found.
     */
    MRErrorRemoteNotFound = 5,
    
    /**
     The MRRemoteManager has not yet been started.
     */
    MRErrorRemoteManagerNotStarted = 6,
};

NS_ASSUME_NONNULL_BEGIN

@interface NSError (MiraRemote)

/**
 The bluetooth state at the time this error occured.
 */
@property (nonatomic, readonly) CBManagerState bluetoothState NS_AVAILABLE(10.13, 10.0);

@end

NS_ASSUME_NONNULL_END
