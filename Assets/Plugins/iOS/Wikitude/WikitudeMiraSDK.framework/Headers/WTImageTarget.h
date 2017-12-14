//
//  WTRecognizedTarget.h
//  WikitudeNativeSDK
//
//  Created by Andreas Schacherbauer on 28/04/15.
//  Copyright (c) 2015 Wikitude. All rights reserved.
//

#import <Foundation/Foundation.h>
#import <CoreGraphics/CGGeometry.h>

#import "WTTarget.h"


NS_ASSUME_NONNULL_BEGIN

@class WTImageTarget;

/**
 * WTImageTargetDelegate can be used to react to internal image target changes
 */
@protocol WTImageTargetDelegate <NSObject>

@required

/**
 * @brief This method is called in case the distance change between two targets exceeds the threshold defined by the `distanceChangedThreshold` property of WTImageTrackerConfiguration.
 *
 * @discussion For accurate distance calculations, physical target heights needs to be provided either through the .wtc or the `physicalTargetImageHeights` property of WTImageTrackerConfiguration
 *
 * @param distance The distance between the two targets in millimeter
 * @param firstTarget The first of the two image targets that changed there distance to each other
 * @param secondTarget The second of the two image targets that changed there distance to each other
 */
- (void)didRecognizeDistanceChange:(NSInteger)distance betweenImageTarget:(WTImageTarget *)firstTarget andImageTarget:(WTImageTarget *)secondTarget;

@end

/**
 * WTImageTarget represents image targets that are found by an image tracker.
 * 
 * @discussion This class should not be instantiated from outside of the Wikitude Native SDK.
 */
@interface WTImageTarget : WTTarget <NSCopying>

/**
 * The object that acts as a delegate for the WTImageTarget
 */
@property (nonatomic, weak) id<WTImageTargetDelegate>               delegate;

/**
 * The name of the associated target image in the wikitude target collection (.wtc).
 *
 * @return name The name of the image target
 */
@property (nonatomic, strong, readonly) NSString                    *name;

/**
 * The unique id of the ImageTarget. This unique id is incremented with every recognition of the same target.
 *
 * @return uniqueId The unique id of the image target
 */
@property (nonatomic, assign, readonly) long                        uniqueId;

/**
 * Defines a normalized scale value in x direction that represents the image dimensions proportionally to the uniform scaling given through the modelView matrix
 *
 * @return xScale normalized scale in x direction
 */
@property (nonatomic, assign, readonly) CGFloat                     xScale;

/**
 * Defines a normalized scale value in y direction that represents the image dimensions proportionally to the uniform scaling given through the modelView matrix
 *
 * @return xScale normalized scale in y direction
 */
@property (nonatomic, assign, readonly) CGFloat                     yScale;

/**
 * The physical height of the image target as it is defined in the .wtc or through the WTImageTrackerConfiguration property `physicalTargetImageHeights`
 *
 * @return physicalTargetHeight the physical target height in millimeter
 */
@property (nonatomic, assign, readonly) NSUInteger                  physicalTargetHeight;

/**
 * @brief The distance from the camera to the image target in millimeter.
 *
 * This value only contains reliable values if the .wtc file or the cloud archive included physical image target heights.
 *
 * @return distanceToTarget The physical distance in millimeter between the camera and the image target.
 */
@property (nonatomic, assign, readonly) NSUInteger                  distanceToTarget;

/**
 * @brief Use this method to calculate the physical distance between two image targets
 *
 * The distance is calculated every time the method is called. In case the distance could not be calculated (imageTarget is nil), 0 is returned.
 *
 * @param imageTarget The image target to which the distance should be calculated to
 *
 * @return The physical distance in millimeter between this target and imageTarget
 */
- (CGFloat)distanceTo:(WTImageTarget*)imageTarget;

@end

NS_ASSUME_NONNULL_END
