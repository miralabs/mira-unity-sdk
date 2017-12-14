//
//  WTCaptureDeviceManager.h
//  WikitudeNativeSDK
//
//  Created by Andreas Schacherbauer on 02/05/15.
//  Copyright (c) 2015 Wikitude. All rights reserved.
//

#import <Foundation/Foundation.h>
#import <AVFoundation/AVFoundation.h>
#import <CoreGraphics/CGBase.h>
#import <OpenGLES/gltypes.h>


NS_ASSUME_NONNULL_BEGIN

@class WTCaptureDeviceManager;

/**
 * WTCaptureDeviceManagerDelegate provides capture device manager specific methods that can be used to react to capture devcie manager specific state changes.
 */
@protocol WTCaptureDeviceManagerDelegate <NSObject>
@optional
/**
 * @brief Called whenever the capture device position changed.
 *
 * @param captureDeviceManager The capture device manger that managed the capture device position change.
 * @param activeCaptureDevicePosition The capture device position that is now active.
 */
- (void)captureDeviceManager:(WTCaptureDeviceManager *)captureDeviceManager didSwitchToActiveCaptureDevicePosition:(AVCaptureDevicePosition)activeCaptureDevicePosition;

/**
 * @brief Called whenever the capture device authorization status changed.
 *
 * @param captureDeviceManager The capture device manager that monitored the capture device authorization change.
 * @param authorizationStatus The authorization status that is now set.
 */
- (void)captureDeviceManager:(WTCaptureDeviceManager *)captureDeviceManager didChangeCaptureDeviceAuthorizationStatus:(AVAuthorizationStatus)authorizationStatus;

@end


/**
 * @brief WTCaptureDeviceManager can be used to change capture device specific settins at runtime.
 */
@interface WTCaptureDeviceManager : NSObject

/**
 * @brief The delegate object that is associated with this capture device manager object.
 */
@property (nonatomic, weak) id<WTCaptureDeviceManagerDelegate>          delegate;

/**
 * @brief The capture device position that is currently used or should be used.
 *
 * @return activeCaptureDevicePosition The capture device position that is currently active.
 */
@property (nonatomic, assign) AVCaptureDevicePosition                   activeCaptureDevicePosition;

/**
 * @brief The focus mode that is currently active or should be used.
 *
 * @return focusMode The focus mode that is currently active.
 */
@property (nonatomic, assign) AVCaptureFocusMode                        focusMode;

/**
 * @brief The focus distance that is currently set or should be used.
 *
 * @discussion Values between 0 and 1 are accepted. Setting an unsupported value will result in an exception.
 *
 * @return focusDistance the focus distance that is currently active.
 */
@property (nonatomic, assign) float                                     focusDistance;

/**
 * @brief The auto focus range restriction that is currently active or should be used.
 *
 * @return autoFocusRangeRestriction The auto focus range restriction that is currently active.
 */
@property (nonatomic, assign) AVCaptureAutoFocusRangeRestriction        autoFocusRangeRestriction;

/**
 * @brief The zoom level that is currently active or should be used.
 *
 * @discussion Setting an unsupported value causes an internal error and does not change the currently set zoom level.
 *
 * @return zoomLevel The zoom level that is currently set.
 */
@property (nonatomic, assign) CGFloat                                   zoomLevel;

/**
 * @brief The maximal supported zoom level.
 *
 * @discussion This value should be read before setting a new zoom level so that a valid value is set.
 *
 * @return maxZoomLevel The maximal supported zoom level.
 */
@property (nonatomic, assign, readonly) CGFloat                         maxZoomLevel;

/**
 * @brief Indicates whether the current device has a torch.
 *
 * @return YES if a torch is available, NO otherwise.
 */
@property (nonatomic, assign, readonly) BOOL                            hasTorch;

/**
 * @brief The torch mode that is currently active or should be used
 *
 * @return torchMode The torch mode that is currently active
 */
@property (nonatomic, assign) AVCaptureTorchMode                        torchMode;

/**
 * @brief The cameras horizontal field of view in degree.
 *
 * @return fieldOfView the current cameras horizontal field of view in degree
 */
@property (nonatomic, assign, readonly) CGFloat                         fieldOfView;

/**
 * The target texture that should be used to draw the camera frame onto.
 *
 * @default -1
 * @discussion You should only use this property if your application has already setup a fixed rendering pipeline and wants the Wikitude SDK to draw the camera frame onto a very specific OpenGL ES 2 texture. Under normal circumstances, WTRenderingMode_External should be used if the Wikitude SDK should draw the camera frame in an already existing rendering environment.
 */
@property (nonatomic, assign) GLuint                                    cameraRenderingTargetTexture;

/**
 * @brief Use this method to define the point of interest which should be used to adopt the exposure
 */
- (void)exposeAtPointOfInterest:(CGPoint)point withMode:(AVCaptureExposureMode)exposureMode;

@end

NS_ASSUME_NONNULL_END
