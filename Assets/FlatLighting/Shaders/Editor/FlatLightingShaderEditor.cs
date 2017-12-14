/// Credit to Bogdan Gochev for licensing this shader code to us. 
/// Usage of the Flat Lighting Shader from this SDK is licensed for Mira applications only.
/// Flat lighting asset: https://www.assetstore.unity3d.com/en/#!/content/67730

using UnityEngine;
using UnityEditor;
using System;
using System.Collections;
using FlatLighting;

//Note: version 5.3.x of Unity could not manage a ShaderGUI class to be in other than the default namespace, sorry :(
public class FlatLightingShaderEditor : ShaderGUI {

	private MaterialProperty lightmapUVChannel = null;
	private MaterialProperty lightMapTexture = null;
	private MaterialProperty shadowTint = null;
	private MaterialProperty shadowBoost = null;
	private MaterialProperty lightNegativeX = null;
	private MaterialProperty lightPositiveX = null;
	private MaterialProperty lightNegativeZ = null;
	private MaterialProperty lightPositiveZ = null;
	private MaterialProperty lightNegativeY = null;
	private MaterialProperty lightPositiveY = null;

	private MaterialProperty lightPositive2X = null;
	private MaterialProperty gradientOriginOffsetPositiveX = null;
	private MaterialProperty gradientWidthPositiveX = null;
	private MaterialProperty lightNegative2X = null;
	private MaterialProperty gradientOriginOffsetNegativeX = null;
	private MaterialProperty gradientWidthNegativeX = null;
	private MaterialProperty lightPositive2Y = null;
	private MaterialProperty gradientOriginOffsetPositiveY = null;
	private MaterialProperty gradientWidthPositiveY = null;
	private MaterialProperty lightNegative2Y = null;
	private MaterialProperty gradientOriginOffsetNegativeY = null;
	private MaterialProperty gradientWidthNegativeY = null;
	private MaterialProperty lightPositive2Z = null;
	private MaterialProperty gradientOriginOffsetPositiveZ = null;
	private MaterialProperty gradientWidthPositiveZ = null;
	private MaterialProperty lightNegative2Z = null;
	private MaterialProperty gradientOriginOffsetNegativeZ = null;
	private MaterialProperty gradientWidthNegativeZ = null;

	private MaterialProperty mainTexture = null;
	private MaterialProperty ambientLight = null;
	private MaterialProperty blendedLightColor = null;
	private MaterialProperty blendedLightIntensities = null;
	private MaterialProperty blendedLightSmoothness = null;
	private MaterialProperty gradientGoalColor = null;
	private MaterialProperty gradientUnitAxis = null;
	private MaterialProperty gradientWidth = null;
	private MaterialProperty gradientOffset = null;
	private MaterialProperty gradientBlending = null;

	protected MaterialEditor materialEditor;
	protected Material material;

	private Space selectedAxisSpace = Space.Local;
	private ValueWrapper<bool> useThreeLightSystem = false;
	private ValueWrapper<bool> useGradientLightXColors = false;
	private ValueWrapper<bool> useGradientLightYColors = false;
	private ValueWrapper<bool> useGradientLightZColors = false;
	private ValueWrapper<bool> useVertexColors = true;

	private enum LightSourcesBlend {
		Individual = 0,
		Multiple = 1
	}

	private LightSourcesBlend selectedLightSourcesBlend = LightSourcesBlend.Individual;
	private Vector4 blendedLightIntensitiesValues;
	private ValueWrapper<bool> useAmbientLight = true;
	private ValueWrapper<bool> useDirectLight = true;
	private ValueWrapper<bool> useSpotLight = false;
	private ValueWrapper<bool> usePointLight = false;

	private enum GradientAxis {
		X = 0,
		Y = 1,
		Z = 2,
		Free = 3
	}

	private enum Space {
		Local = 0,
		World = 1
	}

	private ValueWrapper<bool> useGlobalGradient = false;
	private Space selectedGradientSpace = Space.Local;
	private GradientAxis gradientAxisType = GradientAxis.X;
	private bool isGradientAxisTypeFree = false;
	private Vector3 gradientAxis = Vector3.up;
	private static readonly Vector4 gradientAxisX = new Vector4(1, 0, 0, 0);
	private static readonly Vector4 gradientAxisY = new Vector4(0, 1, 0, 0);
	private static readonly Vector4 gradientAxisZ = new Vector4(0, 0, 1, 0);

