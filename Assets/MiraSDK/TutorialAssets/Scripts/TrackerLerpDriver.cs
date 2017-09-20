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

public class TrackerLerpDriver : MonoBehaviour {

	public MarkerRotationWatcher rotationWatcher;

	public Vector3 startPosition = new Vector3(0, 0.15f, 0);
	public Vector3 endPosition = new Vector3(0,0,0);
	// Use this for initialization
	void Start () {
		
	}
	
	// Update is called once per frame
	void Update () {
		transform.localPosition = Vector3.Lerp (startPosition, endPosition, rotationWatcher.deltaDotProduct);
		
		
		
	}
}
