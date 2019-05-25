Shader "Hidden/composite"
{
    Properties
    {
        _MainTex("Texture", 2D) = "white" {}
		_FogOfWar("Texture", 2D) = "white" {}
		_ViewMask("Texture", 2D) = "white" {}
		_FogOfWarBrightness("Fog Of War Brightness", float) = 0.5
    }
    SubShader
    {
        // No culling or depth
        Cull Off ZWrite Off ZTest Always

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
			sampler2D _FogOfWar;
			sampler2D _ViewMask;
			float _FogOfWarBrightness;
			float4 cameraStartEnd;
			float4 FOWStartEnd;
			float2 FOWsize;
			float2 cameraSize;
			float cameraAngle;

            fixed4 frag (v2f i) : SV_Target
            {
                fixed4 col = tex2D(_MainTex, i.uv);
				fixed viewMask = tex2D(_ViewMask, i.uv);

				float2 ratio = cameraSize / FOWsize;

				float c = cos(cameraAngle);
				float s = sin(cameraAngle);
				float2x2 rot = float2x2(c, -s, s, c);

				float2 FOWtopRight = FOWStartEnd.zw;
				float2 FOWbottomLeft = FOWStartEnd.xy;
				float2 cameraTopRight = cameraStartEnd.zw;
				float2 cameraBottomLeft = cameraStartEnd.xy;
				float2 cameraCenter = (cameraStartEnd.xy + cameraStartEnd.zw) / 2;

				float2 uv = i.uv;
				uv *= cameraSize*2;
				uv = mul(rot, uv);
				uv += cameraBottomLeft;
				uv -= FOWbottomLeft;
				uv /= (FOWsize * 2);

				fixed fow = tex2D(_FogOfWar, uv)*_FogOfWarBrightness;
				return max(viewMask.rrrr, fow.rrrr);
            }
            ENDCG
        }
    }
}
