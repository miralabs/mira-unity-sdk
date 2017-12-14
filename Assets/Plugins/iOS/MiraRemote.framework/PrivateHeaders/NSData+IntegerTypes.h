//
//  NSData+IntegerTypes.h
//  MiraRemote
//
//  Created by Riley Testut on 9/16/17.
//  Copyright © 2017 Mira Labs, Inc. All rights reserved.
//

#import <Foundation/Foundation.h>

@interface NSData (IntegerTypes)

@property (nonatomic, readonly) const uint8_t *unsignedBytes;

- (int16_t)int12AtIndex:(NSUInteger)index byteAligned:(BOOL)byteAligned;
- (int32_t)int24AtIndex:(NSUInteger)index;

- (uint16_t)uint12AtIndex:(NSUInteger)index byteAligned:(BOOL)byteAligned;
- (int32_t)uint16AtIndex:(NSUInteger)index;
- (int32_t)uint24AtIndex:(NSUInteger)index;

@end
