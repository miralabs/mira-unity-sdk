//
//  WTRenderingConfiguration.h
//  WikitudeNativeSDK
//
//  Created by Alexandru Florea on 20/04/2017.
//  Copyright © 2017 Wikitude. All rights reserved.
//

#import <Foundation/Foundation.h>


@class UIView;
@class WTEAGLView;
@class WTMetalView;
@class WTOpenGLESRenderingMode;
@class WTMetalRenderingMode;

/**
 * Base object used to configure rendering within the Wikitude Native SDK.
 */
@interface WTRenderingConfiguration : NSObject

/**
 * @brief Method that returns a pre configured View object appropriate for the rendering API selected that can be added to the view hierarchy.
 *
 * This method is only relevant when using the Wikitude Native SDK with Internal rendering and will return nil otherwise.
 * @discussion Please note that the returned pointer is of type weak and the view should either be added to the view hierarchy within the scope of the calling method or retained (assigned to a strong property) until the view was added to the view hierarchy.
 *
 * @return UIView A weak pointer to a view.
 */
- (UIView *)view;

@end

/** 
 * Object used to configure OpenGL ES rendering within the Wikitude Native SDK.
 */
@interface WTOpenGLESRenderingConfiguration : WTRenderingConfiguration

/**
 * @brief The designated initializer to create an object of this class.
 *
 * @param openGLESRenderingMode The rendering mode that should be used by the Wikitude Native SDK. Please see WTOpenGLESRenderingMode for more information about the different rendering modes.
 * @return WTOpenGLESRenderingConfiguration An object of type WTOpenGLESRenderingConfiguration.
 */
- (instancetype)initWithOpenGLESRenderingMode:(WTOpenGLESRenderingMode *)openGLESRenderingMode;

/**
 * @brief Provides access to the selected OpenGL ES rendering mode.
 * @return WTOpenGLESRenderingMode The rendering mode that was passed during initialization.
 */
- (WTOpenGLESRenderingMode *)openGLESRenderingMode;

/**
 * @brief Method that returns a pre configured OpenGL ES View object that can be added to the view hierachy. Please see -view in WTRenderingConfiguration for more information.
 *
 * @return WTEAGLView A weak pointer to an OpenGL ES View.
 */
- (WTEAGLView *)eaglView;

@end

/**
 * Object used to configure Metal rendering within the Wikitude Native SDK.
 */
@interface WTMetalRenderingConfiguration : WTRenderingConfiguration

/**
 * @brief The designated initializer to create an object of this class.
 *
 * @param metalRenderingMode The rendering mode that should be used by the Wikitude Native SDK. Please see WTMetalRenderingMode for more information about the different rendering modes.
 * @return WTMetalRenderingConfiguration An object of type WTMetalRenderingConfiguration.
 */
- (instancetype)initWithMetalRenderingMode:(WTMetalRenderingMode *)metalRenderingMode;

/**
 * @brief Provides access to the selected Metal rendering mode.
 * @return WTMetalRenderingMode The rendering mode that was passed during initialization.
 */
- (WTMetalRenderingMode *)metalRenderingMode;

/**
 * @brief Method that returns a pre configured Metal View object that can be added to the view hierarchy. Please see -view in WTRenderingConfiguration for more information.
 *
 * @return WTMetalView A weak pointer to a Metal View.
 */
- (WTMetalView *)metalView;

@end
