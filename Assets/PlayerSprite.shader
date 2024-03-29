﻿Shader "Sprites/Player"
{
	Properties
	{
		[PerRendererData] _MainTex("Sprite Texture", 2D) = "white" {}
		_Color("Tint", Color) = (1,1,1,1)
		_Secondary("Tint", Color) = (1,1,1,1)
		[Toggle]_IsWall("Is Wall", int) = 0
		[MaterialToggle] PixelSnap("Pixel snap", Float) = 0
		[HideInInspector] _RendererColor("RendererColor", Color) = (1,1,1,1)
		[HideInInspector] _Flip("Flip", Vector) = (1,1,1,1)
		[PerRendererData] _AlphaTex("External Alpha", 2D) = "white" {}
		[PerRendererData] _EnableExternalAlpha("Enable External Alpha", Float) = 0
	}

		SubShader
		{
			Tags
			{
				"Queue" = "Transparent"
				"IgnoreProjector" = "True"
				"RenderType" = "Transparent"
				"PreviewType" = "Plane"
				"CanUseSpriteAtlas" = "True"
			}

			Cull Off
			Lighting Off
			ZWrite Off
			Blend One OneMinusSrcAlpha

			Pass
			{
			CGPROGRAM
				#pragma vertex SpriteVert
				#pragma fragment SpriteFrag
				#pragma target 2.0
				#pragma multi_compile_instancing
				#pragma multi_compile _ PIXELSNAP_ON
				#pragma multi_compile _ ETC1_EXTERNAL_ALPHA
				#include "UnityCG.cginc"

				#ifdef UNITY_INSTANCING_ENABLED

					UNITY_INSTANCING_BUFFER_START(PerDrawSprite)
					// SpriteRenderer.Color while Non-Batched/Instanced.
					UNITY_DEFINE_INSTANCED_PROP(float4, unity_SpriteRendererColorArray)
					// this could be smaller but that's how bit each entry is regardless of type
					UNITY_DEFINE_INSTANCED_PROP(fixed2, unity_SpriteFlipArray)
					UNITY_INSTANCING_BUFFER_END(PerDrawSprite)

					#define _RendererColor  UNITY_ACCESS_INSTANCED_PROP(PerDrawSprite, unity_SpriteRendererColorArray)
					#define _Flip           UNITY_ACCESS_INSTANCED_PROP(PerDrawSprite, unity_SpriteFlipArray)

				#endif // instancing

				CBUFFER_START(UnityPerDrawSprite)
				#ifndef UNITY_INSTANCING_ENABLED
					float4 _RendererColor;
					fixed2 _Flip;
				#endif
					float _EnableExternalAlpha;
				CBUFFER_END

				// Material Color.
				float4 _Color;
				float4 _Secondary;

				struct appdata_t
				{
					float4 vertex   : POSITION;
					float4 color    : COLOR;
					float2 texcoord : TEXCOORD0;
					UNITY_VERTEX_INPUT_INSTANCE_ID
				};

				struct v2f
				{
					float4 vertex   : SV_POSITION;
					float4 color : COLOR;
					float2 texcoord : TEXCOORD0;
					float4 worldPos : TEXCOORD1;
					UNITY_VERTEX_OUTPUT_STEREO
				};

				inline float4 UnityFlipSprite(in float3 pos, in fixed2 flip)
				{
					return float4(pos.xy * flip, pos.z, 1.0);
				}

				v2f SpriteVert(appdata_t IN)
				{
					v2f OUT;

					UNITY_SETUP_INSTANCE_ID(IN);
					UNITY_INITIALIZE_VERTEX_OUTPUT_STEREO(OUT);

					OUT.vertex = UnityFlipSprite(IN.vertex, _Flip);
					OUT.vertex = UnityObjectToClipPos(OUT.vertex);
					OUT.texcoord = IN.texcoord;
					OUT.color = IN.color * _Color * _RendererColor;
					OUT.worldPos = mul(unity_ObjectToWorld, IN.vertex);

					#ifdef PIXELSNAP_ON
					OUT.vertex = UnityPixelSnap(OUT.vertex);
					#endif

					return OUT;
				}

				sampler2D _MainTex;
				sampler2D _AlphaTex;
				bool _IsWall;
				bool dashing;

				float4 SampleSpriteTexture(float2 uv)
				{
					float4 color = tex2D(_MainTex, uv);

				#if ETC1_EXTERNAL_ALPHA
					float4 alpha = tex2D(_AlphaTex, uv);
					color.a = lerp(color.a, alpha.r, _EnableExternalAlpha);
				#endif

					return color;
				}

				struct buffers
				{
					float4 color : COLOR0;
					half mask : COLOR1;
				};

				//from https://gist.github.com/patriciogonzalezvivo/670c22f3966e662d2f83
				float rand(float2 n) {
					return frac(sin(dot(n, float2(12.9898, 4.1414))) * 43758.5453);
				}

				float noise(float2 p) {
					float2 ip = floor(p);
					float2 u = frac(p);
					u = u * u*(3.0 - 2.0*u);

					float res = lerp(
						lerp(rand(ip), rand(ip + float2(1.0, 0.0)), u.x),
						lerp(rand(ip + float2(0.0, 1.0)), rand(ip + float2(1.0, 1.0)), u.x), u.y);
					return res * res;
				}

				buffers SpriteFrag(v2f IN)
				{
					buffers o = (buffers)0;
					float2 uv = IN.texcoord;
					float4 c = SampleSpriteTexture(uv) * IN.color;
					if (dashing)
					{
						float n = noise(IN.worldPos*10. + float2(_Time.y*4.4, _Time.x *20.))*0.5;
						c.a *= 0.5 + n;
						c.rgb = lerp(c.rgb, _Secondary, n*2);
					}
					c.rgb *= c.a;
					
					o.color = c;
					if (_IsWall)
						o.mask = 1;
					else
						o.mask = 0;
					return o;
				}
				ENDCG
			}
		}
}