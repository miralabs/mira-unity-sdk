/// Credit to Bogdan Gochev for licensing this shader code to us. 
/// Usage of the Flat Lighting Shader from this SDK is licensed for Mira applications only.
/// Flat lighting asset: https://www.assetstore.unity3d.com/en/#!/content/67730

Shader "FlatLighting/Baked/FLSurfaceWater" {
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

		_WaveLength("Length", Float) = 0.5
		_WaveHeight("Height", Float) = 0.5
		_WaveSpeed("Speed", Float) = 1.0
		_RandomHeight("Random Wave Height", Float) = 0.5
		_RandomSpeed("Random Wave Speed", Float) = 0.5
		_Alpha ("Alpha", Range(0.0,1.0)) = 1.0
	}
	SubShader {

		Tags { 
				"Queue"="Transparent" 
				"RenderType"="Transparent"
			   "FlatLightingBakedTag"="FL_COLORS_WORLD;FL_SYMETRIC_COLORS_ON"
			}

		LOD 100

		CGPROGRAM
		#pragma surface surfWater ToonRamp vertex:vertWater alpha
		#pragma target 3.0

			#define FL_COLORS_WORLD
			#define FL_SYMETRIC_COLORS_ON
			#define FL_GRADIENT_AXIS_OFF




		#define FL_CEL_SURFACE
		#define FL_CEL_SURFACE_ALPHA

		#include "../cginc/FlatLightingAxis.cginc"
		#include "../cginc/FlatLightingGradient.cginc"
		#include "../cginc/FlatLightingLightmapping.cginc"
		#include "../cginc/FlatLightingShadows.cginc"
		#include "../cginc/FlatLightingSources.cginc"
		#include "../cginc/FlatLightingCommonSurface.cginc"

		uniform half _WaveLength;
		uniform half _WaveHeight;
		uniform half _WaveSpeed;
		uniform half _RandomHeight;
		uniform half _RandomSpeed;
		
		inline fixed rand(float3 co) {
			return frac(sin(dot(co.xyz, float3(12.9898, 78.233, 45.5432))) * 43758.5453);
		}

		inline fixed rand2(float3 co) {
			return frac(sin(dot(co.xyz, float3(19.9128, 75.2, 34.5122))) * 12765.5213);
		}
		
		inline fixed phase_calculate (float4 v) {
			fixed phase0 = (_WaveHeight) * sin((_Time[1] * _WaveSpeed) + (v.x * _WaveLength) + (v.z * _WaveLength) + 0.5 * rand2(v.xzz));
			fixed phase0_1 = (_RandomHeight) * sin(cos(rand(v.xzz) + cos(_Time[1] * _RandomSpeed * sin(rand(v.xxz)))));
			
			return phase0 + phase0_1;
		}			

		void vertWater (inout appdata_full v, out Input o) {
			v.vertex.y += phase_calculate(v.vertex);
			vert(v, o);
		}

		inline fixed4 calculate_normal (float3 vertex) {
			return fixed4(normalize(cross(ddx(vertex), ddy(vertex))), 1);
		}

		void surfWater (Input IN, inout SurfaceOutput o) {
			#if defined(FL_COLORS_LOCAL)
				IN.worldNormal = calculate_normal(IN.flModelVertex.xyz);
			#endif
			#if defined(FL_COLORS_WORLD)
				IN.worldNormal = calculate_normal(IN.flWorldVertex.xyz);
			#endif

			surf(IN, o);

			o.Alpha = _Alpha;
		}
		
		ENDCG
	}

	CustomEditor "FlatLightingWaterSurfaceShaderEditor"
}
