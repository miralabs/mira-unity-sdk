/// Credit to Bogdan Gochev for licensing this shader code to us. 
/// Usage of the Flat Lighting Shader from this SDK is licensed for Mira applications only.
/// Flat lighting asset: https://www.assetstore.unity3d.com/en/#!/content/67730

#ifndef FLAT_LIGHTING_SOURCES_INCLUDED
#define FLAT_LIGHTING_SOURCES_INCLUDED

#define FL_DIRECT_LIGHT_LIMIT 5 //Maximum number of direct lights sources.
#define FL_POINT_LIGHT_LIMIT 25 //Maximum number of lights sources per light type.
#define FL_SPOT_LIGHT_LIMIT 25 //Maximum number of lights sources per light type.

#ifdef FL_BLEND_LIGHT_SOURCES
	uniform fixed4 _BlendedLightColor;
	uniform half4 _BlendedLightIntensities;
	uniform fixed _BlendedLightSmoothness;

	inline half4 blend_same_light_sources_types(half4 light0, half4 light1) {
		half light0Bright = dot(light0, light0);
		half light1Bright = dot(light1, light1);

		fixed blendFalloff = light0Bright > light1Bright;
		half4 blendLight = blendFalloff * light0 + (1-blendFalloff) * light1;

		return blendLight;
	}
#endif

#ifdef FL_AMBIENT_LIGHT
	uniform half4 _Ambient_Light;
#endif

#ifdef FL_DIRECTIONAL_LIGHT
	uniform fixed4 _DirectionalLightColor[FL_DIRECT_LIGHT_LIMIT];
	uniform int _DirectionalLight_Length;
	uniform half3 _DirectionalLightForward[FL_DIRECT_LIGHT_LIMIT];

	inline half4 directional_light(half3 worldSpaceNormal) {
		half4 directionalColor;
		for (int i = 0; i < _DirectionalLight_Length; i++) {
			fixed4 directionalFalloff = saturate(dot(normalize(worldSpaceNormal), _DirectionalLightForward[i]));
			directionalColor += directionalFalloff * _DirectionalLightColor[i];
		}

		return saturate(directionalColor);
	}
#endif

#ifdef FL_SPOT_LIGHT
	uniform int _SpotLight_Length;
	uniform float4x4 _SpotLightMatrixC0[FL_SPOT_LIGHT_LIMIT];
	uniform half4 _SpotLightObjectSpaceForward[FL_SPOT_LIGHT_LIMIT];
	uniform half _SpotLightBaseRadius[FL_SPOT_LIGHT_LIMIT];
	uniform half _SpotLightHeight[FL_SPOT_LIGHT_LIMIT];
	uniform half4 _SpotLightDistances[FL_SPOT_LIGHT_LIMIT];

	#if !defined(FL_BLEND_LIGHT_SOURCES)
		uniform half4 _SpotLightColor[FL_SPOT_LIGHT_LIMIT];
		uniform half4 _SpotLightIntensities[FL_SPOT_LIGHT_LIMIT];
		uniform half _SpotLightSmoothness[FL_SPOT_LIGHT_LIMIT]; // 0..1 range
	#endif

	inline fixed4 spot_light_color(int i, float4 vertexWorldPosition, fixed smoothness, half4 lightIntensities, fixed4 lightColor) {
			half3 spotLightC0 = (mul(_SpotLightMatrixC0[i], vertexWorldPosition)).xyz;
			half spotLightDistance = max(min(dot(spotLightC0 , _SpotLightObjectSpaceForward[i]), _SpotLightHeight[i]), 0);
			half spotLightRadius = (spotLightDistance / _SpotLightHeight[i]) * _SpotLightBaseRadius[i];
			half spotLightInfluence = length(spotLightC0 - (spotLightDistance * _SpotLightObjectSpaceForward[i]));
			half4 spotLightInflunceVector = half4(spotLightInfluence, spotLightInfluence, spotLightInfluence, spotLightInfluence);

			half4 smoothDistance =  smoothness * smoothstep(_SpotLightDistances[i], half4(0,0,0,0), spotLightInflunceVector);
			half4 sharpDistance = (1.0 - smoothness) * step(spotLightInflunceVector, _SpotLightDistances[i]);
			half4 lightDistances = smoothDistance + sharpDistance;
			half intensity = dot (lightDistances, lightIntensities);

			return step(spotLightInfluence, spotLightRadius) * intensity * lightColor;
	}

	inline half4 spot_light(float4 vertexWorldPosition) {
		half4 spotLightColor;
		for (int i = 0; i < _SpotLight_Length; i++) {
			#if defined(FL_BLEND_LIGHT_SOURCES)
				fixed4 newSpotLightColor = spot_light_color(i, vertexWorldPosition, _BlendedLightSmoothness, _BlendedLightIntensities, _BlendedLightColor);
				spotLightColor = blend_same_light_sources_types(spotLightColor, newSpotLightColor);
			#else
				spotLightColor += spot_light_color(i, vertexWorldPosition, _SpotLightSmoothness[i], _SpotLightIntensities[i], _SpotLightColor[i]);
			#endif
		}

		return saturate(spotLightColor);
	}
