/// Credit to Bogdan Gochev for licensing this shader code to us. 
/// Usage of the Flat Lighting Shader from this SDK is licensed for Mira applications only.
/// Flat lighting asset: https://www.assetstore.unity3d.com/en/#!/content/67730


using UnityEngine;
using System.Collections;

namespace FlatLighting {
	public static class Constants {

		public static string FlatLightingShaderPath = "FlatLighting/FlatLighting";

		public static string FlatLightingTag = "FlatLightingTag";

		public static string FlatLightingBakedTag = "FlatLightingBakedTag";

		public static string GizmoIconsPath = "FlatLighting/";

		public static class Shader {

			public static readonly string AXIS_COLORS_LOCAL = "FL_COLORS_LOCAL";
			public static readonly string AXIS_COLORS_GLOBAL = "FL_COLORS_WORLD";
			public static readonly string SYMETRIC_COLORS_ON_KEYWORD = "FL_SYMETRIC_COLORS_ON";
			public static readonly string SYMETRIC_COLORS_OFF_KEYWORD = "FL_SYMETRIC_COLORS_OFF";
			public static readonly string AXIS_GRADIENT_ON_X_KEYWORD = "FL_GRADIENT_AXIS_ON_X";
			public static readonly string AXIS_GRADIENT_ON_Y_KEYWORD = "FL_GRADIENT_AXIS_ON_Y";
			public static readonly string AXIS_GRADIENT_ON_Z_KEYWORD = "FL_GRADIENT_AXIS_ON_Z";
			public static readonly string AXIS_GRADIENT_OFF_KEYWORD = "FL_GRADIENT_AXIS_OFF";
			public static readonly string VERTEX_COLOR_KEYWORD = "FL_VERTEX_COLOR";
			public static readonly string AMBIENT_LIGHT_KEYWORD = "FL_AMBIENT_LIGHT";
			public static readonly string DIRECT_LIGHT_KEYWORD = "FL_DIRECTIONAL_LIGHT";
			public static readonly string SPOT_LIGHT_KEYWORD = "FL_SPOT_LIGHT";
			public static readonly string POINT_LIGHT_KEYWORD = "FL_POINT_LIGHT";
			public static readonly string BLEND_LIGHT_SOURCES_KEYWORD = "FL_BLEND_LIGHT_SOURCES";
			public static readonly string GRADIENT_LOCAL_KEYWORD = "FL_GRADIENT_LOCAL";
			public static readonly string GRADIENT_WORLD_KEYWORD = "FL_GRADIENT_WORLD";
			public static readonly string CUSTOM_LIGHTMAPPING_KEYWORD = "FL_LIGHTMAPPING";
			public static readonly string UNITY_LIGHTMAPPING_KEYWORD = "FL_UNITY_LIGHTMAPPING";
			public static readonly string RECEIVE_CUSTOM_SHADOW_KEYWORD = "FL_RECEIVESHADOWS";
			public static readonly string CAST_CUSTOM_SHADOW_ON_KEYWORD = "FL_CASTSHADOW_ON";
			public static readonly string CAST_CUSTOM_SHADOW_OFF_KEYWORD = "FL_CASTSHADOW_OFF";
			public static readonly string USE_MAIN_TEXTURE_KEYWORD = "FL_MAIN_TEXTURE";

			public static readonly string LightPositiveX = "_LightPositiveX";
			public static readonly string LightPositiveY = "_LightPositiveY";
			public static readonly string LightPositiveZ = "_LightPositiveZ";
			public static readonly string LightNegativeX = "_LightNegativeX";
			public static readonly string LightNegativeY = "_LightNegativeY";
			public static readonly string LightNegativeZ = "_LightNegativeZ";

			public static readonly string LightPositive2X = "_LightPositive2X";
			public static readonly string LightPositive2Y = "_LightPositive2Y";
			public static readonly string LightPositive2Z = "_LightPositive2Z";
			public static readonly string LightNegative2X = "_LightNegative2X";
			public static readonly string LightNegative2Y = "_LightNegative2Y";
			public static readonly string LightNegative2Z = "_LightNegative2Z";

			public static readonly string GradientWidthPositiveX = "_GradientWidthPositiveX";
			public static readonly string GradientWidthPositiveY = "_GradientWidthPositiveY";
			public static readonly string GradientWidthPositiveZ = "_GradientWidthPositiveZ";
			public static readonly string GradientWidthNegativeX = "_GradientWidthNegativeX";
			public static readonly string GradientWidthNegativeY = "_GradientWidthNegativeY";
			public static readonly string GradientWidthNegativeZ = "_GradientWidthNegativeZ";

			public static readonly string GradientOriginOffsetPositiveX = "_GradientOriginOffsetPositiveX";
			public static readonly string GradientOriginOffsetPositiveY = "_GradientOriginOffsetPositiveY";
			public static readonly string GradientOriginOffsetPositiveZ = "_GradientOriginOffsetPositiveZ";
			public static readonly string GradientOriginOffsetNegativeX = "_GradientOriginOffsetNegativeX";
			public static readonly string GradientOriginOffsetNegativeY = "_GradientOriginOffsetNegativeY";
			public static readonly string GradientOriginOffsetNegativeZ = "_GradientOriginOffsetNegativeZ";


		}
	}
}