//
//  MRRemote_Private.h
//  MiraRemote
//
//  Created by Riley Testut on 9/15/17.
//  Copyright © 2017 Mira Labs, Inc. All rights reserved.
//

#import "MRRemote.h"

#import <CoreBluetooth/CoreBluetooth.h>

NS_ASSUME_NONNULL_BEGIN

typedef NSString *MRRemoteServiceIdentifier NS_TYPED_ENUM;
extern const MRRemoteServiceIdentifier MRRemoteServiceIdentifierDefault;
extern const MRRemoteServiceIdentifier MRRemoteServiceIdentifierPreferred;

@interface MRRemote ()

@property (class, nonatomic, readonly) NSArray<CBUUID *> *serviceUUIDs;

@property (nonatomic, readwrite, getter=isConnected) BOOL connected;
@property (nonatomic, readwrite, getter=isPreferred) BOOL preferred;

@property (nullable, copy, nonatomic, readwrite) NSNumber *RSSI;

@property (copy, nonatomic, readonly) CBPeripheral *peripheral;

- (instancetype)initWithPeripheral:(CBPeripheral *)peripheral NS_DESIGNATED_INITIALIZER;

@end

NS_ASSUME_NONNULL_END
