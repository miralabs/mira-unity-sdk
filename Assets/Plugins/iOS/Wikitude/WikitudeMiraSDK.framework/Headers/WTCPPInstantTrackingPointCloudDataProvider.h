//
//  WTCPPInstantTrackingPointCloudDataProvider.h
//  WikitudeNativeSDK
//
//  Created by Andreas Schacherbauer on 28.08.17.
//  Copyright © 2017 Wikitude. All rights reserved.
//

#import "WTInstantTrackingPointCloudDataProvider.h"

#ifdef __cplusplus

#include <vector>

#import "WTPoint3D.h"


/**
 * @brief the completion handler for the WTCPPInstantTrackingPointCloudDataProvider
 *
 * @param pointCloud the current point cloud as const C++ std::vector<WTPoint3D> reference.
 */
typedef void(^WTCPPInstantTrackingPointCloudRequestCompletionHandler)(const std::vector<WTPoint3D>& pointCloud);


/**
 * @brief A point cloud data provider that delivers the point clous as const C++ std::vector<WTPoint3D> reference.
 */
@interface WTCPPInstantTrackingPointCloudDataProvider : WTInstantTrackingPointCloudDataProvider

/**
 * @brief Creates a WTCPPInstantTrackingPointCloudDataProvider object
 *
 * @param completionHandler The block that is called once the point cloud is retrieved and converted to the data types given as parameter to the block
 */
+ (instancetype)pointCloudDataProviderWithCPPCompletionHandler:(WTCPPInstantTrackingPointCloudRequestCompletionHandler)completionHandler;
@end

#endif
