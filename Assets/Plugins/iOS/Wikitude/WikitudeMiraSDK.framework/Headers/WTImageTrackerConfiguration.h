//
//  WTImageTrackerConfiguration.h
//  WikitudeNativeSDK
//
//  Created by Alexandru Florea on 24/10/16.
//  Copyright © 2016 Wikitude. All rights reserved.
//

#ifndef WTImageTrackerConfiguration_h
#define WTImageTrackerConfiguration_h

#import <Foundation/Foundation.h>

NS_ASSUME_NONNULL_BEGIN


/**
 * @brief WTImageRecognitionRangeExtension defines if ImageTracker should use an optimized algorithm to detect target images in greater distance to the camera.
 *
 * Extending the recognition range requires HD camera frames processing and therefore needs more CPU power than the default recognition range.
 *
 * @discussion Extending the recognition distance requires more computational power of the CPU. Because of this we recommend to let the Wikitude SDK itself decide if this feature should be used on the current device or not.
 */
typedef NS_ENUM(NSUInteger, WTImageRecognitionRangeExtension) {
    /** Image recognition range extension will be used regardless of the current device performance */
    WTImageRecognitionRangeExtension_On,

    /** Image recognition range extension will not be used even though the current device might support it */
    WTImageRecognitionRangeExtension_Off,

    /** Image recognition range extension will be used depending on the current device performance. 32 bit devices would turn this setting off while 64 bit devices would turn it on */
    WTImageRecognitionRangeExtension_Auto
};


/**
 * @brief WTImageTrackerConfiguration represents additional values that can be used to configure how an image tracker behaves.
 */
@interface WTImageTrackerConfiguration : NSObject

/**
 * @brief An array of NSString objects that represent which targets of the .wtc file should be treated as extended targets.
 *
 * @discussion The extended targets array is usually set through the client tracker creation factory method.
 */
@property (nonatomic, strong) NSArray                               *extendedTargets;

/**
 * @brief Defines the image recognition range extension that should be used when target image recognition is active.
 *
 * @warning Setting this property to WTImageRecognitionRangeExtension_On on 32 bit devices will work but slows down computer vision algorithms performance noticable
 *
 * @default WTImageRecognitionRangeExtension_Auto
 */
@property (nonatomic, assign) WTImageRecognitionRangeExtension      imageRecognitionRangeExtension;

/**
 * @brief Use this property to specify the physical height of individual image targets that are included in the .wtc file. 
 *
 * Physical target image heights are necessary as soon as any distance related API is used from the WTImageTarget
 *
 * @discussion This property is helpful if the physical image target height definition is missing in the .wtc file or if the values should be overriden with the given ones.
 */
@property (nonatomic, strong) NSDictionary<NSString *, NSNumber *>  *physicalTargetImageHeights;

/**
 * @brief An integer value denoting how many concurrent targets are to be recognized and tracked simultaneously.
 *
 * @discussion Depending on the current device, more targets can be recognized and tracked simultaneously. 
 * In case the maximum number of simultaneously targets is known for a specific use case, it's good advice to set this property. The Wikitude Native SDK will stop searching for new image targets in case the limit is reached which results in better battery lifetime
 */
@property (nonatomic, assign) int                   maximumNumberOfConcurrentlyTrackableTargets;

/**
 * @brief Use this property to limit the number of distance changed callbacks comming from the WTImageTargetDelegate.
 *
 * The value is interpreted as distance in millimeter.
 *
 * @discussion In order to receive accurate distance values, please provide physical target height information in the .wtc file or through the `physicalTargetImageHeights` property
 */
@property (nonatomic, assign) float                 distanceChangedThreshold;

@end

NS_ASSUME_NONNULL_END

#endif /* WTImageTrackerConfiguration_h */
