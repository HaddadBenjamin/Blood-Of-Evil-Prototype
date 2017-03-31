// Upgrade NOTE: replaced 'mul(UNITY_MATRIX_MVP,*)' with 'UnityObjectToClipPos(*)'

Shader "guiraffe/SubstanceOrb"
{
	Properties
	{
		_Fill("Fill rate", Range(0, 1)) = 1

		[Space(10)]

		_SurfaceColor("Surface color", Color) = (1, 1, 1, 1)
		_Color("Accent color", Color) = (1, 1, 1, 1)
		_BaseColor("Base color", Color) = (1, 1, 1, 1)

		[Space(10)]

		_P1Mul("Particles 1 multiplier", Range(0, 1)) = 0.35
		_P2Mul("Particles 2 multiplier", Range(0, 1)) = 0.75

		[Space(10)]

		_Smoke1Tiling("Smoke 1 tiling", Float) = 0.8
		_Smoke2Tiling("Smoke 2 tiling", Float) = 0.75
		_Particles1Tiling("Particles 1 tiling", Float) = 1.0
		_Particles2Tiling("Particles 2 tiling", Float) = 0.5

		[Space(10)]

		[NoScaleOffset]_SurfaceTexture("Surface texture (Add)", 2D) = "black" {}

		[NoScaleOffset]_uva("UV+A map", 2D) = "black" {}
		[NoScaleOffset]_Smoke("Smoke", 2D) = "black" {}
		[NoScaleOffset]_Particles("Particles", 2D) = "black" {}
		[NoScaleOffset]_SAlphaTex("Surface alpha map", 2D) = "white" {}
		[NoScaleOffset]_Gradient("Gradient (Add)", 2D) = "black" {}
		[NoScaleOffset]_Overlay("Overlay (Add)", 2D) = "black" {}
		[NoScaleOffset]_Shadow("Shadow (Normal)", 2D) = "white" {}

		[HideInInspector]_SurfaceOffsetX("Surface Offset X", Float) = 0
		[HideInInspector]_Smoke1OffsetX("Smoke 1 Offset X", Float) = 0
		[HideInInspector]_Smoke1OffsetY("Smoke 1 Offset Y", Float) = 0
		[HideInInspector]_Smoke2OffsetX("Smoke 2 Offset X", Float) = 0
		[HideInInspector]_Smoke2OffsetY("Smoke 2 Offset Y", Float) = 0
		[HideInInspector]_Particles1OffsetX("Particles 1 Offset X", Float) = 0
		[HideInInspector]_Particles1OffsetY("Particles 1 Offset Y", Float) = 0
		[HideInInspector]_Particles2OffsetX("Particles 2 Offset X", Float) = 0
		[HideInInspector]_Particles2OffsetY("Particles 2 Offset Y", Float) = 0
	}

	SubShader
	{
		Tags{ "RenderType" = "Transparent" "IgnoreProjector" = "True" "Queue" = "Transparent" }
		ZWrite Off
		Blend One OneMinusSrcAlpha
		Cull Off
		Lighting Off

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

			sampler2D _uva;

			sampler2D _Smoke;
			sampler2D _Particles;

			sampler2D _Gradient;
			sampler2D _Overlay;
			sampler2D _Shadow;

			sampler2D _SAlphaTex;
			sampler2D _SurfaceTexture;

			float _Fill;
			float _P1Mul;
			float _P2Mul;

			float _Smoke1Tiling;
			float _Smoke2Tiling;
			float _Particles1Tiling;
			float _Particles2Tiling;

			float _SurfaceOffsetX;
			float _Smoke1OffsetX;
			float _Smoke1OffsetY;
			float _Smoke2OffsetX;
			float _Smoke2OffsetY;
			float _Particles1OffsetX;
			float _Particles1OffsetY;
			float _Particles2OffsetX;
			float _Particles2OffsetY;

			fixed4 _SurfaceColor;
			fixed4 _Color;
			fixed4 _BaseColor;

			v2f vert(appdata v)
			{
				v2f o;
				o.vertex = UnityObjectToClipPos(v.vertex);
				o.uv = v.uv;
				return o;
			}

			fixed4 frag(v2f i) : SV_Target
			{
				float4 uva = tex2D(_uva, i.uv);

				fixed4 col = fixed4(0, 0, 0, 0);
				fixed sAlpha = tex2D(_SAlphaTex, float2(_Fill, 0)).r;

				float fillSign = sign(_Fill - i.uv.y);

				// fill color
				fixed4 fillCol = _BaseColor;

				fillCol.rgb += tex2D(_Smoke, uva.rg * _Smoke1Tiling + float2(_Smoke1OffsetX, _Smoke1OffsetY)).rgb;
				fillCol.rgb -= 0.75 * tex2D(_Smoke, uva.rg * _Smoke2Tiling + float2(_Smoke2OffsetX, _Smoke2OffsetY)).rgb;

				fillCol.rgb += tex2D(_Gradient, i.uv).rgb;

				fillCol.rgb *= _Color.rgb;

				fixed4 particleCol = tex2D(_Particles, uva.rg * _Particles1Tiling + float2(_Particles1OffsetX, _Particles1OffsetY));
				particleCol *= particleCol.a;
				fillCol.rgb += _P1Mul * particleCol.rgb;

				particleCol = tex2D(_Particles, uva.rg * _Particles2Tiling + float2(_Particles2OffsetX, _Particles2OffsetY));
				particleCol *= particleCol.a;
				fillCol.rgb += _P2Mul * particleCol.rgb;

				fixed4 surfaceCol = sAlpha * 0.9 * tex2D(_SurfaceTexture, float2(i.uv.x + _SurfaceOffsetX, i.uv.y - _Fill - 0.005f));
				surfaceCol *= surfaceCol.a;
				fillCol.rgb += surfaceCol.rgb;


				// surface color
				fixed4 surfaceCol1 = sAlpha * _SurfaceColor * tex2D(_SurfaceTexture, float2(i.uv.x + 0.5 + _SurfaceOffsetX, -i.uv.y + _Fill - 0.005f));
				surfaceCol1.rgb *= surfaceCol1.a;
				fixed4 surfaceCol2 = sAlpha * _SurfaceColor * tex2D(_SurfaceTexture, float2(-i.uv.x + 0.2 + _SurfaceOffsetX, -i.uv.y + _Fill - 0.005f));
				surfaceCol2 *= surfaceCol2.a;
				surfaceCol = surfaceCol1 + surfaceCol2;


				col += max(0, fillSign) * fillCol;
				col += max(0, -fillSign) * surfaceCol;

				fixed4 overlayCol = tex2D(_Overlay, i.uv);
				overlayCol.rgb *= overlayCol.a;
				col += overlayCol;

				fixed4 shadowCol = tex2D(_Shadow, i.uv);
				col.rgb = shadowCol.rgb + (1 - shadowCol.a) * col.rgb;
				col.a = shadowCol.a + (1 - shadowCol.a) * col.a;

				col *= uva.a;
				return col;
			}

			ENDCG
		}
	}
	Fallback "guiraffe/SubstanceOrb_SM2_0"
}
