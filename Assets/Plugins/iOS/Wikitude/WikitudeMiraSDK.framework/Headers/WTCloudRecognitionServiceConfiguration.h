//
//  WTCloudRecognitionServiceConfiguration.h
//  WikitudeNativeSDK
//
//  Created by Andreas Schacherbauer on 20.06.17.
//  Copyright © 2017 Wikitude. All rights reserved.
//

#import <Foundation/Foundation.h>


NS_ASSUME_NONNULL_BEGIN


FOUNDATION_EXPORT NSString * const WTCloudRecognitionServerURL_Americas;
FOUNDATION_EXPORT NSString * const WTCloudRecognitionServerURL_Europe;

/**
 * @brief WTCloudRecognitionServiceConfiguration can be used to define WTCloudRecognitionService specific settings
 *
 * @discussion This class should not be instantiated outside of the Wikitude Native SDK.
 */
@interface WTCloudRecognitionServiceConfiguration : NSObject

/**
 * Defines the URL to which the WTCloudRecognitionService should connect to
 */
@property (nonatomic, copy) NSString            *cloudRecognitionServerURL;

@end

NS_ASSUME_NONNULL_END
