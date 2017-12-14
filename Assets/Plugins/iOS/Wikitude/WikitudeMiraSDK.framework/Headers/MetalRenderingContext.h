//
//  MetalRenderingContext.h
//  CommonLibrary
//
//  Created by Alexandru Florea on 09/05/2017.
//  Copyright © 2017 Wikitude. All rights reserved.
//

#ifndef __CommonLibrary__MetalRenderingContext__
#define __CommonLibrary__MetalRenderingContext__

#ifdef __cplusplus

#include "RenderingContext.h"

#ifdef __APPLE__
#import <Metal/Metal.h>
#endif

namespace wikitude { namespace sdk {
 
    namespace impl {
        
        /**
         * The MetalRenderingContext can be used by plugins to access the native Metal objects from inside the SDK.
         * This is useful for plugins that would like to do some rendering using the Metal rendering API.
         */
        class MetalRenderingContext : public RenderingContext {
        public:
            MetalRenderingContext() { }
            virtual ~MetalRenderingContext() { }
            
#ifdef __APPLE__
            MetalRenderingContext(id<MTLDevice> metalDevice_, id<MTLRenderCommandEncoder> metalCommandEncoder_)
            {
                _metalDevice = metalDevice_;
                _metalCommandEncoder = metalCommandEncoder_;
            }
            
            /**
             * Returns the Metal device used by the SDK for rendering with Metal.
             * @return the current MTLDevice
             */
            id<MTLDevice>               getMetalDevice()            { return _metalDevice; }
            /**
             * Returns the Metal command encoder currently used by the SDK. This is only valid during calls to startRender and endRender inside the plugin.
             * @return the current MTLRenderCommandEncoder
             */
            id<MTLRenderCommandEncoder> getMetalCommandEncoder()    { return _metalCommandEncoder; }
#endif
        private:
#ifdef __APPLE__
            id<MTLDevice>               _metalDevice;
            id<MTLRenderCommandEncoder> _metalCommandEncoder;
#endif
        };
    }
    using impl::MetalRenderingContext;
}}

#endif /* __cplusplus */

#endif /* __CommonLibrary__MetalRenderingContext__ */
