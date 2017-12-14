//
//  WTEAGLView.h
//  WikitudeSDK
//
//  Created by Andreas Schacherbauer on 8/29/12.
//
//

#import <UIKit/UIKit.h>


@class WTRenderer;

/**
 * @brief a UIView subclass ready for OpenGL ES rendering within Wikitude products
 */
@interface WTEAGLView : UIView

/**
 * The renderer that is associated with this view
 */
@property (nonatomic, weak) WTRenderer                *renderer;

@end
