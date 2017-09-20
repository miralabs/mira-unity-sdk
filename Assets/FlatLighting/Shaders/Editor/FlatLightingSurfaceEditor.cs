/// Credit to Bogdan Gochev for licensing this shader code to us. 
/// Usage of the Flat Lighting Shader from this SDK is licensed for Mira applications only.
/// Flat lighting asset: https://www.assetstore.unity3d.com/en/#!/content/67730

using UnityEngine;
using UnityEditor;
using System;
using System.Collections;
using FlatLighting;

//Note: version 5.3.x of Unity could not manage a ShaderGUI class to be in other than the default namespace, sorry :(
public class FlatLightingSurfacerEditor : FlatLightingShaderEditor {

	protected override void ShaderPropertiesGUI() {
		ShowLightingProperties();
		GUILayout.Space (defaultSpace);
		ShowGlobalGradientProperties();
		GUILayout.Space (defaultSpace);
		shouldShowLightSourcesProperties = UITools.GroupHeader(new GUIContent(Labels.LightSourcesHeader), shouldShowLightSourcesProperties);
		if (shouldShowLightSourcesProperties) {
			ShowAmbientLightSettings();
		}
	}
}
