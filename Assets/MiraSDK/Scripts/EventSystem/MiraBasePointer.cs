// Copyright (c) Mira Labs, Inc., 2017. All rights reserved.
//
// Downloading and/or using this MIRA SDK is under license from MIRA,
// and subject to all terms and conditions of the Mira SDK License Agreement,
// found here: https://www.mirareality.com/Mira_SDK_License_Agreement.pdf
//
// By downloading this SDK, you agree to the Mira SDK License Agreement.
//
// This SDK may only be used in connection with the development of
// applications that are exclusively created for, and exclusively available
// for use with, MIRA hardware devices. This SDK may only be commercialized
// in the U.S. and Canada, subject to the terms of the License.

using UnityEngine;
using UnityEngine.EventSystems;

/// <summary>
/// MiraBasePointer is an abstract class describing what a pointer (ie: a reticle or cursor) should do,
/// this is useful if you want to create your own pointer besides the provided reticle.
///
/// Functions are called from MiraInputModule and help the pointer graphically respond to interaction events.
/// </summary>
public abstract class MiraBasePointer
{
    /// <summary>
    /// This is used by MiraBaseRaycaster to provide raycasts to the input system if it's in Camera mode.
    /// </summary>
    /// <value>The max distance.</value>
    public float maxDistance
    {
        get;
        set;
    }

    // Called on MiraInputModule enable and disable
    public abstract void OnInputModuleEnabled();

    public abstract void OnInputModuleDisabled();

    public abstract void OnPointerEnter(GameObject targetGameObject, RaycastResult ray, bool isTargetInteractive);

    public abstract void OnPointerHover(GameObject targetGameObject, RaycastResult ray, bool isTargetInteractive);

    public abstract void OnPointerExit(GameObject targetGameObject);

    public abstract void OnPointerClick();

    public abstract void OnPointerClickDown();

    public abstract void OnPointerClickUp();

    /// <summary>
    /// Importantly instances of the pointer must call this on Start to register themselves
    /// </summary>
	public void OnStart(GameObject pointer)
    {
		MiraPointerManager.Instance.OnPointerCreated(pointer, this);
    }
}