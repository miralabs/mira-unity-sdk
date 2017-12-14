//
//  NSUserDefaults+MiraRemote.h
//  MiraRemote
//
//  Created by Riley Testut on 9/17/17.
//  Copyright © 2017 Mira Labs, Inc. All rights reserved.
//

#import <Foundation/Foundation.h>

NS_ASSUME_NONNULL_BEGIN

@interface NSUserDefaults (MiraRemote)

@property (nullable, copy, nonatomic) NSUUID *previousRemoteUUID;

@end

NS_ASSUME_NONNULL_END
