//
//  WTTrackerManager.h
//  WikitudeNativeSDK
//
//  Created by Andreas Schacherbauer on 28/04/15.
//  Copyright (c) 2015 Wikitude. All rights reserved.
//

#import <Foundation/Foundation.h>
#import "WTTargetCollectionResource.h"
#import "WTCloudRecognitionService.h"

#define WT_DEPRECATED_SINCE(__version__, __msg__) __attribute__((deprecated("Deprecated in Wikitude SDK " #__version__ ". " __msg__)))


NS_ASSUME_NONNULL_BEGIN

/**
 * WTCloudRecognitionServerRegion defines the possible cloud recognition server regions that are available.
 * The region should be set depending on where the final application will be used most likely.
 * If the region does not match the actual user location, longer round trip times should be expected.
 */
typedef NS_ENUM(NSInteger, WTCloudRecognitionServerRegion) {
    /**
     * WTCloudRecognitionServerRegion_Americas should be used when the user is currently in North or South America.
     */
    WTCloudRecognitionServerRegion_Americas,

    /**
     * WTCloudRecognitionServerRegion_Europe should be used when the user is currently in Europe, Africa or Middle East.
     */
    WTCloudRecognitionServerRegion_Europe
};

@protocol WTTargetCollectionResourceDelegate;
@class WTImageTrackerConfiguration;
@class WTImageTracker;
@protocol WTImageTrackerDelegate;
@class WTBaseTracker;
@class WTClientTracker;
@protocol WTClientTrackerDelegate;
@class WTCloudTracker;
@protocol WTCloudTrackerDelegate;
@class WTInstantTrackerConfiguration;
@class WTInstantTracker;
@protocol WTInstantTrackerDelegate;
@class WTCloudRecognitionService;
@class WTCloudRecognitionServiceConfiguration;
@class WTObjectTracker;
@protocol WTObjectTrackerDelegate;

/**
 * WTTrackerManger provides factory methods to create certain tracker objects.
 */
@interface WTTrackerManager : NSObject

/**
 * @brief Returns a weak pointer to a newly created WTTargetCollectionResource object, used to load a target collection file.
 *
 * @discussion Callers need to make sure to retain the reference count e.g. by assigning to a strong property.
 *
 * @param resourceURL A URL from where the target collection resource should be loaded from. Files can be loaded from the application bundle or a remote server.
 * @param errorHandler An object that conforms to the WTTargetCollectionResourceCompletionHandler protocol.
 * @return WTTargetCollectionResource * A pointer to a newly created WTTargetCollectionResource object.
 */
- (WTTargetCollectionResource *)createTargetCollectionResourceFromURL:(nonnull NSURL *)resourceURL completion:(WTTargetCollectionResourceCompletionHandler)errorHandler;

/**
 * @brief Returns a weak pointer to a newly created WTCloudRecognitionService object, used to offload image target recognition to the Wikitude cloud recognition servers.
 *
 * @discussion Callers need to make sure to retain the reference count e.g. by assigning to a strong property.
 *
 * @param clientToken The client token that is associated with your Wikitude cloud recognition account.
 * @param targetCollectionId The identifier for the target collection that should be loaded on the Wikitude cloud recognition server.
 * @param initializationHandler An object that conforms to the WTCloudRecognitionServiceInitializationHandler protocol.
 * @return WTCloudRecognitionService * A pointer to a newly created WTCloudRecognitionService object.
 */
- (WTCloudRecognitionService *)createCloudRecognitionServiceWithClientToken:(nonnull NSString *)clientToken targetCollectionId:(nonnull NSString *)targetCollectionId completion:(WTCloudRecognitionServiceInitializationHandler)initializationHandler;

/**
 * @brief Returns a weak pointer to a newly created WTCloudRecognitionService object, used to offload image target recognition to the Wikitude cloud recognition servers.
 *
 * @discussion Callers need to make sure to retain the reference count e.g. by assigning to a strong property.
 *
 * @param clientToken The client token that is associated with your Wikitude cloud recognition account.
 * @param targetCollectionId The identifier for the target collection that should be loaded on the Wikitude cloud recognition server.
 * @param configuration The configuration that should be used when the WTCloudRecognitionService is created. Please have a look at WTCloudRecognitionServiceConfiguration for all possible configurations.
 * @param initializationHandler An object that conforms to the WTCloudRecognitionServiceInitializationHandler protocol.
 * @return WTCloudRecognitionService * A pointer to a newly created WTCloudRecognitionService object.
 */