	private ValueWrapper<bool> useCustomLightmapping = false;
	private ValueWrapper<bool> useUnityLightmapping = true;
	private int[] uvChannels = {0, 1};
	private int selectedUVChannel = 0;

	private ValueWrapper<bool> receiveCustomShadows = false;
	private ValueWrapper<bool> castCustomShadows = true;

	protected bool shouldShowLightingProperties = true;
	protected bool shouldShowLightSourcesProperties = false;
	protected bool shouldShowGradientProperties = false;
	protected bool shouldShowLightmapProperties = false;
	protected bool shouldShowCustomShadowProperties = false;

	protected const float defaultSpace = 5.0f;

	private bool isFirstInvocation = true;

	protected virtual void FindProperties (MaterialProperty[] props) {
		lightNegativeX = FindProperty("_LightNegativeX", props);
		lightPositiveX = FindProperty("_LightPositiveX", props);
		lightNegativeZ = FindProperty("_LightNegativeZ", props);
		lightPositiveZ = FindProperty("_LightPositiveZ", props);
		lightNegativeY = FindProperty("_LightNegativeY", props);
		lightPositiveY = FindProperty("_LightPositiveY", props);
		lightPositive2X = FindProperty ("_LightPositive2X", props);
		gradientOriginOffsetPositiveX = FindProperty("_GradientOriginOffsetPositiveX", props);
		gradientWidthPositiveX = FindProperty ("_GradientWidthPositiveX", props);

		lightNegative2X = FindProperty ("_LightNegative2X", props);
		gradientOriginOffsetNegativeX = FindProperty("_GradientOriginOffsetNegativeX", props);
		gradientWidthNegativeX = FindProperty ("_GradientWidthNegativeX", props);

		lightPositive2Y = FindProperty ("_LightPositive2Y", props);
		gradientOriginOffsetPositiveY = FindProperty("_GradientOriginOffsetPositiveY", props);
		gradientWidthPositiveY = FindProperty ("_GradientWidthPositiveY", props);

		lightNegative2Y = FindProperty ("_LightNegative2Y", props);
		gradientOriginOffsetNegativeY = FindProperty("_GradientOriginOffsetNegativeY", props);
		gradientWidthNegativeY = FindProperty ("_GradientWidthNegativeY", props);

		lightPositive2Z = FindProperty ("_LightPositive2Z", props);
		gradientOriginOffsetPositiveZ = FindProperty("_GradientOriginOffsetPositiveZ", props);
		gradientWidthPositiveZ = FindProperty ("_GradientWidthPositiveZ", props);

		lightNegative2Z = FindProperty ("_LightNegative2Z", props);
		gradientOriginOffsetNegativeZ = FindProperty("_GradientOriginOffsetNegativeZ", props);
		gradientWidthNegativeZ = FindProperty ("_GradientWidthNegativeZ", props);

		mainTexture = FindProperty("_MainTex", props);
		ambientLight = FindProperty("_Ambient_Light", props);
		blendedLightColor = FindProperty("_BlendedLightColor", props, false);
		blendedLightIntensities = FindProperty("_BlendedLightIntensities", props, false);
		blendedLightSmoothness = FindProperty("_BlendedLightSmoothness", props, false);
		gradientGoalColor = FindProperty("_GradienColorGoal", props);
		gradientBlending = FindProperty("_GradientBlending", props);
		gradientUnitAxis = FindProperty("_GradientUnitAxis",props);
		gradientWidth = FindProperty("_GradientWidth", props);
		gradientOffset = FindProperty("_GradientOffset", props);
		lightMapTexture = FindProperty("_CustomLightmap", props);
		shadowTint = FindProperty("_ShadowTint", props);
		shadowBoost = FindProperty("_ShadowBoost", props);
		lightmapUVChannel = FindProperty("_UVChannel", props);
	}

	public override void OnGUI(MaterialEditor matEditor, MaterialProperty[] properties) {
		FindProperties(properties);
		materialEditor = matEditor;
		material = materialEditor.target as Material;


		InitMaterial();
		if (isFirstInvocation)
		{ 
			isFirstInvocation = false;
			StartGUI();
		}
		ShaderPropertiesGUI();
		ShowBakeShaderButton();
	}

