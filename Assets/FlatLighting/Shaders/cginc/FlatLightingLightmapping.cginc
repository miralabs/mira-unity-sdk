/// Credit to Bogdan Gochev for licensing this shader code to us. 
/// Usage of the Flat Lighting Shader from this SDK is licensed for Mira applications only.
/// Flat lighting asset: https://www.assetstore.unity3d.com/en/#!/content/67730

#ifndef FLAT_LIGHTING_LIGHTMAPPING_INCLUDED
#define FLAT_LIGHTING_LIGHTMAPPING_INCLUDED

#define LIGHTMAP_COORDINATES half2 lightmap0 : TEXCOORD0; half2 lightmap1 : TEXCOORD1;

#ifdef FL_LIGHTMAPPING
	UNITY_DECLARE_TEX2D(_CustomLightmap);
	uniform float4 _CustomLightmap_ST;
	uniform half4 _ShadowTint;
	uniform half _ShadowBoost;
	uniform half _UVChannel;

	inline half2 GetLightmapUV(half2 uv0, half2 uv1) {
		return step(0.15, 1-_UVChannel) * uv0 + step(0.15, _UVChannel) * uv1;
	}

	inline half4 CustomLightmap(half2 lightmapUV) {
		half4 lightmap = UNITY_SAMPLE_TEX2D(_CustomLightmap, lightmapUV);

	//	we need to change black<->white to shadow tint<->white
	//	scale a range [min,max] to [a,b] according to :
	//	       (b-a)(x - min)
	//	f(x) = --------------  + a
	//	          max - min
	//	Our function was simpplified because of range [0,1] for [min, max]

		lightmap.r = ((1-_ShadowTint.r) * lightmap.r * (1-_ShadowBoost))+_ShadowTint.r;
		lightmap.g = ((1-_ShadowTint.g) * lightmap.g * (1-_ShadowBoost))+_ShadowTint.g;
		lightmap.b = ((1-_ShadowTint.b) * lightmap.b * (1-_ShadowBoost))+_ShadowTint.b;
		
		return lightmap;
	}
#endif

#ifdef FL_UNITY_LIGHTMAPPING
    uniform half _UVChannel;

    inline half2 GetLightmapUV(half2 uv0, half2 uv1) {
		return step(0.15, 1-_UVChannel) * uv0 + step(0.15, _UVChannel) * uv1;
	}

    inline half2 calculateUnityLightmapVertexCoordinates(half2 uv) {
    	half2 vertexLightmapCoordinates = uv * unity_LightmapST.xy + unity_LightmapST.zw;
    	return vertexLightmapCoordinates;
    }

    inline half4 UnityLightmap(half2 lightmapUV) {
    	half3 lightmap = DecodeLightmap(UNITY_SAMPLE_TEX2D(unity_Lightmap, lightmapUV));
    	return half4(lightmap, 1.0);
    }

#endif

#endif // FLAT_LIGHTING_LIGHTMAPPING_INCLUDED