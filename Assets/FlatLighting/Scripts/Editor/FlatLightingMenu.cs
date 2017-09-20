/// Credit to Bogdan Gochev for licensing this shader code to us. 
/// Usage of the Flat Lighting Shader from this SDK is licensed for Mira applications only.
/// Flat lighting asset: https://www.assetstore.unity3d.com/en/#!/content/67730


using UnityEngine;
using UnityEditor;
using System.Collections.Generic;
using System.IO;

namespace FlatLighting {
	public class FlatLightingMenu {

		[MenuItem("GameObject/Flat Lighting/Directional Light", false, 1)]
		public static void AddDirectionalLightToCurrentObject() {
			GameObject directionalLight = new GameObject("Directional Light");
			directionalLight.AddComponent<DirectionalLight>();
		}

		[MenuItem("GameObject/Flat Lighting/Spot Light", false, 2)]
		public static void AddSpotLightToCurrentObject() {
			GameObject spotLight = new GameObject("Spot Light");
			spotLight.AddComponent<SpotLight>();
		}

		[MenuItem("GameObject/Flat Lighting/Point Light", false, 3)]
		public static void AddPointLightToCurrentObject() {
			GameObject pointLight = new GameObject("Point Light");
			pointLight.AddComponent<PointLight>();
		}

		[MenuItem("GameObject/Flat Lighting/Shadow Projector", false, 4)]
		public static void AddShadowProjectorToCurrentObject() {
			GameObject shadowProjector = new GameObject("Shadow Projector");
			shadowProjector.AddComponent<ShadowProjector>();
		}

		[MenuItem("FL/Unpack Flat Lighting Surface shader", false, 10)]
		public static void UnpackFLSurface() {

			bool choise = EditorUtility.DisplayDialog(
				"Unpack Flat Lighting Surface Shader", 
				"Unpacking Flat Lighting Surface can take some time due to compiling multiple shader variants, please be patient.", 
				"Unpack", "Cancel");

			if(choise)
			{
				ShaderPacker.UnpackShader("/FlatLighting/Shaders/", "FlatLightingSurface");
			}
		}

		[MenuItem("FL/Unpack Flat Lighting Cell Surface shaders", false, 11)]
		public static void UnpackFLCelSurface() {

			bool choise = EditorUtility.DisplayDialog(
				"Unpack Flat Lighting Cell/Toon Surface Shaders", 
				"Unpacking Flat Lighting Cell/Toon Surface can take some time due to compiling multiple shader variants, please be patient.", 
				"Unpack", "Cancel");

			if(choise)
			{
				ShaderPacker.UnpackShader("/FlatLighting/Shaders/", "FlatLightingCelSurface");
				ShaderPacker.UnpackShader("/FlatLighting/Shaders/", "FlatLightingCelSurfaceTransparent");
			}
		}

		[MenuItem("FL/Unpack Water Surface shader", false, 12)]
		public static void UnpackFLWaterSurface() {

			bool choise = EditorUtility.DisplayDialog(
				"Unpack Flat Lighting Water Surface Shader", 
				"Unpacking Flat Lighting Water Surface  can take some time due to compiling multiple shader variants, please be patient", 
				"Unpack", "Cancel");

			if(choise)
			{
				ShaderPacker.UnpackShader("/FlatLighting/Shaders/Animated", "FlatLightingWaterSurface");
			}
		}

		[MenuItem("FL/Pack All Flat Lighting Surface shaders", false, 30)]
		public static void PackFL() {

			bool choise = EditorUtility.DisplayDialog(
				"Pack Flat Lighting Surface Shaders", 
				"Packing all the FL Surface shaders will return them in their original \".fl\" format and they will not be usable by Unity until a new unpacking. Do you want to continue?", 
				"Yes", "No");

			if(choise)
			{
				ShaderPacker.PackShader("/FlatLighting/Shaders/", "FlatLightingSurface");
				ShaderPacker.PackShader("/FlatLighting/Shaders/", "FlatLightingCelSurface");
				ShaderPacker.PackShader("/FlatLighting/Shaders/", "FlatLightingCelSurfaceTransparent");

				ShaderPacker.PackShader("/FlatLighting/Shaders/Animated", "FlatLightingWaterSurface");
			}
		}
	}
}