	protected void InitMaterial() {
		selectedAxisSpace = isKeywordEnabled(Constants.Shader.AXIS_COLORS_LOCAL) ? Space.Local : Space.World;
		useThreeLightSystem = isKeywordEnabled(Constants.Shader.SYMETRIC_COLORS_ON_KEYWORD);
		useGradientLightXColors = isKeywordEnabled(Constants.Shader.AXIS_GRADIENT_ON_X_KEYWORD);
		useGradientLightYColors = isKeywordEnabled(Constants.Shader.AXIS_GRADIENT_ON_Y_KEYWORD);
		useGradientLightZColors = isKeywordEnabled(Constants.Shader.AXIS_GRADIENT_ON_Z_KEYWORD);
		useVertexColors = isKeywordEnabled(Constants.Shader.VERTEX_COLOR_KEYWORD);
		useAmbientLight = isKeywordEnabled(Constants.Shader.AMBIENT_LIGHT_KEYWORD);
		useDirectLight = isKeywordEnabled(Constants.Shader.DIRECT_LIGHT_KEYWORD);
		useSpotLight = isKeywordEnabled(Constants.Shader.SPOT_LIGHT_KEYWORD);
		usePointLight = isKeywordEnabled(Constants.Shader.POINT_LIGHT_KEYWORD);
		selectedLightSourcesBlend = isKeywordEnabled(Constants.Shader.BLEND_LIGHT_SOURCES_KEYWORD) ? LightSourcesBlend.Multiple : LightSourcesBlend.Individual;
		useGlobalGradient = isKeywordEnabled(Constants.Shader.GRADIENT_LOCAL_KEYWORD) || isKeywordEnabled(Constants.Shader.GRADIENT_WORLD_KEYWORD);
		selectedGradientSpace =isKeywordEnabled(Constants.Shader.GRADIENT_LOCAL_KEYWORD) ? Space.Local : Space.World;
		useCustomLightmapping = isKeywordEnabled(Constants.Shader.CUSTOM_LIGHTMAPPING_KEYWORD);
		useUnityLightmapping = isKeywordEnabled(Constants.Shader.UNITY_LIGHTMAPPING_KEYWORD);
		receiveCustomShadows = isKeywordEnabled(Constants.Shader.RECEIVE_CUSTOM_SHADOW_KEYWORD);
		castCustomShadows = isKeywordEnabled(Constants.Shader.CAST_CUSTOM_SHADOW_ON_KEYWORD);

		if (useCustomLightmapping || useUnityLightmapping) {
			selectedUVChannel = (int)lightmapUVChannel.floatValue;
		}
	}

	private bool isKeywordEnabled(string keyword) {
		return material.IsKeywordEnabled(keyword) || IsShaderBaked(material) && ShaderHasKeyword(keyword);
	}

	protected virtual void StartGUI() {
		InitShaderPropertiesGUI ();
	}

	private void InitShaderPropertiesGUI() {
		shouldShowLightSourcesProperties = (useAmbientLight || useDirectLight || useSpotLight || usePointLight) || shouldShowLightSourcesProperties;
		shouldShowGradientProperties = (isKeywordEnabled(Constants.Shader.GRADIENT_LOCAL_KEYWORD) || isKeywordEnabled(Constants.Shader.GRADIENT_WORLD_KEYWORD)) || shouldShowGradientProperties;
		shouldShowLightmapProperties = (useCustomLightmapping || useUnityLightmapping) || shouldShowLightmapProperties;
		shouldShowCustomShadowProperties = (receiveCustomShadows || castCustomShadows) || shouldShowCustomShadowProperties;
	}

	private void ShowBakeShaderButton() {
		UITools.DrawSeparatorLine();
		if (IsShaderBaked(material)) {
			EditorGUILayout.HelpBox("This is a baked shader, so you can't turn on/off features.",MessageType.Warning, true);
		} else {			
			if (GUILayout.Button("Bake Shader")) {
				ShaderBaker.ShowWindow(material);
			}
			EditorGUILayout.HelpBox("Creates a new shader that only includes the used options. Use this when you have problems compilling the big shader on mobile.",MessageType.Info, true);
		}
	}
		
	protected virtual void ShaderPropertiesGUI() {
		ShowLightingProperties();
		GUILayout.Space (defaultSpace);
		ShowGlobalGradientProperties();
		GUILayout.Space (defaultSpace);
		ShowLightmapProperties();
		GUILayout.Space (defaultSpace);
		ShowLightSources();
		GUILayout.Space (defaultSpace);
		ShowCustomShadowsProperties();
	}

