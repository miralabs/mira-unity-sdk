//
//  WTCloudRecognitionService.h
//  WikitudeNativeSDK
//
//  Created by Alexandru Florea on 31/10/16.
//  Copyright © 2016 Wikitude. All rights reserved.
//

#ifndef WTCloudRecognitionService_h
#define WTCloudRecognitionService_h

#import <Foundation/Foundation.h>

NS_ASSUME_NONNULL_BEGIN

extern NSString * __nonnull const WTCloudRecognitionServiceResponseKey_TargetName;
extern NSString * __nonnull const WTCloudRecognitionServiceResponseKey_TargetRating;

@class WTCloudRecognitionServiceResponse;

/**
 * WTCloudRecognitionServiceInitializationHandler represents a block object which is invoked when the cloud recognition service finished its initialization, either successfully, or with an error.
 */
typedef void(^WTCloudRecognitionServiceInitializationHandler)(BOOL success, NSError* _Nullable error);

/**
 * WTCloudRecognitionServiceHandler represents a block object which is invoked when the cloud recognition service receives a valid response from the server or an error occurred when communicating with it.
 */
typedef void(^WTCloudRecognitionServiceHandler)(WTCloudRecognitionServiceResponse* _Nullable response, NSError* _Nullable error);

/**
 * WTCloudRecognitionServiceHandler represents a block object which is called every time a new cloud recognition request is invoked, but the previous one did not finish yet.
 */
typedef void(^WTContinuousCloudRecognitionServiceInterruptionHandler)(NSTimeInterval suggestedTimeInterval);

/**
 * Represents a connection to the Wikitude cloud recognition servers that was initialized with a client token and a target collection id
 *
 * Cloud recognition services use a cloud target collection which can be created using a REST JavaScript API or the Wikitude Target Manager. Once such a cloud target collection id was created, it can be loaded using a cloud recognitions ervice.
 *
 * @discussion The cloud recognition service requires a continuous internet connection in order to communicate with the Wikitude cloud recognition server.
 */
@interface WTCloudRecognitionService : NSObject


/**
 * @brief Represents the client token that was used to initialize this cloud recognition service and should correspond to the one that is associated with your Wikitude cloud recognition account.
 */
@property (nonatomic, copy, readonly) NSString              *clientToken;

/**
 * @brief Represents the target collection id that was used to initialize this cloud recognition service and should correspond to the target collection that should be loaded on the Wikitude cloud recognition server.
 */
@property (nonatomic, copy, readonly) NSString              *targetCollectionId;

/**
 * @brief Represents if the communication with the cloud recognition servers could be initialized or not.
 */
@property (nonatomic, assign, readonly) BOOL                initialized;

/**
 * @brief Triggers a single device/server communication to find out if a image target occurs in the current camera frame
 *
 * @discussion This method can be used to trigger a single cloud recognition session. This includes capturing the current camera frame, sending it to the Wikitude cloud recognition server, evaluate the frame and sending back the response. If the server could process the camera frame, the recognitionHandler is called with a nonnull value for the response parameter, independent of the evaluation result. One can use this paramater, of type WTCloudRecognitionServiceResponse, to retrieve more information about the server processing. If no connection to the server could be established or any other problem occured, the first parameter of the recognitionHandler will be nil and the second parameter will contain a valid NSError object with more information about what went wrong.
 *
 * @param recognitionHandler A block object that handles events regarding the device/server communication
 */
- (void)recognize:(WTCloudRecognitionServiceHandler)recognitionHandler;

/**
 * @brief Starts a continuous cloud recognition session
 *
 * @discussion Calling this method does essentially the same as a single call to -recognize:errorHandler, but repeats this with the given interval. If the given interval is too short and the previous request hasn't finished when the next one should be send, the interruption handler is called with a newly suggested interval. So within the interruption handler, the current continuous recognition session should be stopped and started again with the given interval. If not, requests will be dropped.
 *
 * @param interval The interval in which new camera frames should be send to the Wikitude cloud recognition server
 * @param interruptionHandler A block object that is invoked every time the given interval is to short in order to process one request after the other.
 * @param recognitionHandler A block object that handles events regarding the device/server communication
 */
- (void)startContinuousRecognitionWithInterval:(NSTimeInterval)interval interruptionHandler:(nullable WTContinuousCloudRecognitionServiceInterruptionHandler)interruptionHandler responseHandler:(nonnull WTCloudRecognitionServiceHandler)recognitionHandler NS_SWIFT_NAME(startContinuousRecognition(withInterval:interruptionHandler:responseHandler:));

/**
 * @brief Stops the current continuous recognition session
 *
 * Calling this method will immediately stop any new device/server communication but still deliver the result from any, currently ongoing, device/server communication
 */
- (void)stopContinuousRecognition;

@end

NS_ASSUME_NONNULL_END

#endif /* WTCloudRecognitionService_h */
