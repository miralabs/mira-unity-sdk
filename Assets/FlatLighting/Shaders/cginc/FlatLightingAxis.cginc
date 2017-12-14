/// Credit to Bogdan Gochev for licensing this shader code to us. 
/// Usage of the Flat Lighting Shader from this SDK is licensed for Mira applications only.
/// Flat lighting asset: https://www.assetstore.unity3d.com/en/#!/content/67730

#ifndef FLAT_LIGHTING_AXIS_INCLUDED
#define FLAT_LIGHTING_AXIS_INCLUDED

static const fixed3 _POSITIVE_X = fixed3(1,0,0);
static const fixed3 _NEGATIVE_X = fixed3(-1,0,0);
static const fixed3 _NEGATIVE_Y = fixed3(0,-1,0);
static const fixed3 _POSITIVE_Y = fixed3(0,1,0);
static const fixed3 _NEGATIVE_Z = fixed3(0,0,-1);
static const fixed3 _POSITIVE_Z = fixed3(0,0,1);	

uniform fixed4 _LightPositiveX;
uniform fixed4 _LightNegativeX;
uniform fixed4 _LightNegativeZ;
uniform fixed4 _LightPositiveZ;
uniform fixed4 _LightNegativeY;
uniform fixed4 _LightPositiveY;

inline fixed4 mix_tree_axis_color(half3 normal, fixed4 ColorX, fixed4 ColorY, fixed4 ColorZ) {
	half3 blendWeights = abs(normalize(normal));
	blendWeights = saturate(blendWeights / (blendWeights.x + blendWeights.y + blendWeights.z));
	return (ColorX * blendWeights.x) + (ColorY * blendWeights.y) + (ColorZ * blendWeights.z);
}

inline fixed4 mix_six_axis_color(half3 normal, 
	fixed4 ColorPositiveX, 
	fixed4 ColorPositiveY, 
	fixed4 ColorPositiveZ,
	fixed4 ColorNegativeX, 
	fixed4 ColorNegativeY,
	fixed4 ColorNegativeZ) {

	half3 blendWeights = normalize(normal);
	blendWeights = (blendWeights / (abs(blendWeights.x) + abs(blendWeights.y) + abs(blendWeights.z)));
	return (ColorPositiveX * max(0, blendWeights.x)) + 
			(ColorPositiveY * max(0, blendWeights.y)) + 
			(ColorPositiveZ * max(0, blendWeights.z)) +
			(ColorNegativeX * abs(min(0, blendWeights.x))) + 
			(ColorNegativeY * abs(min(0, blendWeights.y))) + 
			(ColorNegativeZ * abs(min(0, blendWeights.z)));
}

#if defined (FL_GRADIENT_AXIS_ON_X)
	uniform fixed4 _LightPositive2X;
	uniform half _GradientOriginOffsetPositiveX;
	uniform half _GradientWidthPositiveX;

	#if defined(FL_SYMETRIC_COLORS_OFF)
		uniform fixed4 _LightNegative2X;
		uniform half _GradientOriginOffsetNegativeX;
		uniform half _GradientWidthNegativeX;
	#endif
#endif

#if defined (FL_GRADIENT_AXIS_ON_Y)
	uniform fixed4 _LightPositive2Y;
	uniform half _GradientOriginOffsetPositiveY;
	uniform half _GradientWidthPositiveY;

	#if defined(FL_SYMETRIC_COLORS_OFF)
		uniform fixed4 _LightNegative2Y;
		uniform half _GradientOriginOffsetNegativeY;
		uniform half _GradientWidthNegativeY;
	#endif
#endif

#if defined (FL_GRADIENT_AXIS_ON_Z)
	uniform fixed4 _LightPositive2Z;
	uniform half _GradientOriginOffsetPositiveZ;
	uniform half _GradientWidthPositiveZ;

	#if defined(FL_SYMETRIC_COLORS_OFF)
		uniform fixed4 _LightNegative2Z;
		uniform half _GradientOriginOffsetNegativeZ;
		uniform half _GradientWidthNegativeZ;
	#endif
#endif

#if defined(FL_GRADIENT_AXIS_ON_X) || defined(FL_GRADIENT_AXIS_ON_Y) || defined(FL_GRADIENT_AXIS_ON_Z)
	inline fixed4 gradient_axis_color(
		fixed3 axisGradient,
		half3 vertexNormal,
		half4 vertexGradientPosition,
		half4 gradientCenterPosition,
		fixed4 color1, 
		fixed4 color2,
		half gradientOffset, 
		half gradientWidth) {

		half gradientCenter = length(axisGradient * gradientCenterPosition) + gradientOffset;
		half minGradient = gradientCenter - gradientWidth;
		half maxGradient = gradientCenter + gradientWidth;
		fixed gradientInfluence = dot(axisGradient, vertexGradientPosition);
		fixed gradient = smoothstep(minGradient, maxGradient, gradientInfluence);
		fixed4 gradientColor = lerp(color2, color1, gradient);

		return gradientColor;
	}
#endif

