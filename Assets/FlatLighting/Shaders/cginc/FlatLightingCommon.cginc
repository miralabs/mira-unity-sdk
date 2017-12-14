/// Credit to Bogdan Gochev for licensing this shader code to us. 
/// Usage of the Flat Lighting Shader from this SDK is licensed for Mira applications only.
/// Flat lighting asset: https://www.assetstore.unity3d.com/en/#!/content/67730

#ifndef FLAT_LIGHTING_COMMON_INCLUDED
#define FLAT_LIGHTING_COMMON_INCLUDED

#include "UnityCG.cginc"
#include "FlatLightingAxis.cginc"
#include "FlatLightingGradient.cginc"
#include "FlatLightingLightmapping.cginc"
#include "FlatLightingShadows.cginc"
#include "FlatLightingSources.cginc"

struct fl_vertex_input {
	half4 vertex : POSITION;
	half3 normal : NORMAL;
	#ifdef FL_MAIN_TEXTURE
		half2 uv : TEXCOORD0;
	#endif
	#if defined(FL_LIGHTMAPPING) || defined(FL_UNITY_LIGHTMAPPING)
		LIGHTMAP_COORDINATES
	#endif
	#ifdef FL_VERTEX_COLOR
		fixed4 vertexColor : COLOR0;
	#endif
};

struct fl_vertex2fragment {
	half4 vertex : SV_POSITION;
	#ifdef FL_VERTEX_COLOR
		fixed4 vertexColor : COLOR0;
	#endif
	#ifdef FL_MAIN_TEXTURE
		half2 uv : TEXCOORD0;
	#endif
	#if defined(FL_LIGHTMAPPING) || defined(FL_UNITY_LIGHTMAPPING)
		half2 lightmapUV : TEXCOORD1;
	#endif
	#if defined(FL_COLORS_WORLD) || defined(FL_SPOT_LIGHT) || defined(FL_POINT_LIGHT) || defined(FL_DIRECTIONAL_LIGHT) || defined(FL_RECEIVESHADOWS) || defined(FL_VERTEX_WORLD)
		float4 flWorldVertex : TEXCOORD2;
		half3 flWorldNormal: TEXCOORD3;
	#endif
	#if defined(FL_COLORS_LOCAL) || defined(FL_MODEL_VERTEX)
		float4 flModelVertex : TEXCOORD4;
		half3 flModelNormal: TEXCOORD5;
	#endif
};

float4 _MainTex_ST;

////////////////////////////////////////////////////////////////////////////////////////////////////////////Vertex Shaders
inline fl_vertex2fragment fl_v_textures_setup(fl_vertex_input v, fl_vertex2fragment o) {
	#ifdef FL_MAIN_TEXTURE
		o.uv = TRANSFORM_TEX(v.uv, _MainTex);
	#endif

	#ifdef FL_LIGHTMAPPING
		half2 uv = GetLightmapUV(v.lightmap0, v.lightmap1);
		o.lightmapUV = TRANSFORM_TEX(uv, _CustomLightmap);
	#endif

	#ifdef FL_UNITY_LIGHTMAPPING
		half2 uv = GetLightmapUV(v.lightmap0, v.lightmap1);
		o.lightmapUV = calculateUnityLightmapVertexCoordinates(uv);
	#endif

	return o;
}

inline fl_vertex2fragment fl_v_vertex_info(fl_vertex_input v, fl_vertex2fragment o) {
	#if defined(FL_COLORS_LOCAL) || defined(FL_MODEL_VERTEX)
		o.flModelVertex = v.vertex;
		o.flModelNormal = v.normal;
	#endif
	#if defined(FL_COLORS_WORLD) || defined(FL_SPOT_LIGHT) || defined(FL_POINT_LIGHT) || defined(FL_DIRECTIONAL_LIGHT) || defined(FL_RECEIVESHADOWS) || defined(FL_VERTEX_WORLD)
		o.flWorldVertex = mul(unity_ObjectToWorld, v.vertex); 
		o.flWorldNormal = UnityObjectToWorldNormal(v.normal);
	#endif

	return o;
}

inline fl_vertex2fragment FLvertex(fl_vertex_input v) {
	fl_vertex2fragment o;
	o.vertex = UnityObjectToClipPos(v.vertex);

	#ifdef FL_VERTEX_COLOR
		o.vertexColor = v.vertexColor;
	#endif

	o = fl_v_vertex_info(v,o);
	o = fl_v_textures_setup(v,o);

	return o;
}

////////////////////////////////////////////////////////////////////////////////////////////////////////////Fragment Shaders
inline half4 FLfragment(fl_vertex2fragment i) {

	half4 vertex = half4(0,0,0,0);
	half3 normal = half3(0,0,0);
	#if defined(FL_COLORS_LOCAL)
		vertex = i.flModelVertex;
		normal = i.flModelNormal;
	#endif
	#if defined(FL_COLORS_WORLD)
		vertex = i.flWorldVertex;
		normal = i.flWorldNormal;
	#endif

	fixed4 flatLight = fl_axis_light(normal, vertex);

	#ifdef FL_VERTEX_COLOR
		flatLight *= i.vertexColor;
	#endif

	#ifdef FL_MAIN_TEXTURE
		flatLight = fl_axis_apply_texture(flatLight, i.uv);
	#endif

	#if defined(FL_GRADIENT_WORLD) || defined(FL_GRADIENT_LOCAL)
		flatLight = CustomGradient(vertex, flatLight);
	#endif

	half4 light = half4(0, 0, 0, 0);
	half4 directLight = half4(0, 0, 0, 0);
	half4 ambientColor = half4(0, 0, 0, 0);

	#ifdef FL_AMBIENT_LIGHT
		ambientColor = flatLight * _Ambient_Light;
	#endif

	#ifdef FL_DIRECTIONAL_LIGHT
		directLight = directional_light(i.flWorldNormal);
	#endif

	#ifdef FL_SPOT_LIGHT
		half4 spotLight = spot_light(i.flWorldVertex);
		light += spotLight;
	#endif

	#ifdef FL_POINT_LIGHT
		half4 pointLight = point_light(i.flWorldVertex);

		#ifdef FL_SPOT_LIGHT
			pointLight = blend_spot_into_point_lights(spotLight, pointLight);
		#endif

		light += pointLight;
	#endif

	#ifdef FL_LIGHTMAPPING
		half4 lightmap = CustomLightmap(i.lightmapUV);
		flatLight *= lightmap;
	#endif

	#ifdef FL_UNITY_LIGHTMAPPING
		half4 unity_lightmap = UnityLightmap(i.lightmapUV);
		flatLight *= unity_lightmap;
	#endif

	fixed4 result = flatLight + (light + ambientColor + directLight);

	#ifdef FL_RECEIVESHADOWS
		fixed4 shadow = get_shadow(i.flWorldVertex, result);
		result = max(shadow, light);
	#endif

	return result;
}

#endif // FLAT_LIGHTING_COMMON_INCLUDED