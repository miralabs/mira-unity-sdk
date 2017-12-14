//
//  WTMetalRenderingMode.h
//  WikitudeNativeSDK
//
//  Created by Alexandru Florea on 20/04/2017.
//  Copyright © 2017 Wikitude. All rights reserved.
//

#import <Foundation/Foundation.h>

#import <Metal/MTLDevice.h>
#import <Metal/MTLCommandQueue.h>
#import <Metal/MTLRenderCommandEncoder.h>

#import <CoreGraphics/CGGeometry.h>

NS_ASSUME_NONNULL_BEGIN

@class WTMetalView;
@class WTRenderer;
@class WTWikitudeNativeSDK;
@class WTNativeSDKManager;

/** 
 * WTCustomMetalUpdatehandler is used when WTInternalMetalRenderingMode is selected. Please refer to -wikitudeNativeSDKNeedsExternalMetalUpdateHandler: defined in WTInternalMetalRenderingProtocol for more information.
 */
typedef void(^WTCustomMetalUpdateHandler)(void);

/**
 * WTCustomMetalDrawHandler is used when WTInternalMetalRenderingMode is selected. Please refer to -wikitudeNativeSDKNeedsExternalMetalDrawHandler: defined in WTInternalMetalRenderingProtocol for more information.
 */
typedef void(^WTCustomMetalDrawHandler)(id<MTLRenderCommandEncoder>);

/**
 * WTWikitudeMetalUpdateHandler is used when WTExternalMetalRenderingMode is selected. Please refer to -wikitudeNativeSDK:didCreateExternalMetalUpdateHandler: defined in WTExternalMetalRenderingProtocol for more information.
 */
typedef void(^WTWikitudeMetalUpdateHandler)(void);

/**
 * WTWikitudeMetalDrawHandler is used when WTExternalMetalRenderingMode is selected. Please refer to -wikitudeNativeSDK:didCreateExternalMetalDrawHandler: defined in WTExternalMetalRenderingProtocol for more information.
 */
typedef void(^WTWikitudeMetalDrawHandler)(id<MTLRenderCommandEncoder>);


/**
 * WTExternalMetalRenderingProtocol is used to handle information exchange between the Wikitude Native SDK and an external Metal renderer.
 */
@protocol WTExternalMetalRenderingProtocol <NSObject>

/**
 * @brief Called during -start:completion: method of the WTWikitudeNativeSDK.
 *
 * This method is called to retrieve the Metal view size.
 *
 * @param wikitudeNativeSDK The Wikitude Native SDK object that requests the Metal view size.
 * @return CGRect A rect representing the current Metal view size. This method needs to return a non zero rect.
 */
- (CGRect)viewSizeForExternalRenderingInWikitudeNativeSDK:(WTWikitudeNativeSDK *)wikitudeNativeSDK;

/**
 * @brief Called during -start:completion: method of the WTWikitudeNativeSDK.
 *
 * This method is called to pass a Wikitude internal update block to the external renderer.
 * 
 * @param wikitudeNativeSDK The Wikitude Native SDK object that created the update handler.
 * @param updateHandler An ObjC block object that needs to be called every frame in order to update the Wikitude Native SDK internally. If this handler is not called, no computer vision related updates are done.
 *
 */
- (void)wikitudeNativeSDK:(WTWikitudeNativeSDK *)wikitudeNativeSDK didCreateExternalMetalUpdateHandler:(WTWikitudeMetalUpdateHandler)updateHandler;

/**
 * @brief Called during -start:completion: method of the WTWikitudeNativeSDK.
 *
 * This method is called to pass a Wikitude internal draw handler to the external renderer.
 *
 * @param wikitudeNativeSDK The Wikitude Native SDK object that created the draw handler.
 * @param drawHandler An ObjC block object that needs to be called every frame in order to draw the camera background.
 */
- (void)wikitudeNativeSDK:(WTWikitudeNativeSDK *)wikitudeNativeSDK didCreateExternalMetalDrawHandler:(WTWikitudeMetalDrawHandler)drawHandler;

/**
 * @brief Called during -start:completion: method of the WTWikitudeNativeSDK.
 *
 * This method is calld to retrieve the MTLDevice that should be used to perform camera related Metal calls.
 *
 * @param wikitudeNativeSDK The Wikitude Native SDK object that needs an MTLDevice object.
 * @return MTLDevice The MTLDevice object that should be used to perform Metal calls that are camera related.
 */
