using UnityEngine;
using System.Collections;
using System.Collections.Generic;

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