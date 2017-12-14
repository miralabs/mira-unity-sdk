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
using UnityEngine.EventSystems;

/// <summary>
/// A graphical representation of the Mira Physics and Graphics raycasts
/// The Mira reticle shifts distance
/// </summary>
public class MiraReticle : MonoBehaviour
{
    private MiraReticlePointer reticlepointer;

    // static singleton property
    private static MiraReticle _instance;

    /// <summary>
    /// Instance of Mira Reticle Singleton
    /// </summary>
    /// <returns></returns>
    public static MiraReticle Instance
    {
        get
        {
            if (_instance == null)
            {
                _instance = GameObject.FindObjectOfType<MiraReticle>();
            }
            return _instance;
        }
    }

    private void validateInstance()
    {
        if (_instance != null && _instance != this)
        {
            Debug.Log("There was multiple MiraReticle instances in your scene, destroying one");
            Destroy(this.gameObject);
        }
        else
        {
            _instance = this;
        }
    }

    private Transform cameraRig;

    [SerializeField]
    private bool onlyVisibleOnHover = false;

    // these are in M
    private float maxDistance = 3f;

    private float minDistance = 0.5f;

    private float minScale = 0.1f;
    private float maxScale = 0.25f;

    private float lastDistance;

    public float externalMultiplier = 1f;

    // public MiraLaserPointerLength laser;
    /// <summary>
    /// Color of the reticle when an object is being hovered over
    /// </summary>
    public Color reticleHoverColor = Color.green;

    /// <summary>
    /// Color of the reticle when nothing is being hovered over
    /// </summary>
    public Color reticleIdleColor = Color.white;

    private SpriteRenderer reticleRenderer;
    private Vector3 reticleOriginalScale;

    private void Awake()
    {
        validateInstance();
        reticlepointer = new MiraReticlePointer();

        
    }

    private void Start()
    {
        cameraRig = MiraArController.Instance.cameraRig.transform;

        reticleRenderer = GetComponent<SpriteRenderer>();
        reticleOriginalScale = this.transform.localScale;

        if (onlyVisibleOnHover)
            GetComponent<SpriteRenderer>().enabled = false;

        float scaleAdjust = (1 / MiraArController.scaleMultiplier) * 100.0f;
        minScale *= scaleAdjust;
        maxScale *= scaleAdjust;
        minDistance *= scaleAdjust;
        maxDistance *= scaleAdjust;

		reticlepointer.OnStart(this.gameObject);
        reticlepointer.maxDistance = maxDistance;


        reticleExit();
    }


    /// <summary>
    /// Un-hide the reticle
    /// </summary>
    public void showReticle()
    {
        GetComponent<SpriteRenderer>().enabled = true;
    }

    /// <summary>
    /// Hide the reticle
    /// </summary>
    public void hideReticle()
    {
        GetComponent<SpriteRenderer>().enabled = true;
    }

    private void reactToObject(bool isTargetInteractive)
    {
        if (isTargetInteractive)
        {
            reticleRenderer.color = reticleHoverColor;
        }
        else
        {
            reticleRenderer.color = reticleIdleColor;
        }
    }

    public void reticleExit()
    {
        reactToObject(false);
        setLength(maxDistance);
        lastDistance = maxDistance;
        hideReticle();
    }

    public void reticleHover(RaycastResult ray, bool isTargetInteractive)
    {
        reactToObject(isTargetInteractive);
        transform.LookAt(cameraRig.position);
        setLocation(ray.worldPosition);
        showReticle();
    }

    public void reticleEnter(RaycastResult ray, bool isTargetInteractive)
    {
        reactToObject(isTargetInteractive);
        transform.LookAt(cameraRig.position);
        setLocation(ray.worldPosition);
        showReticle();
    }

    private void setLength(float dist)
    {
        Vector3 pos = MiraController.Transform.forward.normalized;
        pos.Scale(new Vector3(dist, dist, dist));
        pos += MiraController.Transform.position;

        transform.position = pos;
        transform.LookAt(cameraRig.position);
    }

    private void setLocation(Vector3 worldLocation)
    {
        float dist = (worldLocation - MiraArController.Instance.transform.position).magnitude;
        lastDistance = dist;

        if (dist > maxDistance)
        {
            dist = maxDistance;
            setLength(dist);
        }
        else if (dist < minDistance)
        {
            dist = minDistance;
            setLength(dist);
        }

        transform.position = worldLocation;
    }

    private void setScale()
    {
        // normalize distance
        float step = (lastDistance - minDistance) / (maxDistance - minDistance);
        float scale = Mathf.Lerp(minScale, maxScale, step);
        transform.localScale = new Vector3(scale, scale, scale) * externalMultiplier;
    }

    private void LateUpdate()
    {
#if UNITY_EDITOR
        Debug.DrawLine(MiraController.Position, transform.position, Color.green);
#endif
        setScale();

        // if (laser != null) {
        // 	laser.MatchReticle (this.transform.position, lastDistance);
        // }
    }
}