// Original work: Copyright 2016 Google Inc. All rights reserved.
// Modified work: Copyright 2017 Mira Labs, Inc. All rights reserved.
//
// Licensed under the Apache License, Version 2.0 (the "License");
// you may not use this file except in compliance with the License.
// You may obtain a copy of the License at
//
//     http://www.apache.org/licenses/LICENSE-2.0
//
// Unless required by applicable law or agreed to in writing, software
// distributed under the License is distributed on an "AS IS" BASIS,
// WITHOUT WARRANTIES OR CONDITIONS OF ANY KIND, either express or implied.
// See the License for the specific language governing permissions and
// limitations under the License.

using UnityEngine;

/// <summary>
/// Clears the camera before final rendering of the distortion mesh
/// </summary>
public class MiraPreRender : MonoBehaviour
{
    #region Properties

    /// <summary>
    /// Reference to the distortion camera
    /// </summary>
    /// <returns></returns>
    public Camera cam { get; private set; }

    #endregion Properties

    #region Unity callbacks

    private void Awake()
    {
        cam = GetComponent<Camera>();
    }

    private void Reset()
    {
#if UNITY_EDITOR
        var cam = GetComponent<Camera>();
#endif
        cam.clearFlags = CameraClearFlags.SolidColor;
        cam.backgroundColor = Color.black;
        cam.cullingMask = 0;
        cam.useOcclusionCulling = false;
        cam.depth = -100;
    }

    private void OnPreCull()
    {
        cam.clearFlags = CameraClearFlags.SolidColor;
    }

    #endregion Unity callbacks
}