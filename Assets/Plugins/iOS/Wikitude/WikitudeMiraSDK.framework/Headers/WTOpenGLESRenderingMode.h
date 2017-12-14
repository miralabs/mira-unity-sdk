//
//  WTOpenGLESRenderingMode.h
//  WikitudeNativeSDK
//
//  Created by Alexandru Florea on 20/04/2017.
//  Copyright © 2017 Wikitude. All rights reserved.
//

#import <Foundation/Foundation.h>
#import <CoreGraphics/CGGeometry.h>
#import <OpenGLES/EAGL.h>

NS_ASSUME_NONNULL_BEGIN

@class EAGLContext;
@class WTEAGLView;
@class WTRenderer;
@class WTWikitudeNativeSDK;
@class WTNativeSDKManager;

/**
 * WTCustomOpenGLESUpdateHandler is used when WTInternalOpenGLESRenderingMode is selected. Please refer to -wikitudeNativeSDKNeedsExternalOpenGLESUpdateHandler: defined in WTInternalOpenGLESRenderingProtocol for more information.
 */
typedef void(^WTCustomOpenGLESUpdateHandler)(void);

/**
 * WTCustomOpenGLESDrawHandler is used when WTInternalOpenGLESRenderingMode is selected. Please refer to -wikitudeNativeSDKNeedsExternalOpenGLESDrawHandler: defined in WTInternalOpenGLESRenderingProtocol for more information.
 */
typedef void(^WTCustomOpenGLESDrawHandler)(void);

/**
 * WTWikitudeOpenGLESUpdateHandler is used when WTExternalOpenGLESRenderingMode is selected. Please refer to -wikitudeNativeSDK:didCreateExternalOpenGLESUpdateHandler: defined in WTExternalOpenGLESRenderingProtocol for more information.
 */
typedef void(^WTWikitudeOpenGLESUpdateHandler)(void);

/**
 * WTWikitudeOpenGLESDrawHandler is used when WTExternalOpenGLESRenderingMode is selected. Please refer to -wikitudeNativeSDK:didCreateExternalOpenGLESDrawHandler: defined in WTExternalOpenGLESRenderingProtocol for more information.
 */
typedef void(^WTWikitudeOpenGLESDrawHandler)(void);

/**
 * WTExternalOpenGLESRenderingProtocol is used to handle information exchange between the Wikitude Native SDK and an external OpenGLES renderer.
 */
@protocol WTExternalOpenGLESRenderingProtocol <NSObject>

/**
 * @brief Called during -start:completion: method of the WTWikitudeNativeSDK.
 *
 * This method is called to pass a Wikitude internal update block to the external renderer.
 *
 * @param wikitudeNativeSDK The Wikitude Native SDK object that create the update handler.
 * @param updateHandler An ObjC block object that needs to be called every frame in order to update the Wikitude Native SDK internally. If this handler is not called, no computer vision related updates are done.
 */
- (void)wikitudeNativeSDK:(WTWikitudeNativeSDK *)wikitudeNativeSDK didCreateExternalOpenGLESUpdateHandler:(WTWikitudeOpenGLESUpdateHandler)updateHandler;

/**
 * @brief Called during -start:completion: method of the WTWikitudeNativeSDK.
 *
 * This method is called to pass a Wikitude internal draw handler to the external renderer.
 *
 * @param wikitudeNativeSDK The Wikitude Native SDK object that created the draw handler.
 * @param drawHandler An ObjC block object that needs to be called every frame in order to draw the camera background.
 */
- (void)wikitudeNativeSDK:(WTWikitudeNativeSDK *)wikitudeNativeSDK didCreateExternalOpenGLESDrawHandler:(WTWikitudeOpenGLESDrawHandler)drawHandler;

/**
 * @brief Called during -start:completion: method of the WTWikitudeNativeSDK.
 *
 * This method is called to retrieve the OpenGL ES view size.
 *
 * @param wikitudeNativeSDK The Wikitude Native SDK object that requests the eagl view size.
 * @return CGRect A rect representing the current OpenGL ES view size. This method needs to return a non zero rect.
 */
- (CGRect)eaglViewSizeForExternalRenderingInWikitudeNativeSDK:(WTWikitudeNativeSDK *)wikitudeNativeSDK;

/**
 * @brief Called during -start:completion:
 *
 * This method is called to retrieve the EAGLContext that should be used to perform camera related OpenGL ES calls.
 *
 * @param wikitudeNativeSDK The Wikitude Native SDK object that needs an EAGLContext object.
 * @return EAGLContext The EAGLContext object that should be used to perform OpenGL ES calls that are camera related. Note that this context can be a shared context from another EAGLContext object.
 */
- (EAGLContext *)eaglContextForVideoCameraInWikitudeNativeSDK:(WTWikitudeNativeSDK *)wikitudeNativeSDK;

@optional

