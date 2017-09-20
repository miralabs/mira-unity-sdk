// Copyright (c) Mira Labs, Inc., 2017. All rights reserved.
// 
// Downloading and/or using this MIRA SDK is under license from MIRA, 
// and subject to all terms and conditions of the Mira Software License,
// found here: www.mirareality.com/sdk-license/
// 
// By downloading this SDK, you agree to the Mira Software License.
//
// This SDK may only be used in connection with the development of
// applications that are exclusively created for, and exclusively available
// for use with, MIRA hardware devices. This SDK may only be commercialized
// in the U.S. and Canada, subject to the terms of the License.
// 
// The MIRA SDK includes software under license from The Apache Software Foundation.

using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.Runtime.InteropServices;


public class MiraiOSBridge : MonoBehaviour
{
#if UNITY_IPHONE
	[DllImport ("__Internal")]
	private static extern void _ForceBrightness();
#endif

#if UNITY_IPHONE
	public static void ForceBrightness()
	{
		if (Application.platform != RuntimePlatform.OSXEditor)
			_ForceBrightness();
	}

#endif
}