//
//  WTInstantTrackingCoordinateConversionHandler.h
//  WikitudeNativeSDK
//
//  Created by Daniel Guttenberg on 28/07/17.
//  Copyright © 2017 Wikitude. All rights reserved.
//

#import "WTPoint3D.h"

/**
 * @brief Block typedef for the completion handler of `convertScreenCoordinate:toPointCloudCoordinateOnQueue:completion`
 */
typedef void(^WTInstantTrackingCoordinateConversionHandler)(BOOL success, WTPoint3D * __nullable pointCloudCoordinate);