	protected void ShowLightingProperties() {
		shouldShowLightingProperties = UITools.GroupHeader(new GUIContent(Labels.LightingHeader), shouldShowLightingProperties);
		if (shouldShowLightingProperties) {
			using (new UITools.GUIVertical(UITools.VGroupStyle)) {
				ShowVertexColorOption();
				UITools.DrawSeparatorThinLine();
				using (new UITools.GUIDisable(IsShaderBaked(material))) {
					UITools.ToggleWithLabel(useThreeLightSystem, Labels.UseSymmetrcialColors);
				}

				GUILayout.Space(defaultSpace);
				using (new UITools.GUIDisable(IsShaderBaked(material))) {
					selectedAxisSpace = (Space)EditorGUILayout.Popup(Labels.Space, (int)selectedAxisSpace, Labels.SpaceEnumLabels);
				}
				if (selectedAxisSpace == Space.Local) {
					material.EnableKeyword(Constants.Shader.AXIS_COLORS_LOCAL);
					material.DisableKeyword(Constants.Shader.AXIS_COLORS_GLOBAL);
				} else {
					material.EnableKeyword(Constants.Shader.AXIS_COLORS_GLOBAL);
					material.DisableKeyword(Constants.Shader.AXIS_COLORS_LOCAL);
				}

				GUILayout.Space(defaultSpace);

				GUILayout.Space(defaultSpace);
				if (useThreeLightSystem) {
					UseSymmetricalLightSystem();
				} else {
					UseNonSymmetricalLightSystem();
				}

				UITools.DrawSeparatorThinLine();
				Texture settedTexture = materialEditor.TextureProperty (mainTexture, Labels.MainTextureLabel, true);
				if (settedTexture != null)
				{
					material.EnableKeyword (Constants.Shader.USE_MAIN_TEXTURE_KEYWORD);
				} else
				{
					material.DisableKeyword (Constants.Shader.USE_MAIN_TEXTURE_KEYWORD);
				}
			}
		}
	}

	private void ShowVertexColorOption() {
		using (new UITools.GUIDisable(IsShaderBaked(material))) {
			UITools.ToggleWithLabel(useVertexColors, Labels.VertexColor);
		}

		if (useVertexColors) {
			material.EnableKeyword(Constants.Shader.VERTEX_COLOR_KEYWORD);
		} else {
			material.DisableKeyword(Constants.Shader.VERTEX_COLOR_KEYWORD);
		}
	}

	private void ShowGradientColorXAxisOption() {
		using (new UITools.GUIDisable(IsShaderBaked(material))) {
			UITools.ToggleWithLabel(useGradientLightXColors, Labels.UseAxisGradientXColors);
		}
		UITools.DrawSeparatorThinLine();

		if (useGradientLightXColors) {
			material.EnableKeyword(Constants.Shader.AXIS_GRADIENT_ON_X_KEYWORD);
		} else {
			material.DisableKeyword(Constants.Shader.AXIS_GRADIENT_ON_X_KEYWORD);
		}
	}

	private void ShowGradientColorYAxisOption() {
		using (new UITools.GUIDisable(IsShaderBaked(material))) {
			UITools.ToggleWithLabel(useGradientLightYColors, Labels.UseAxisGradientYColors);
		}
		UITools.DrawSeparatorThinLine();

		if (useGradientLightYColors) {
			material.EnableKeyword(Constants.Shader.AXIS_GRADIENT_ON_Y_KEYWORD);
		} else {
			material.DisableKeyword(Constants.Shader.AXIS_GRADIENT_ON_Y_KEYWORD);
		}
	}

	private void ShowGradientColorZAxisOption() {
		using (new UITools.GUIDisable(IsShaderBaked(material))) {
			UITools.ToggleWithLabel(useGradientLightZColors, Labels.UseAxisGradientZColors);
		}
		UITools.DrawSeparatorThinLine();

		if (useGradientLightZColors) {
			material.EnableKeyword(Constants.Shader.AXIS_GRADIENT_ON_Z_KEYWORD);
		} else {
			material.DisableKeyword(Constants.Shader.AXIS_GRADIENT_ON_Z_KEYWORD);
		}
	}

