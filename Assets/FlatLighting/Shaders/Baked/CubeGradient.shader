/// Credit to Bogdan Gochev for licensing this shader code to us. 
/// Usage of the Flat Lighting Shader from this SDK is licensed for Mira applications only.
/// Flat lighting asset: https://www.assetstore.unity3d.com/en/#!/content/67730

Shader "FlatLighting/Baked/CubeGradient" {
	Properties {
		_LightNegativeX ("Light -X", Color) = (1,1,1,1)
		_LightNegative2X ("Light2 -X", Color) = (1,1,1,1)
		_GradientOriginOffsetNegativeX ("Gradient Width", Float) = 3.0
		_GradientWidthNegativeX ("Gradient Offset", Float) = 0.0

		_LightPositiveX ("Light X", Color) = (1,1,1,1)
		_LightPositive2X ("Light2 -X", Color) = (1,1,1,1)
		_GradientOriginOffsetPositiveX ("Gradient Width", Float) = 3.0
		_GradientWidthPositiveX ("Gradient Offset", Float) = 0.0

		_LightNegativeZ ("Light -Z", Color) = (1,1,1,1)
		_LightNegative2Z ("Light2 -Z", Color) = (1,1,1,1)
		_GradientOriginOffsetNegativeZ ("Gradient Width", Float) = 3.0
		_GradientWidthNegativeZ ("Gradient Offset", Float) = 0.0

		_LightPositiveZ ("Light Z", Color) = (1,1,1,1)
		_LightPositive2Z ("Light2 Z", Color) = (1,1,1,1)
		_GradientOriginOffsetPositiveZ ("Gradient Width", Float) = 3.0
		_GradientWidthPositiveZ ("Gradient Offset", Float) = 0.0


		_LightNegativeY ("Light -Y", Color) = (1,1,1,1)
		_LightNegative2Y ("Light2 -Y", Color) = (1,1,1,1)
		_GradientOriginOffsetNegativeY ("Gradient Width", Float) = 3.0
		_GradientWidthNegativeY ("Gradient Offset", Float) = 0.0

		_LightPositiveY ("Light Y", Color) = (1,1,1,1)
		_LightPositive2Y ("Light2 Y", Color) = (1,1,1,1)
		_GradientOriginOffsetPositiveY ("Gradient Width", Float) = 3.0
		_GradientWidthPositiveY ("Gradient Offset", Float) = 0.0

		_MainTex("Main Texture", 2D) = "white" {}

		_GradienColorGoal ("Gradient Goal Color", Color) = (1,1,1,1)
		_GradientBlending ("Gradient Blending" , Range(0.0, 1.0)) = 0.0
		_GradientUnitAxis ("Gradient Axis", Vector) = (0,1,0,0)
		_GradientWidth ("Gradient Width", Float) = 3.0
		_GradientOffset ("Gradient Offset", Float) = 0.0

		_UVChannel ("Lightmap UV Channel", Int) = 0
		_CustomLightmap ("Lightmap (Greyscale)", 2D) = "white" {}
		_ShadowTint ("Shadow Tint", Color) = (1,1,1,1)
		_ShadowBoost ("Shadow Boost", Range(0.0, 1.0)) = 0.0

		_Ambient_Light ("Ambient Light", Color) = (1,1,1,1)

		_BlendedLightColor ("Blended Light Color", Color) = (1,1,1,1)
		_BlendedLightIntensities ("Blended Light Intensities", Vector) = (0,1,0,0)
		[Toggle] _BlendedLightSmoothness ("Blended Light Smoothness", Float) = 0.0
	}
	SubShader
	{
		Tags { "RenderType"="Opaque" 
			   "FlatLightingBakedTag"="FL_COLORS_LOCAL;FL_SYMETRIC_COLORS_ON;FL_GRADIENT_AXIS_ON;FL_AMBIENT_LIGHT;FL_GRADIENT_WORLD;FL_DIRECTIONAL_LIGHT;FL_SPOT_LIGHT;FL_POINT_LIGHT;FL_BLEND_LIGHT_SOURCES"
			}

		LOD 100

		Pass
		{
			CGPROGRAM
			#pragma vertex vert
			#pragma fragment frag
			#pragma target 3.0

			#define FL_COLORS_LOCAL
			#define FL_SYMETRIC_COLORS_ON
			#define FL_GRADIENT_AXIS_ON
			#define FL_AMBIENT_LIGHT
			#define FL_GRADIENT_WORLD
			#define FL_DIRECTIONAL_LIGHT
			#define FL_SPOT_LIGHT
			#define FL_POINT_LIGHT
			#define FL_BLEND_LIGHT_SOURCES



			#include "../cginc/FlatLightingCommon.cginc"

			fl_vertex2fragment vert (fl_vertex_input v) {
				return FLvertex(v);
			}

			half4 frag (fl_vertex2fragment i) : SV_Target {
				return FLfragment(i);
			}
			ENDCG
		}
	}

	CustomEditor "FlatLightingShaderEditor"
}
