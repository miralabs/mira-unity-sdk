//
//  WTCloudTracker.h
//  WikitudeNativeSDK
//
//  Created by Andreas Schacherbauer on 28/04/15.
//  Copyright (c) 2015 Wikitude. All rights reserved.
//

#import <Foundation/Foundation.h>

#import "WTBaseTracker.h"


NS_ASSUME_NONNULL_BEGIN

extern NSString * __nonnull const WTCloudTrackerResponseKey_TargetName;
extern NSString * __nonnull const WTCloudTrackerResponseKey_TargetRating;


@class WTCloudTracker;
@class WTCloudRecognitionResponse;

/**
 * WTCloudRecognitionSuccessHandler represents a block object which is invoked when the cloud recognition server returns a valid response.
 */
#pragma clang diagnostic push
#pragma clang diagnostic ignored "-Wdeprecated-declarations"
typedef void(^WTCloudRecognitionSuccessHandler)(WTCloudRecognitionResponse * cloudRecognitionResponse);
#pragma clang diagnostic pop

/**
 * WTCloudRecognitionErrorHandler represents a block object which is invoked when the cloud recognition server could not be reached or returned an internal error.
 */
typedef void(^WTCloudRecognitionErrorHandler)(NSError *error);

/**
 * WTContinuousCloudRecognitionInterruptionHandler represents a block object which is called every time a new cloud recognition request is invoked, but the previous one did not finish yet.
 */
typedef void(^WTContinuousCloudRecognitionInterruptionHandler)(NSTimeInterval suggestedTimeInterval);

/**
 * WTCloudTrackerDelegate provides cloud tracker specific methods that can be used to react to cloud tracker specific state changes.
 *
 * @deprecated Since version 2.0.0. Use WTImageTrackerDelegate and/or WTCloudRecognitionService instead.
 */
WT_DEPRECATED_SINCE(2.0.0, "Use WTImageTrackerDelegate and/or WTCloudRecognitionService instead.")
@protocol WTCloudTrackerDelegate <WTBaseTrackerDelegate>

/**
 * @brief Called whenever a cloud tracker successfully loaded with the given client token and target collection id.
 *
 * @param cloudTracker The cloud tracker that loaded successfully
 */
- (void)cloudTrackerFinishedLoading:(WTCloudTracker *)cloudTracker;

/**
 * @brief Called whenever a cloud tracker could not be loaded with the given client token and target collection id
 *
 * @param cloudTracker The cloud tracker that could not be loaded
 * @param error A NSError object that contians more information about why the cloud tracker could not be loaded
 */
- (void)cloudTracker:(WTCloudTracker *)cloudTracker failedToLoadWithError:(NSError *)error;

@end

/**
 * @brief Represents a tracker who was initialized with a client token and target collection id.
 *
 * Cloud tracker use a cloud target collection which can be created using a REST JavaScript API or the Wikitude Target Manager. Once such a cloud target collection id was created, it can be loaded using a cloud tracker.
 *
 * @discussion Cloud tracker require a continuous internet connection in order to communicate with the Wikitude cloud recognition server.
 *
 * @deprecated Since version 2.0.0. Use WTImageTracker in combination with an WTCloudRecognitionService instead.
 */
WT_DEPRECATED_SINCE(2.0.0, "Use WTImageTracker in combination with an WTCloudRecognitionService instead.")
@interface WTCloudTracker : WTBaseTracker

/**
 * @brief Represents if the tracker could be loaded or not.
 */
@property (nonatomic, assign, readonly) BOOL                                isLoaded;

/**
 * @brief The delegate object that is associated with this cloud tracker object.
 *
 * @discussion The delegate object is usually set through the appropriate WTTrackerManager factory method.
 */
@property (nonatomic, weak) id<WTCloudTrackerDelegate>                      delegate;

/**
 * @brief An array of NSString objects that represent which targets of the .wtc file should be treated as extended targets.
 *
 * @discussion The extended targets array is usually set through the client tracker creation factory method.
 */
- (nullable NSArray *)extendedTargets;

/**
 * @brief Triggers a single device/server communication to find out if a image target occurs in the current camera frame
 *
 * @discussion This method can be used to trigger a single cloud recognition session. This includes capturing the current camera frame, sending it to the Wikitude cloud recognition server, evaluate the frame and sending back the response. If the server could process the camera frame, the successHandler is called, independent of the evaluation result. One can use the WTCloudRecognitionResponse object, that is the only parameter to the successHandler, to retrieve more information about the server processing. If no connection to the server could be established or any other problem occured, the errorHanlder block gets invoked, providing more information about what went wrong.
 *
 * @param successHandler A block object that is called whenever the device/server communication was successful
 * @param errorHandler A block object that is called whenever the device/server communication could not be established or any other error occured
 */
- (void)recognize:(WTCloudRecognitionSuccessHandler)successHandler errorHandler:(WTCloudRecognitionErrorHandler)errorHandler;

/**
 * @brief Starts a continuous cloud recognition session
 *
 * @discussion Calling this method does essentially the same as a single call to -recognize:errorHandler, but repeats this with the given interval. If the given interval is too short and the previous request hasn't finished when the next one should be send, the interruption handler is called with a newly suggested interval. So within the interruption handler, the current continuous recognition session should be stopped and started again with the given interval. If not, requests will be dropped.
 *
 * @param interval The interval in which new camera frames should be send to the Wikitude cloud recognition server
 * @param successHandler A block object that is invoked whenever teh device/server communication was successful. This happens with the given interval.
 * @param interruptionHandler A block object that is invoked every time the given interval is to short in order to process one request after the other.
 * @param errorHandler A block object that is invoked whenever the device/server communication could not be established or any other error occured
 */
- (void)startContinuousRecognitionWithInterval:(NSTimeInterval)interval successHandler:(nullable WTCloudRecognitionSuccessHandler)successHandler interruptionHandler:(nullable WTContinuousCloudRecognitionInterruptionHandler)interruptionHandler errorHandler:(nullable WTCloudRecognitionErrorHandler)errorHandler NS_SWIFT_NAME(startContinuousRecognition(withInterval:successHandler:interruptionHandler:errorHandler:));

/**
 * @brief Stops the current continuous recognition session
 *
 * Calling this method will immediately stop any new device/servier communication but still deliver the result from any, currently ongoing, device/server communication
 */
- (void)stopContinuousRecognition;

@end

NS_ASSUME_NONNULL_END
