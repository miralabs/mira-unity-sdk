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

using System.Collections;
using UnityEngine;

using UnityEngine.EventSystems;

using UnityEngine.UI;

public class MiraExampleInteractionScript : MonoBehaviour,
             IPointerEnterHandler, IPointerExitHandler, IPointerDownHandler, IPointerUpHandler, IPointerClickHandler
{
    public Text textBeneathPlanet;

    private float timeSinceLastInteraction;

    private Vector3 startingLocation;

    private bool isPlanetFeelingSatisfied;

    private bool isUserPointingAtPlanet = false;

    private AxisSpin spin;

    // these OnPointer functions are automatically called when
    // the pointer interacts with a game object that this script is attached to
    public void OnPointerEnter(PointerEventData pointerData)
    {
        isUserPointingAtPlanet = true;
        spin.spinRate = 5.0f;
    }

    public void OnPointerExit(PointerEventData pointerData)
    {
        isUserPointingAtPlanet = false;
        // slow the spin down again
        spin.spinRate = 1.0f;
    }

    public void OnPointerClick(PointerEventData pointerData)
    {
        // the planet is satsified and the timer is reset
        isPlanetFeelingSatisfied = true;
        timeSinceLastInteraction = 0.0f;
        spin.spinRate = 1.0f;
        StopCoroutine("temperTantrum");
    }

    public void OnPointerDown(PointerEventData pointerData)
    {
        this.transform.localScale = new Vector3(1.1f, 1.1f, 1.1f);
    }

    public void OnPointerUp(PointerEventData pointerData)
    {
        // return to the normal size
        this.transform.localScale = new Vector3(1.0f, 1.0f, 1.0f);
    }

    // Use this for initialization
    private void Start()
    {
        textBeneathPlanet.text = "Hi!";
        timeSinceLastInteraction = 0.0f;
        startingLocation = transform.position;
        isPlanetFeelingSatisfied = false;

        spin = this.gameObject.GetComponent<AxisSpin>();
    }

    // Update is called once per frame
    private void Update()
    {
        timeSinceLastInteraction += Time.deltaTime;

        if (isPlanetFeelingSatisfied == false)
        {
            // get impatient if ten seconds has gone by
            if (timeSinceLastInteraction > 10.0f)
            {
                textBeneathPlanet.text = "Click me!";

                // get really impatent every eight seconds afterwards
                if (timeSinceLastInteraction % 8 < 0.1 && isUserPointingAtPlanet == false)
                {
                    StartCoroutine("temperTantrum");
                }
            }
        }
        else
        {
            // someone finally clicked on the planet
            textBeneathPlanet.text = "Thank you!";
        }

        // urge them on if the planet is unsatisfied
        if (isUserPointingAtPlanet == true && isPlanetFeelingSatisfied == false)
        {
            textBeneathPlanet.text = "Go on and Click!";
        }

        // this planet doesn't stay satisified for very long
        if (isPlanetFeelingSatisfied == true && timeSinceLastInteraction > 6.0f)
        {
            textBeneathPlanet.text = "Hi!";
            isPlanetFeelingSatisfied = false;
            timeSinceLastInteraction = 0.0f;
        }
    }

    private IEnumerator temperTantrum()
    {
        // buzz around for five frames
        for (int i = 0; i < 10; i++)
        {
            // this planet is having a temper tantrum
            textBeneathPlanet.text = "!??!?!!";

            float distance = (transform.position - startingLocation).magnitude;
            // buzz around randomly but don't get too far from the starting point
            if (distance < 1.0f)
            {
                transform.position += new Vector3(Random.Range(-0.025f, 0.025f), Random.Range(-0.025f, 0.025f), Random.Range(-0.025f, 0.025f));
            }
            yield return null;
        }

        // slowly go back to the starting point
        for (int i = 0; i < 60; i++)
        {
            transform.position = Vector3.Lerp(transform.position, startingLocation, i / 60.0f);
            yield return null;
        }
    }
}