/// Credit to Bogdan Gochev for licensing this shader code to us. 
/// Usage of the Flat Lighting Shader from this SDK is licensed for Mira applications only.
/// Flat lighting asset: https://www.assetstore.unity3d.com/en/#!/content/67730

using UnityEngine;
using UnityEditor;
using FlatLighting;
using System.Collections;

public class FlatLightingWaterSurfaceShaderEditor : FlatLightingShaderEditor {

	private MaterialProperty waveLength = null;
	private MaterialProperty waveHeight = null;
	private MaterialProperty waveSpeed = null;
	private MaterialProperty waveRandomHeight = null;
	private MaterialProperty waveRandomSpeed = null;
	private MaterialProperty alpha = null;
	private MaterialProperty celThreshold = null;

	protected override void FindProperties(MaterialProperty[] properties) {
		base.FindProperties(properties);
		waveLength = FindProperty("_WaveLength", properties);
		waveHeight = FindProperty("_WaveHeight", properties);
		waveSpeed = FindProperty("_WaveSpeed", properties);
		waveRandomHeight = FindProperty("_RandomHeight", properties);
		waveRandomSpeed = FindProperty("_RandomSpeed", properties);
		alpha = FindProperty("_Alpha", properties);
		celThreshold = FindProperty("_CelThreshold", properties);
	}

	protected override void ShaderPropertiesGUI() {
		ShowWaveSettings();
		ShowCelThresholdProperty();
		base.ShaderPropertiesGUI();
	}

	private void ShowWaveSettings() {
		using (new UITools.GUIVertical(UITools.VGroupStyle)) {
			UITools.Header(Labels.Wave);
			base.materialEditor.ShaderProperty(waveLength, waveLength.displayName);
			base.materialEditor.ShaderProperty(waveHeight, waveHeight.displayName);
			base.materialEditor.ShaderProperty(waveSpeed, waveSpeed.displayName);
			base.materialEditor.ShaderProperty(waveRandomHeight, waveRandomHeight.displayName);
			base.materialEditor.ShaderProperty(waveRandomSpeed, waveRandomSpeed.displayName);
			UITools.DrawSeparatorThinLine();
			base.materialEditor.ShaderProperty(alpha, alpha.displayName);
		}
	}

	private void ShowCelThresholdProperty() {
		using (new UITools.GUIVertical(UITools.VGroupStyle)) {
			materialEditor.ShaderProperty(celThreshold, Labels.CelThreshold);
		}
	}
}