inline fixed4 fl_axis_light(half3 normal, half4 vertex) {
	fixed4 flatLightColor = fixed4(0,0,0,0);

	//Local by default
	half4 centerGradientPosition = half4(0.0, 0.0, 0.0, 0.0);
	#if (defined(FL_GRADIENT_AXIS_ON_X) || defined(FL_GRADIENT_AXIS_ON_Y) || defined(FL_GRADIENT_AXIS_ON_Z)) && defined(FL_COLORS_WORLD)
		centerGradientPosition = mul(unity_ObjectToWorld, half4(0.0, 0.0, 0.0, 0.0));
	#endif

	#if defined(FL_SYMETRIC_COLORS_OFF)
		fixed4 ColorPositiveX, ColorNegativeX;
		fixed4 ColorPositiveY, ColorNegativeY;
		fixed4 ColorPositiveZ, ColorNegativeZ;

		#if defined(FL_GRADIENT_AXIS_ON_X)
			ColorPositiveX = gradient_axis_color(_POSITIVE_Y, normal, vertex, centerGradientPosition,
				_LightPositiveX, _LightPositive2X, _GradientOriginOffsetPositiveX, _GradientWidthPositiveX);
			ColorNegativeX = gradient_axis_color(_POSITIVE_Y, normal, vertex, centerGradientPosition, 
				_LightNegativeX, _LightNegative2X, _GradientOriginOffsetNegativeX, _GradientWidthNegativeX);
		#else
			ColorPositiveX = _LightPositiveX;
			ColorNegativeX = _LightNegativeX;
		#endif

		#if defined(FL_GRADIENT_AXIS_ON_Y)
			ColorPositiveY = gradient_axis_color(_POSITIVE_Z, normal, vertex, centerGradientPosition, 
				_LightPositiveY, _LightPositive2Y, _GradientOriginOffsetPositiveY, _GradientWidthPositiveY);
			ColorNegativeY = gradient_axis_color(_POSITIVE_Z, normal, vertex, centerGradientPosition, 
				_LightNegativeY, _LightNegative2Y, _GradientOriginOffsetNegativeY, _GradientWidthNegativeY);
		#else
			ColorPositiveY = _LightPositiveY;
			ColorNegativeY = _LightNegativeY;
		#endif

		#if defined(FL_GRADIENT_AXIS_ON_Z)
			ColorPositiveZ = gradient_axis_color(_POSITIVE_Y, normal, vertex, centerGradientPosition, 
				_LightPositiveZ, _LightPositive2Z, _GradientOriginOffsetPositiveZ, _GradientWidthPositiveZ);
			ColorNegativeZ = gradient_axis_color(_POSITIVE_Y, normal, vertex, centerGradientPosition, 
				_LightNegativeZ, _LightNegative2Z, _GradientOriginOffsetNegativeZ, _GradientWidthNegativeZ);
		#else
			ColorPositiveZ = _LightPositiveZ;
			ColorNegativeZ = _LightNegativeZ;
		#endif

		flatLightColor = mix_six_axis_color(normal, ColorPositiveX, ColorPositiveY, ColorPositiveZ,
													ColorNegativeX, ColorNegativeY, ColorNegativeZ);
	#else
		fixed4 ColorPositiveX;
		fixed4 ColorPositiveY;
		fixed4 ColorPositiveZ;

		#if defined(FL_GRADIENT_AXIS_ON_X)
			ColorPositiveX = gradient_axis_color(_POSITIVE_Y, normal, vertex, centerGradientPosition,
				_LightPositiveX, _LightPositive2X, _GradientOriginOffsetPositiveX, _GradientWidthPositiveX);
		#else
			ColorPositiveX = _LightPositiveX;
		#endif

		#if defined(FL_GRADIENT_AXIS_ON_Y)
			ColorPositiveY = gradient_axis_color(_POSITIVE_Z, normal, vertex, centerGradientPosition,
				_LightPositiveY, _LightPositive2Y, _GradientOriginOffsetPositiveY, _GradientWidthPositiveY);
		#else
			ColorPositiveY = _LightPositiveY;
		#endif

		#if defined(FL_GRADIENT_AXIS_ON_Z)
			ColorPositiveZ = gradient_axis_color(_POSITIVE_Y, normal, vertex, centerGradientPosition,
				_LightPositiveZ, _LightPositive2Z, _GradientOriginOffsetPositiveZ, _GradientWidthPositiveZ);
		#else
			ColorPositiveZ = _LightPositiveZ;
		#endif

		flatLightColor = mix_tree_axis_color(normal, ColorPositiveX, ColorPositiveY, ColorPositiveZ);
	#endif

	return flatLightColor;
}

UNITY_DECLARE_TEX2D(_MainTex);

inline fixed4 fl_axis_apply_texture(fixed4 flatLight, half2 uv) {
	fixed4 mainTexture = UNITY_SAMPLE_TEX2D(_MainTex, uv);
	return ((1-mainTexture.a)* flatLight) + (flatLight * mainTexture * mainTexture.a);
}

#endif // FLAT_LIGHTING_AXIS_INCLUDED