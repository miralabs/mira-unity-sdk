//
//  WTImageTracker.h
//  WikitudeNativeSDK
//
//  Created by Alexandru Florea on 21/10/16.
//  Copyright © 2016 Wikitude. All rights reserved.
//

#ifndef WTImageTracker_h
#define WTImageTracker_h

#import <Foundation/Foundation.h>

#import "WTBaseTracker.h"

NS_ASSUME_NONNULL_BEGIN

@class WTImageTarget;
@class WTImageTracker;
@class WTImageTrackerConfiguration;

/**
 * @Brief WTImageTrackerDelegate provides methods that propagate information about tracker state changes.
 *
 * Tracker objects are not created by calling their init method, but through the provided factory methods of WTTrackerManager.
 *
 * @discussion Matrix pointer that are provided inside the WTImageTarget objects are only valid in the current method scope. If they are needed at a later point in time, they have to be copied into different memory that is controlled by the Wikitude Native SDK hosting application.
 */
@protocol WTImageTrackerDelegate <NSObject>

@required
/**
 * @Brief Called whenever a tracker recognizes a new image target that was previously unknown.
 *
 * @discussion All WTImageTarget matrix pointer are set to nullptr and should not be used.
 *
 * @param imageTracker The tracker object that recognized the new image target
 * @param recognizedTarget The image target that was recognised.
 */
- (void)imageTracker:(nonnull WTImageTracker *)imageTracker didRecognizeImage:(nonnull WTImageTarget *)recognizedTarget;

/**
 * @brief Called whenever a previously known image target was tracked again.
 *
 * @param imageTracker The tracker object that tracked the image target
 * @param trackedTarget The image target tat was tracked again
 */
- (void)imageTracker:(nonnull WTImageTracker *)imageTracker didTrackImage:(nonnull WTImageTarget *)trackedTarget;

/**
 * @brief Called whenever a previously know image target is not found anymore in the current camera frame.
 *
 * @discussion All WTImageTarget matrix pointer are set to nullptr and should not be used.
 *
 * @param imageTracker The tracker object that lost the image target.
 * @param lostTarget The image target that was lost
 */
- (void)imageTracker:(nonnull WTImageTracker *)imageTracker didLoseImage:(nonnull WTImageTarget *)lostTarget;

@optional
/**
 * @brief Called when an image tracker was successfully initialized.
 *
 * @param imageTracker The image tracker that was initialized
 */
- (void)imageTrackerDidLoadTargets:(nonnull WTImageTracker *)imageTracker;

/**
 * @brief Called whenever an image tracker could not be initialized.
 *
 * @param imageTracker The image tracker that could not be initialized
 * @param error A NSError object that contains more information about why the image tracker could not be initialized
 */
- (void)imageTracker:(nonnull WTImageTracker *)imageTracker didFailToLoadTargets:(nonnull NSError *)error;

/**
 * @brief Called whenever a tracker is marked to be extended and the extended tracking quality changes.
 *
 * During a tracking session, the extended tracking quality might get better or worse, depending on the scene that is captured.
 *
 * @discussion This method is also called when extended tracking processed the first frame, meaning that oldTrackingQuality is then set to a non WTExtendedTrackingQuality value.
 *
 * @param imageTracker The tracker object that changed the extended tracking quality
 * @param extendedTarget The name of the image target that changed the extended tracking quality
 * @param oldTrackingQuality The previously extended tracking quality
 * @param newTrackingQuality The extended tracking quality that is now reached
 */
- (void)imageTracker:(nonnull WTImageTracker *)imageTracker didUpdateExtendedTrackingQualityForTarget:(NSString *)extendedTarget fromQuality:(WTExtendedTrackingQuality)oldTrackingQuality toQuality:(WTExtendedTrackingQuality)newTrackingQuality;

@end

/**
 * @brief Represents a tracker that was initialized with a WTTargetCollectionResource or a WTCloudRecognitionService. It is intended to track images only.
 */
@interface WTImageTracker : NSObject

/**
 * @brief The delegate object that is associated with this image tracker object.
 *
 * @discussion The delegate object is usually set through the appropriate WTTrackerManger factory method.
 */
@property (nonatomic, weak) id<WTImageTrackerDelegate>                 delegate;


/**
 * @brief Use this method to define which targets should be extended after the image tracker was created.
 *
 * @discussion Please note that calling this method will replace all extended target definitions that were set before (also the ones from the image tracker configuration).
 *
 * @param extendedTargets An array of strings containing the names of targets that should be treated as extended targets.
 */
- (void)setExtendedTargets:(NSArray<NSString *> *)extendedTargets;

/**
 * @brief Stops any extended tracking session if started.
 *
 * @discussion After stopping an extended tracking session, the tracker will look again for a target image and resume extended tracking when it finds one.
 */
- (void)stopExtendedTracking;


/**
 * @brief Returns true if the image tracker is configured to run in extended tracking mode.
 */
- (BOOL)isExtendedTrackingActive;

/**
 * @breif Use this method to change the frequency of distance changed callbacks from the WTImageTargetDelegate
 *
 * Based on the value given, distance changed callbacks are called more or less frequently
 *
 * @param distanceChangedThreshold The threshold in millimeter that should be used.
 */
- (void)setDistanceChangedThreshold:(float)distanceChangedThreshold;

@end


NS_ASSUME_NONNULL_END

#endif /* WTImageTracker_h */
