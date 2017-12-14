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

namespace Mira
{
    /// <summary>
    /// Uses touchpad data to scroll the length of the fishingline
    /// </summary>
    public class FishinglineCast : MonoBehaviour
    {
        /// <summary>
        /// The minimum value that the fishingline can be (in CM)
        /// </summary>
        public float minCast = 50f;

        /// <summary>
        /// The maximum value that the fishingline can be (in CM)
        /// </summary>
        public float maxCast = 2000f;

        /// <summary>
        /// Adjusts the speed of the cast/scroll
        /// </summary>
        public float castSpeedMultiplier = 5f;

        // Triggered the first time touchpad data is streamed (essential in scroll functionality)
        private bool castStart = true;

        private float lastCast;

        private Fishingline fishingline;
        public float currentScaleMult = 1f;

        private void Start()
        {
            fishingline = gameObject.GetComponent<Fishingline>();
        }

        private void Update()
        {
            Cast();
        }

        private void SetFishingScale(float scalevalue)
        {
            fishingline.ScaleDistance(scalevalue);
            currentScaleMult = scalevalue;
        }

        private void Cast()
        {
            if (castStart == true)
            {
                lastCast = MiraController.TouchPos.y;
                castStart = false;
            }

            if (MiraController.TouchHeld)
            {
                currentScaleMult += ((MiraController.TouchPos.y - lastCast) * castSpeedMultiplier);
                lastCast = MiraController.TouchPos.y;
                if (currentScaleMult < minCast)
                    currentScaleMult = minCast;
                else if (currentScaleMult > maxCast)
                    currentScaleMult = maxCast;

                SetFishingScale(currentScaleMult);
            }
            else
            {
                castStart = true;
            }
        }
    }
}