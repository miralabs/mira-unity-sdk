//
//  WTNativeSDKStartupConfiguration.h
//  WikitudeNativeSDK
//
//  Created by Andreas Schacherbauer on 25/11/16.
//  Copyright © 2016 Wikitude. All rights reserved.
//

#import "WTStartupConfiguration.h"
#import <OpenGLES/gltypes.h>


NS_ASSUME_NONNULL_BEGIN

/**
 * WTNativeSDKStartupConfiguration represents configuration values that are specific to the Wikitude Native SDK.
 */
@interface WTNativeSDKStartupConfiguration : WTStartupConfiguration

/**
 * The target texture that should be used to draw the camera frame onto.
 *
 * @default 0
 * @discussion You should only use this property if your application has already setup a fixed rendering pipeline and wants the Wikitude SDK to draw the camera frame onto a very specific OpenGL ES 2 texture. Under normal circumstances, WTRenderingMode_External should be used if the Wikitude SDK should draw the camera frame in an already existing rendering environment.
 */
@property (nonatomic, assign) GLuint                                cameraRenderingTargetTexture;

/**
 * The rendering API that should be used to draw the camera frame
 *
 * @default WTRenderingAPI_OpenGL_ES_2
 * @discussion You can use this to tell the SDK which rendering API to use, so that it matches with the one you are using to render.
 */
@property (nonatomic, copy) NSOrderedSet<WTRenderingAPI>            *targetRenderingAPI WT_DEPRECATED_SINCE(3.0.0, "Use WTRenderingConfiguration needed.");

@end

NS_ASSUME_NONNULL_END
