/// Credit to Bogdan Gochev for licensing this shader code to us. 
/// Usage of the Flat Lighting Shader from this SDK is licensed for Mira applications only.
/// Flat lighting asset: https://www.assetstore.unity3d.com/en/#!/content/67730

#ifndef FLAT_LIGHTING_GRADIENT_INCLUDED
#define FLAT_LIGHTING_GRADIENT_INCLUDED

#if defined(FL_GRADIENT_LOCAL) || defined(FL_GRADIENT_WORLD)
	uniform half4 _GradienColorGoal;
	uniform half3 _GradientUnitAxis;
	uniform half _GradientWidth;
	uniform half _GradientOffset;
	uniform fixed _GradientBlending;

	inline fixed4 calculate_gradient(half4 vertexGradientPosition, half4 centerGradientPosition, fixed4 color) {
		half axisInfluence = dot(_GradientUnitAxis, vertexGradientPosition);
		half minGradient = centerGradientPosition + _GradientOffset - _GradientWidth;
		half maxGradient = centerGradientPosition + _GradientOffset + _GradientWidth;
		half gradient = smoothstep(minGradient, maxGradient, axisInfluence);
		fixed4 targetColor = (color * (1 - _GradienColorGoal.a)) +
							 (_GradienColorGoal * _GradienColorGoal.a) + 
							 ((Luminance(color) * _GradientBlending) * _GradienColorGoal.a);
		fixed4 gradientColor = lerp(targetColor, color, gradient);
		return gradientColor;
	}

	inline fixed4 CustomGradient(half4 vertex, fixed4 flatLightingColor) {
		half4 centerGradientPosition = half4(0.0, 0.0, 0.0, 0.0);
		#if defined(FL_GRADIENT_WORLD)
			centerGradientPosition = mul(unity_ObjectToWorld, half4(0.0, 0.0, 0.0, 0.0));
		#endif

		return calculate_gradient(vertex, centerGradientPosition, flatLightingColor);
	}
#endif

#endif // FLAT_LIGHTING_GRADIENT_INCLUDED