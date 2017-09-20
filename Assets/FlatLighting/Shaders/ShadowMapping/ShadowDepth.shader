/// Credit to Bogdan Gochev for licensing this shader code to us. 
/// Usage of the Flat Lighting Shader from this SDK is licensed for Mira applications only.
/// Flat lighting asset: https://www.assetstore.unity3d.com/en/#!/content/67730

Shader "Hidden/ShadowDepth" {
	Properties {
	}
	SubShader
	{
		Tags { "RenderType"="Opaque" }
		Cull Off

		Pass
		{
			CGPROGRAM
			#pragma vertex vert
			#pragma fragment frag
			
			#include "UnityCG.cginc"

			#pragma multi_compile FL_CASTSHADOW_ON FL_CASTSHADOW_OFF

			struct appdata {
				float4 vertex : POSITION;
			};

			struct v2f {
				float4 pos : SV_POSITION;
				float4 depth : TEXCOORD0;
			};
			
			v2f vert (appdata v) {
				v2f o;

				#ifdef FL_CASTSHADOW_ON
					o.pos = UnityObjectToClipPos(v.vertex);
				#else
					o.pos = half4(100000.0, 100000.0, 100000.0, 100000.0);
				#endif


				UNITY_TRANSFER_DEPTH(o.depth);

				return o;
			}
			
			float4 frag (v2f i) : SV_Target {
				UNITY_OUTPUT_DEPTH(i.depth);
			}
			ENDCG
		}
	}
}
