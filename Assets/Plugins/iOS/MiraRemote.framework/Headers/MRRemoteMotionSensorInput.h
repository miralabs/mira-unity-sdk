//
//  MRRemoteMotionSensorInput.h
//  MiraRemote
//
//  Created by Riley Testut on 9/22/17.
//  Copyright © 2017 Mira Labs, Inc. All rights reserved.
//

#import <Foundation/Foundation.h>

NS_ASSUME_NONNULL_BEGIN

/**
 The MRRemoteMotionSensorInput class represents motion data measured by one of the Mira Prism Remote's
 motion sensors. Currently, this includes an accelerometer and a gyroscope.
 
 Each motion sensor measures motion in a three-dimensional space, and the value for each axis
 can be read independently via the relevant property (x, y, z).
 
 You may choose to either poll the state of the input every event loop, or be notified when the values change.
 When the values change, MRRemoteMotionSensorInput's values will reflect the current x, y, and z axis values
 as measured by the sensor, and the valueChangedHandler will be called, if one has been assigned.
 */
@interface MRRemoteMotionSensorInput : NSObject

/**
 The sensor's measurement along the x-axis.
 */
@property (nonatomic, readonly) float x;

/**
 The sensor's measurement along the y-axis.
 */
@property (nonatomic, readonly) float y;

/**
 The sensor's measurement along the z-axis.
 */
@property (nonatomic, readonly) float z;

/**
 A block to be called whenever the sensor's measured values changes.
 
 @discussion
 Passes in the MRRemoteMotionSensorInput itself, and the current sensor values.
 */
@property (nullable, copy, nonatomic) void (^valueChangedHandler)(MRRemoteMotionSensorInput *sensorInput, float x, float y, float z);

@end

NS_ASSUME_NONNULL_END
