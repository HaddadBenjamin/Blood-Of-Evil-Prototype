Shader "Ninjutsu Games/Map FOW"
{
	Properties
	{
		_Color ("_Color", Color) = (1,1,1,1)
		_MainTex ("Base (RGB) Trans (A)", 2D) = "white" {}
		_Revealed ("Revealed", 2D) = "white" {}
		_Hidden ("Hidden", 2D) = "white" {}
		_Mask ("Culling Mask", 2D) = "white" {}
		_MaskColor ("Discard Color", Color) = (1,1,1,1)
		_AlphaMultiplier ("Alpha Multiplier", float) = 2
	}
	SubShader
	{
		LOD 100

		Tags
		{
			"Queue" = "Transparent"
			"IgnoreProjector" = "True" 
			"RenderType" = "Transparent"
		}

		Pass
		{	
			Cull Off
			Lighting Off
			ZWrite Off
			Fog { Mode Off }
			ColorMask RGB
			AlphaTest Greater .01
			Offset -1, -1
			Blend SrcAlpha OneMinusSrcAlpha
			ColorMaterial AmbientAndDiffuse
			
			CGPROGRAM
				#pragma vertex Vert
				#pragma fragment Frag

				#include "UnityCG.cginc"
				
				uniform float _AlphaMultiplier;
				uniform float4 _Params;
				uniform fixed4 _Color;
				uniform float4x4 _Matrix; 
				uniform float4x4 _Matrix2; 
				uniform sampler2D _MainTex;
				uniform sampler2D _Mask;
				uniform sampler2D _Revealed;
				uniform sampler2D _Hidden;
				float3 _MaskColor;
				
				struct v2f
                {
                    float4 pos  : SV_POSITION;
                    float2 uv  : TEXCOORD0;
                    float2 uv2  : TEXCOORD1;
					fixed4 color : COLOR;
                };
                               
                void Vert(appdata_full i, out v2f o)
                {
					o.pos = mul(UNITY_MATRIX_MVP, i.vertex);
                    o.uv = mul(_Matrix, i.texcoord).xy;
					o.uv2 = mul(_Matrix2, i.texcoord).xy;    
					o.color = _Color;
                }
				
				void Frag(v2f i, out half4 o : COLOR)
                {
                    float4 tex = tex2D(_MainTex, i.uv.xy);
                    float4 r = tex2D(_Revealed, i.uv.xy);
                    float4 h = tex2D(_Hidden, i.uv.xy);

					tex.a = tex.a * _AlphaMultiplier;

					float4 mask = tex2D(_Mask, i.uv2);

					float4 res = tex * r.g;
					
                    o = (h + res) * mask * _Color;
                }
			ENDCG
		} 
	}
}