	private void ShowAxisLightColor(
		MaterialProperty axisColorProperty,	
		MaterialProperty axisColor2Property, 
		MaterialProperty axisGradientOriginOffsetProperty,
		MaterialProperty axisGradientWidthProperty,
		ValueWrapper<bool> isGradientOnAxis,
		string label) {

		if (isGradientOnAxis) {
			materialEditor.ShaderProperty(axisColorProperty, label);
			materialEditor.ShaderProperty(axisColor2Property, Labels.LightSecondColor);
			materialEditor.ShaderProperty(axisGradientOriginOffsetProperty, Labels.LightAxisGradientOffset);
			materialEditor.ShaderProperty(axisGradientWidthProperty, Labels.LightAxisGradientWidth);
			GUILayout.Space (defaultSpace);
		} else {
			materialEditor.ShaderProperty(axisColorProperty, label);
		}
	}

	private void UseSymmetricalLightSystem() {
		material.EnableKeyword(Constants.Shader.SYMETRIC_COLORS_ON_KEYWORD);
		material.DisableKeyword(Constants.Shader.SYMETRIC_COLORS_OFF_KEYWORD);

		ShowGradientColorXAxisOption();
		ShowAxisLightColor(lightPositiveX, lightPositive2X, gradientOriginOffsetPositiveX, gradientWidthPositiveX, useGradientLightXColors,
			Labels.LightPositiveX);
		GUILayout.Space(defaultSpace);

		ShowGradientColorYAxisOption();
		ShowAxisLightColor(lightPositiveY, lightPositive2Y, gradientOriginOffsetPositiveY, gradientWidthPositiveY, useGradientLightYColors,
			Labels.LightPositiveY);	
		GUILayout.Space(defaultSpace);

		ShowGradientColorZAxisOption();
		ShowAxisLightColor(lightPositiveZ, lightPositive2Z, gradientOriginOffsetPositiveZ, gradientWidthPositiveZ, useGradientLightZColors,
			Labels.LightPositiveZ);
		GUILayout.Space(defaultSpace);
	}

	private void UseNonSymmetricalLightSystem() {
		material.EnableKeyword(Constants.Shader.SYMETRIC_COLORS_OFF_KEYWORD);
		material.DisableKeyword(Constants.Shader.SYMETRIC_COLORS_ON_KEYWORD);

		ShowGradientColorXAxisOption();
		ShowAxisLightColor(lightPositiveX, lightPositive2X, gradientOriginOffsetPositiveX, gradientWidthPositiveX, useGradientLightXColors,
			Labels.LightPositiveX);
		ShowAxisLightColor(lightNegativeX, lightNegative2X, gradientOriginOffsetNegativeX, gradientWidthNegativeX, useGradientLightXColors,
			Labels.LightNegativeX);
		UITools.DrawSeparatorThinLine();
		GUILayout.Space(defaultSpace);

		ShowGradientColorYAxisOption();
		ShowAxisLightColor(lightPositiveY, lightPositive2Y, gradientOriginOffsetPositiveY, gradientWidthPositiveY, useGradientLightYColors,
			Labels.LightPositiveY);
		ShowAxisLightColor(lightNegativeY, lightNegative2Y, gradientOriginOffsetNegativeY, gradientWidthNegativeY, useGradientLightYColors,
			Labels.LightNegativeY);
		UITools.DrawSeparatorThinLine();
		GUILayout.Space(defaultSpace);

		ShowGradientColorZAxisOption();
		ShowAxisLightColor(lightPositiveZ, lightPositive2Z, gradientOriginOffsetPositiveZ, gradientWidthPositiveZ, useGradientLightZColors,
			Labels.LightPositiveZ);
		ShowAxisLightColor(lightNegativeZ, lightNegative2Z, gradientOriginOffsetNegativeZ, gradientWidthNegativeZ, useGradientLightZColors,
			Labels.LightNegativeZ);
		UITools.DrawSeparatorThinLine();
		GUILayout.Space(defaultSpace);
	}

	protected void ShowLightSources() {
		shouldShowLightSourcesProperties = UITools.GroupHeader(new GUIContent(Labels.LightSourcesHeader), shouldShowLightSourcesProperties);
		if (shouldShowLightSourcesProperties) {
			ShowAmbientLightSettings();
			ShowLightSourcesToggles();
		}
	}

