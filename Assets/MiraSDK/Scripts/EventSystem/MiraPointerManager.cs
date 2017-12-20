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
using Mira;

/// <summary>
/// MiraPointerManager primarily provides the active pointer to MiraInputModule
/// normally only one pointer should be in the scene but if there are many MiraPointerManager
/// is used to set which one is primary.
/// </summary>
public class MiraPointerManager : MonoBehaviour
{
    // static singleton property
    private static MiraPointerManager _instance;

    public static MiraPointerManager Instance
    {
        get
        {
            if (_instance == null)
            {
                _instance = GameObject.FindObjectOfType<MiraPointerManager>();
            }

            if (_instance == null)
            {
                GameObject go = GameObject.FindObjectOfType<MiraArController>().gameObject;
                go.AddComponent<MiraPointerManager>();
                _instance = go.GetComponent<MiraPointerManager>();
            }
            return _instance;
        }
    }

    private void validateInstance()
    {
        if (_instance != null && _instance != this)
        {
            Debug.Log("There was multiple MiraPointerManager instances in your scene, destroying one");
            Destroy(this.gameObject);
        }
        else
        {
            _instance = this;
        }
    }

    private void Awake()
    {
        validateInstance();
    }

    private MiraBasePointer _pointer;

    public static MiraBasePointer Pointer
    {
        get
        {
            return Instance._pointer;
        }
        set
        {
            Instance._pointer = value;
        }
    }

	private GameObject _pointerGameObject;

	public static GameObject PointerGameObject
	{
		get
		{
			return Instance._pointerGameObject;
		}
		set
		{
			Instance._pointerGameObject = value;
		}
	}

    public void OnPointerCreated(GameObject pointerObject, MiraBasePointer newPointer)
    {
        validateInstance();
        _pointer = newPointer;
		_pointerGameObject = pointerObject;
    }
}