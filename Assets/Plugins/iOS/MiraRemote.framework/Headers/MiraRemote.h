//
//  MiraRemote.h
//  MiraRemote
//
//  Created by Riley Testut on 9/15/17.
//  Copyright © 2017 Mira Labs, Inc. All rights reserved.
//

#import <UIKit/UIKit.h>

//! Project version number for MiraRemote.
FOUNDATION_EXPORT double MiraRemoteVersionNumber;

//! Project version string for MiraRemote.
FOUNDATION_EXPORT const unsigned char MiraRemoteVersionString[];

// In this header, you should import all the public headers of your framework using statements like #import <MiraRemote/PublicHeader.h>

// Remote
#import <MiraRemote/MRRemote.h>

// Inputs
#import <MiraRemote/MRRemoteButtonInput.h>
#import <MiraRemote/MRRemoteTouchInput.h>
#import <MiraRemote/MRRemoteAxisInput.h>
#import <MiraRemote/MRRemoteTouchPadInput.h>
#import <MiraRemote/MRRemoteMotionInput.h>
#import <MiraRemote/MRRemoteOrientationInput.h>
#import <MiraRemote/MRRemoteMotionSensorInput.h>

// Manager
#import <MiraRemote/MRRemoteManager.h>

// Categories
#import <MiraRemote/NSError+MiraRemote.h>

