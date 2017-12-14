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
    /// Lerps the position of an object to follow the user's gaze.
    /// This script is preferable to sticking elements directly to the camera rig, which is uncomfortable UX
    /// </summary>
    public class FollowHeadAutomaticScale : MonoBehaviour
    {
        /// <summary>
        /// Adjust offsets for how the object follows the user's gaze.
        /// Note that these offsets will scale when the scale multiplier is adjusted.
        /// For this reason, treat the offset values as if they were in CM (the default offset is 1 meter in Z).
        /// You'll notice the value scales itself on Start - though slightly annoying, this is designed to make the script as automatic as possible (so the default values work regardless of scene scale)
        /// </summary>
        /// <returns></returns>
        public Vector3 headFollowPositionM = new Vector3(0f, 0f, 0.7f);

        /// <summary>
        /// Adjust how quickly the object follows head movements
        /// </summary>
        public float lerpSpeed = 3f;

        private Transform head;

        private void Start()
        {
            head = MiraArController.Instance.cameraRig.transform;

            headFollowPositionM *= 100 * (1 / MiraArController.scaleMultiplier);

			transform.localScale *= 100 * (1/MiraArController.scaleMultiplier);

            transform.localRotation = head.rotation;
            transform.localPosition = FindRelativePosition();
        }

        private Vector3 FindRelativePosition()
        {
            return head.position + (head.rotation * headFollowPositionM);
        }

        private void Update()
        {
            transform.rotation = Quaternion.Lerp(transform.rotation, head.rotation, Time.fixedDeltaTime * lerpSpeed);
            transform.position = Vector3.Lerp(transform.position, FindRelativePosition(), Time.fixedDeltaTime * lerpSpeed);
        }
    }
}