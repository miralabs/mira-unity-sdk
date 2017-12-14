//
//  MRRemoteOrientationInput.h
//  MiraRemote
//
//  Created by Riley Testut on 9/22/17.
//  Copyright © 2017 Mira Labs, Inc. All rights reserved.
//

#import <Foundation/Foundation.h>

NS_ASSUME_NONNULL_BEGIN

/**
 The MRRemoteOrientationInput class represents the current attitude of the Mira Prism Remote
 as a series of rotations around the x, y, and z axis.
 
 The Mira Prism Remote is capable of detecting pitch, yaw, and roll in three-dimensional space.
 Whenever one of these values updates, the remote's orientation is considered to have changed, and so
 this input will update to reflect the remote's rotation along each axis.
 
 You may choose to either poll the state of the input every event loop, or be notified when the values change.
 When the values change, MRRemoteOrientationInput's values will reflect the current pitch, yaw, and roll
 of the remote, and the valueChangedHandler will be called, if one has been assigned.
 */
@interface MRRemoteOrientationInput : NSObject

/**
 The remote's rotation around the x-axis.
 */
@property (nonatomic, readonly) float pitch;

/**
 The remote's rotation around the y-axis.
 */
@property (nonatomic, readonly) float yaw;

/**
 The remote's rotation around the z-axis.
 */
@property (nonatomic, readonly) float roll;

/**
 A block to be called whenever the remote has rotated.
 
 @discussion
 Passes in the MRRemoteOrientationInput itself, and the current Euler angles.
 */
@property (nullable, copy, nonatomic) void (^valueChangedHandler)(MRRemoteOrientationInput *orientation, float pitch, float yaw, float roll);

@end

NS_ASSUME_NONNULL_END
