Shader "FlatLighting/FlatLightingCelSurfaceTransparent" {
	Properties {
		_LightNegativeX ("Light -X", Color) = (1,1,1,1)
		_LightNegative2X ("Light2 -X", Color) = (1,1,1,1)
		_GradientOriginOffsetNegativeX ("Gradient Width", Float) = 3.0
		_GradientWidthNegativeX ("Gradient Offset", Float) = 0.0

		_LightPositiveX ("Light X", Color) = (1,1,1,1)
		_LightPositive2X ("Light2 X", Color) = (1,1,1,1)
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
		_CelThreshold("Cel Threshold", Range(-1.0, 1.0)) = 0.0
		_Alpha ("Alpha", Range(0.0,1.0)) = 1.0

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
	}
	SubShader {
		Tags {	"Queue"="Transparent" 
				"RenderType"="Transparent" 
				"FlatLightingTag"="FlatLighting/FlatLightingCelSurfaceTransparent"
			}

		LOD 100
		
		CGPROGRAM
		#pragma surface surf ToonRamp vertex:vert alpha
		#pragma target 3.0
		#pragma shader_feature __ FL_VERTEX_COLOR
		#pragma shader_feature FL_COLORS_WORLD FL_COLORS_LOCAL
		#pragma shader_feature FL_SYMETRIC_COLORS_ON FL_SYMETRIC_COLORS_OFF
		#pragma shader_feature __ FL_GRADIENT_AXIS_ON_X
		#pragma shader_feature __ FL_GRADIENT_AXIS_ON_Y
		#pragma shader_feature __ FL_GRADIENT_AXIS_ON_Z
		#pragma shader_feature __ FL_AMBIENT_LIGHT
		#pragma shader_feature __ FL_GRADIENT_LOCAL FL_GRADIENT_WORLD
		#pragma shader_feature __ FL_MAIN_TEXTURE

		#define FL_CEL_SURFACE
		#define FL_CEL_SURFACE_ALPHA

		#include "./cginc/FlatLightingAxis.cginc"
		#include "./cginc/FlatLightingGradient.cginc"
		#include "./cginc/FlatLightingLightmapping.cginc"
		#include "./cginc/FlatLightingShadows.cginc"
		#include "./cginc/FlatLightingSources.cginc"
		#include "./cginc/FlatLightingCommonSurface.cginc"

		ENDCG
	} 
	CustomEditor "FlatLightingCelSurfacerTransparentEditor"
}
