//
//  WTCloudRecognitionServiceResponse.h
//  WikitudeNativeSDK
//
//  Created by Alexandru Florea on 31/10/16.
//  Copyright © 2016 Wikitude. All rights reserved.
//

#ifndef WTCloudRecognitionServiceResponse_h
#define WTCloudRecognitionServiceResponse_h

#import <Foundation/Foundation.h>


NS_ASSUME_NONNULL_BEGIN

/**
 * @brief A class that represents a cloud recognition response.
 */
@interface WTCloudRecognitionServiceResponse : NSObject

/**
 * @brief Represents if a image target was found in the camera frame that was processed on the cloud recognition server.
 */
@property (nonatomic, assign, readonly) BOOL                        recognized;

/**
 * @brief If a image target was found, this dictionary contains more information about the image target.
 *
 * Possible informations are: image target name and rating
 */
@property (nonatomic, strong, readonly) NSDictionary                *targetInformations;

/**
 * @brief Custom metadata that was associated with a certain image target once it was created.
 */
@property (nonatomic, strong, readonly) NSDictionary                *metadata;

@end

NS_ASSUME_NONNULL_END


#endif /* WTCloudRecognitionServiceResponse_h */
