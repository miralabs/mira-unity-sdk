//
//  MRRemoteTouchPadInput.h
//  MiraRemote
//
//  Created by Riley Testut on 9/16/17.
//  Copyright © 2017 Mira Labs, Inc. All rights reserved.
//

#import "MRRemoteTouchInput.h"

#import "MRRemoteAxisInput.h"
#import "MRRemoteButtonInput.h"

#import <Foundation/Foundation.h>

NS_ASSUME_NONNULL_BEGIN

/**
 The MRRemoteTouchPadInput class represents an touch-sensitive pad on the Mira Prism Remote.
 
 At its core, a MRRemoteTouchPadInput is simply a two-dimensional MRRemoteTouchInput that also
 can be pressed like a MRRemoteButtonInput. However, it exposes these states through multiple abstractions,
 where each abstraction is a collection of several other inputs, allowing you to adapt it to easily
 fit your game logic.
 
 For example, if you just needed to know when the user taps or presses the Touch Pad, you can poll
 -[MRRemoteTouchPadInput isActive] and -[MRRemoteTouchPadInput.button isPressed] (or set the change
 handlers on both). Or, if you needed to use the Touch Pad as an analog stick equivalent, you can read
 the x- and y- axis values, normalized from -1 to 1.
 
 You may choose to either poll the state of the input every event loop, or be notified when the value changes.
 When the value changes, MRRemoteTouchPadInput's inputs will each reflect their own state,
 and the valueChangedHandler will be called, if one has been assigned.
 */
@interface MRRemoteTouchPadInput : MRRemoteTouchInput

/**
 A MRRemoteButtonInput representing the pressed state of the Touch Pad.
 */
@property (nonatomic, readonly) MRRemoteButtonInput *button;

/**
 A MRRemoteAxisInput representing the position of the user's finger along the horizontal axis.
 */
@property (nonatomic, readonly) MRRemoteAxisInput *xAxis;

/**
 A MRRemoteAxisInput representing the position of the user's finger along the vertical axis.
 */
@property (nonatomic, readonly) MRRemoteAxisInput *yAxis;

/**
 A MRRemoteTouchInput representing whether the user's finger is in the top portion of the Touch Pad.
 */
@property (nonatomic, readonly) MRRemoteTouchInput *up;

/**
 A MRRemoteTouchInput representing whether the user's finger is in the bottom portion of the Touch Pad.
 */
@property (nonatomic, readonly) MRRemoteTouchInput *down;

/**
 A MRRemoteTouchInput representing whether the user's finger is in the left portion of the Touch Pad.
 */
@property (nonatomic, readonly) MRRemoteTouchInput *left;

/**
 A MRRemoteTouchInput representing whether the user's finger is in the right portion of the Touch Pad.
 */
@property (nonatomic, readonly) MRRemoteTouchInput *right;

/**
 A block to be called whenever the composite state of the input changes.
 
 @discussion
 Passes in the MRRemoteTouchPadInput itself, the current active state, and the x- and y- axis values.
 These values can be combined with -[MRRemoteTouchPadInput.button isPressed] to further determine the exact state of the input.
 */
@property (nullable, copy, nonatomic) void (^valueChangedHandler)(MRRemoteTouchPadInput *touchPad, BOOL active, float xValue, float yValue);

@end

NS_ASSUME_NONNULL_END