#endif

#ifdef FL_POINT_LIGHT
	uniform int _PointLight_Length;
	uniform float4x4 _PointLightMatrixC0[FL_POINT_LIGHT_LIMIT];
	uniform half4 _PointLightDistances[FL_POINT_LIGHT_LIMIT];

	#if !defined(FL_BLEND_LIGHT_SOURCES)
		uniform half4 _PointLightColor[FL_POINT_LIGHT_LIMIT];
		uniform half4 _PointLightIntensities[FL_POINT_LIGHT_LIMIT];
		uniform half _PointLightSmoothness[FL_POINT_LIGHT_LIMIT]; // 0..1 range
	#endif

	inline fixed4 point_light_color(int i, float4 vertexWorldPosition, fixed smoothness, half4 lightIntensities, fixed4 lightColor) {
			half3 pointLightC0 = (mul(_PointLightMatrixC0[i], vertexWorldPosition)).xyz;
			half pointLoghtInfluence = sqrt(dot(pointLightC0,pointLightC0));
			half4 pointLightInflunceVector = half4(pointLoghtInfluence, pointLoghtInfluence, pointLoghtInfluence, pointLoghtInfluence);
			half4 smoothDistance =  smoothness * smoothstep(_PointLightDistances[i], half4(0,0,0,0), pointLightInflunceVector);
			half4 sharpDistance = (1 - smoothness) * step(pointLightInflunceVector, _PointLightDistances[i]);
			half4 lightDistances = smoothDistance + sharpDistance;
			half intensity = dot (lightDistances, lightIntensities);

			return intensity  * lightColor;
	}

	inline half4 point_light(float4 vertexWorldPosition) {
		half4 pointLightColor;
		for (int i = 0; i < _PointLight_Length; i++) {
			#if defined(FL_BLEND_LIGHT_SOURCES)
				fixed4 newPointLightColor = point_light_color(i, vertexWorldPosition, _BlendedLightSmoothness, _BlendedLightIntensities, _BlendedLightColor);
				pointLightColor = blend_same_light_sources_types(pointLightColor, newPointLightColor);
			#else
				pointLightColor += point_light_color(i, vertexWorldPosition, _PointLightSmoothness[i], _PointLightIntensities[i], _PointLightColor[i]);
			#endif
		}

		return saturate(pointLightColor);
	}
#endif

#ifdef FL_POINT_LIGHT
	#ifdef FL_SPOT_LIGHT
		inline half4 blend_spot_into_point_lights(half4 spotLight, half4 pointLight) {
			half spotBright = dot(spotLight, spotLight);
			half pointBright = dot(pointLight, pointLight);

//			if (spotBright > pointBright) {
//				pointLight = half4(0,0,0,0);
//			} else {
//				light -=spotLight; // accumulated light (ambien+direect+spot)
//			}
//NOTE: The code below is optimized, but in reality is just the above if statment
			half4 blendLight = pointLight;
			fixed blendFalloff = spotBright > pointBright;
			blendLight *= (1-blendFalloff);
			blendLight += -(1-blendFalloff)*spotLight;

			return blendLight;
		}
	#endif

#endif

#endif // FLAT_LIGHTING_SOURCES_INCLUDED