- (WTCloudRecognitionService *)createCloudRecognitionServiceWithClientToken:(nonnull NSString *)clientToken targetCollectionId:(nonnull NSString *)targetCollectionId configuration:(nullable void (^)(WTCloudRecognitionServiceConfiguration *cloudRecognitionServiceConfiguration))configuration completion:(WTCloudRecognitionServiceInitializationHandler)initializationHandler;

/**
 * @brief Returns a weak pointer to a newly created WTImageTracker object, configured for 2d tracking using a wtc file loaded by a WTTargetCollectionResource.
 *
 * @discussion Callers need to make sure to retain the reference count e.g. by assigning to a strong property.
 *
 * @param targetCollectionResource The target collection resource used to load the wtc file.
 * @param delegate An object that conforms to the WTImageTrackerDelegate protocol.
 * @param configuration A configuration object used to define how the image tracker behaves.
 * @return WTImageTracker * A pointer to a newly created WTImageTracker object.
 */
- (WTImageTracker *)createImageTrackerFromTargetCollectionResource:(nonnull WTTargetCollectionResource *)targetCollectionResource delegate:(nonnull id<WTImageTrackerDelegate>)delegate configuration:(nullable void (^)(WTImageTrackerConfiguration * imageTrackerConfiguration))configuration;

/**
 * @brief Returns a weak pointer to a newly created WTImageTracker object, configured for 2d tracking using the WTCloudRecognitionService to offload recognition to the Wikitude cloud recognition servers.
 *
 * @discussion Callers need to make sure to retain the reference count e.g. by assigning to a strong property.
 *
 * @param cloudRecognitionService The cloud recognition service used to communicate with the Wikitude cloud recognition servers.
 * @param delegate An object that conforms to the WTImageTrackerDelegate protocol.
 * @param configuration A configuration object used to define how the image tracker behaves.
 * @return WTImageTracker * A pointer to a newly created WTImageTracker object.
 */
- (WTImageTracker *)createImageTrackerFromCloudRecognitionService:(nonnull WTCloudRecognitionService *)cloudRecognitionService delegate:(nonnull id<WTImageTrackerDelegate>)delegate configuration:(nullable void (^)(WTImageTrackerConfiguration * imageTrackerConfiguration))configuration;


/**
 * @brief Returns a weak pointer to a newly created WTObjectTracker object, configured for 3d tracking using a wmc file loaded by a WTTargetCollectionResource.
 *
 * @discussion Callers need to make sure to retain the reference count e.g. by assigning to a strong property.
 *
 * @param targetCollectionResource The target collection resource used to load the wmc file.
 * @param delegate An object that conforms to the WTObjectTrackerDelegate protocol.
 * @return WTObjectTracker * A pointer to a newly created WTObjectTracker object.
 */
- (WTObjectTracker *)createObjectTrackerFromTargetCollectionResource:(nonnull WTTargetCollectionResource *)targetCollectionResource delegate:(nonnull id<WTObjectTrackerDelegate>)delegate;

/**
 * @brief Returns a weak pointer to a newly created WTInstantTracker object, configured for 3d tracking of a scene without any markers
 *
 * @discussion Callers need to make sure to retain the reference count e.g. by assigning to a strong property.
 *
 * @param delegate An object that conforms to the WTObjectTrackerDelegate protocol.
 * @param configuration A configuration object used to define how the instant tracker should behave.
 * @return WTObjectTracker * A pointer to a newly created WTObjectTracker object.
 */
- (WTInstantTracker *)createInstantTracker:(nonnull id<WTInstantTrackerDelegate>)delegate configuration:(nullable void (^)(WTInstantTrackerConfiguration *instantTrackerConfiguration))configuration;

