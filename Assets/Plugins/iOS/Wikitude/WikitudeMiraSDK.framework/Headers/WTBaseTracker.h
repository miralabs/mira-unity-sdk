//
//  WTBaseTracker.h
//  WikitudeNativeSDK
//
//  Created by Andreas Schacherbauer on 28/04/15.
//  Copyright (c) 2015 Wikitude. All rights reserved.
//

#import <Foundation/Foundation.h>

#import "WTWikitudeTypes.h"


NS_ASSUME_NONNULL_BEGIN

/** 
 * Key in the options dictionary to refer to targets that should be treated as extended. The value for this key should be an NSArray containing NSStrings
 */
extern NSString * const kWTTrackerOptionKey_ExtendedTargets;

/**
 * Key in the options dictionary to define if a certain tracker should only perform recognitions instead of recognition + tracking.
 */
extern NSString * const kWTTrackerOptionKey_RecognitionOnly;


/**
 * WTExtendedTrackingQuality indicates the current extended tracking quality and how likely it is, that extended tracking will work.
 */
typedef NS_ENUM(NSInteger, WTExtendedTrackingQuality) {
    /**
     * WTExtendedTrackingQuality_Bad indicates that no extended tracking would work, if the target moves out of the camera image.
     */
    WTExtendedTrackingQuality_Bad = -1,
    
    /**
     * WTExtendedTrackingQuality_Average indicates that extended tracking might work, but not very likely or very good.
     */
    WTExtendedTrackingQuality_Average,
    
    /**
     * WTExtendedTrackingQuality_Good indicates that extended tracking will work very likely and stable.
     */
    WTExtendedTrackingQuality_Good
};


@class WTImageTarget;
@class WTBaseTracker;

/**
 * @Brief WTBaseTrackerDelegate provides methods that propagate information about tracker state changes.
 *
 * Tracker objects are not created by calling there init method, but through the provided factory methods of WTTrackerManager.
 *
 * @discussion Matrix pointer that are provided inside the WTImageTarget objects are only valid in the current method scope. If they are needed at a later point in time, they have to be copied into different memory that is controlled by the Wikitude Native SDK hosting application.
 *
 * @deprecated Since version 2.0.0. Use WTImageTrackerDelegate instead.
 */
WT_DEPRECATED_SINCE(2.0.0, "Use WTImageTrackerDelegate instead.")
@protocol WTBaseTrackerDelegate <NSObject>

@required
/**
 * @Brief Called whenever a tracker recognizes a new image target that was previously unknown.
 *
 * @discussion All WTImageTarget matrix pointer are set to nullptr and should not be used.
 *
 * @param baseTracker The tracker object that recognized the new image target
 * @param recognizedTarget The image target that was recognised.
 */
- (void)baseTracker:(nonnull WTBaseTracker *)baseTracker didRecognizedTarget:(nonnull WTImageTarget *)recognizedTarget NS_SWIFT_NAME(baseTracker(baseTracker:didRecognizeTarget:));

/**
 * @brief Called whenever a previously known image target was tracked again.
 *
 * @param baseTracker The tracker object that tracked the image target
 * @param trackedTarget The image target tat was tracked again
 */
- (void)baseTracker:(nonnull WTBaseTracker *)baseTracker didTrackTarget:(nonnull WTImageTarget *)trackedTarget;

/**
 * @brief Called whenever a previously know image target is not found anymore in the current camera frame.
 *
 * @discussion All WTImageTarget matrix pointer are set to nullptr and should not be used.
 *
 * @param baseTracker The tracker object that lost the image target.
 * @param lostTarget The image target that was lost
 */
- (void)baseTracker:(nonnull WTBaseTracker *)baseTracker didLostTarget:(nonnull WTImageTarget *)lostTarget NS_SWIFT_NAME(baseTracker(baseTracker:didLoseTarget:));

@optional
/**
 * @brief Called whenever a tracker is marked to be extended and the extended tracking quality changes.
 *
 * During a tracking session, the extended tracking quality might get better or worse, depending on the scene that is captured.
 *
 * @discussion This method is also called when extended tracking processed the first frame, meaning that oldTrackingQuality is then set to a non WTExtendedTrackingQuality value.
 *
 * @param baseTracker The tracker object that changed the extended tracking quality
 * @param extendedTarget The name of the image target that changed the extended tracking quality
 * @param oldTrackingQuality The previously extended tracking quality
 * @param newTrackingQuality The extended tracking quality that is now reached
 */
- (void)baseTracker:(nonnull WTBaseTracker *)baseTracker didUpdateExtendedTrackingQualityForTarget:(NSString *)extendedTarget fromQuality:(WTExtendedTrackingQuality)oldTrackingQuality toQuality:(WTExtendedTrackingQuality)newTrackingQuality;
@end


/**
 * @brief Base class for all types of tracker
 * 
 * A tracker represents a collection of image targets that should be searched for.
 *
 * @deprecated Since version 2.0.0. Use WTImageTracker in combination with an WTTargetCollectionResource or WTCloudRecognitionService instead.
 */
WT_DEPRECATED_SINCE(2.0.0, "Use WTImageTracker in combination with an WTTargetCollectionResource or WTCloudRecognitionService instead.")
@interface WTBaseTracker : NSObject

/**
 * @brief Stops any extended tracking session if started.
 *
 * After stopping an extended tracking session, it can't be resumed. A new tracker has to be created to enable extended tracking again.
 */
- (void)stopExtendedTracking;

@end

NS_ASSUME_NONNULL_END
