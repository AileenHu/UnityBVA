﻿Shader "Hidden/MetalGlossChannelSwap"
{
	Properties
	{
		_MainTex ("Texture", 2D) = "white" {}
	}
	SubShader
	{
		Cull Off ZWrite Off ZTest Always

		Pass
		{
			HLSLPROGRAM
			#pragma vertex vert
			#pragma fragment frag

			#include "Packages/com.unity.render-pipelines.universal/ShaderLibrary/Core.hlsl"

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
				o.vertex = GetVertexPositionInputs(v.vertex.xyz).positionCS;
				o.uv = v.uv;
				return o;
			}
			
			sampler2D _MainTex;

			float4 frag (v2f i) : SV_Target
			{
				float4 col = tex2D(_MainTex, i.uv);
				// From the GLTF 2.0 spec
				// The metallic-roughness texture. The metalness values are sampled from the B channel. 
				// The roughness values are sampled from the G channel. These values are linear. 
				// If other channels are present (R or A), they are ignored for metallic-roughness calculations.
				//
				// Unity, by default, puts metallic in R channel and glossiness in A channel.
				// Unity uses a metallic-gloss texture so we need to invert the value in the g channel.
				//
				// Conversion Summary
				// Unity R channel goes into B channel
				// Unity A channel goes into G channel, then inverted
				//return float4(1, 1 - col.a, col.r, 1);
				return col;
			}
			ENDHLSL
		}
	}
}
