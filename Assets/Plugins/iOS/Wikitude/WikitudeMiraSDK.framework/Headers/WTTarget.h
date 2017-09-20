//
//  WTTarget.h
//  WikitudeNativeSDK
//
//  Created by Andreas Schacherbauer on 17/11/15.
//  Copyright © 2015 Wikitude. All rights reserved.
//

#import <Foundation/Foundation.h>

NS_ASSUME_NONNULL_BEGIN


/**
 * WTTarget is the base class for any target that is detected by the Wikitude Native SDK.
 *
 * It defines the most basic properties that can be used to place objects at a position where something was recognized.
 *
 * Objects of these class contain there data as long as the the object itself lives.
 */
@interface WTTarget : NSObject <NSCopying>

/**
 * @brief A 4x4 projection matrix that can be used to define how a image target is projected in 3d space.
 *
 * Might be set to NULL
 *
 * @return projection A 4x4 projection matrix that can be used to project a image target in a 3d space.
 */
@property (nonatomic, assign) const float*                projection;

/**
 * @brief A 4x4 modelView matrix that represents translation and rotation of a image target.
 *
 * Might be set to NULL
 *
 * @return modelView A 4x4 matrix that defines translation and rotation of a image target in a 3d space.
 */
@property (nonatomic, assign) const float*                modelView;

/**
 * @brief A combined model view projection matrix.
 *
 * Might be set to NULL
 *
 * @return modelViewProjection A 4x4 matrix that can be used to render a image target in 3d space.
 */
@property (nonatomic, assign) const float*                modelViewProjection;

@end

NS_ASSUME_NONNULL_END
