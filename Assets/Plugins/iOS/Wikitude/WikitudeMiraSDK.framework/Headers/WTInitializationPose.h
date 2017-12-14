//
//  WTInitializationPose.h
//  WikitudeNativeSDK
//
//  Created by Alexandru Florea on 17/11/16.
//  Copyright © 2016 Wikitude. All rights reserved.
//

#ifndef WTInitializationPose_h
#define WTInitializationPose_h

#import <Foundation/Foundation.h>
#import "WTTarget.h"

NS_ASSUME_NONNULL_BEGIN

/**
 * @brief Contains tracking information returned by the instant tracker when it is running in initializing state.
 */
@interface WTInitializationPose : WTTarget

@end

NS_ASSUME_NONNULL_END

#endif /* WTInitializationPose_h */
