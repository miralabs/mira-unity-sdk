/// Credit to Bogdan Gochev for licensing this shader code to us. 
/// Usage of the Flat Lighting Shader from this SDK is licensed for Mira applications only.
/// Flat lighting asset: https://www.assetstore.unity3d.com/en/#!/content/67730

using UnityEngine;
using UnityEditor;
using System.IO;
using System.Collections;
using System.Collections.Generic;
using System.Linq;

namespace FlatLighting {
	
	public class ShaderBaker : EditorWindow {

		private static ShaderBaker window;
		private static string newName;
		private static bool bOverrite;
		public static Material targetMaterial;

		private const string BAKED_SHADERS_RELATIVE_PATH = "/Shaders/Baked/";

		public static void ShowWindow(Material _material) {
			ShaderBaker.window = (ShaderBaker)ScriptableObject.CreateInstance(typeof(ShaderBaker));
			ShaderBaker.targetMaterial = _material;
			if (ShaderBaker.targetMaterial == null) {
				ShaderBaker.newName = string.Empty;
			} else {
				ShaderBaker.newName = ShaderBaker.targetMaterial.name;
			}
			ShaderBaker.bOverrite = false;
			ShaderBaker.window.Focus();
			ShaderBaker.window.ShowAsDropDown(new Rect((float)(Screen.currentResolution.width / 2 - 160), (float)(Screen.currentResolution.height / 2 - 200), 320f, 100f), new Vector2(400f, 130f));
		}

		private void OnGUI() {
			if (ShaderBaker.window == null) {
				if (this != null) {
					base.Close();
				}
				return;
			}
			GUILayout.Space(16f);
			EditorGUILayout.BeginHorizontal(new GUILayoutOption[0]);
			EditorGUI.BeginChangeCheck();
			EditorGUILayout.LabelField(" Shader Name:", new GUILayoutOption[]
				{
					GUILayout.MaxWidth(100f)
				});
			GUI.SetNextControlName("textField");
			string fullNewShaderPath = ShaderBaker.FindBakedShadersRelativePath() + ShaderBaker.newName + ".shader";
			bool shouldOptimizeMaterial = true;
			bool isShaderNameInvalid = false;
			bool shaderAlreadyExists = false;
			if (ShaderContainsInvalidCharacters()) {
				isShaderNameInvalid = true;
				shouldOptimizeMaterial = false;
			}

			if (File.Exists(fullNewShaderPath)) {
				shaderAlreadyExists = true;
				shouldOptimizeMaterial = false;
			}

			ShaderBaker.newName = EditorGUILayout.TextField(string.Empty, ShaderBaker.newName, new GUILayoutOption[] { GUILayout.Width(280f)});

			EditorGUILayout.EndHorizontal();
			EditorGUI.FocusTextInControl("textField");
			if (Event.current.keyCode == KeyCode.Return && GUI.GetNameOfFocusedControl() == "textField") {
				if (shouldOptimizeMaterial) {
					ShaderBaker.OptimizeMaterial(ShaderBaker.targetMaterial);
				}
				ShaderBaker.window.Close();
				return;
			}
			if (Event.current.keyCode == KeyCode.Escape && GUI.GetNameOfFocusedControl() == "textField") {
				ShaderBaker.window.Close();
				return;
			}
			using (new UITools.GUIEnable(false)) {
				GUILayout.Space(5f);
				if (shouldOptimizeMaterial) {
					EditorGUILayout.LabelField(fullNewShaderPath, new GUILayoutOption[0]);
				} else if (isShaderNameInvalid) {
					EditorGUILayout.LabelField(" Shader name contains unsopported characters", new GUILayoutOption[0]);
				} else if (shaderAlreadyExists)	{
					EditorGUILayout.LabelField(" Shader with such name already exists", new GUILayoutOption[0]);
				}
			}
			if (shaderAlreadyExists) {
					GUILayout.BeginHorizontal(new GUILayoutOption[0]);
					GUILayout.Space(10f);
					ShaderBaker.bOverrite = EditorGUILayout.ToggleLeft(" Overrite", ShaderBaker.bOverrite, new GUILayoutOption[0]);
					GUILayout.EndHorizontal();

				if (ShaderBaker.bOverrite) {
					shouldOptimizeMaterial = true;
				}
			}
			using (new UITools.GUIEnable(shouldOptimizeMaterial)) {
				if (GUILayout.Button("Bake Shader", GUILayout.MinHeight(30.0f))) {
					if (shouldOptimizeMaterial) {
						ShaderBaker.OptimizeMaterial(ShaderBaker.targetMaterial);
					}
					ShaderBaker.window.Close();
					return;
				}
			}
			using (new UITools.GUIHorizontal ()) {
				using (new UITools.GUIFlexiSpace ()) {
					if (GUILayout.Button ("Cancel", GUILayout.ExpandWidth (false), GUILayout.MinWidth(100.0f)))
					{
						ShaderBaker.window.Close ();
					}
				}
			}
		}

		private static bool ShaderContainsInvalidCharacters() {
			return ShaderBaker.newName == string.Empty || 
				ShaderBaker.newName.Contains("//") || 
				ShaderBaker.newName.Contains("\\") || 
				ShaderBaker.newName.Contains(".") || 
				ShaderBaker.newName.Contains("\"") || 
				(ShaderBaker.newName.Contains("/") && ShaderBaker.newName.LastIndexOf("/") == ShaderBaker.newName.Length - 1) || 
				ShaderBaker.newName.Contains("!") || 
				ShaderBaker.newName.Contains("?") || 
				ShaderBaker.newName.Contains("@") || 
				ShaderBaker.newName.Contains("#") || 
				ShaderBaker.newName.Contains("$") || 
				ShaderBaker.newName.Contains("%") || 
				ShaderBaker.newName.Contains("^") || 
				ShaderBaker.newName.Contains("&") || 
				ShaderBaker.newName.Contains("*");
		}