- (id<MTLDevice>)metalDeviceForVideoCameraInWikitudeNativeSDK:(WTWikitudeNativeSDK *)wikitudeNativeSDK;

/**
 * @brief Called during -start:completion: method of the WTWikitudeNativeSDK.
 *
 * This method is called to retrieve the MTLCommandQueue that should be used to perform camera related Metal calls.
 *
 * @param wikitudeNativeSDK The Wikitude Native SDK object that needs an MTLCommandQueue object.
 * @return MTLCommandQueue the MTLCommandQueue object that should be used to perform Metal calls that are camera related.
 */
- (id<MTLCommandQueue>)metalCommandQueueForVideoCameraInWikitudeNativeSDK:(WTWikitudeNativeSDK *)wikitudeNativeSDK;

@end

/**
 * WTInternalMetalRenderingProtocol is used to handle information exchange between the Wikitude Native SDK and an app that is using internal rendering.
 */
@protocol WTInternalMetalRenderingProtocol <NSObject>

/**
 * @brief Called during -start:completion: method of the WTWikitudeNativeSDK.
 *
 * This method is called to retrieve a custom update handler object. The handler is then invoked every frame to update Wikitude external logic.
 *
 * @param wikitudeNativeSDK The WikitudeNative SDK object that needs an external update handler.
 * @return WTCustomMetalUpdateHandler A custom update handler object that is invoked every frame by the Wikitude Native SDK when Metal rendering is used.
 */
- (nonnull WTCustomMetalUpdateHandler)wikitudeNativeSDKNeedsExternalMetalUpdateHandler:(WTWikitudeNativeSDK *)wikitudeNativeSDK;

/**
 * @brief Called during -start:completion: method of the WTWikitudeNativeSDK.
 *
 * This method is called to retrieve a custom draw handler object. The handler is then invoked every frame to issue Wikitude external draw commands in Metal.
 *
 * @param wikitudeNativeSDK The Wikitude Native SDK object that needs an external Metal draw handler.
 * @return WTCustomMetalDrawHandler A custom draw handler object that is invoked every frame by the Wikitude Native SDK when Metal rendering is used.
 */
- (nonnull WTCustomMetalDrawHandler)wikitudeNativeSDKNeedsExternalMetalDrawHandler:(WTWikitudeNativeSDK *)wikitudeNativeSDK;

@end

/**
 * Base object for internal and external rendering modes for Metal rendering.
 */
@interface WTMetalRenderingMode : NSObject

/**
 * @brief Method that returns a pre configured Metal View object that can be added to the view hierarchy.
 *
 * This method is only relevant when using the Wikitude Native SDK with Internal renderering and will return nil otherwise.
 * @discussion Please note that the returned pointer is of type weak and the view should either be added to the view hierarchy within the scope of the calling method or retained (assigned to a strong property) until the view was added to the view hierarchy.
 *
 * @return WTMetalView A weak pointer to a Metal view.
 */
- (WTMetalView *)metalView;

@end

/**
 * Object used by the Wikitude Native SDK to access data required for External rendering with Metal.
 * Using rendering mode external, the Wikitude native SDK is driven by an external render loop. It's the external renderer's responsibility to start an appropriate render loop (30 or 60 FPS) and to pause and resume it when the application enters background/foreground.
 */
@interface WTExternalMetalRenderingMode : WTMetalRenderingMode

/**
 * @brief The designated initializer to create an object of this class.
 *
 * @param delegate The object that acts as a delegate for the External rendering calls.
 * @return WTExternalMetalRenderingMode An object of type WTExternalMetalRenderingMode.
 */
- (instancetype)initWithDelegate:(id<WTExternalMetalRenderingProtocol>)delegate;

@end

/**
 * Object used by the Wikitude Native SDK to access data required for Internal rendering with Metal.
 * Using rendering mode internal, the Wikitude Native SDK is setting up a rendering loop and additional custom update and draw handlers can be supplied using the WTInternalMetalRenderingProtocol.
 */
@interface WTInternalMetalRenderingMode : WTMetalRenderingMode

/**
 * @brief The designated initializer to create an object of this class.
 *
 * @param delegate The object that acts as a delegate for the Internal rendering calls.
 * @return WTInternalMetalRenderingMode An object of type WTInternalMetalRenderingMode.
 */
- (instancetype)initWithDelegate:(id<WTInternalMetalRenderingProtocol>)delegate;

@end

NS_ASSUME_NONNULL_END
