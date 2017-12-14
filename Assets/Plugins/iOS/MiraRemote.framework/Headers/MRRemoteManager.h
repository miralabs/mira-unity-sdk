//
//  MRRemoteManager.h
//  MiraRemote
//
//  Created by Riley Testut on 9/15/17.
//  Copyright © 2017 Mira Labs, Inc. All rights reserved.
//

#import <Foundation/Foundation.h>
#import <CoreBluetooth/CoreBluetooth.h>

#import "MRRemote.h"

NS_ASSUME_NONNULL_BEGIN

/**
 The MRRemoteManager class serves as a singleton who manages discovering and
 connecting to nearby Mira Prism Remotes.
 
 While MRRemote is a representation of the physical remote, almost any actions performed
 involving remotes, such as discovering or connecting to them, is handled by
 MRRemoteManager.
 
 Additionally, MRRemoteManager also serves as a way to retrieve the device's current Bluetooth
 state, which can help dictate what needs to be shown in your UI at any point.
 */
@interface MRRemoteManager : NSObject

/**
 Returns the shared manager instance.
 
 @discussion
 You never intialize a new MRRemoteManager object. Instead, use this class method to get the
 shared instance.
 */
@property (class, nonatomic, readonly) MRRemoteManager *sharedManager;

/**
 Indicates whether the MRRemoteManager has been started yet.
 
 @discussion
 No operations can be performed until the MRRemoteManager has started.
 */
@property (nonatomic, readonly, getter=isStarted) BOOL started;

/**
 Indicates whether the MRRemoteManager is currently attempting to discover nearby remotes.
 
 @discussion
 You cannot start multiple remote discovery sessions at once, so make sure to check this
 property before starting a session.
 */
@property (nonatomic, readonly, getter=isDiscoveringRemotes) BOOL discoveringRemotes;

/**
 The current Bluetooth state of the device.
 
 @discussion
 If -[MRRemoteManager bluetoothState] is not CBManagerStatePoweredOn,
 then no Bluetooth functionality will be available.
 */

#if TARGET_OS_OSX
@property (nonatomic, readonly) CBCentralManagerState bluetoothState;
#else
@property (nonatomic, readonly) CBManagerState bluetoothState;
#endif

/**
 The currently connected remote.
 
 @discussion
 Only one remote can be connected at any time, so when a new remote is connected,
 the previous remote will be disconnected.
 */
@property (nullable, nonatomic, readonly) MRRemote *connectedRemote;

/**
 The currently discovered nearby remotes.
 
 @discussion
 This array is only populated when MRRemoteManager is currently attempting to discover nearby remotes.
 When MRRemoteManager is no longer searching for remotes, this array will be empty.
 */
@property (nonatomic, readonly) NSArray<MRRemote *> *discoveredRemotes;

/**
 Determines whether the MRRemoteManager should attempt to connect to the previously connected
 remote when first started, or after Bluetooth becomes available.
 */
@property (nonatomic) BOOL automaticallyConnectsToPreviousConnectedRemote;

- (instancetype)init NS_UNAVAILABLE;

/**
 Starts the MRRemoteManager.
 
 @discussion
 No operations can be performed until the MRRemoteManager has started, so make sure to call this
 early in your app's lifecycle.
 */
- (void)start;

/**
 Starts searching for nearby remotes.

 @param error The error that occured if MRRemoteManager could not begin searching for nearby remotes.
 @param discoveryHandler Block called every time a new remote is discovered. Can be used to update UI.
 @return YES on success, NO if there was an error starting remote discovery.
 */
- (BOOL)startRemoteDiscoveryWithError:(NSError **)error discoveryHandler:(void (^)(MRRemote *))discoveryHandler;

/**
 Stops searching for nearby remotes.
 */
- (void)stopRemoteDiscovery;


/**
 Attempts to connect to the provided MRRemote.

 @param remote The remote to try to connect to.
 @param completionHandler Block called when either the connection is established, or an error occured.
 
 @discussion
 Only one remote can be connected at any time, so if this method is successful,
 the previous remote will be disconnected.
 */
- (void)connectRemote:(MRRemote *)remote completionHandler:(void (^)(NSError *_Nullable))completionHandler;


/**
 Disconnects the currently connected remote.

 @param completionHandler Block called when either the remote has been disconnected, or an error occured.
 */
- (void)disconnectConnectedRemoteWithCompletionHandler:(void (^)(NSError *_Nullable))completionHandler;

@end

NS_ASSUME_NONNULL_END
