//
//  MRRemoteTouchInput.h
//  MiraRemote
//
//  Created by Riley Testut on 9/16/17.
//  Copyright © 2017 Mira Labs, Inc. All rights reserved.
//

#import <Foundation/Foundation.h>

NS_ASSUME_NONNULL_BEGIN

/**
 The MRRemoteTouchInput class represents an input triggered by a user's touch on the Mira Prism Remote.
 
 A MRRemoteTouchInput can be in one of two states: active, or inactive.
 
 You may choose to either poll the state of the input every event loop, or be notified when the active state changes.
 When the active state changes, -[MRRemoteTouchInput isActive] will reflect the new state,
 and the activeChangedHandler will be called, if one has been assigned.
 */
@interface MRRemoteTouchInput : NSObject

/**
 The current active state of the input.
 
 When YES, the user is touching the input with their finger.
 When NO, the user is not touching the input.
 */
@property (nonatomic, readonly, getter=isActive) BOOL active;

/**
 A block to be called whenever the active state of the input changes.
 
 @discussion
 Passes in the MRRemoteTouchInput itself, and the current active state.
 */
@property (nullable, copy, nonatomic) void (^activeChangedHandler)(MRRemoteTouchInput *touchInput, BOOL active);

@end

NS_ASSUME_NONNULL_END
