/// Credit to Bogdan Gochev for licensing this shader code to us. 
/// Usage of the Flat Lighting Shader from this SDK is licensed for Mira applications only.
/// Flat lighting asset: https://www.assetstore.unity3d.com/en/#!/content/67730

Shader "FlatLighting/Animated/Water" {
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

		_WaveLength("Length", Float) = 0.5
		_WaveHeight("Height", Float) = 0.5
		_WaveSpeed("Speed", Float) = 1.0
		_RandomHeight("Random Wave Height", Float) = 0.5
		_RandomSpeed("Random Wave Speed", Float) = 0.5
		_Alpha ("Alpha", Range(0.0,1.0)) = 1.0
	}
	SubShader {

		Tags { "Queue"="Transparent" 
				"IgnoreProjector"="True" 
				"RenderType"="Transparent" 
			    "FlatLightingTag"="FlatLighting/FlatLightingWater"
		}

		LOD 100

		Cull Off
		Blend SrcAlpha OneMinusSrcAlpha 

		Pass {
			CGPROGRAM
			#pragma vertex vert
			#pragma fragment frag
			#pragma target 3.0

			#pragma shader_feature __ FL_VERTEX_COLOR
			#pragma shader_feature FL_COLORS_WORLD FL_COLORS_LOCAL
			#pragma shader_feature FL_SYMETRIC_COLORS_ON FL_SYMETRIC_COLORS_OFF
			#pragma shader_feature __ FL_GRADIENT_AXIS_ON_X
			#pragma shader_feature __ FL_GRADIENT_AXIS_ON_Y
			#pragma shader_feature __ FL_GRADIENT_AXIS_ON_Z
			#pragma shader_feature __ FL_AMBIENT_LIGHT
			#pragma shader_feature __ FL_GRADIENT_LOCAL FL_GRADIENT_WORLD
			#pragma shader_feature __ FL_DIRECTIONAL_LIGHT
			#pragma shader_feature __ FL_SPOT_LIGHT
			#pragma shader_feature __ FL_POINT_LIGHT
			#pragma shader_feature __ FL_BLEND_LIGHT_SOURCES
			#pragma shader_feature __ FL_RECEIVESHADOWS
			#pragma shader_feature __ FL_LIGHTMAPPING FL_UNITY_LIGHTMAPPING
			#pragma shader_feature __ FL_MAIN_TEXTURE

			#include "../cginc/FlatLightingCommon.cginc"
			
			uniform half _WaveLength;
			uniform half _WaveHeight;
			uniform half _WaveSpeed;
			uniform half _RandomHeight;
			uniform half _RandomSpeed;
			uniform fixed _Alpha;
			
			inline fixed rand(half3 co) {
				return frac(sin(dot(co.xyz, float3(12.9898, 78.233, 45.5432))) * 43758.5453);
			}

			inline fixed rand2(half3 co) {
				return frac(sin(dot(co.xyz, float3(19.9128, 75.2, 34.5122))) * 12765.5213);
			}
			
			inline fixed phase_calculate (half4 v) {
				fixed phase0 = (_WaveHeight) * sin((_Time[1] * _WaveSpeed) + (v.x * _WaveLength) + (v.z * _WaveLength) + 0.5 * rand2(v.xzz));
				fixed phase0_1 = (_RandomHeight) * sin(cos(rand(v.xzz) + cos(_Time[1] * _RandomSpeed * sin(rand(v.xxz)))));
				
				return phase0 + phase0_1;
			}			

			fl_vertex2fragment vert(fl_vertex_input v) {
				v.vertex.y += phase_calculate(v.vertex);
				return FLvertex(v);
			}

			inline fixed4 calculate_normal (half4 vertex) {
				return fixed4(normalize(cross(ddx(vertex), ddy(vertex))), 1);
			}
			
			fixed4 frag(fl_vertex2fragment i) : SV_Target {
				#if defined(FL_COLORS_LOCAL)
					i.flModelNormal = calculate_normal(i.flModelVertex);
				#endif
				#if defined(FL_COLORS_WORLD)
					i.flWorldNormal = calculate_normal(i.flWorldVertex);
				#endif
	
				fixed4 color = FLfragment(i);
				color.a = _Alpha;

				return color;
			}
			
			ENDCG
		}
	}

	CustomEditor "FlatLightingWaterShaderEditor"
}