/**
 * @brief Returns a weak pointer to a newly created WTClientTracker object, configured for 2d tracking.
 *
 * @discussion Callers need to make sure to retain the reference count e.g. by assigning to a strong property.
 *
 * @deprecated Since version 2.0.0. Use -createImageTrackerFromTargetCollectionResource:delegate:configuration: instead.
 *
 * @param clientTrackerURL A URL from where the client tracker should be loaded from. Client tracker can be loaded from the application bundle or a remote server.
 * @param extendedTargets An array containing string objects where each one of them represents an image target that should be tracked as extended target.
 * @param delegate A object that conforms to the WTClientTrackerDelegate protocol.
 * @return WTClientTracker * A pointer to a newly created WTClientTracker object.
 */
- (WTClientTracker *)create2DClientTrackerFromURL:(nonnull NSURL *)clientTrackerURL extendedTargets:(nullable NSArray *)extendedTargets andDelegate:(nonnull id<WTClientTrackerDelegate>)delegate WT_DEPRECATED_SINCE(2.0.0, "Use -createImageTrackerFromTargetCollectionResource:delegate:configuration: instead.");


/**
 * @brief Returns a weak pointer to a newly created WTClientTracker object, configured for 3d tracking.
 *
 * @discussion Callers need to make sure to retian the reference count e.g. by assigning to a strong property.
 *
 * @deprecated Since version 2.0.0. Use -createObjectTrackerFromTargetCollectionResource:delegate: instead.
 *
 * @param trackingMapURL A URL from where the tracking map should be loaded from. Tracking maps can be loaded from the application bundle or a remote server.
 * @param delegate A object that conforms to the WTClientTrackerDelegate protocol.
 * @return WTClientTracker * A pointer to a newly created WTClientTracker object.
 */
- (WTClientTracker *)create3DClientTrackerFromURL:(nonnull NSURL *)trackingMapURL andDelegate:(nonnull id<WTClientTrackerDelegate>)delegate WT_DEPRECATED_SINCE(2.0.0, "Use -createObjectTrackerFromTargetCollectionResource:delegate: instead.");

/**
 * @brief Returns a weak pointer to a newly created WTCloudTracker object.
 *
 * @discussion Callers need to make sure to retian the reference count e.g. by assigning to a strong property.
 *
 * @deprecated Since version 2.0.0. Use -createImageTrackerFromCloudRecognitionService:delegate:configuration: instead.
 *
 * @param authenticationToken The client token that is associated with your Wikitude cloud recognition account.
 * @param targetCollectionId An identifier which target collection should be loaded on the Wikitude cloud recognition server.
 * @param extendedTargets An array containing string objects where each one of them represents an image target that should be tracked as extended target.
 * @param delegate A object that conforms to the WTCloudTrackerDelegate protocol.
 * @return WTCloudTracker * A pointer to a newly created WTCloudTracker object.
 */
- (WTCloudTracker *)createCloudTrackerWithToken:(nonnull NSString *)authenticationToken targetCollectionId:(nonnull NSString *)targetCollectionId extendedTargets:(nullable NSArray *)extendedTargets andDelegate:(id<WTCloudTrackerDelegate>)delegate WT_DEPRECATED_SINCE(2.0.0, "Use -createImageTrackerFromCloudRecognitionService:delegate:configuration: instead.");

/**
 * @brief Specifies the regional-distributed Wikitude server the SDK should contact when using cloud recognition.
 *
 * @discussion After this method is called, every cloud tracker that is created will contact the specified regional cloud recognition server. If a tracker was already created, it will still point to the previously defined region.
 * 
 * This method is typically called after the Wikitude Native SDK was created and the user location was determined if necessary.
 *
 * The Wikitude Native SDK will by default connect to WTCloudRecognitionServerRegion_Europe.
 *
 * @param cloudRecognitionServerRegion A constant of type WTCloudRecognitionServerRegion that specifies the regional cloud recognition server that should be contacted.
 */
- (void)setCloudRecognitionServerRegion:(WTCloudRecognitionServerRegion)cloudRecognitionServerRegion WT_DEPRECATED_SINCE(7.0.0, "Use -createCloudRecognitionServiceWithClientToken:targetCollectionId:configuration:completion: instead and define the server region in the configuration.");

@end

NS_ASSUME_NONNULL_END
