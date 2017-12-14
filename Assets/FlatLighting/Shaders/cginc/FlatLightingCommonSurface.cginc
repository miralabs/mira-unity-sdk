/// Credit to Bogdan Gochev for licensing this shader code to us. 
/// Usage of the Flat Lighting Shader from this SDK is licensed for Mira applications only.
/// Flat lighting asset: https://www.assetstore.unity3d.com/en/#!/content/67730

#ifndef FLAT_LIGHTING_COMMON_SURFACE_INCLUDED
#define FLAT_LIGHTING_COMMON_SURFACE_INCLUDED

#include "UnityCG.cginc"

struct Input {
	#ifdef FL_MAIN_TEXTURE
		half2 uv_MainTex : TEXCOORD0;
	#endif

	#ifdef FL_VERTEX_COLOR
		fixed4 vertexColor : COLOR0;
	#endif

	#if defined(FL_COLORS_WORLD) || defined(FL_VERTEX_WORLD)
		float4 flWorldVertex : TEXCOORD1;
	#endif
	#if defined(FL_COLORS_LOCAL) || defined(FL_MODEL_VERTEX)
		float4 flModelVertex : TEXCOORD1;
	#endif

	float3 worldNormal;
};

void vert (inout appdata_full v, out Input o) {
	UNITY_INITIALIZE_OUTPUT(Input,o);

	#if defined(FL_COLORS_LOCAL) || defined(FL_MODEL_VERTEX)
		o.flModelVertex = v.vertex;
	#endif
	#if defined(FL_COLORS_WORLD) || defined(FL_SPOT_LIGHT) || defined(FL_POINT_LIGHT) || defined(FL_DIRECTIONAL_LIGHT) || defined(FL_RECEIVESHADOWS) || defined(FL_VERTEX_WORLD)
		o.flWorldVertex = mul(unity_ObjectToWorld, v.vertex); 
	#endif
}

void surf (Input IN, inout SurfaceOutput o) {
	half4 vertex = half4(0,0,0,0);
	half3 normal = half3(0,0,0);
	#if defined(FL_COLORS_LOCAL)
		vertex = IN.flModelVertex;
		normal = UnityWorldToObjectDir(IN.worldNormal);
	#endif
	#if defined(FL_COLORS_WORLD)
		vertex = IN.flWorldVertex;
		normal = IN.worldNormal;
	#endif

	fixed4 flatLight = fl_axis_light(normal, vertex);

	#ifdef FL_VERTEX_COLOR
		flatLight *= IN.vertexColor;
	#endif

	#ifdef FL_MAIN_TEXTURE
		flatLight = fl_axis_apply_texture(flatLight, IN.uv_MainTex);
	#endif

	#if defined(FL_GRADIENT_WORLD) || defined(FL_GRADIENT_LOCAL)
		flatLight = CustomGradient(vertex, flatLight);
	#endif

	#ifdef FL_AMBIENT_LIGHT
		half4 ambientColor = flatLight * _Ambient_Light;
		flatLight += ambientColor;
	#endif

	o.Albedo = flatLight.rgb;
	o.Alpha = flatLight.a;
}


/////////////////////////////////////////////////////////////////////////////Cel shading
#if defined(FL_CEL_SURFACE)
	fixed _CelThreshold;

	#if defined(FL_CEL_SURFACE_ALPHA)
		fixed _Alpha;
	#endif

	inline half cel_cuts(half NdotL) {
//			if (NdotL >= 0.75) {
//				NdotL = 1;
//			} else if (NdotL < 0.75 && NdotL >= 0.5) {
//				NdotL = 0.75;
//			} else if (NdotL < 0.5 && NdotL >= 0.25) {
//				NdotL = 0.5;
//			} else if (NdotL < 0.25 && NdotL >= 0.005) {
//				NdotL = 0.25;
//			} else {
//				NdotL = 0;
//			}
//			Above is the branch version
		half NdotL_with_cuts = ((int)(NdotL >= 0.75) * 1) + 
								((int)(NdotL < 0.75 && NdotL >= 0.5) * 0.75) + 
								((int)(NdotL < 0.5 && NdotL >= 0.25) * 0.5) + 
								((int)(NdotL < 0.25 && NdotL >= 0.005) * 0.25) + 
								((int)(NdotL < 0.005) * 0.0);
		return saturate(NdotL_with_cuts);
	}

    half4 LightingToonRamp (SurfaceOutput s, half3 lightDir, half atten) {
        half NdotL = saturate(_CelThreshold + dot(s.Normal, lightDir));
        NdotL = cel_cuts(NdotL);
		half4 c;
		c.rgb = s.Albedo * _LightColor0.rgb * (NdotL * cel_cuts(atten) * 0.5);
		c.a = s.Alpha;

		#if defined(FL_CEL_SURFACE_ALPHA)
			c.a = _Alpha;
		#endif

		return c;
    }
#endif


#endif // FLAT_LIGHTING_COMMON_SURFACE_INCLUDED