//
//  WTMetalView.h
//  WikitudeSDK
//
//  Created by Andreas Schacherbauer on 5/15/17.
//
//

#import <UIKit/UIKit.h>


@class WTRenderer;

/**
 * @brief A UIView subclass ready for Metal rendering within Wikitude products
 */
@interface WTMetalView : UIView

/**
 * The renderer that is associated with this view
 */
@property (nonatomic, weak) WTRenderer                *renderer;

@end
