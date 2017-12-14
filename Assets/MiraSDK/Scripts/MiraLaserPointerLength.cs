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

using Mira;
using UnityEngine;

/// <summary>
/// Turns the controller into a laser pointer; the length adapts when the reticle jumps position during a collision
/// </summary>
public class MiraLaserPointerLength : MonoBehaviour
{
    /// <summary>
    /// Scale up the laser pointer width
    /// </summary>
    public float widthMultiplier = 1f;

    private MiraReticle reticle;
    private LineRenderer rend;

    private void Start()
    {
        reticle = GameObject.FindObjectOfType<MiraReticle>();

        rend = gameObject.GetComponent<LineRenderer>();
        if (rend)
        {
            float width = 1 * widthMultiplier * 1 / MiraArController.scaleMultiplier;
            rend.endWidth = width;
            rend.startWidth = width;
        }
    }

    public void MatchReticle(Vector3 reticleWorldPos, float length)
    {
        if (rend)
        {
            rend.SetPosition(0, transform.position);
            rend.SetPosition(1, reticle.transform.position);
        }
    }
}