/**
 * @brief Called during -start:completion:
 *
 * This method is called to retrieve the EAGLContext object that should be used to perform offsceen rendering related OpenGL ES calls.
 *
 * @param wikitudeNativeSDK The Wikitude Native SDK object that needs an EAGLContext object for video to texture rendering.
 * @return EAGLContext The EAGLContext object that manages the OpenGL ES texture which is used as a render target.
 */
- (EAGLContext *)eaglContextForVideoCameraToTextureRenderingInWikitudeNativeSDK:(WTWikitudeNativeSDK *)wikitudeNativeSDK;

@end

/**
 * WTInternalOpenGLESRenderingProtocol is used to handle information exchange between the Wikitude Native SDK and an app that is using internal rendering.
 */
@protocol WTInternalOpenGLESRenderingProtocol <NSObject>

/**
 * @brief Called during -start:completion: method of the WTWikitudeNativeSDK.
 *
 * This method is called to retrieve a custom update handler object. The handler is then invoked every frame to update Wikitude external logic.
 *
 * @param wikitudeNativeSDK The Wikitude Native SDK object that needs an external update handler.
 * @return WTCustomOpenGLESUpdateHandler A custom update handler object that is invoked every frame by the Wikitude native SDK when OpenGL ES rendering is used.
 */
- (nonnull WTCustomOpenGLESUpdateHandler)wikitudeNativeSDKNeedsExternalOpenGLESUpdateHandler:(WTWikitudeNativeSDK *)wikitudeNativeSDK;

/**
 * @brief Called during -start:completion: method of the WTWikitudeNativeSDK.
 *
 * This method is called to retrieve a custom draw handler object. The handler is then invoked every frame to issue Wikitude external draw commands in OpenGL ES.
 *
 * @param wikitudeNativeSDK The Wikitude Native SDK object that needs an external OpenGL ES draw handler.
 * @return WTCustomOpenGLESDrawHandler A custom draw handler object that is invoked every frame by the Wikitude Native SDK when OpenGL ES rendering is used.
 */
- (nonnull WTCustomOpenGLESDrawHandler)wikitudeNativeSDKNeedsExternalOpenGLESDrawHandler:(WTWikitudeNativeSDK *)wikitudeNativeSDK;

@end

/**
 * Base object for internal and external rendering modes for OpenGL ES rendering.
 */
@interface WTOpenGLESRenderingMode : NSObject

/**
 * @brief Method that returns the OpenGL ES version with which this rendering mode was initialized.
 * @return OpenGL ES version
 */
- (EAGLRenderingAPI)version;

/**
 * @brief Method that returns a pre configured OpenGL ES View object that can be added to the view hierarchy.
 *
 * This method is only relevant when using the Wikitude Native SDK with Internal rendering and will return nil otherwise.
 * @discussion Please note that the returned pointer is of type weak and the view should either be added to the view hierarchy within the scope of the calling method or retained (assigned to a strong propery) until the view was added to the view hierarhcy.
 *
 * @return WTEAGLView A weak pointer to an OpenGL ES View.
 */
- (WTEAGLView *)eaglView;

@end

/**
 * Object used by the Wikitude Native SDK to access data required for External rendering with OpenGL ES.
 * Using rendering mode external, the Wikitude Native SDK is driven by an external render loop. It's the external renderer's responsibility to start an appropriate render lool (30 or 60 FPS) and to pause and resume it when the application enters background/foreground.
 */
@interface WTExternalOpenGLESRenderingMode : WTOpenGLESRenderingMode

/**
 * @brief The designated initialized to create an object of this class.
 *
 * @param delegate The object that acts as a delegate for External rendering calls.
 * @param version The OpenGL ES version that should be used.
 * @return WTExternalOpenGLESRenderingMode An object of type WTExternalOpenGLESRenderingMode.
 */
- (instancetype)initWithDelegate:(id<WTExternalOpenGLESRenderingProtocol>)delegate andVersion:(EAGLRenderingAPI)version;

@end

/**
 * Object used by the Wikitude native SDK to access data required for Internal rendering with OpenGL ES.
 * Using rendering mode internal, the Wikitude native SDK is setting up a rendering loop and additional custom udpate and draw handlers can be supplied using the WTInternalOpenGLESRenderingProtocol.
 */
@interface WTInternalOpenGLESRenderingMode : WTOpenGLESRenderingMode

/**
 * @brief The designated initialized to create an object of this class.
 *
 * @param delegate The object that acts as a delegate for the Internal rendering calls.
 * @param version The OpenGL ES version that should be used.
 * @return WTInternalOpenGLESRenderingMode An object of type WTInternalOpenGLESRenderingMode.
 */
- (instancetype)initWithDelegate:(id<WTInternalOpenGLESRenderingProtocol>)delegate andVersion:(EAGLRenderingAPI)version;

@end

NS_ASSUME_NONNULL_END
