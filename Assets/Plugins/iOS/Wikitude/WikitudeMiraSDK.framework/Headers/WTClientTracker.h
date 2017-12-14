//
//  WTTracker.h
//  WikitudeNativeSDK
//
//  Created by Andreas Schacherbauer on 28/04/15.
//  Copyright (c) 2015 Wikitude. All rights reserved.
//

#import <Foundation/Foundation.h>

#import "WTBaseTracker.h"

NS_ASSUME_NONNULL_BEGIN


@class WTClientTracker;

/**
 * WTClientTrackerDelegate provides client tracker specific methods that can be used to react to client tracker specific state changes.
 *
 * @deprecated Since version 2.0.0. Use WTImageTrackerDelegate instead.
 */
WT_DEPRECATED_SINCE(2.0.0, "Use WTImageTrackerDelegate and/or WTTargetCollectionResource instead.")
@protocol WTClientTrackerDelegate <WTBaseTrackerDelegate>

@optional
/**
 * @brief Called whenever a client tracker successfully loaded from the given URL.
 *
 * @param clientTracker The clientTracker that loaded successfully
 * @param URL The URL where the client tracker was loaded from
 */
- (void)clientTracker:(nonnull WTClientTracker *)clientTracker didFinishedLoadingTargetCollectionFromURL:(nonnull NSURL *)URL NS_SWIFT_NAME(clientTracker(clientTracker:didFinishLoadingTargetCollectionFromURL:));

/**
 * @brief Called whenever a client tracker could not be loaded from the given URL.
 *
 * @param clientTracker The client tracker that could not be loaded
 * @param URL The URL where the the client tracker should have been loaded from
 * @param error A NSError object that contains more information about why the client tracker could not be loaded
 */
- (void)clientTracker:(nonnull WTClientTracker *)clientTracker didFailToLoadTargetCollectionFromURL:(nonnull NSURL *)URL withError:(nonnull NSError *)error;

@end

/**
 * @brief Represents a tracker that was initialized with a .wtc (Wikitude Target Collection) file.
 *
 * @deprecated Since version 2.0.0. Use WTImageTracker in combination with an WTTargetCollectionResource instead.
 *
 * @discussion Client tracker retrieve there image targets from a .wtc file that was either loaded from the application bundle or a remote server. 
 * If the .wtc file was loaded from a server, no internet connection is required anymore after the loading finished successfully.
 */
WT_DEPRECATED_SINCE(2.0.0, "Use WTImageTracker in combination with an WTTargetCollectionResource instead.")
@interface WTClientTracker : WTBaseTracker

/**
 * @brief Represents if the tracker could be loaded or not.
 */
@property (nonatomic, assign, readonly) BOOL                            isLoaded;

/**
 * @brief The delegate object that is associated with this client tracker object.
 *
 * @discussion The delegate object is usually set through the appropriate WTTrackerManger factory method.
 */
@property (nonatomic, weak) id<WTClientTrackerDelegate>                 delegate;

/**
 * @brief An array of NSString objects that represent which targets of the .wtc file should be treated as extended targets.
 *
 * @discussion The extended targets array is usually set through the client tracker creation factory method.
 */
- (nullable NSArray *)extendedTargets;

@end

NS_ASSUME_NONNULL_END
