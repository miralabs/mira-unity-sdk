//
//  WTInstantTrackingPointCloudDataProvider.h
//  WikitudeNativeSDK
//
//  Created by Andreas Schacherbauer on 28.08.17.
//  Copyright © 2017 Wikitude. All rights reserved.
//

#import <Foundation/NSObject.h>
#import <Foundation/NSArray.h>
#import <Foundation/NSValue.h>


/**
 * @brief The completion handler for the WTDefaultInstantTrackingPointCloudDataProvider
 *
 * The NSValues given in the NSArray contain WTPoint3D structs. To convert the NSValue to the actual underlying struct, use a snippet like the following:
 *
 * for ( NSValue *pointValue in pointCloud) {
 *     WTPoint3D point;
 *     [pointValue getValue:&point];
 *     NSLog(@"point.x: %f - point.y: %f - point.z: %f", point.x, point.y, point.z);
 * }
 *
 * @param pointCloud The current point cloud as NSArray of NSValue objects.
 */
typedef void(^WTDefaultInstantTrackingPointCloudRequestCompletionHandler)(NSArray<NSValue *> * pointCloud);


/**
 * @brief A point cloud data provider defines how the point cloud is represented
 *
 * Different data provider all deliver the same data, just the types in which the point cloud is represented is different.
 * This helps reducing type conversion in situations where performance matters
 *
 * WTInstantTrackingPointCloudDataProvider defines a empty base class for different data provider
 */
@interface WTInstantTrackingPointCloudDataProvider : NSObject
@end


/**
 * @brief A point cloud data provider that delivers the point cloud as ObjectiveC data types
 */
@interface WTDefaultInstantTrackingPointCloudDataProvider : WTInstantTrackingPointCloudDataProvider

- (instancetype)init NS_UNAVAILABLE;
+ (instancetype)new NS_UNAVAILABLE;

/**
 * @brief Creates a WTDefaultInstantTrackingPointCloudDataProvider object
 *
 * @param completionHandler The block that is called once the point cloud is retrieved and converted to the data types given as parameter to the block
 */
+ (instancetype)pointCloudDataProviderWithCompletionHandler:(WTDefaultInstantTrackingPointCloudRequestCompletionHandler)completionHandler;

@end
