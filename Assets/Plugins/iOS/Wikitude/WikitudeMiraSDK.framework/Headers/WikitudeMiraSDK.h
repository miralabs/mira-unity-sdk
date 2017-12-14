//
//  WikitudeMiraSDK.h
//  WikitudeMiraSDK
//
//  Created by Andreas Schacherbauer on 03.07.17.
//  Copyright © 2017 Wikitude. All rights reserved.
//

#import <UIKit/UIKit.h>

//! Project version number for WikitudeMiraSDK.
FOUNDATION_EXPORT double WikitudeMiraSDKVersionNumber;

//! Project version string for WikitudeMiraSDK.
FOUNDATION_EXPORT const unsigned char WikitudeMiraSDKVersionString[];

// In this header, you should import all the public headers of your framework using statements like #import <WikitudeMiraSDK/PublicHeader.h>
#import <WikitudeMiraSDK/WTWikitudeNativeSDK.h>

#import <WikitudeMiraSDK/WTSDKBuildInformation.h>

#import <WikitudeMiraSDK/WTAuthorizationRequestManager.h>
#import <WikitudeMiraSDK/WTNativeSDKStartupConfiguration.h>
#import <WikitudeMiraSDK/WTExternalCMMotionManagerDataAccessMode.h>

#import <WikitudeMiraSDK/WTCaptureDeviceManager.h>

#import <WikitudeMiraSDK/WTEAGLView.h>
#import <WikitudeMiraSDK/WTMetalView.h>

#import <WikitudeMiraSDK/WTTrackerManager.h>

#import <WikitudeMiraSDK/WTTarget.h>

#import <WikitudeMiraSDK/WTTargetCollectionResource.h>

#import <WikitudeMiraSDK/WTCloudRecognitionService.h>
#import <WikitudeMiraSDK/WTCloudRecognitionServiceConfiguration.h>
#import <WikitudeMiraSDK/WTCloudRecognitionServiceResponse.h>
#import <WikitudeMiraSDK/WTCloudRecognitionResponse.h>

#import <WikitudeMiraSDK/WTImageTracker.h>
#import <WikitudeMiraSDK/WTImageTrackerConfiguration.h>
#import <WikitudeMiraSDK/WTImageTarget.h>

#import <WikitudeMiraSDK/WTInstantTracker.h>
#import <WikitudeMiraSDK/WTInstantTarget.h>
#import <WikitudeMiraSDK/WTInstantTrackerConfiguration.h>
#import <WikitudeMiraSDK/WTInitializationPose.h>
#import <WikitudeMiraSDK/WTInstantTrackingCoordinateConversionHandler.h>
#import <WikitudeMiraSDK/WTInstantTrackingPointCloudDataProvider.h>
#import <WikitudeMiraSDK/WTCPPInstantTrackingPointCloudDataProvider.h>

#import <WikitudeMiraSDK/WTObjectTracker.h>
#import <WikitudeMiraSDK/WTObjectTarget.h>

#import <WikitudeMiraSDK/WTBaseTracker.h>
#import <WikitudeMiraSDK/WTClientTracker.h>
#import <WikitudeMiraSDK/WTCloudTracker.h>

#import <WikitudeMiraSDK/WTWikitudeNativeSDK+Plugins.h>
#import <WikitudeMiraSDK/Frame.h>
#import <WikitudeMiraSDK/Matrix4.h>
#import <WikitudeMiraSDK/Geometry.h>
#import <WikitudeMiraSDK/FrameColorSpace.h>
#import <WikitudeMiraSDK/RecognizedTarget.h>
#import <WikitudeMiraSDK/InterfaceOrientation.h>
#import <WikitudeMiraSDK/Plugin.h>
#import <WikitudeMiraSDK/InputRenderSettings.h>
#import <WikitudeMiraSDK/InputFrameSettings.h>
#import <WikitudeMiraSDK/InputPlugin.h>
#import <WikitudeMiraSDK/RenderingAPI.h>
#import <WikitudeMiraSDK/RenderingContext.h>
#import <WikitudeMiraSDK/MetalRenderingContext.h>
#import <WikitudeMiraSDK/RecognizedTargetsBucket.h>
