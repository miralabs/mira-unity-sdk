//
//  MRRemoteAxisInput.h
//  MiraRemote
//
//  Created by Riley Testut on 9/16/17.
//  Copyright © 2017 Mira Labs, Inc. All rights reserved.
//

#import <Foundation/Foundation.h>

NS_ASSUME_NONNULL_BEGIN

/**
 The MRRemoteAxisInput class represents an analog input measuring a user's touch along a linear axis.
 
 A MRRemoteAxisInput's value ranges from -1 to 1, depending on the position of the touch along the axis.
 
 You may choose to either poll the state of the input every event loop, or be notified when the value changes.
 When the value changes, -[MRRemoteAxisInput value] will reflect the new state,
 and the valueChangedHandler will be called, if one has been assigned.
 */
@interface MRRemoteAxisInput : NSObject

/**
 The current position of a user's touch along the axis. Normalized from -1 to 1.
 */
@property (nonatomic, readonly) float value;

/**
 A block to be called whenever the value of the input changes.
 
 @discussion
 Passes in the MRRemoteAxisInput itself, and the current value.
 */
@property (nullable, copy, nonatomic) void (^valueChangedHandler)(MRRemoteAxisInput *axis, float value);

@end

NS_ASSUME_NONNULL_END
