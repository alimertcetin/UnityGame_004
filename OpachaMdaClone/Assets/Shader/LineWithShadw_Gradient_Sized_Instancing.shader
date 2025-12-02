Shader "Custom/LineWithShadow_Gradient_Sized_Instanced"
{
    Properties
    {
        _MainTex ("Texture", 2D) = "white" {}
        _ShadowColor ("Shadow Color", Color) = (0,0,0,0.4)
        _ShadowOffset ("Shadow Offset", Vector) = (0, -0.1, 0, 0)
        _ShadowSize ("Shadow Size", Vector) = (1.05, 1.05, 0, 0)
        _SegCount ("Segment Count", Float) = 1
    }

    SubShader
    {
        Tags { "Queue"="Transparent" "RenderType"="Transparent" }
        Blend SrcAlpha OneMinusSrcAlpha
        ZWrite Off
        Cull Off

        Pass        // ---------- SHADOW PASS ----------
        {
            Name "ShadowPass"
            CGPROGRAM
            #pragma vertex vert
            #pragma fragment frag
            #pragma multi_compile_instancing
            #include "UnityCG.cginc"

            sampler2D _MainTex;

            // ----------------- INSTANCING -----------------
            UNITY_INSTANCING_BUFFER_START(Props)
                UNITY_DEFINE_INSTANCED_PROP(float4, _ShadowColor)
                UNITY_DEFINE_INSTANCED_PROP(float4, _ShadowOffset)
                UNITY_DEFINE_INSTANCED_PROP(float4, _ShadowSize)
                UNITY_DEFINE_INSTANCED_PROP(float,  _SegCount)
            UNITY_INSTANCING_BUFFER_END(Props)

            struct appdata
            {
                float4 vertex : POSITION;
                float3 normal : NORMAL;
                float2 uv : TEXCOORD0;
                float4 color : COLOR;

                UNITY_VERTEX_INPUT_INSTANCE_ID
            };

            struct v2f
            {
                float2 uv : TEXCOORD0;
                float4 color : COLOR;
                float4 vertex : SV_POSITION;

                UNITY_VERTEX_OUTPUT_STEREO
            };

            float SegmentMask(float uvx, float segCount)
            {
                if (segCount <= 0.0) return 1.0;

                float segLen = 1.0 / segCount;
                float local = fmod(uvx, segLen);
                float filled = segLen * 0.9;

                return step(local, filled);
            }

            v2f vert(appdata v)
            {
                v2f o;

                UNITY_SETUP_INSTANCE_ID(v);
                UNITY_INITIALIZE_VERTEX_OUTPUT_STEREO(o);

                float4 shadowOffset = UNITY_ACCESS_INSTANCED_PROP(Props, _ShadowOffset);
                float4 shadowSize   = UNITY_ACCESS_INSTANCED_PROP(Props, _ShadowSize);

                float3 worldPos = mul(unity_ObjectToWorld, v.vertex).xyz;
                float3 worldNormal = normalize(mul((float3x3)unity_ObjectToWorld, v.normal));
                float3 tangent = normalize(mul((float3x3)unity_ObjectToWorld, float3(1,0,0)));
                float3 binormal = normalize(cross(worldNormal, tangent));

                float3 localOffset =
                    (v.uv.x - 0.5) * tangent * (shadowSize.x - 1.0) +
                    binormal * (shadowSize.y - 1.0) * 0.5;

                worldPos += localOffset;
                worldPos += shadowOffset.xyz;

                o.vertex = UnityWorldToClipPos(worldPos);
                o.uv = v.uv;
                o.color = v.color;
                return o;
            }

            fixed4 frag(v2f i) : SV_Target
            {
                float segCount = UNITY_ACCESS_INSTANCED_PROP(Props, _SegCount);
                float m = SegmentMask(i.uv.x, segCount);

                float4 shadowColor = UNITY_ACCESS_INSTANCED_PROP(Props, _ShadowColor);
                return shadowColor * i.color.a * m;
            }
            ENDCG
        }

        Pass        // ---------- MAIN PASS ----------
        {
            Name "MainPass"
            CGPROGRAM
            #pragma vertex vert
            #pragma fragment frag
            #pragma multi_compile_instancing
            #include "UnityCG.cginc"

            sampler2D _MainTex;

            UNITY_INSTANCING_BUFFER_START(Props)
                UNITY_DEFINE_INSTANCED_PROP(float, _SegCount)
            UNITY_INSTANCING_BUFFER_END(Props)

            struct appdata
            {
                float4 vertex : POSITION;
                float2 uv : TEXCOORD0;
                float4 color : COLOR;

                UNITY_VERTEX_INPUT_INSTANCE_ID
            };

            struct v2f
            {
                float2 uv : TEXCOORD0;
                float4 color : COLOR;
                float4 vertex : SV_POSITION;

                UNITY_VERTEX_OUTPUT_STEREO
            };

            float SegmentMask(float uvx, float segCount)
            {
                if (segCount <= 0.0) return 1.0;

                float segLen = 1.0 / segCount;
                float local = fmod(uvx, segLen);
                float filled = segLen * 0.9;

                return step(local, filled);
            }

            v2f vert(appdata v)
            {
                v2f o;

                UNITY_SETUP_INSTANCE_ID(v);
                UNITY_INITIALIZE_VERTEX_OUTPUT_STEREO(o);

                o.vertex = UnityObjectToClipPos(v.vertex);
                o.uv = v.uv;
                o.color = v.color;
                return o;
            }

            fixed4 frag(v2f i) : SV_Target
            {
                float segCount = UNITY_ACCESS_INSTANCED_PROP(Props, _SegCount);

                float m = SegmentMask(i.uv.x, segCount);
                return tex2D(_MainTex, i.uv) * i.color * m;
            }
            ENDCG
        }
    }
}
