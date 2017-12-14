Shader "Mira/MiraReticle"
{
	Properties
	{
		_MainTex ("Texture", 2D) = "white" {}
		_ColorMult ("ColorMult", Color) = (1,1,1,1)
	}
	SubShader
	{
		Cull Off 
		AlphaTest Off
		Lighting Off
		ZWrite Off 
		ZTest Always
		Tags {"Queue"="Overlay" "IgnoreProjector"="True" "RenderType"="Transparent" }
		Blend SrcAlpha OneMinusSrcAlpha

		Pass
		{
			CGPROGRAM
			#pragma vertex vert
			#pragma fragment frag
			
			#include "UnityCG.cginc"

			struct appdata
			{
				float4 vertex : POSITION;
				float2 uv : TEXCOORD0;
			};

			struct v2f
			{
				float2 uv : TEXCOORD0;
				float4 vertex : SV_POSITION;
			};

			v2f vert (appdata v)
			{
				v2f o;
				o.vertex = UnityObjectToClipPos(v.vertex);
				o.uv = v.uv;
				return o;
			}
			
			sampler2D _MainTex;
			fixed4 _ColorMult;

			fixed4 frag (v2f i) : SV_Target
			{
				fixed4 col = tex2D(_MainTex, i.uv);
				return col * _ColorMult;
			}
			ENDCG
		}
	}
}
