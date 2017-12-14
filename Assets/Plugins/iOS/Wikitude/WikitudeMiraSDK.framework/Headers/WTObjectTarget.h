//
//  WTObjectTarget.h
//  WikitudeNativeSDK
//
//  Created by Alexandru Florea on 03/11/16.
//  Copyright © 2016 Wikitude. All rights reserved.
//

#import <Foundation/Foundation.h>
#import <CoreGraphics/CGGeometry.h>

#import "WTTarget.h"


NS_ASSUME_NONNULL_BEGIN

/**
 * WTObjectTarget represents object targets that are found by an object tracker.
 *
 * x/y/z scale values can be used to convert the 'unit cube' modelView matrix into a box that covers just the outer dimensions of the object.
 * Example: Consider a truck like shaped object target. In case an OpenGL/Metal cube with edge length of one would be drawn with the modelView matrix, the cube would be drawn without distortions in the center of the object.
 * To render a box around the truck, x/y/z scale values are used to create another scale matrix that is multiplied with the given modelView matrix. The result is then a box that encompasses the object target at it's outer dimensions.
 */
@interface WTObjectTarget : WTTarget

/**
 * The name is defined through the video file name that was uploaded in the Wikitude Target Manager in order to generate the object target
 *
 * @return name The name of the object target
 */
@property (nonatomic, strong, readonly) NSString                    *name;

/**
 * Defines a normalized scale value in x direction that represents the object dimensions proportionally to the uniform scaling given through the modelView matrix
 *
 * @return xScale normalized scale in x direction
 */
@property (nonatomic, assign, readonly) CGFloat                     xScale;

/**
 * Defines a normalized scale value in y direction that represents the object dimensions proportionally to the uniform scaling given through the modelView matrix
 *
 * @return xScale normalized scale in y direction
 */
@property (nonatomic, assign, readonly) CGFloat                     yScale;

/**
 * Defines a normalized scale value in x direction that represents the object dimensions proportionally to the uniform scaling given through the modelView matrix
 *
 * @return xScale normalized scale in x direction
 */
@property (nonatomic, assign, readonly) CGFloat                     zScale;

@end

NS_ASSUME_NONNULL_END
