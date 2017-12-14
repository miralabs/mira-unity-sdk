//
//  MRRemoteButtonInput.h
//  MiraRemote
//
//  Created by Riley Testut on 9/16/17.
//  Copyright © 2017 Mira Labs, Inc. All rights reserved.
//

#import <Foundation/Foundation.h>

NS_ASSUME_NONNULL_BEGIN

/**
 The MRRemoteButtonInput class represents a physical button on the Mira Prism Remote.
 
 A MRRemoteButtonInput can be in one of two states: pressed, or not pressed.
 
 You may choose to either poll the state of the button every event loop, or be notified when the pressed state changes.
 When the pressed state changes, -[MRRemoteButtonInput isPressed] will reflect the new state,
 and the pressedChangedHandler will be called, if one has been assigned.
 */
@interface MRRemoteButtonInput : NSObject

/**
 The current pressed state of the button.
 */
@property (nonatomic, readonly, getter=isPressed) BOOL pressed;

/**
 A block to be called whenever the pressed state of the button changes.
 
 @discussion
 Passes in the MRRemoteButtonInput itself, and the current pressed state.
 */
@property (nullable, copy, nonatomic) void (^pressedChangedHandler)(MRRemoteButtonInput *button, BOOL pressed);

@end

NS_ASSUME_NONNULL_END