	private void ShowLightSourcesToggles() {
		using (new UITools.GUIVertical(UITools.VGroupStyle)) {
			ShowDirectLightStettings();
			GUILayout.Space(defaultSpace);
			ShowSpotLightSettings();
			GUILayout.Space(defaultSpace);
			ShowPointLightSettings();

			if (usePointLight || useSpotLight) {
				GUILayout.Space(defaultSpace);
				UITools.DrawSeparatorThinLine();
				ShowLightSourcesBlendedProperties();
			}

			if (useDirectLight || usePointLight || useSpotLight) {
				GUILayout.Space(defaultSpace);
				EditorGUILayout.HelpBox(Labels.HelpUseFlatLightingLightSources, MessageType.Info, true);
			}
		}
	}

	private void ShowLightSourcesBlendedProperties() {
		using (new UITools.GUIDisable(IsShaderBaked(material))) {
			selectedLightSourcesBlend = (LightSourcesBlend)EditorGUILayout.Popup(Labels.LightSourceBlend, (int)selectedLightSourcesBlend, Labels.LightSourceBlendEnumLabels);
		}
		if (selectedLightSourcesBlend == LightSourcesBlend.Multiple) {
			material.EnableKeyword(Constants.Shader.BLEND_LIGHT_SOURCES_KEYWORD);
			materialEditor.ShaderProperty(blendedLightColor, Labels.BlendedLightColor);
			EditorGUILayout.LabelField(Labels.BlendedLightIntensities);
			int originalIndentLevel = EditorGUI.indentLevel;
			EditorGUI.indentLevel += 1;
			blendedLightIntensitiesValues = blendedLightIntensities.vectorValue;
			EditorGUI.BeginChangeCheck();
			blendedLightIntensitiesValues.x = EditorGUILayout.Slider(Labels.BlendedLightIntensities0, blendedLightIntensitiesValues.x, -1.0f, 1.0f);
			blendedLightIntensitiesValues.y = EditorGUILayout.Slider(Labels.BlendedLightIntensities1, blendedLightIntensitiesValues.y, -1.0f, 1.0f);
			blendedLightIntensitiesValues.z = EditorGUILayout.Slider(Labels.BlendedLightIntensities2, blendedLightIntensitiesValues.z, -1.0f, 1.0f);
			if (EditorGUI.EndChangeCheck()) {
				blendedLightIntensities.vectorValue = blendedLightIntensitiesValues;
			}
			EditorGUI.indentLevel = originalIndentLevel;


			materialEditor.ShaderProperty(blendedLightSmoothness, Labels.BlendedLightSmoothness);

		} else {
			material.DisableKeyword(Constants.Shader.BLEND_LIGHT_SOURCES_KEYWORD);
		}
	}

	protected void ShowAmbientLightSettings () {
		using (new UITools.GUIVertical(UITools.VGroupStyle)) {

			using (new UITools.GUIDisable(IsShaderBaked(material))) {
				UITools.ToggleHeader(useAmbientLight, Labels.AmbientLight);
			}

			if (useAmbientLight) {
				GUILayout.Space(defaultSpace);
				material.EnableKeyword(Constants.Shader.AMBIENT_LIGHT_KEYWORD);
				materialEditor.ShaderProperty(ambientLight, Labels.EmptyText);
			} else {
				material.DisableKeyword(Constants.Shader.AMBIENT_LIGHT_KEYWORD);
			}
		}
	}

	private void ShowDirectLightStettings () {
		using (new UITools.GUIDisable(IsShaderBaked(material))) {
			UITools.ToggleHeader(useDirectLight, Labels.DirectLight);
		}

		if (useDirectLight) {
			material.EnableKeyword(Constants.Shader.DIRECT_LIGHT_KEYWORD);
		} else {
			material.DisableKeyword(Constants.Shader.DIRECT_LIGHT_KEYWORD);
		}
	}

	private void ShowPointLightSettings() {
		using (new UITools.GUIDisable(IsShaderBaked(material))) {
			UITools.ToggleHeader(usePointLight, Labels.PointLight);
		}

		if (usePointLight) {
			material.EnableKeyword(Constants.Shader.POINT_LIGHT_KEYWORD);
		} else {
			material.DisableKeyword(Constants.Shader.POINT_LIGHT_KEYWORD);
		}
	}

	private void ShowSpotLightSettings() {
		using (new UITools.GUIDisable(IsShaderBaked(material))) {
			UITools.ToggleHeader(useSpotLight, Labels.SpotLight);
		}

		if (useSpotLight) {
			material.EnableKeyword(Constants.Shader.SPOT_LIGHT_KEYWORD);
		} else {
			material.DisableKeyword(Constants.Shader.SPOT_LIGHT_KEYWORD);
		}
	}

