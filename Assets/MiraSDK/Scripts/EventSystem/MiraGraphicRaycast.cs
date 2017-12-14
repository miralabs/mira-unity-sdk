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
using UnityEngine.UI;

[RequireComponent(typeof(Canvas))]
/// <summary>
/// This is a raycaster to be used with canvas-ui elements in combination with MiraInputModule.
/// Simply create an event system, add MiraInputMoudle as a component above any other input module,
/// then attach this script to a UI canvas and all UI within the canvas will be interactive.
/// If you wish you could attach an Event Trigger to UI as well and respond to click events.
/// MiraGraphicRaycast is based on Unity's GraphicRaycaster.
/// </summary>
public class MiraGraphicRaycast : MiraBaseRaycaster
{
    // based on UnityEngineUI.GraphicRaycaster
    protected const int NoEventMaskSet = -1;

    public LayerMask blockingMask = NoEventMaskSet;
    public bool ignoreReversedGraphics = true;
    public GraphicRaycaster.BlockingObjects blockingObjects = GraphicRaycaster.BlockingObjects.None;

    private List<Graphic> raycastResults = new List<Graphic>();

    // this canvas could be public and assignable for flexiblity but as-is may be easier to use
    private Canvas _canvas;

    private Canvas canvas
    {
        get
        {
            if (_canvas != null)
            {
                return _canvas;
            }
            _canvas = GetComponent<Canvas>();
            if (canvas == null)
            {
                Debug.Log("MiraGraphicRaycaster must be attached to a game object containing a canvas");
            }
            return _canvas;
        }
    }

    public override Camera eventCamera
    {
        get
        {
            if (canvas.worldCamera == null)
            {
                Debug.Log("GameObject " + canvas.gameObject.name + " canvas component had no event camera, assigning automatically");
                canvas.worldCamera = Camera.main;
            }
            return canvas.worldCamera;
        }
    }

    public override void Raycast(PointerEventData eventData, List<RaycastResult> resultAppendList)
    {
        if (canvas == null)
        {
            Debug.LogError("MiraGraphicRayster attempted to cast a ray without a canvas");
            return;
        }
        if (eventCamera == null)
        {
            Debug.LogError("MiraGraphicRayster attempted to cast a ray without an eventCamera");
            return;
        }
        if (MiraController.Instance == null)
        {
            Debug.LogError("MiraGraphicRayster attempted to cast a ray but a MiraController was not found in the scene");
            return;
        }
        if (canvas.renderMode != RenderMode.WorldSpace)
        {
            Debug.LogError("MiraGraphicRayster requires the canvas renderMode set to WorldSpace");
        }

        // based on UnityEngineUI.GrahpicRaycaster 	public override void Raycast (PointerEventData eventData, List<RaycastResult> resultAppendList)
        float hitDistance = float.MaxValue;
        Ray ray = GetRay();

        if (blockingObjects != GraphicRaycaster.BlockingObjects.None)
        {
            float dist = eventCamera.farClipPlane - eventCamera.nearClipPlane;

            if (blockingObjects == GraphicRaycaster.BlockingObjects.ThreeD || blockingObjects == GraphicRaycaster.BlockingObjects.All)
            {
                RaycastHit hit;
                if (Physics.Raycast(ray, out hit, dist, blockingMask))
                {
                    hitDistance = hit.distance;
                }
            }
            if (blockingObjects == GraphicRaycaster.BlockingObjects.TwoD || blockingObjects == GraphicRaycaster.BlockingObjects.All)
            {
                RaycastHit2D raycastHit2D = Physics2D.Raycast(ray.origin, ray.direction, dist, blockingMask);
                if (raycastHit2D.collider != null)
                {
                    hitDistance = raycastHit2D.fraction * dist;
                }
            }
        }
        raycastResults.Clear();

        if (hitDistance > 10.0f)
        {
            hitDistance = 10.0f;
        }

        GraphicsRaycast(canvas, ray, hitDistance, eventCamera, raycastResults);

        // here we calculate a new ray from screen space based on the hit point from the world space ray
        // without doing this the collisions with world space UI don't look right
        // if you wish to see the difference replace references below to screenSpaceRay with ray
        Ray screenSpaceRay = eventCamera.ScreenPointToRay(eventCamera.WorldToScreenPoint(ray.GetPoint(hitDistance)));

        for (int i = 0; i < raycastResults.Count; i++)
        {
            GameObject go = raycastResults[i].gameObject;
            bool appendGraphic = true;

            if (ignoreReversedGraphics)
            {
                Vector3 camDir = eventCamera.transform.rotation * Vector3.forward;
                Vector3 goDir = go.transform.rotation * Vector3.forward;
                appendGraphic = (Vector3.Dot(camDir, goDir) > 0);
            }

            if (appendGraphic)
            {
                float dist = 0.0f;
                Transform transform = go.transform;
                Vector3 forward = transform.forward;
                dist = Vector3.Dot(forward, transform.position - screenSpaceRay.origin) / Vector3.Dot(forward, screenSpaceRay.direction);

                // behind cam
                if (dist < 0)
                {
                    continue;
                }

                if (dist < hitDistance)
                {
                    Vector3 hitWorldPos = screenSpaceRay.origin + (screenSpaceRay.direction * dist);

                    RaycastResult item = new RaycastResult
                    {
                        gameObject = go,
                        module = this,
                        distance = dist,
                        worldPosition = hitWorldPos,
                        screenPosition = eventCamera.WorldToScreenPoint(hitWorldPos),
                        index = (float)resultAppendList.Count,
                        depth = raycastResults[i].depth,
                        sortingLayer = canvas.sortingLayerID,
                        sortingOrder = canvas.sortingOrder
                    };
                    resultAppendList.Add(item);
                }
            }
        }
    }

    // based on UnityEngineUI.GrahpicRaycaster  private static void Raycast (Canvas canvas, Camera eventCamera, Vector2 pointerPosition, List<Graphic> results)
    // for internal use in graphicRaycast
    private static readonly List<Graphic> sortedGraphics = new List<Graphic>();

    private static void GraphicsRaycast(Canvas canvas, Ray ray, float hitDistance, Camera camera, List<Graphic> results)
    {
        IList<Graphic> graphicsForCanvas = GraphicRegistry.GetGraphicsForCanvas(canvas);
        for (int i = 0; i < graphicsForCanvas.Count; i++)
        {
            Graphic graphic = graphicsForCanvas[i];

            if (graphic.depth != -1 && !graphic.raycastTarget)
                continue;

            Vector3 screenPoint = camera.WorldToScreenPoint(ray.GetPoint(hitDistance));
            if (!RectTransformUtility.RectangleContainsScreenPoint(graphic.rectTransform, screenPoint, camera))
                continue;

            if (graphic.Raycast(screenPoint, camera))
            {
                sortedGraphics.Add(graphic);
            }
        }

        sortedGraphics.Sort((Graphic g1, Graphic g2) => g2.depth.CompareTo(g1.depth));
        for (int i = 0; i < sortedGraphics.Count; i++)
        {
            results.Add(sortedGraphics[i]);
        }
        sortedGraphics.Clear();
    }
}