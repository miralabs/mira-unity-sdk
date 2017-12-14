/// Credit to Bogdan Gochev for licensing this shader code to us. 
/// Usage of the Flat Lighting Shader from this SDK is licensed for Mira applications only.
/// Flat lighting asset: https://www.assetstore.unity3d.com/en/#!/content/67730

#ifndef FLAT_LIGHTING_SHADOWS_INCLUDED
#define FLAT_LIGHTING_SHADOWS_INCLUDED

#define FL_SHADOWS_PROJECTORS_LIMIT 5 //Maximum number of shadow projectors.

#ifdef FL_RECEIVESHADOWS
	uniform int _ShadowProjectors_Length;
	uniform float4x4 _ShadowMapMat[FL_SHADOWS_PROJECTORS_LIMIT];
	uniform float4x4 _ShadowMapMV[FL_SHADOWS_PROJECTORS_LIMIT];
	uniform half4 _ShadowColor[FL_SHADOWS_PROJECTORS_LIMIT];
	uniform half _ShadowBlur[FL_SHADOWS_PROJECTORS_LIMIT];
	uniform float2 _ShadowCameraSettings[FL_SHADOWS_PROJECTORS_LIMIT];
	uniform sampler2D _ShadowMapTexture0;
	uniform sampler2D _ShadowMapTexture1;
	uniform sampler2D _ShadowMapTexture2;
	uniform sampler2D _ShadowMapTexture3;
	uniform sampler2D _ShadowMapTexture4;

	inline half3 calculate_shadows_coordinates(half4 vertexWorldPosition, int i) {
		half3 shadow;
		half2 shadowCoordinates = (mul(_ShadowMapMat[i], vertexWorldPosition)).xy; 
		half shadowDepth = -((mul(_ShadowMapMV[i], vertexWorldPosition)).z);
		shadow.xy = shadowCoordinates;
		shadow.z = shadowDepth;

		return shadow;
	}

	inline half check_shadow_sample(half2 sample, float shadowMapDepth, half3 shadowProperties, sampler2D shadowMapTexture, half shadowBlur) {
		half2 shadowSample = (shadowProperties.xy + (sample * shadowBlur));
		half shadowSampleDepth = tex2D(shadowMapTexture, shadowSample).x;
		#ifdef UNITY_REVERSED_Z
            shadowSampleDepth = 1.0 - shadowSampleDepth;
        #endif
		half isOutsideShadowUVBoundaries = 1.0 - (shadowProperties.x >= 1.0 || shadowProperties.y >= 1.0 || shadowProperties.x <= 0.0 || shadowProperties.y <= 0.0);
		half isInShadow = isOutsideShadowUVBoundaries * half(shadowMapDepth >= shadowSampleDepth);

		return isInShadow;
	}

	inline half check_is_in_light_with_blur(float shadowMapDepth, half3 shadowProperties, sampler2D shadowMapTexture, half shadowBlur) {
		half isShadowRight = check_shadow_sample(half2(1.0, 0.0), shadowMapDepth, shadowProperties, shadowMapTexture, shadowBlur);
		half isShadowLeft = check_shadow_sample(half2(-1.0, 0.0), shadowMapDepth, shadowProperties, shadowMapTexture, shadowBlur);
		half isShadowUp = check_shadow_sample(half2(0.0, 1.0), shadowMapDepth, shadowProperties, shadowMapTexture, shadowBlur);
		half isShadowDown = check_shadow_sample(half2(0.0, -1.0), shadowMapDepth, shadowProperties, shadowMapTexture, shadowBlur);
		half isInShadow = ((isShadowRight + isShadowLeft + isShadowUp + isShadowDown)/ 4.0);

		return isInShadow;
	}

	inline fixed check_is_in_light_single_sample(float shadowMapDepth, half3 shadowProperties, sampler2D shadowMapTexture, half shadowBlur) {
		fixed isInShadow = check_shadow_sample(half2(0.0, 0.0), shadowMapDepth, shadowProperties, shadowMapTexture, shadowBlur);

		return isInShadow;
	}

	inline fixed4 get_shadow(half4 vertexWorldPosition, half4 currentLightColor) {

		fixed isInShadow = 0;
		fixed4 shadowColor = fixed4(0,0,0,0);
		for (int i = 0; i < _ShadowProjectors_Length; i++) {
			half3 shadowProperties = calculate_shadows_coordinates(vertexWorldPosition, i);

			half shadowMapDepth = ((shadowProperties.z - _ShadowCameraSettings[i].y) / _ShadowCameraSettings[i].x);
			fixed isInShadowLocal = 0;

			if (i == 0) {
				isInShadowLocal = check_is_in_light_with_blur(shadowMapDepth, shadowProperties, _ShadowMapTexture0, _ShadowBlur[i]);
			} else if (i == 1) {
				isInShadowLocal = check_is_in_light_with_blur(shadowMapDepth, shadowProperties, _ShadowMapTexture1, _ShadowBlur[i]);
			} else if (i == 2) {
				isInShadowLocal = check_is_in_light_with_blur(shadowMapDepth, shadowProperties, _ShadowMapTexture2, _ShadowBlur[i]);
			} else if (i == 3) {
				isInShadowLocal = check_is_in_light_with_blur(shadowMapDepth, shadowProperties, _ShadowMapTexture3, _ShadowBlur[i]);
			} else {
				isInShadowLocal = check_is_in_light_with_blur(shadowMapDepth, shadowProperties, _ShadowMapTexture4, _ShadowBlur[i]);
			}

			isInShadow += saturate(isInShadowLocal);
			shadowColor = max(isInShadowLocal * _ShadowColor[i], shadowColor);
		}

		fixed4 finalLight = currentLightColor * (isInShadow * shadowColor + (1-isInShadow));
		return finalLight;
	}

#endif

#endif // FLAT_LIGHTING_SHADOWS_INCLUDED