	protected void ShowLightmapProperties() {
		shouldShowLightmapProperties = UITools.GroupHeader(new GUIContent(Labels.LightmapHeader), shouldShowLightmapProperties);
		if (shouldShowLightmapProperties) {
			using (new UITools.GUIVertical(UITools.VGroupStyle)) {

				using (new UITools.GUIDisable(IsShaderBaked(material))) {
					UITools.ToggleHeader(useCustomLightmapping, useUnityLightmapping, Labels.UseLightmap, Labels.CustomLightmap, Labels.UnityLightmap);
				}

				if (useCustomLightmapping) {
					material.EnableKeyword(Constants.Shader.CUSTOM_LIGHTMAPPING_KEYWORD);
					material.DisableKeyword(Constants.Shader.UNITY_LIGHTMAPPING_KEYWORD);

					UITools.DrawSeparatorThinLine ();
					GUILayout.Space(defaultSpace);
					ShowLightmapUVSelector();
					materialEditor.ShaderProperty(lightMapTexture, Labels.EmptyText);
					materialEditor.ShaderProperty(shadowTint, Labels.ShadowTint);
					materialEditor.ShaderProperty(shadowBoost, Labels.ShadowBoost);
				} else if(useUnityLightmapping) {
					
					UITools.DrawSeparatorThinLine ();
					GUILayout.Space(defaultSpace);
					material.DisableKeyword(Constants.Shader.CUSTOM_LIGHTMAPPING_KEYWORD);
					material.EnableKeyword(Constants.Shader.UNITY_LIGHTMAPPING_KEYWORD);
					ShowLightmapUVSelector();

					EditorGUILayout.HelpBox("Using Unity Baked Lightmaps to apply GI.",MessageType.Info, true);
				} else {
					material.DisableKeyword(Constants.Shader.CUSTOM_LIGHTMAPPING_KEYWORD);
					material.DisableKeyword(Constants.Shader.UNITY_LIGHTMAPPING_KEYWORD);
				}
			}
		}
	}

	private void ShowLightmapUVSelector() {
		EditorGUI.BeginChangeCheck ();
		selectedUVChannel = EditorGUILayout.IntPopup(Labels.LightmapUVChannelsLabel, selectedUVChannel, Labels.LightmapUVChannels, uvChannels);
		if (EditorGUI.EndChangeCheck()) {
			lightmapUVChannel.floatValue = selectedUVChannel;
		}
	}

	protected void ShowGlobalGradientProperties() {
		shouldShowGradientProperties = UITools.GroupHeader(new GUIContent(Labels.GlobalGradientHeader), shouldShowGradientProperties);
		if (shouldShowGradientProperties) {
			using (new UITools.GUIVertical(UITools.VGroupStyle)) {

				using (new UITools.GUIDisable(IsShaderBaked(material))) {
					UITools.ToggleHeader(useGlobalGradient, Labels.UseGlobalGradient);
				}

				if (useGlobalGradient) {
					UITools.DrawSeparatorThinLine ();
					GUILayout.Space(defaultSpace);
					using (new UITools.GUIDisable(IsShaderBaked(material))) {
						selectedGradientSpace = (Space)EditorGUILayout.Popup(Labels.Space, (int)selectedGradientSpace, Labels.SpaceEnumLabels);
					}
					if (selectedGradientSpace == Space.Local) {
						material.EnableKeyword(Constants.Shader.GRADIENT_LOCAL_KEYWORD);
						material.DisableKeyword(Constants.Shader.GRADIENT_WORLD_KEYWORD);
					} else {
						material.DisableKeyword(Constants.Shader.GRADIENT_LOCAL_KEYWORD);
						material.EnableKeyword(Constants.Shader.GRADIENT_WORLD_KEYWORD);
					}
						
					materialEditor.ShaderProperty(gradientGoalColor, Labels.GlobalGradientColor);
					materialEditor.ShaderProperty(gradientBlending, Labels.GlobalGradientBlending);
					materialEditor.ShaderProperty(gradientOffset, Labels.GlobalGradientOffset);
					materialEditor.ShaderProperty(gradientWidth, Labels.GlobalGradientWidth);
					GUILayout.Space(defaultSpace);
					InitGlobalGradientAxisType();
					ShowGlobalGradientAxisSettings();
				} else {
					material.DisableKeyword(Constants.Shader.GRADIENT_LOCAL_KEYWORD);
					material.DisableKeyword(Constants.Shader.GRADIENT_WORLD_KEYWORD);
				}
			}
		}
	}

