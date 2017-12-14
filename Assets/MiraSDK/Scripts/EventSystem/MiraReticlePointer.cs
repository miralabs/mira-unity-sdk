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
/// MiraReticlePointer extends MiraBasePointer and calls the scripts within MiraReticle that help it change appearance
/// when the event system processes interaction events
/// </summary>
public class MiraReticlePointer : MiraBasePointer
{
    public override void OnInputModuleEnabled()
    {
    }

    public override void OnInputModuleDisabled()
    {
    }

    public override void OnPointerEnter(GameObject targetGameObject, RaycastResult ray, bool isTargetInteractive)
    {
        MiraReticle.Instance.reticleEnter(ray, isTargetInteractive);
    }

    public override void OnPointerHover(GameObject targetGameObject, RaycastResult ray, bool isTargetInteractive)
    {
        MiraReticle.Instance.reticleHover(ray, isTargetInteractive);
    }

    public override void OnPointerExit(GameObject targetGameObject)
    {
        MiraReticle.Instance.reticleExit();
    }

    public override void OnPointerClick()
    {
    }

    public override void OnPointerClickDown()
    {
    }

    public override void OnPointerClickUp()
    {
    }
}