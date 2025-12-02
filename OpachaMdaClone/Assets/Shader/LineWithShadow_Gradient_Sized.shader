Shader "Custom/LineWithShadow_Gradient_Sized"
{
    Properties
    {
        _MainTex ("Texture", 2D) = "white" {}
        _ShadowColor ("Shadow Color", Color) = (0,0,0,0.4)
        _ShadowOffset ("Shadow Offset", Vector) = (0, -0.1, 0, 0)
        _ShadowSize ("Shadow Size", Vector) = (1.05, 1.05, 0, 0)
        _SegCount ("Segment Count", Float) = 1
//        _SegGap ("Gap Fraction", Range(0,1)) = 0.2
    }

    SubShader
    {
        Tags { "Queue"="Transparent" "RenderType"="Transparent" }
        LOD 100

        Blend SrcAlpha OneMinusSrcAlpha
        ZWrite Off
        Cull Off
        Lighting Off

        // --- PASS 1: Shadow ---
        Pass
        {
            Name "ShadowPass"
            CGPROGRAM
            #pragma vertex vert
            #pragma fragment frag
            #include "UnityCG.cginc"

            sampler2D _MainTex;
            fixed4 _ShadowColor;
            float4 _ShadowOffset;
            float3 _ShadowSize;
            float _SegCount;
            // float _SegGap;

            struct appdata
            {
                float4 vertex : POSITION;
                float3 normal : NORMAL;   // LineRenderer provides this in newer Unitys
                float2 uv : TEXCOORD0;
                fixed4 color : COLOR;
            };

            struct v2f
            {
                float2 uv : TEXCOORD0;
                fixed4 color : COLOR;
                float4 vertex : SV_POSITION;
            };

            float SegmentMask(float uvx)
            {
                float count = max(_SegCount, 0);
                if (count <= 0)
                {
                    return 1;
                }
                float segLen = 1.0 / count;
                float local = fmod(uvx, segLen);
                // float filled = segLen * (1.0 - _SegGap);
                float filled = segLen * (0.9);
                
                return step(local, filled);
            }

            v2f vert(appdata v)
            {
                v2f o;

                // World-space position and normal
                float3 worldPos = mul(unity_ObjectToWorld, v.vertex).xyz;
                float3 worldNormal = normalize(mul((float3x3)unity_ObjectToWorld, v.normal));

                // Approximate tangent along the line using derivative of UV.x
                // In practice, the line is built along local X, so tangent ≈ (1,0,0)
                float3 tangent = normalize(mul((float3x3)unity_ObjectToWorld, float3(1,0,0)));

                // Compute a perpendicular "up" vector based on tangent and normal
                float3 binormal = normalize(cross(worldNormal, tangent));

                // Expand outward relative to the line's local basis:
                //   _ShadowSize.x -> along tangent
                //   _ShadowSize.y -> along binormal
                float3 localCenterOffset = (v.uv.x - 0.5) * tangent * (_ShadowSize.x - 1.0);
                float3 radialOffset = binormal * (_ShadowSize.y - 1.0) * 0.5;

                // Combine offsets for full inflation
                worldPos += localCenterOffset + radialOffset;

                // Apply global shadow offset (projection drop)
                worldPos += _ShadowOffset.xyz;

                o.vertex = UnityWorldToClipPos(worldPos);
                o.uv = v.uv;
                o.color = v.color;
                return o;
            }

            fixed4 frag(v2f i) : SV_Target
            {
                float m = SegmentMask(i.uv.x);
                return _ShadowColor * i.color.a * m;
            }
            ENDCG
        }

        // --- PASS 2: Main Line ---
        Pass
        {
            Name "MainPass"
            CGPROGRAM
            #pragma vertex vert
            #pragma fragment frag
            #include "UnityCG.cginc"

            sampler2D _MainTex;
            float _SegCount;
            float _SegGap;

            struct appdata
            {
                float4 vertex : POSITION;
                float2 uv : TEXCOORD0;
                fixed4 color : COLOR;
            };

            struct v2f
            {
                float2 uv : TEXCOORD0;
                fixed4 color : COLOR;
                float4 vertex : SV_POSITION;
            };

            v2f vert(appdata v)
            {
                v2f o;
                o.vertex = UnityObjectToClipPos(v.vertex);
                o.uv = v.uv;
                o.color = v.color;
                return o;
            }
            
            float SegmentMask(float uvx)
            {
                float count = max(_SegCount, 0);
                if (count <= 0)
                {
                    return 1;
                }
                float segLen = 1.0 / count;
                float local = fmod(uvx, segLen);
                // float filled = segLen * (1.0 - _SegGap);
                float filled = segLen * (0.9);
                
                return step(local, filled);
            }

            fixed4 frag(v2f i) : SV_Target
            {
                float m = SegmentMask(i.uv.x);
                fixed4 texCol = tex2D(_MainTex, i.uv);
                return texCol * i.color * m;
            }
            ENDCG
        }
    }
}