	private void InitGlobalGradientAxisType() {
		Vector4 currentAxis = gradientUnitAxis.vectorValue;
		if (currentAxis == gradientAxisX && !isGradientAxisTypeFree) {
			gradientAxisType = GradientAxis.X;
		} else if (currentAxis == gradientAxisY && !isGradientAxisTypeFree) {
			gradientAxisType = GradientAxis.Y;
		} else if (currentAxis == gradientAxisZ && !isGradientAxisTypeFree) {
			gradientAxisType = GradientAxis.Z;
		} else {
			gradientAxisType = GradientAxis.Free;
		}
	}

	private void ShowGlobalGradientAxisSettings() {
		gradientAxisType = (GradientAxis)EditorGUILayout.Popup(Labels.GlobalGradientAxis, (int)gradientAxisType, Labels.GradientAxisEnumLabels);
		if (gradientAxisType == GradientAxis.X) {
			isGradientAxisTypeFree = false;
			gradientUnitAxis.vectorValue = gradientAxisX;
		} else if (gradientAxisType == GradientAxis.Y) {
			isGradientAxisTypeFree = false;
			gradientUnitAxis.vectorValue = gradientAxisY;
		} else if (gradientAxisType == GradientAxis.Z) {
			isGradientAxisTypeFree = false;
			gradientUnitAxis.vectorValue = gradientAxisZ;
		} else {
			isGradientAxisTypeFree = true;
			EditorGUI.BeginChangeCheck();
			gradientAxis = gradientUnitAxis.vectorValue;
			gradientAxis.x = EditorGUILayout.Slider(Labels.GradientAxisX, gradientAxis.x, -1.0f, 1.0f);
			gradientAxis.y = EditorGUILayout.Slider(Labels.GradientAxisY, gradientAxis.y, -1.0f, 1.0f);
			gradientAxis.z = EditorGUILayout.Slider(Labels.GradientAxisZ, gradientAxis.z, -1.0f, 1.0f);
			if (EditorGUI.EndChangeCheck()) {
				gradientUnitAxis.vectorValue = gradientAxis;
			}
		}
	}

	protected void ShowCustomShadowsProperties() {
		shouldShowCustomShadowProperties = UITools.GroupHeader(new GUIContent(Labels.ShadowsHeader), shouldShowCustomShadowProperties);
		if (shouldShowCustomShadowProperties) {
			using (new UITools.GUIVertical(UITools.VGroupStyle)) {

				using (new UITools.GUIDisable(IsShaderBaked(material))) {
					UITools.ToggleWithLabel(receiveCustomShadows, Labels.ReceiveShadowsHeader);
					if (receiveCustomShadows) {
						material.EnableKeyword(Constants.Shader.RECEIVE_CUSTOM_SHADOW_KEYWORD);
						GUILayout.Space(defaultSpace);
						EditorGUILayout.HelpBox(Labels.HelpReceiveCustomShadows, MessageType.Info, true);
					} else {
						material.DisableKeyword(Constants.Shader.RECEIVE_CUSTOM_SHADOW_KEYWORD);
					}
				}
					
				UITools.ToggleWithLabel(castCustomShadows, Labels.CastShadowsHeader);
				if (castCustomShadows) {
					material.EnableKeyword(Constants.Shader.CAST_CUSTOM_SHADOW_ON_KEYWORD);
					material.DisableKeyword(Constants.Shader.CAST_CUSTOM_SHADOW_OFF_KEYWORD);
				} else {
					material.EnableKeyword(Constants.Shader.CAST_CUSTOM_SHADOW_OFF_KEYWORD);
					material.DisableKeyword(Constants.Shader.CAST_CUSTOM_SHADOW_ON_KEYWORD);
				}
			}
		}
	}

	protected bool ShaderHasKeyword(string keyword) {
		string bakedTag = material.GetTag(Constants.FlatLightingBakedTag, false);
		if (!string.IsNullOrEmpty(bakedTag)) {
			 return bakedTag.Contains(keyword);
		}

		return true;
	}

	public static bool IsShaderBaked(Material _material) {
		string bakedTag = _material.GetTag(Constants.FlatLightingBakedTag, false).Trim();
		return !string.IsNullOrEmpty(bakedTag);
	}
}