		private static void OptimizeMaterial(Material _material) {
			if (_material == null || string.IsNullOrEmpty(_material.GetTag(Constants.FlatLightingTag, false))) {
				return;
			}
			if (FlatLightingShaderEditor.IsShaderBaked(_material)) {
				return;
			}
			string assetPath = AssetDatabase.GetAssetPath(_material.shader.GetInstanceID());
			List<string> shaderStringRows = new List<string>(File.ReadAllLines(assetPath));
			string[] shaderKeywords = _material.shaderKeywords;
			List<string> activeKeywords = new List<string>();
			for (int i = 0; i < shaderStringRows.Count; i++) {
				if (shaderStringRows[i].Contains("Shader \"FlatLighting/")) {
					shaderStringRows[i] = "Shader \"FlatLighting/Baked/" + ShaderBaker.newName + "\" {";
				}
				if (shaderStringRows[i].Contains("\"./cginc/")) {
					shaderStringRows[i] = shaderStringRows[i].Replace("./cginc/", "../cginc/");
				}
				if (shaderStringRows[i].Contains("MaterialToggle")) {
					shaderStringRows[i] = string.Empty;
				}
				if (shaderStringRows[i].Contains("KeywordEnum")) {
					shaderStringRows[i] = string.Empty;
				}
				if (shaderStringRows[i].Contains(" multi_compile ")) {
					string[] source = shaderStringRows[i].Replace("#pragma", string.Empty)
						.Replace("multi_compile", string.Empty)
						.Trim().Split(new char[0]);
					string shaderKeyword = string.Empty;
					shaderKeyword = ShaderBaker.IsKeyInKeyArray(ref shaderKeywords, ref source);
					if (!string.IsNullOrEmpty(shaderKeyword)) {
						shaderStringRows[i] = "\t\t\t#define " + shaderKeyword;
						if (!shaderKeyword.Contains("_OFF")) {
							activeKeywords.Add(shaderKeyword);
						}
					} else {
						shaderStringRows[i] = string.Empty;
					}
				}
				if (shaderStringRows[i].Contains(" shader_feature ")) {
					string[] source = shaderStringRows[i].Replace("#pragma", string.Empty)
						.Replace("shader_feature", string.Empty)
						.Replace("__", string.Empty)
						.Trim().Split(new char[0]);
					string shaderKeyword = string.Empty;
					shaderKeyword = ShaderBaker.IsKeyInKeyArray(ref shaderKeywords, ref source);
					if (!string.IsNullOrEmpty(shaderKeyword)) {
						shaderStringRows[i] = "\t\t\t#define " + shaderKeyword;
						if (!shaderKeyword.Contains("_OFF")) {
							activeKeywords.Add(shaderKeyword);
						}
					} else {
						shaderStringRows[i] = string.Empty;
					}
				}
			}

			activeKeywords = activeKeywords.Distinct<string>().ToList<string>();
			if (activeKeywords.Count == 0) {
				activeKeywords.Add("EMPTY");
			}
			for (int j = 0; j < shaderStringRows.Count; j++) {
				if (shaderStringRows[j].Contains(Constants.FlatLightingTag)) {
					shaderStringRows[j] = string.Concat(new string[]
						{
							"\t\t\t   \"",
							Constants.FlatLightingBakedTag,
							"\"=\"",
							string.Join(";", activeKeywords.ToArray()),
							"\""
						});
				}
			}
			string bakedShadersPath = ShaderBaker.FindBakedShadersRelativePath();
			if (!Directory.Exists(bakedShadersPath)) {
				Directory.CreateDirectory(bakedShadersPath);
			}
			string shaderPath = bakedShadersPath + ShaderBaker.newName + ".shader";
			File.WriteAllLines(shaderPath, shaderStringRows.ToArray());
			AssetDatabase.ImportAsset(shaderPath);
			Object @object = AssetDatabase.LoadMainAssetAtPath(shaderPath);
			EditorGUIUtility.PingObject(@object.GetInstanceID());
			Undo.RecordObject(_material, "Assign Baked Shader");
			_material.shader = ((Shader)@object);
		}

		private static string IsKeyInKeyArray(ref string[] keyWords, ref string[] shaderDefines) {
			for (int i = 0; i < keyWords.Length; i++) {
				if (shaderDefines.Contains(keyWords[i])) {
					return keyWords[i];
				}
			}
			return string.Empty;
		}

		public static string FindBakedShadersRelativePath() {
			string[] results = AssetDatabase.FindAssets("ShaderBaker t:Script", null);
			if (results.Length > 0) {
				string p = AssetDatabase.GUIDToAssetPath(results[0]);
				p = System.IO.Path.GetDirectoryName(p);
				p = p.Substring(0, p.LastIndexOf('/'));
				p = p.Substring(0, p.LastIndexOf('/'));
				return p + BAKED_SHADERS_RELATIVE_PATH;
			}

			Debug.LogError("Could not find ShaderBaker class file.");
			return string.Empty;
		}
	}
}
