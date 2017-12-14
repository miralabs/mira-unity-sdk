//
//  MRRemote.h
//  MiraRemote
//
//  Created by Riley Testut on 9/15/17.
//  Copyright © 2017 Mira Labs, Inc. All rights reserved.
//

#import "MRRemoteButtonInput.h"
#import "MRRemoteTouchPadInput.h"
#import "MRRemoteMotionInput.h"

#import <Foundation/Foundation.h>

NS_ASSUME_NONNULL_BEGIN

/**
 Posted immediately after a remote has been refreshed, either manually or automatically.
 
 The notification object is the MRRemote object that represents the refreshed remote.
 */
extern NSNotificationName const MRRemoteDidRefreshNotification;

/**
 Posted immediately after a new remote is connected to the device.
 
 The notification object is the MRRemote object that represents the connected remote.
 */
extern NSNotificationName const MRRemoteDidConnectNotification;

/**
 Posted immediately after a remote is disconnected from the device.
 
 The notification object is the MRRemote object that represents the disconnected remote.
 */
extern NSNotificationName const MRRemoteDidDisconnectNotification;

/**
 A key to retrieve the error describing the reason for a remote disconnecting, if one exists.
 */
extern NSString *const MRRemoteDisconnectionErrorKey;


/**
 The MRRemote class represents a Mira Prism Remote that is within Bluetooth LE range of the device.
 
 When first discovered, only the identifier and name fields will be populated, and the remaining information will be nil.
 
 To retrieve the remaining information from the device, you must first connect to the MRRemote with
 MRRemoteManager, at which point the remaining properties will be automatically populated.
 
 Example:
 
 @code [MRRemoteManager.sharedManager connectRemote:remote completionHandler:nil];
 */
@interface MRRemote : NSObject

/**
 The UUID associated with the remote.
 */
@property (nonatomic, readonly) NSUUID *identifier;

/**
 The name of the remote.
 */
@property (nullable, copy, nonatomic, readonly) NSString *name;

/**
 The product name given by the manufacturer of the device.
 
 @note The remote must be connected to the device for this property to be non-nil.
 */
@property (nullable, copy, nonatomic, readonly) NSString *productName;

/**
 The serial number of the remote.
 
 @note The remote must be connected to the device for this property to be non-nil.
 */
@property (nullable, copy, nonatomic, readonly) NSString *serialNumber;

/**
 The hardware identifier of the remote.
 
 @note The remote must be connected to the device for this property to be non-nil.
 */
@property (nullable, copy, nonatomic, readonly) NSString *hardwareIdentifier;

/**
 The current version of the firmware installed on the remote.
 
 @note The remote must be connected to the device for this property to be non-nil.
 */
@property (nullable, copy, nonatomic, readonly) NSString *firmwareVersion;


/**
 The current battery percentage of the remote. Represented as a float from 0 to 1.
 
 @note The remote must be connected to the device for this property to be non-nil.
 */
@property (nullable, copy, nonatomic, readonly) NSNumber *batteryPercentage;

/**
 The RSSI, in decibels, of the remote.
 
 @discussion You can use a connected remote's RSSI property to determine the remote's proximity.
 
 @note The remote must be connected to the device for this property to be non-nil.
 */
@property (nullable, copy, nonatomic, readonly) NSNumber *RSSI;


/**
 Indicates whether the device has established a connection to the remote.
 
 @discussion
 When the value of this property is NO, the remote has been discovered by the device, but no connection has been made.
 Without a connection, several properties cannot be read, such as the remote's battery percentage.
 */
@property (nonatomic, readonly, getter=isConnected) BOOL connected;

/**
 Indicates whether the remote is preferred by the user to other nearby remotes.
 
 @discussion
 To determine whether a remote is preferred or not, the device checks the advertised service UUIDs of the remote.
 Normally, the advertised service UUIDs contain the default UUID, and in this case the device is not considered preferred.
 However, if the user presses and holds both the menu and home buttons, this advertised UUID changes,
 and the remote is considered to be a preferred remote.
 */
@property (nonatomic, readonly, getter=isPreferred) BOOL preferred;


/**
 The remote's Menu button.
 
 @discussion The Menu button is the top circular button on the front of the remote.
 */
@property (nonatomic, readonly) MRRemoteButtonInput *menuButton;

/**
 The remote's Home button.
 
 @discussion The Home button is the bottom circular button on the front of the remote.
 */
@property (nonatomic, readonly) MRRemoteButtonInput *homeButton;

/**
 The remote's trigger.
 
 @discussion The trigger is located on the back of the remote.
 */
@property (nonatomic, readonly) MRRemoteButtonInput *trigger;

/**
 The remote's Touch Pad.
 
 @discussion The Touch Pad is smooth circular pad on the front of the remote.
 */
@property (nonatomic, readonly) MRRemoteTouchPadInput *touchPad;


/**
 The remote's current motion.
 
 @discussion Consists of the remote's pitch, yaw, and roll.
 */
@property (nonatomic, readonly) MRRemoteMotionInput *motion;

- (instancetype)init NS_UNAVAILABLE;


/**
 Updates the remote's properties by reading them from the device.
 
 @param completionHandler
 Once the remote's properties have been refreshed, the completion handler will be called,
 passing in any errors that occured during the refresh.
 
 @discussion
 While most properties are static, some, such as battery percentage, need to be explicitly refreshed
 in order to reflect the current state of the remote.
 
 If there were errors when communicating with the remote, they will be provided in the completionHandler's errors set.
 */
- (void)refreshWithCompletionHandler:(void (^)(NSSet *_Nullable errors))completionHandler;

@end

NS_ASSUME_NONNULL_END
