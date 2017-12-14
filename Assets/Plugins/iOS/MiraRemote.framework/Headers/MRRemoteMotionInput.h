//
//  MRRemoteMotionInput.h
//  MiraRemote
//
//  Created by Riley Testut on 9/16/17.
//  Copyright © 2017 Mira Labs, Inc. All rights reserved.
//

#import <Foundation/Foundation.h>

#import "MRRemoteOrientationInput.h"
#import "MRRemoteMotionSensorInput.h"

NS_ASSUME_NONNULL_BEGIN

/**
 The MRRemoteMotionInput class represents the current motion of the Mira Prism Remote.
 
 The Mira Prism Remote has both an accelerometer and gyroscope, and provides access to the raw
 sensor data through its acceleration and rotationRate properties. Additionally, the remote can
 use these instruments to determine its orientation in three-dimensional space, which is exposed
 through its orientation property.
 
 You may choose to either poll the state of the input every event loop, or be notified when the values change.
 When the values change, MRRemoteMotionInput's values will reflect the current orientation, acceleration, and
 rotation rate of the remote, and the valueChangedHandler will be called, if one has been assigned.
 */
@interface MRRemoteMotionInput : NSObject

/**
 A MRRemoteOrientationInput representing the orientation of the remote in 3D space.
 
 Measured in Degrees.
 */
@property (nonatomic, readonly) MRRemoteOrientationInput *orientation;

/**
 A MRRemoteMotionSensorInput representing the remote's acceleration.
 
 Measured in g's (where one g = acceleration due to gravity).
 */
@property (nonatomic, readonly) MRRemoteMotionSensorInput *acceleration;

/**
 A MRRemoteMotionSensorInput representing the remote's rotation rate.
 
 Measured in Degrees per Second.
 */
@property (nonatomic, readonly) MRRemoteMotionSensorInput *rotationRate;

/**
 A block to be called whenever the composite state of the remote's motion changes.
 
 @discussion
 Passes in the MRRemoteMotionInput itself, as well as its orientataion, acceleration, and rotationRate.
 */
@property (nullable, copy, nonatomic) void (^valueChangedHandler)(MRRemoteMotionInput *motion, MRRemoteOrientationInput *orientation, MRRemoteMotionSensorInput *acceleration, MRRemoteMotionSensorInput *rotationRate);

@end

NS_ASSUME_NONNULL_END
