/// Credit to Bogdan Gochev for licensing this shader code to us. 
/// Usage of the Flat Lighting Shader from this SDK is licensed for Mira applications only.
/// Flat lighting asset: https://www.assetstore.unity3d.com/en/#!/content/67730

#ifndef FLAT_LIGHTING_BLEND_MODES_INCLUDED
#define FLAT_LIGHTING_BLEND_MODES_INCLUDED

/*
* Blend functions by Elringus
* https://gist.github.com/Elringus/d21c8b0f87616ede9014
*/

inline fixed G (fixed4 c) { return .299 * c.r + .587 * c.g + .114 * c.b; }
 
inline fixed4 Darken (fixed4 a, fixed4 b) { 
	fixed4 r = min(a, b);
	r.a = b.a;
	return r;
}

inline fixed4 Multiply (fixed4 a, fixed4 b) { 
	fixed4 r = a * b;
	r.a = b.a;
	return r;
}

inline fixed4 ColorBurn (fixed4 a, fixed4 b) { 
	fixed4 r = 1.0 - (1.0 - a) / b;
	r.a = b.a;
	return r;
}

inline fixed4 LinearBurn (fixed4 a, fixed4 b) { 
	fixed4 r = a + b - 1.0;
	r.a = b.a;
	return r;
}

inline fixed4 DarkerColor (fixed4 a, fixed4 b) { 
	fixed4 r = G(a) < G(b) ? a : b;
	r.a = b.a;
	return r; 
}

inline fixed4 Lighten (fixed4 a, fixed4 b) { 
	fixed4 r = max(a, b);
	r.a = b.a;
	return r;
}

inline fixed4 Screen (fixed4 a, fixed4 b) { 	
	fixed4 r = 1.0 - (1.0 - a) * (1.0 - b);
	r.a = b.a;
	return r;
}

inline fixed4 ColorDodge (fixed4 a, fixed4 b) { 
	fixed4 r = a / (1.0 - b);
	r.a = b.a;
	return r;
}

inline fixed4 LinearDodge (fixed4 a, fixed4 b) { 
	fixed4 r = a + b;
	r.a = b.a;
	return r;
} 

inline fixed4 LighterColor (fixed4 a, fixed4 b) { 
	fixed4 r = G(a) > G(b) ? a : b;
	r.a = b.a;
	return r; 
}

inline fixed4 Overlay (fixed4 a, fixed4 b) {
	fixed4 r = a > .5 ? 1.0 - 2.0 * (1.0 - a) * (1.0 - b) : 2.0 * a * b;
	r.a = b.a;
	return r;
}

inline fixed4 SoftLight (fixed4 a, fixed4 b) {
	fixed4 r = (1.0 - a) * a * b + a * (1.0 - (1.0 - a) * (1.0 - b));
	r.a = b.a;
	return r;
}

inline fixed4 HardLight (fixed4 a, fixed4 b) {
	fixed4 r = b > .5 ? 1.0 - (1.0 - a) * (1.0 - 2.0 * (b - .5)) : a * (2.0 * b);
	r.a = b.a;
	return r;
}

inline fixed4 VividLight (fixed4 a, fixed4 b) {
	fixed4 r = b > .5 ? a / (1.0 - (b - .5) * 2.0) : 1.0 - (1.0 - a) / (b * 2.0);
	r.a = b.a;
	return r;
}

inline fixed4 LinearLight (fixed4 a, fixed4 b) {
	fixed4 r = b > .5 ? a + 2.0 * (b - .5) : a + 2.0 * b - 1.0;
	r.a = b.a;
	return r;
}

inline fixed4 PinLight (fixed4 a, fixed4 b) {
	fixed4 r = b > .5 ? max(a, 2.0 * (b - .5)) : min(a, 2.0 * b);
	r.a = b.a;
	return r;
}

inline fixed4 HardMix (fixed4 a, fixed4 b) {
	fixed4 r = (b > 1.0 - a) ? 1.0 : .0;
	r.a = b.a;
	return r;
}

inline fixed4 Difference (fixed4 a, fixed4 b) { 
	fixed4 r = abs(a - b);
	r.a = b.a;
	return r; 
}

inline fixed4 Exclusion (fixed4 a, fixed4 b) { 
	fixed4 r = a + b - 2.0 * a * b;
	r.a = b.a;
	return r; 
}

inline fixed4 Subtract (fixed4 a, fixed4 b) { 
	fixed4 r = a - b;
	r.a = b.a;
	return r; 
}

inline fixed4 Divide (fixed4 a, fixed4 b) { 
	fixed4 r = a / b;
	r.a = b.a;
	return r; 
}

#endif // FLAT_LIGHTING_BLEND_MODES_INCLUDED