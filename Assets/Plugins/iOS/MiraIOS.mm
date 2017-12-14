
#import "MiraIOS.h"

void _ForceBrightness()
{
	
	[UIScreen mainScreen].brightness = 1.0;
	[UIApplication sharedApplication].idleTimerDisabled = NO;
    [UIApplication sharedApplication].idleTimerDisabled = YES;

	// To do: Reset the brightness whenever the application enters the background
    // To do: Save original brightness to a variable, and re-activate it when app closes
}

@implementation MiraIOS

@end
	


