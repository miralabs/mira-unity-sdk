/// Credit to Bogdan Gochev for licensing this shader code to us. 
/// Usage of the Flat Lighting Shader from this SDK is licensed for Mira applications only.
/// Flat lighting asset: https://www.assetstore.unity3d.com/en/#!/content/67730

using UnityEngine;
using UnityEditor;
using FlatLighting;
using System.Collections;

public class FlatLightingTransparentShaderEditor : FlatLightingShaderEditor {

	private MaterialProperty alpha = null;

	protected override void FindProperties(MaterialProperty[] properties) {
		base.FindProperties(properties);
		alpha = FindProperty("_Alpha", properties);
	}

	protected override void ShaderPropertiesGUI() {
		ShowAlphaProperty();
		base.ShaderPropertiesGUI();
	}

	private void ShowAlphaProperty() {
		using (new UITools.GUIVertical(UITools.VGroupStyle)) {
			base.materialEditor.ShaderProperty(alpha, Labels.Alpha);
		}
	}
}
