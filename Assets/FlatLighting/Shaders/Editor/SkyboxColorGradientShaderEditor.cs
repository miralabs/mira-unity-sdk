/// Credit to Bogdan Gochev for licensing this shader code to us. 
/// Usage of the Flat Lighting Shader from this SDK is licensed for Mira applications only.
/// Flat lighting asset: https://www.assetstore.unity3d.com/en/#!/content/67730

using UnityEngine;
using UnityEditor;
using System.Collections;
using FlatLighting;

public class SkyboxColorGradientShaderEditor : ShaderGUI {

	private MaterialProperty colorTop;
	private MaterialProperty colorBottom;
	private MaterialProperty upVector;
	private MaterialProperty intensity;
	private MaterialProperty exponent;
	private MaterialProperty pitch;
	private MaterialProperty yaw;

	private MaterialEditor materialEditor;

	private static class Labels	{
		public static readonly string ColorTop = "Bottom Color";
		public static readonly string ColorBottom = "Top Color";
		public static readonly string Intensity = "Intensity";
		public static readonly string Exponent = "Exponent";
		public static readonly string Pitch = "Pitch (horizontal)";
		public static readonly string Yaw = "Yaw (vertical)";
		public static readonly string ColorRotationHelp = "Move the \"Top Color\" center position with these two parameters.";
	}

	private void FindProperties (MaterialProperty[] props) {
		colorTop = FindProperty("_Color1", props);
		colorBottom = FindProperty("_Color2", props);
		upVector = FindProperty("_UpVector", props);
		intensity = FindProperty("_Intensity", props);
		exponent = FindProperty("_Exponent", props);
		pitch = FindProperty("_UpVectorPitch", props);
		yaw = FindProperty("_UpVectorYaw", props);
	}

	public override void OnGUI (MaterialEditor materialEditor, MaterialProperty[] properties) {
		FindProperties(properties);
		this.materialEditor = materialEditor;

		ShowShaderControls();
	}

	private void ShowShaderControls() {
		EditorGUI.BeginChangeCheck ();
		
		materialEditor.ShaderProperty (colorBottom, Labels.ColorBottom);
		materialEditor.ShaderProperty (colorTop, Labels.ColorTop);
		materialEditor.ShaderProperty (intensity, Labels.Intensity);
		materialEditor.ShaderProperty (exponent, Labels.Exponent);
		
		if (pitch.hasMixedValue || yaw.hasMixedValue) {
			EditorGUILayout.HelpBox ("Editing angles is disabled because they have mixed values.", MessageType.Warning);
		} else {
			UITools.DrawSeparatorLine();
			EditorGUILayout.LabelField(new GUIContent("Rotation Control"),  EditorStyles.boldLabel);
			materialEditor.ShaderProperty (pitch, Labels.Pitch);
			materialEditor.ShaderProperty (yaw, Labels.Yaw);
			EditorGUILayout.HelpBox (Labels.ColorRotationHelp, MessageType.Info);
		}
		
		if (EditorGUI.EndChangeCheck ()) {
			var rp = pitch.floatValue * Mathf.Deg2Rad;
			var ry = yaw.floatValue * Mathf.Deg2Rad;
			
			Vector4 upVectorNewValue = new Vector4 (
				Mathf.Sin (rp) * Mathf.Sin (ry),
				Mathf.Cos (rp),
				Mathf.Sin (rp) * Mathf.Cos (ry),
				0.0f
				);

			upVector.vectorValue = upVectorNewValue;			
		}
	}
}