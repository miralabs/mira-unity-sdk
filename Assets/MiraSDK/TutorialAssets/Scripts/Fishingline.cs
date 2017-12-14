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
    /// Creates a tentacle-like "whip" from the MiraController that feels like it has weight
    /// </summary>
    public class Fishingline : MonoBehaviour
    {
        private Transform[] currentSegments;
        private Transform[] referencePoints;
        private Vector3[] defaultPositions;

        /// <summary>
        /// Default length of the fishingline (in CM)
        /// </summary>
        public float defaultFishlineLength = 200;

        private int numSegments = 15;

        private Transform destination;
        private float hitDistance;
        private MiraReticle reticle;

        [SerializeField]
        private
        /// <summary>
        /// How quickly the fishinline lerps when the controller is moved
        /// Setting this to a small value can make the fishingline feel like it's underwater
        /// Setting this to a large value will turn the fishinline in to a very inefficient laser pointer.!-- Don't do this
        /// </summary>
        float lerpSpeedMult = 1;

        private Transform fishinglineParent;

        private LineRenderer lineRend;

        [SerializeField]
        private bool colliders = false;

        [SerializeField]
        private
        /// <summary>
        /// To stick an object to the end of the fishinline (like the rocketship in our demo scene, connect it here)
        /// This is enabled on start, and designed for an object that is permanently attached to the end of the fishinline
        /// If you would like to dynamically parent/unparent objects, parent it to the attachPoint Transform below
        /// </summary>
        GameObject parentedToEnd;

        public Transform attachPoint;

        private void Start()
        {
            fishinglineParent = transform.parent;
            currentSegments = new Transform[numSegments];
            referencePoints = new Transform[numSegments];
            defaultPositions = new Vector3[numSegments];

            hitDistance = defaultFishlineLength * (1 / MiraArController.scaleMultiplier);

            lineRend = GetComponent<LineRenderer>();

            // Set to Scene Scale
            // lineRend.startWidth *= (1 / MiraArController.scaleMultiplier);
            // lineRend.endWidth *= (1 / MiraArController.scaleMultiplier);

            CreateSegments();
            CreateRefPoints();
        }

        private void CreateSegments()
        {
            float segmentLength = hitDistance / (numSegments - 1);
            float widthDelta = 0f;
            if (colliders == true)
            {
                widthDelta = (lineRend.startWidth - lineRend.endWidth) / (numSegments - 1);
            }
            // Segment 0 is closest to you, aka the snappiest/fastest moving
            for (int i = 0; i < numSegments; i++)
            {
                Transform segment = new GameObject("Segment").transform;
                segment.SetParent(null);
                segment.localPosition = new Vector3(0, 0, segmentLength * i);
                currentSegments[i] = segment;
                if (fishinglineParent == null)
                {
                    segment.SetParent(null);
                }
                segment.SetParent(null);

                if (colliders == true)
                {
                    SphereCollider sphereCol = segment.gameObject.AddComponent<SphereCollider>();
                    sphereCol.radius = (lineRend.startWidth - (widthDelta * i));
                    segment.gameObject.layer = 14;
                }
                if (i == numSegments - 1)
                {
                    attachPoint = segment;
                    if (parentedToEnd != null)
                    {
                        parentedToEnd.transform.SetParent(segment);
                        parentedToEnd.transform.localPosition = Vector3.zero;
                    }
                }
            }
        }

        private void CreateRefPoints()
        {
            float segmentLength = hitDistance / (numSegments - 1);
            // Segment 0 is closest to you, aka the snappiest/fastest moving
            for (int i = 0; i < numSegments; i++)
            {
                Transform refPoint = new GameObject("RefPoint").transform;
                refPoint.SetParent(transform);
                refPoint.localPosition = new Vector3(0, 0 * i, segmentLength * i);
                defaultPositions[i] = new Vector3(0, 0 * i, segmentLength * i);
                referencePoints[i] = refPoint;
            }
        }

        private void ConfigureLineRenderer()
        {
            lineRend.positionCount = numSegments + 1;
            lineRend.SetPosition(0, transform.position);
            for (int i = 0; i < numSegments; i++)
            {
                lineRend.SetPosition(i + 1, currentSegments[i].position);
            }
        }

        // Update is called once per frame
        private void Update()
        {
            OverlappingAction();
            ConfigureLineRenderer();
        }

        /// <summary>
        /// Adjusts the length of the fishingline by multiplying the default length by a scalar
        /// </summary>
        /// <param name="scalar">The current length of the fishingline == the defaultFishinglineLength * scalar</param>

        public void ScaleDistance(float scalar)
        {
            for (int i = 0; i < numSegments; i++)
            {
                referencePoints[i].localPosition = defaultPositions[i] * scalar;
            }
        }

        public void SetDistance(float distance)
        {
            float segmentLength = distance / (numSegments - 1);
            // Segment 0 is closest to you, aka the snappiest/fastest moving
            for (int i = 0; i < numSegments; i++)
            {
                referencePoints[i].localPosition = new Vector3(0, 0 * i, segmentLength * i);
            }
        }

        /// <summary>
        /// To snap the fishingline to the reference points (when the fishinline should snap to a distance) call this function
        /// </summary>
        public void SnapSegments()
        {
            for (int i = 0; i < numSegments; i++)
            {
                currentSegments[i].position = referencePoints[i].position;
            }
        }

        private void OverlappingAction()
        {
            for (int i = 1; i <= numSegments; i++)
            {
                // Starting the for loop at the last (furthest) item, and ending at the closest
                SegmentLerp(currentSegments[numSegments - i], referencePoints[numSegments - i], lerpSpeedMult * i);
            }
        }

        private void SegmentLerp(Transform segment, Transform refPt, float segmentLerpSpeed)
        {
            segment.position = Vector3.Lerp(segment.transform.position, refPt.position, Time.fixedDeltaTime * segmentLerpSpeed);
        }
    }
}