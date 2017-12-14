//
//  WTTargetCollectionResource.h
//  WikitudeNativeSDK
//
//  Created by Alexandru Florea on 19/10/16.
//  Copyright © 2016 Wikitude. All rights reserved.
//

#ifndef WTTargetCollectionResource_h
#define WTTargetCollectionResource_h

#import <Foundation/Foundation.h>

NS_ASSUME_NONNULL_BEGIN

/**
 * WTTargetCollectionResourceCompletionHandler represents a block object which is invoked when the target collection resource finished loading the target collection file, either successfully, or with an error.
 */
typedef void(^WTTargetCollectionResourceCompletionHandler)(BOOL, NSError * _Nullable);

/**
 * @brief Represents a resource that was initialized with a .wtc (Wikitude Target Collection) file or a .wmc (Wikitude Map Collection) file.
 *
 * @discussion Target Collection Resources can either retrieve image targets from a .wtc file or object targets from a .wmc file. The file can be loaded eitherfrom the application bundle or a remote server.
 * If the file was loaded from a server, no internet connection is required anymore after the loading finished successfully.
 */
@interface WTTargetCollectionResource : NSObject

/**
 * Specifies the URL that will be used to locate and load the file.
 */
@property (nonatomic, copy, readonly) NSURL                 *URL;

/**
 * Specifies if the target collection resource is currently loading the file, or if it already finished (successfully or not)
 */
@property (nonatomic, assign, readonly) BOOL                loading;

/**
 * @brief Causes the target collection resource to cancel any ongoing loading operation.
 *
 * @discussion Cancel should only be called when the target collection resource is actively loading a file. Callind cancel after the loading operation finished has no effect.
 */
- (void)cancel;

@end

NS_ASSUME_NONNULL_END

#endif /* WTTargetCollectionResource_h */
