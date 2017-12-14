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

using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;

/// <summary>
/// This is a raycaster to be used with non-canvas-ui elements such as meshes in combination with MiraInputModule.
/// Simply create an event system, add MiraInputMoudle as a component above any other input module,
/// then attach this script to a game object. If you wish you could attach an Event Trigger as well and respond to click events.
/// MiraPhysicsRaycast is heavily based on Unity's PhysicsRaycaster
/// </summary>
public class MiraPhysicsRaycast : MiraBaseRaycaster
{
    /// <summary>
    /// Const to use for clarity when no event mask is set
    /// </summary>
    protected const int _NoEventMaskSet = -1;

    protected Camera _EventCamera;

    /// <summary>
    /// Layer mask used to filter events. Always combined with the camera's culling mask if a camera is used.
    /// </summary>
    [SerializeField]
    protected LayerMask _EventMask = _NoEventMaskSet;

    protected MiraPhysicsRaycast()
    { }

    public override Camera eventCamera
    {
        get
        {
            if (_EventCamera == null)
                _EventCamera = GetComponent<Camera>();
            return _EventCamera != null ? _EventCamera : Camera.main;
        }
    }

    /// <summary>
    /// Event mask used to determine which objects will receive events.
    /// </summary>
    public int finalEventMask
    {
        get { return (eventCamera != null) ? eventCamera.cullingMask & _EventMask : _NoEventMaskSet; }
    }

    /// <summary>
    /// Layer mask used to filter events. Always combined with the camera's culling mask if a camera is used.
    /// </summary>
    public LayerMask eventMask
    {
        get { return _EventMask; }
        set { _EventMask = value; }
    }

    public override void Raycast(PointerEventData eventData, List<RaycastResult> resultAppendList)
    {
        if (eventCamera == null)
        {
            Debug.LogError("MiraPhysicsRaycaster attempted to cast a ray without an eventCamera");
            return;
        }
        if (MiraController.Instance == null)
        {
            Debug.LogError("MiraPhysicsRaycaster attempted to cast a ray but a MiraController was not found in the scene");
            return;
        }

        // this is literally the only difference in this script and PhysicsRaycaster
        Ray ray = GetRay();

        float dist = eventCamera.farClipPlane - eventCamera.nearClipPlane;

        var hits = Physics.RaycastAll(ray, dist, finalEventMask);

        if (hits.Length > 1)
            System.Array.Sort(hits, (r1, r2) => r1.distance.CompareTo(r2.distance));

        if (hits.Length != 0)
        {
            for (int b = 0, bmax = hits.Length; b < bmax; ++b)
            {
                var result = new RaycastResult
                {
                    gameObject = hits[b].collider.gameObject,
                    module = this,
                    distance = hits[b].distance,
                    worldPosition = hits[b].point,
                    worldNormal = hits[b].normal,
                    screenPosition = eventData.position,
                    index = resultAppendList.Count,
                    sortingLayer = 0,
                    sortingOrder = 0
                };
                resultAppendList.Add(result);
            }
        }
    }
}