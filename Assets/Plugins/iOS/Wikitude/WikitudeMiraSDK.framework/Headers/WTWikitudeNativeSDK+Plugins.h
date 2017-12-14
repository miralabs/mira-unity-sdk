//
//  WTWikitudeNativeSDK+Plugins.h
//  WikitudeNativeSDK
//
//  Created by Andreas Schacherbauer on 01/07/15.
//  Copyright (c) 2015 Wikitude. All rights reserved.
//

#ifdef __cplusplus

#include <memory>


namespace wikitude {
    namespace sdk {
        namespace impl {
            class Plugin;
        }
        using impl::Plugin;
    }
}

/**
 * WTWikitudeNativeSDK+Plugins defines some methods that can be used to register C++ plugins in the Wikitude Native SDK.
 *
 * To use C++ code within your Objective-C class, set the file extension to .mm or change the type to Objective-C++ source in the Xcode Identity and Type inspector.
 *
 * Plugins need to derive from wikitude::sdk::Plugin and have to be passes as std::shared_ptr.
 */
@interface WTWikitudeNativeSDK (Plugins)

/**
 * @brief Call this method to register a new plugin.
 *
 * @discussion If a plugin with the same identifier is already registered, NO is returned, YES otherwise.
 *
 * @param plugin The plugin that should be registered
 * @param error An error object that will be filled when plugin registration fails
 *
 * @return BOOL YES if the plugin could be registered successfully, NO otherwise
 */
- (BOOL)registerPlugin:(std::shared_ptr<wikitude::sdk::Plugin>)plugin error:(NSError * __autoreleasing *)error;

/**
 * @brief Call this method to register a new plugin.
 *
 * @discussion If a plugin with the same identifier is already registered, NO is returned, YES otherwise.
 *
 * @deprecated Since version 2.1.0. Use -registerPlugin:plugin:error: instead.
 *
 * @param plugin The plugin that should be registered
 * @return BOOL YES if the plugin could be registered successfully, NO otherwise
 */
- (BOOL)registerPlugin:(std::shared_ptr<wikitude::sdk::Plugin>)plugin WT_DEPRECATED_SINCE(2.1.0, "Use -registerPlugin:plugin:error: instead.");

/**
 * @brief Call this method to remove an already registered plugin.
 *
 * @discussion If the given plugin ptr is not registered, NO is returned, YES otherwise.
 *
 * @param plugin The plugin that should be removed.
 * @return BOOL YES if the plugin was known and could be removed, NO otherwise.
 */
- (BOOL)removePlugin:(std::shared_ptr<wikitude::sdk::Plugin>)plugin;

/**
 * @brief Call this method to remove an already registered plugin.
 *
 * @discussion Same as -removePlugin but searches for the plugin to remove by the given identifier. Using this method, no shared_ptr property needs to be added to any object because the Wikitude Native SDK takes ownership of any plugin.
 *
 * @param pluginIdentifier The Identifier that uniquely identifies the plugin that should be removed.
 * @return BOOL YES if the plugin was known and could be removed, NO otherwise.
 */
- (BOOL)removeNamedPlugin:(NSString *)pluginIdentifier;

@end

#endif /* __cplusplus */
