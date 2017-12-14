//
//  WTObjectTracker.h
//  WikitudeNativeSDK
//
//  Created by Alexandru Florea on 03/11/16.
//  Copyright © 2016 Wikitude. All rights reserved.
//

#ifndef WTObjectTracker_h
#define WTObjectTracker_h

#import <Foundation/Foundation.h>

NS_ASSUME_NONNULL_BEGIN

@class WTObjectTracker;
@class WTObjectTarget;

/**
 * @Brief WTOjectTrackerDelegate provides methods that propagate information about tracker state changes.
 *
 * Tracker objects are not created by calling their init method, but through the provided factory methods of WTTrackerManager.
 *
 * @discussion Matrix pointer that are provided inside the WTObjectTarget objects are only valid in the current method scope. If they are needed at a later point in time, they have to be copied into different memory that is controlled by the Wikitude Native SDK hosting application.
 */
@protocol WTObjectTrackerDelegate <NSObject>

@required
/**
 * @Brief Called whenever a tracker recognizes a new object target that was previously unknown.
 *
 * @discussion All WTObjectTarget matrix pointer are set to nullptr and should not be used.
 *
 * @param objectTracker The tracker object that recognized the new object target
 * @param recognizedObject The object target that was recognised.
 */
- (void)objectTracker:(nonnull WTObjectTracker *)objectTracker didRecognizeObject:(nonnull WTObjectTarget *)recognizedObject;

/**
 * @brief Called whenever a previously known object target was tracked again.
 *
 * @param objectTracker The tracker object that tracked the object target
 * @param trackedObject The object target tat was tracked again
 */
- (void)objectTracker:(nonnull WTObjectTracker *)objectTracker didTrackObject:(nonnull WTObjectTarget *)trackedObject;

/**
 * @brief Called whenever a previously know object target is not found anymore in the current camera frame.
 *
 * @discussion All WTObjectTarget matrix pointer are set to nullptr and should not be used.
 *
 * @param objectTracker The tracker object that lost the object target.
 * @param lostObject The object target that was lost
 */
- (void)objectTracker:(nonnull WTObjectTracker *)objectTracker didLoseObject:(nonnull WTObjectTarget *)lostObject;

@optional
/**
 * @brief Called when an object tracker was successfully initialized.
 *
 * @param objectTracker The object tracker that was initialized
 */
- (void)objectTrackerDidLoadTargets:(nonnull WTObjectTracker *)objectTracker;

/**
 * @brief Called whenever an object tracker could not be initialized.
 *
 * @param objectTracker The object tracker that could not be initialized
 * @param error A NSError object that contains more information about why the object tracker could not be initialized
 */
- (void)objectTracker:(nonnull WTObjectTracker *)objectTracker didFailToLoadTargets:(nonnull NSError *)error;

@end

/**
 * @brief Represents a tracker that was initialized with a WTTargetCollectionResource. It is intended to track objects only.
 */
@interface WTObjectTracker : NSObject

/**
 * @brief The delegate object that is associated with this object tracker object.
 *
 * @discussion The delegate object is usually set through the appropriate WTTrackerManger factory method.
 */
@property (nonatomic, weak) id<WTObjectTrackerDelegate>                 delegate;

@end

NS_ASSUME_NONNULL_END

#endif /* WTObjectTracker_h */
