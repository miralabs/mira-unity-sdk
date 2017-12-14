//
//  MRRemoteMotionInput_Private.h
//  MiraRemote
//
//  Created by Riley Testut on 9/16/17.
//  Copyright © 2017 Mira Labs, Inc. All rights reserved.
//

#import "MRRemoteMotionInput.h"

NS_ASSUME_NONNULL_BEGIN

@interface MRRemoteMotionInput ()

- (void)setPitch:(float)pitch yaw:(float)yaw roll:(float)roll
   accelerationX:(float)accelerationX accelerationY:(float)accelerationY accelerationZ:(float)accelerationZ
    rotationRateX:(float)rotationRateX rotationRateY:(float)rotationRateY rotationRateZ:(float)rotationRateZ;

@end

NS_ASSUME_NONNULL_END
