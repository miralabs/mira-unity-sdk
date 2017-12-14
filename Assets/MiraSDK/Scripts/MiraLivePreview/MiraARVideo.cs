// Copyright (c) Mira Labs, Inc., 2017. All rights reserved.
//
// Licensed under the MIT License.
// See LICENSE.md at the root of this project for more details.

using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MiraARVideo : MonoBehaviour {
	public Texture2D m_clearTexture;

	// Use this for initialization
	void Start() {
		Camera.main.targetTexture = null;
	}
	
	// Update is called once per frame
	void OnPostRender() {
		Graphics.Blit(m_clearTexture, null as RenderTexture);
	}
}
