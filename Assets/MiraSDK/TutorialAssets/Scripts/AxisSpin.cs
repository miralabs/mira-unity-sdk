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
/// Spin objects in a randomized direction
/// </summary>
public class AxisSpin : MonoBehaviour, IPointerEnterHandler
{
    [SerializeField]
    /// <summary>
    /// The speed of the object's spin
    /// </summary>
    public float spinRate = 5f;

    [SerializeField]
    private
    /// <summary>
    /// The direction of the spin
    /// </summary>
    Vector3 spinDirection;

    /// <summary>
    /// This function is called when the Mira Physics Raycast collides with this object
    /// </summary>
    /// <param name="eventData">Gives additional information regarding the pointer event</param>
    public void OnPointerEnter(PointerEventData eventData)
    {
        Debug.Log("Pointer has entered " + this.gameObject.name);
    }

    private void Start()
    {
        spinDirection = new Vector3(0.2f, 1, 0.2f);
    }

    private void Update()
    {
        transform.Rotate(spinDirection, spinRate);
    }

    /// <summary>
    /// Randomly sets the spin direction
    /// </summary>
    public void randomSpinDirection()
    {
        spinDirection = new Vector3(Random.Range(-1.0f, 1.0f), Random.Range(-1.0f, 1.0f), Random.Range(-1.0f, 1.0f));
    }
}