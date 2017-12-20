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
/// An example of how to use MiraController and the event system to grab 3D objects with Unity's physics simulation system
/// </summary>
public class MiraPhysicsGrabExample : MonoBehaviour, IPointerDownHandler
{
    private bool isGrabbing = false;

    private RigidbodyConstraints originalConstraints;
    private Rigidbody rigidBody;

    private float lastTouchPosition;

    // these OnPointer functions are automatically called when
    // the pointer interacts with a game object that this script is attached to
    public void OnPointerDown(PointerEventData pointerData)
    {
        // onPointerDown is called every frame the pointer is held down on the object
        // we only want to grab objects if the click button was just pressed
        // this prevents multiple objects from unintentionally getting grabbed
        if (MiraController.ClickButtonPressed)
        {
            isGrabbing = true;
        }
    }

    // Use this for initialization
    private void Start()
    {
        rigidBody = this.gameObject.GetComponent<Rigidbody>();
        originalConstraints = rigidBody.constraints;
    }

    // Update is called once per frame
    private void Update()
    {
        // stop grabbing if the user isn't clicking
        if (isGrabbing == true && MiraController.ClickButton == false)
        {
            rigidBody.constraints = originalConstraints;
            isGrabbing = false;
        }

        if (isGrabbing == true)
        {
            // freeze the position of the physics simulation temporarily so the object doesn't
            // spiral out of control while its being interacted with
            // you could freeze the rotation as well if you wanted
            rigidBody.constraints = RigidbodyConstraints.FreezePosition;

            float touchInfluence = 0.0f;
            float thisTouch = 0.0f;
            if (MiraController.TouchHeld == true)
            {
                // MiraController.Touchpos.Y goes from 1 to 0 , near to far
                // we want to change this so the touchpad closer to the user returns negative values
                // and the upper half returns positive values
                thisTouch = MiraController.TouchPos.y;
                // now its 0.5 to -0.5
                thisTouch -= 0.5f;
                // now its -0.5 to 0.5
                thisTouch *= -1.0f;
                // scale it down so it's not too strong
                thisTouch *= 0.05f;

                touchInfluence = lastTouchPosition - thisTouch;
            }
            lastTouchPosition = thisTouch;

            // get the distance from this object to the controller
            float currentDistance = (MiraController.Position - transform.position).magnitude;
            // the new distance of the grabbed object is the current distance,
            // adjusted by the users touch, in the direction it was from the controller
			Vector3 newLength = MiraController.Direction.normalized * (currentDistance + touchInfluence);
            Vector3 newPosition = MiraController.Position + newLength;
            transform.position = newPosition;
        }
    }
}